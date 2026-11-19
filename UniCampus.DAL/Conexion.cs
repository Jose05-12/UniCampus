using System.Configuration;
using System.Data.SqlClient;

namespace UniCampus.DAL
{
    /// <summary>
    /// Centraliza la lectura de la cadena de conexión "UniCampusDb" definida en Web.config.
    /// </summary>
    internal static class Conexion
    {
        private const string NombreCadena = "UniCampusDb";

        public static SqlConnection Crear()
        {
            var cadena = ConfigurationManager.ConnectionStrings[NombreCadena];
            if (cadena == null || string.IsNullOrWhiteSpace(cadena.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "No se encontró la cadena de conexión '" + NombreCadena + "' en Web.config.");
            }
            return new SqlConnection(cadena.ConnectionString);
        }

        /// <summary>
        /// Escapa los comodines de LIKE para que el texto del usuario se busque literalmente.
        /// </summary>
        public static string PatronLike(string texto)
        {
            var limpio = texto.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
            return "%" + limpio + "%";
        }
    }
}
