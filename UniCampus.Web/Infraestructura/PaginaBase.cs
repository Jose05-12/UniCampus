using System;
using System.Web.UI;
using UniCampus.Entities;

namespace UniCampus.Web.Infraestructura
{
    /// <summary>
    /// Página base para todas las páginas que requieren sesión iniciada.
    /// </summary>
    public abstract class PaginaBase : Page
    {
        public Usuario UsuarioActual
        {
            get { return SesionUsuario.Obtener(); }
        }

        protected bool EsAdministrador
        {
            get { return UsuarioActual != null && UsuarioActual.EsAdministrador; }
        }

        protected override void OnInit(EventArgs e)
        {
            if (UsuarioActual == null)
            {
                Response.Redirect("~/Login.aspx", true);
                return;
            }

            // Vincula el ViewState a la sesión para mitigar ataques CSRF.
            ViewStateUserKey = Session.SessionID;
            base.OnInit(e);
        }

        /// <summary>Clase CSS de la etiqueta de estado de una actividad.</summary>
        protected string ClaseEstado(object estado)
        {
            switch (Convert.ToString(estado))
            {
                case EstadosActividad.Activa: return "estado estado-activa";
                case EstadosActividad.Proxima: return "estado estado-proxima";
                case EstadosActividad.Finalizada: return "estado estado-finalizada";
                case EstadosActividad.Cancelada: return "estado estado-cancelada";
                default: return "estado";
            }
        }

        private static readonly System.Globalization.CultureInfo Espanol = new System.Globalization.CultureInfo("es-CO");

        /// <summary>Mes abreviado en español sin punto final (p. ej. "nov").</summary>
        protected string Mes(object fecha)
        {
            if (!(fecha is DateTime)) return "";
            return Espanol.DateTimeFormat.GetAbbreviatedMonthName(((DateTime)fecha).Month).TrimEnd('.');
        }

        protected string Dia(object fecha)
        {
            return fecha is DateTime ? ((DateTime)fecha).ToString("dd") : "";
        }

        /// <summary>Hasta dos iniciales del nombre, para los avatares.</summary>
        protected string Iniciales(object nombre)
        {
            var partes = Convert.ToString(nombre).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0) return "?";
            string ini = partes[0].Substring(0, 1);
            if (partes.Length > 1) ini += partes[1].Substring(0, 1);
            return ini.ToUpperInvariant();
        }

        protected int Ocupacion(object inscritos, object cupos)
        {
            int c = Convert.ToInt32(cupos);
            return c <= 0 ? 0 : Math.Min(100, (int)Math.Round(Convert.ToInt32(inscritos) * 100.0 / c));
        }

        protected void MostrarToast(System.Web.UI.WebControls.Literal destino, bool exito, string titulo, System.Collections.Generic.IList<string> detalles)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("<div class=\"toast ").Append(exito ? "toast-exito" : "toast-error").Append("\" role=\"alert\">");
            sb.Append("<i class=\"bi ").Append(exito ? "bi-check-circle-fill" : "bi-exclamation-triangle-fill").Append("\"></i><div>");
            sb.Append("<strong>").Append(Server.HtmlEncode(titulo)).Append("</strong>");
            if (detalles != null && detalles.Count > 0 && !(detalles.Count == 1 && detalles[0] == titulo))
            {
                sb.Append("<ul>");
                foreach (var d in detalles) sb.Append("<li>").Append(Server.HtmlEncode(d)).Append("</li>");
                sb.Append("</ul>");
            }
            sb.Append("</div><button type=\"button\" class=\"toast-cerrar\" onclick=\"this.parentNode.remove()\" aria-label=\"Cerrar\">&times;</button></div>");
            destino.Text = sb.ToString();
        }
    }
}
