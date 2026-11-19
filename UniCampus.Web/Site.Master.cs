using System;
using System.IO;
using System.Web.UI;
using UniCampus.Web.Infraestructura;

namespace UniCampus.Web
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = SesionUsuario.Obtener();
            if (usuario != null)
            {
                litNombreUsuario.Text = Server.HtmlEncode(usuario.Nombre);
                litRolUsuario.Text = Server.HtmlEncode(usuario.Rol);
                litIniciales.Text = Server.HtmlEncode(Iniciales(usuario.Nombre));
                phMenuReportes.Visible = usuario.EsAdministrador;
            }
        }

        protected string ClaseMenu(string pagina)
        {
            string actual = Path.GetFileName(Request.Path);
            if (string.IsNullOrEmpty(actual)) actual = "Default.aspx";
            return string.Equals(actual, pagina, StringComparison.OrdinalIgnoreCase) ? "activo" : "";
        }

        private static string Iniciales(string nombre)
        {
            var partes = (nombre ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0) return "?";
            string ini = partes[0].Substring(0, 1);
            if (partes.Length > 1) ini += partes[1].Substring(0, 1);
            return ini.ToUpperInvariant();
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            SesionUsuario.Cerrar();
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
