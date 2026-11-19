using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public class UsuarioRepository
    {
        public UsuarioCredencial ObtenerCredencialPorCorreo(string correo)
        {
            const string sql = @"
                SELECT UsuarioId, Nombre, Correo, Programa, Rol, Activo, PasswordHash, PasswordSalt
                  FROM dbo.Usuario
                 WHERE Correo = @Correo;";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = correo;
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new UsuarioCredencial
                    {
                        Usuario = Mapear(r),
                        PasswordHash = (byte[])r["PasswordHash"],
                        PasswordSalt = (byte[])r["PasswordSalt"]
                    };
                }
            }
        }

        public Usuario ObtenerPorId(int usuarioId)
        {
            const string sql = @"
                SELECT UsuarioId, Nombre, Correo, Programa, Rol, Activo
                  FROM dbo.Usuario WHERE UsuarioId = @UsuarioId;";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    return r.Read() ? Mapear(r) : null;
                }
            }
        }

        public List<Usuario> ListarEstudiantesNoInscritos(int actividadId)
        {
            const string sql = @"
                SELECT u.UsuarioId, u.Nombre, u.Correo, u.Programa, u.Rol, u.Activo
                  FROM dbo.Usuario u
                 WHERE u.Rol = N'Estudiante' AND u.Activo = 1
                   AND NOT EXISTS (SELECT 1 FROM dbo.Inscripcion i
                                    WHERE i.UsuarioId = u.UsuarioId
                                      AND i.ActividadId = @ActividadId
                                      AND i.Estado = N'Activa')
                 ORDER BY u.Nombre;";

            var lista = new List<Usuario>();
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) lista.Add(Mapear(r));
                }
            }
            return lista;
        }

        private static Usuario Mapear(SqlDataReader r)
        {
            int iPrograma = r.GetOrdinal("Programa");
            return new Usuario
            {
                UsuarioId = r.GetInt32(r.GetOrdinal("UsuarioId")),
                Nombre = r.GetString(r.GetOrdinal("Nombre")),
                Correo = r.GetString(r.GetOrdinal("Correo")),
                Programa = r.IsDBNull(iPrograma) ? "" : r.GetString(iPrograma),
                Rol = r.GetString(r.GetOrdinal("Rol")),
                Activo = r.GetBoolean(r.GetOrdinal("Activo"))
            };
        }
    }
}
