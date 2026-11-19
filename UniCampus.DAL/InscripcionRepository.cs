using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public class InscripcionRepository
    {
        private const string SelectBase = @"
            SELECT i.InscripcionId, i.ActividadId, i.UsuarioId, i.FechaInscripcion, i.Estado,
                   u.Nombre, u.Programa,
                   a.Titulo, t.Nombre AS Tipo, a.FechaInicio, a.FechaFin, a.HoraInicio, a.HoraFin,
                   a.Lugar, a.Estado AS EstadoActividad
            FROM dbo.Inscripcion i
            INNER JOIN dbo.Usuario u ON u.UsuarioId = i.UsuarioId
            INNER JOIN dbo.ActividadAcademica a ON a.ActividadId = i.ActividadId
            INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId";

        public List<Inscripcion> ListarPorActividad(int actividadId)
        {
            string sql = SelectBase + @"
             WHERE i.ActividadId = @ActividadId AND i.Estado = N'Activa'
             ORDER BY i.FechaInscripcion;";
            return Consultar(sql, "@ActividadId", actividadId);
        }

        public List<Inscripcion> ListarPorUsuario(int usuarioId)
        {
            string sql = SelectBase + @"
             WHERE i.UsuarioId = @UsuarioId AND i.Estado = N'Activa'
             ORDER BY a.FechaInicio DESC;";
            return Consultar(sql, "@UsuarioId", usuarioId);
        }

        public Inscripcion ObtenerPorId(int inscripcionId)
        {
            string sql = SelectBase + " WHERE i.InscripcionId = @InscripcionId;";
            var lista = Consultar(sql, "@InscripcionId", inscripcionId);
            return lista.Count > 0 ? lista[0] : null;
        }

        public bool EstaInscrito(int actividadId, int usuarioId)
        {
            const string sql = @"
                SELECT COUNT(1) FROM dbo.Inscripcion
                 WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId AND Estado = @Estado;";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = EstadosInscripcion.Activa;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// Registra la inscripción dentro de una transacción. El bloqueo UPDLOCK sobre la
        /// fila de la actividad hace que dos solicitudes simultáneas se atiendan una tras
        /// otra, por lo que nunca se supera el cupo ni se duplica la inscripción.
        /// </summary>
        public ResultadoInscripcion Inscribir(int actividadId, int usuarioId)
        {
            using (var cn = Conexion.Crear())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        // 1. Bloquear la actividad y leer su cupo.
                        int cupos;
                        using (var cmd = new SqlCommand(
                            "SELECT Cupos FROM dbo.ActividadAcademica WITH (UPDLOCK, ROWLOCK) WHERE ActividadId = @A;", cn, tx))
                        {
                            cmd.Parameters.Add("@A", SqlDbType.Int).Value = actividadId;
                            object valor = cmd.ExecuteScalar();
                            if (valor == null)
                            {
                                tx.Rollback();
                                return ResultadoInscripcion.ActividadNoExiste;
                            }
                            cupos = (int)valor;
                        }

                        // 2. ¿Existe ya un registro de este usuario para la actividad?
                        int? inscripcionExistente = null;
                        string estadoExistente = null;
                        using (var cmd = new SqlCommand(
                            "SELECT InscripcionId, Estado FROM dbo.Inscripcion WHERE ActividadId = @A AND UsuarioId = @U;", cn, tx))
                        {
                            cmd.Parameters.Add("@A", SqlDbType.Int).Value = actividadId;
                            cmd.Parameters.Add("@U", SqlDbType.Int).Value = usuarioId;
                            using (var r = cmd.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    inscripcionExistente = r.GetInt32(0);
                                    estadoExistente = r.GetString(1);
                                }
                            }
                        }

                        if (estadoExistente == EstadosInscripcion.Activa)
                        {
                            tx.Rollback();
                            return ResultadoInscripcion.YaInscrito;
                        }

                        // 3. Verificar cupos disponibles.
                        int inscritos;
                        using (var cmd = new SqlCommand(
                            "SELECT COUNT(1) FROM dbo.Inscripcion WHERE ActividadId = @A AND Estado = N'Activa';", cn, tx))
                        {
                            cmd.Parameters.Add("@A", SqlDbType.Int).Value = actividadId;
                            inscritos = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        if (inscritos >= cupos)
                        {
                            tx.Rollback();
                            return ResultadoInscripcion.SinCupos;
                        }

                        // 4. Guardar (reactivar si estaba cancelada, o insertar).
                        string sql = inscripcionExistente.HasValue
                            ? "UPDATE dbo.Inscripcion SET Estado = N'Activa', FechaInscripcion = SYSDATETIME() WHERE InscripcionId = @Id;"
                            : "INSERT INTO dbo.Inscripcion (ActividadId, UsuarioId) VALUES (@A, @U);";

                        using (var cmd = new SqlCommand(sql, cn, tx))
                        {
                            if (inscripcionExistente.HasValue)
                            {
                                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = inscripcionExistente.Value;
                            }
                            else
                            {
                                cmd.Parameters.Add("@A", SqlDbType.Int).Value = actividadId;
                                cmd.Parameters.Add("@U", SqlDbType.Int).Value = usuarioId;
                            }
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return ResultadoInscripcion.Exitosa;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool Cancelar(int inscripcionId)
        {
            const string sql = @"
                UPDATE dbo.Inscripcion SET Estado = @Estado
                 WHERE InscripcionId = @InscripcionId AND Estado = N'Activa';";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = EstadosInscripcion.Cancelada;
                cmd.Parameters.Add("@InscripcionId", SqlDbType.Int).Value = inscripcionId;
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private static List<Inscripcion> Consultar(string sql, string nombreParametro, int valor)
        {
            var lista = new List<Inscripcion>();
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add(nombreParametro, SqlDbType.Int).Value = valor;
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new Inscripcion
                        {
                            InscripcionId = r.GetInt32(0),
                            ActividadId = r.GetInt32(1),
                            UsuarioId = r.GetInt32(2),
                            FechaInscripcion = r.GetDateTime(3),
                            Estado = r.GetString(4),
                            NombreEstudiante = r.GetString(5),
                            Programa = r.IsDBNull(6) ? "" : r.GetString(6),
                            TituloActividad = r.GetString(7),
                            TipoActividad = r.GetString(8),
                            FechaInicioActividad = r.GetDateTime(9),
                            FechaFinActividad = r.GetDateTime(10),
                            HoraInicioActividad = r.GetTimeSpan(11),
                            HoraFinActividad = r.GetTimeSpan(12),
                            LugarActividad = r.GetString(13),
                            EstadoActividad = r.GetString(14)
                        });
                    }
                }
            }
            return lista;
        }
    }
}
