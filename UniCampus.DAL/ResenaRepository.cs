using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public class ResenaRepository
    {
        private const string SelectBase = @"
            SELECT r.ResenaId, r.ActividadId, r.UsuarioId, r.Calificacion, r.Comentario,
                   r.FechaCreacion, u.Nombre
            FROM dbo.Resena r
            INNER JOIN dbo.Usuario u ON u.UsuarioId = r.UsuarioId";

        public List<Resena> ListarPorActividad(int actividadId)
        {
            string sql = SelectBase + " WHERE r.ActividadId = @Id ORDER BY r.FechaCreacion DESC;";
            return Consultar(sql, "@Id", actividadId);
        }

        public List<Resena> ListarPorUsuario(int usuarioId)
        {
            string sql = SelectBase + " WHERE r.UsuarioId = @Id ORDER BY r.FechaCreacion DESC;";
            return Consultar(sql, "@Id", usuarioId);
        }

        public Resena ObtenerPorId(int resenaId)
        {
            string sql = SelectBase + " WHERE r.ResenaId = @Id;";
            var lista = Consultar(sql, "@Id", resenaId);
            return lista.Count > 0 ? lista[0] : null;
        }

        public bool Existe(int actividadId, int usuarioId)
        {
            const string sql = "SELECT COUNT(1) FROM dbo.Resena WHERE ActividadId = @A AND UsuarioId = @U;";
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@A", SqlDbType.Int).Value = actividadId;
                cmd.Parameters.Add("@U", SqlDbType.Int).Value = usuarioId;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public int Insertar(Resena resena)
        {
            const string sql = @"
                INSERT INTO dbo.Resena (ActividadId, UsuarioId, Calificacion, Comentario)
                VALUES (@A, @U, @Calificacion, @Comentario);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@A", SqlDbType.Int).Value = resena.ActividadId;
                cmd.Parameters.Add("@U", SqlDbType.Int).Value = resena.UsuarioId;
                cmd.Parameters.Add("@Calificacion", SqlDbType.TinyInt).Value = (byte)resena.Calificacion;
                cmd.Parameters.Add("@Comentario", SqlDbType.NVarChar, 500).Value = resena.Comentario;
                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Eliminar(int resenaId)
        {
            const string sql = "DELETE FROM dbo.Resena WHERE ResenaId = @Id;";
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = resenaId;
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private static List<Resena> Consultar(string sql, string parametro, int valor)
        {
            var lista = new List<Resena>();
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add(parametro, SqlDbType.Int).Value = valor;
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new Resena
                        {
                            ResenaId = r.GetInt32(0),
                            ActividadId = r.GetInt32(1),
                            UsuarioId = r.GetInt32(2),
                            Calificacion = r.GetByte(3),
                            Comentario = r.GetString(4),
                            FechaCreacion = r.GetDateTime(5),
                            NombreUsuario = r.GetString(6)
                        });
                    }
                }
            }
            return lista;
        }
    }
}
