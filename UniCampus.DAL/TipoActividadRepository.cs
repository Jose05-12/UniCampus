using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public class TipoActividadRepository
    {
        public List<TipoActividad> Listar()
        {
            const string sql = "SELECT TipoId, Nombre, Descripcion FROM dbo.TipoActividad ORDER BY Nombre;";
            var lista = new List<TipoActividad>();

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new TipoActividad
                        {
                            TipoId = r.GetInt32(0),
                            Nombre = r.GetString(1),
                            Descripcion = r.IsDBNull(2) ? "" : r.GetString(2)
                        });
                    }
                }
            }
            return lista;
        }

        public bool Existe(int tipoId)
        {
            const string sql = "SELECT COUNT(1) FROM dbo.TipoActividad WHERE TipoId = @TipoId;";
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = tipoId;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }
}
