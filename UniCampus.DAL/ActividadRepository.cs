using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public class ActividadRepository
    {
        private const string SelectBase = @"
            SELECT a.ActividadId, a.Titulo, a.Descripcion, a.TipoId, t.Nombre AS TipoNombre,
                   a.FechaInicio, a.FechaFin, a.HoraInicio, a.HoraFin,
                   a.Modalidad, a.Lugar, a.Cupos, a.Estado,
                   (SELECT COUNT(1) FROM dbo.Inscripcion i
                     WHERE i.ActividadId = a.ActividadId AND i.Estado = N'Activa') AS Inscritos,
                   (SELECT AVG(CAST(r.Calificacion AS DECIMAL(4,2))) FROM dbo.Resena r
                     WHERE r.ActividadId = a.ActividadId) AS Promedio,
                   (SELECT COUNT(1) FROM dbo.Resena r
                     WHERE r.ActividadId = a.ActividadId) AS TotalResenas
            FROM dbo.ActividadAcademica a
            INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId";

        public List<Actividad> Listar(FiltroActividad filtro)
        {
            filtro = filtro ?? new FiltroActividad();
            var resultado = new List<Actividad>();

            string sql = SelectBase + @"
            WHERE (@Texto IS NULL OR a.Titulo LIKE @Texto OR a.Descripcion LIKE @Texto OR a.Lugar LIKE @Texto)
              AND (@TipoId IS NULL OR a.TipoId = @TipoId)
              AND (@Estado IS NULL OR a.Estado = @Estado)
              AND (@Fecha IS NULL OR @Fecha BETWEEN a.FechaInicio AND a.FechaFin)
            ORDER BY a.ActividadId;";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Texto", SqlDbType.NVarChar, 130).Value =
                    string.IsNullOrWhiteSpace(filtro.Texto) ? (object)DBNull.Value : Conexion.PatronLike(filtro.Texto.Trim());
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value =
                    filtro.TipoId.HasValue ? (object)filtro.TipoId.Value : DBNull.Value;
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value =
                    string.IsNullOrWhiteSpace(filtro.Estado) ? (object)DBNull.Value : filtro.Estado;
                cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value =
                    filtro.Fecha.HasValue ? (object)filtro.Fecha.Value.Date : DBNull.Value;

                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultado.Add(Mapear(reader));
                    }
                }
            }
            return resultado;
        }

        public Actividad ObtenerPorId(int actividadId)
        {
            string sql = SelectBase + " WHERE a.ActividadId = @ActividadId;";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? Mapear(reader) : null;
                }
            }
        }

        public int Insertar(Actividad a)
        {
            const string sql = @"
                INSERT INTO dbo.ActividadAcademica
                    (Titulo, Descripcion, TipoId, FechaInicio, FechaFin, HoraInicio, HoraFin,
                     Modalidad, Lugar, Cupos, Estado)
                VALUES
                    (@Titulo, @Descripcion, @TipoId, @FechaInicio, @FechaFin, @HoraInicio, @HoraFin,
                     @Modalidad, @Lugar, @Cupos, @Estado);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                AgregarParametros(cmd, a);
                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Actualizar(Actividad a)
        {
            const string sql = @"
                UPDATE dbo.ActividadAcademica
                   SET Titulo = @Titulo, Descripcion = @Descripcion, TipoId = @TipoId,
                       FechaInicio = @FechaInicio, FechaFin = @FechaFin,
                       HoraInicio = @HoraInicio, HoraFin = @HoraFin,
                       Modalidad = @Modalidad, Lugar = @Lugar, Cupos = @Cupos, Estado = @Estado
                 WHERE ActividadId = @ActividadId;";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                AgregarParametros(cmd, a);
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = a.ActividadId;
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Eliminar(int actividadId)
        {
            const string sql = "DELETE FROM dbo.ActividadAcademica WHERE ActividadId = @ActividadId;";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool CambiarEstado(int actividadId, string estado)
        {
            const string sql = "UPDATE dbo.ActividadAcademica SET Estado = @Estado WHERE ActividadId = @ActividadId;";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = estado;
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ExisteDuplicado(string titulo, DateTime fechaInicio, int excluirActividadId)
        {
            const string sql = @"
                SELECT COUNT(1) FROM dbo.ActividadAcademica
                 WHERE Titulo = @Titulo AND FechaInicio = @FechaInicio AND ActividadId <> @ActividadId;";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 120).Value = titulo;
                cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = fechaInicio.Date;
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = excluirActividadId;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public int ContarDependencias(int actividadId)
        {
            const string sql = @"
                SELECT (SELECT COUNT(1) FROM dbo.Inscripcion WHERE ActividadId = @ActividadId)
                     + (SELECT COUNT(1) FROM dbo.Resena WHERE ActividadId = @ActividadId);";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public ResumenActividades ObtenerResumen()
        {
            const string sql = @"
                SELECT
                    (SELECT COUNT(1) FROM dbo.ActividadAcademica),
                    (SELECT COUNT(1) FROM dbo.Inscripcion WHERE Estado = N'Activa'),
                    (SELECT COUNT(1) FROM dbo.ActividadAcademica WHERE Estado = N'Activa'),
                    (SELECT COUNT(1) FROM dbo.ActividadAcademica WHERE Estado = N'Próxima'),
                    (SELECT COUNT(1) FROM dbo.ActividadAcademica WHERE Estado = N'Finalizada'),
                    (SELECT COUNT(1) FROM dbo.Resena),
                    (SELECT AVG(CAST(Calificacion AS DECIMAL(4,2))) FROM dbo.Resena);";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    r.Read();
                    return new ResumenActividades
                    {
                        TotalActividades = r.GetInt32(0),
                        EstudiantesInscritos = r.GetInt32(1),
                        ActividadesActivas = r.GetInt32(2),
                        ActividadesProximas = r.GetInt32(3),
                        ActividadesFinalizadas = r.GetInt32(4),
                        TotalResenas = r.GetInt32(5),
                        PromedioGeneral = r.IsDBNull(6) ? (decimal?)null : r.GetDecimal(6)
                    };
                }
            }
        }

        public List<ReporteTipo> ReportePorTipo()
        {
            const string sql = @"
                SELECT t.Nombre,
                       COUNT(a.ActividadId) AS Cantidad,
                       ISNULL(SUM(a.Cupos), 0) AS TotalCupos,
                       ISNULL(SUM(x.Inscritos), 0) AS TotalInscritos
                FROM dbo.TipoActividad t
                LEFT JOIN dbo.ActividadAcademica a ON a.TipoId = t.TipoId
                OUTER APPLY (SELECT COUNT(1) AS Inscritos FROM dbo.Inscripcion i
                              WHERE i.ActividadId = a.ActividadId AND i.Estado = N'Activa') x
                GROUP BY t.Nombre
                ORDER BY Cantidad DESC, t.Nombre;";

            var lista = new List<ReporteTipo>();
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new ReporteTipo
                        {
                            Tipo = r.GetString(0),
                            CantidadActividades = r.GetInt32(1),
                            TotalCupos = r.GetInt32(2),
                            TotalInscritos = r.GetInt32(3)
                        });
                    }
                }
            }
            return lista;
        }

        private static void AgregarParametros(SqlCommand cmd, Actividad a)
        {
            cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 120).Value = a.Titulo;
            cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 1000).Value = a.Descripcion;
            cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = a.TipoId;
            cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = a.FechaInicio.Date;
            cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = a.FechaFin.Date;
            cmd.Parameters.Add("@HoraInicio", SqlDbType.Time).Value = a.HoraInicio;
            cmd.Parameters.Add("@HoraFin", SqlDbType.Time).Value = a.HoraFin;
            cmd.Parameters.Add("@Modalidad", SqlDbType.NVarChar, 20).Value = a.Modalidad;
            cmd.Parameters.Add("@Lugar", SqlDbType.NVarChar, 150).Value = a.Lugar;
            cmd.Parameters.Add("@Cupos", SqlDbType.Int).Value = a.Cupos;
            cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = a.Estado;
        }

        private static Actividad Mapear(SqlDataReader r)
        {
            int iPromedio = r.GetOrdinal("Promedio");
            return new Actividad
            {
                ActividadId = r.GetInt32(r.GetOrdinal("ActividadId")),
                Titulo = r.GetString(r.GetOrdinal("Titulo")),
                Descripcion = r.GetString(r.GetOrdinal("Descripcion")),
                TipoId = r.GetInt32(r.GetOrdinal("TipoId")),
                TipoNombre = r.GetString(r.GetOrdinal("TipoNombre")),
                FechaInicio = r.GetDateTime(r.GetOrdinal("FechaInicio")),
                FechaFin = r.GetDateTime(r.GetOrdinal("FechaFin")),
                HoraInicio = r.GetTimeSpan(r.GetOrdinal("HoraInicio")),
                HoraFin = r.GetTimeSpan(r.GetOrdinal("HoraFin")),
                Modalidad = r.GetString(r.GetOrdinal("Modalidad")),
                Lugar = r.GetString(r.GetOrdinal("Lugar")),
                Cupos = r.GetInt32(r.GetOrdinal("Cupos")),
                Estado = r.GetString(r.GetOrdinal("Estado")),
                Inscritos = r.GetInt32(r.GetOrdinal("Inscritos")),
                PromedioCalificacion = r.IsDBNull(iPromedio) ? (decimal?)null : r.GetDecimal(iPromedio),
                TotalResenas = r.GetInt32(r.GetOrdinal("TotalResenas"))
            };
        }
    }
}
