using System.Web;
using UniCampus.Entities;

namespace UniCampus.Web.Infraestructura
{
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
