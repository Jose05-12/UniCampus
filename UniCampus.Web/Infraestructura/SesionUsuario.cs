using System.Web;
using UniCampus.Entities;

namespace UniCampus.Web.Infraestructura
{
    /// <summary>
    /// Guarda el usuario autenticado en la sesión del servidor.
    /// El Id del usuario nunca se toma de un control de la página.
    /// </summary>
    public static class SesionUsuario
    {
        private const string Clave = "UniCampus.UsuarioActual";

        public static Usuario Obtener()
        {
            var contexto = HttpContext.Current;
            if (contexto == null || contexto.Session == null) return null;
            return contexto.Session[Clave] as Usuario;
        }

        public static void Guardar(Usuario usuario)
        {
            HttpContext.Current.Session[Clave] = usuario;
        }

        public static void Cerrar()
        {
            var sesion = HttpContext.Current.Session;
            sesion.Clear();
            sesion.Abandon();
        }
    }
}
