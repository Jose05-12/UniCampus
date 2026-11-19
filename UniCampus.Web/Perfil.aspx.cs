using System;
using UniCampus.Entities;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    public partial class Perfil : PaginaBase, IPerfilView
    {
        private PerfilPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new PerfilPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) _presenter.Cargar();
        }

        public void MostrarUsuario(Usuario u)
        {
            litInicial.Text = Server.HtmlEncode(Iniciales(u.Nombre));
            litNombre.Text = Server.HtmlEncode(u.Nombre);
            litRol.Text = Server.HtmlEncode(u.Rol);
            litCorreo.Text = Server.HtmlEncode(u.Correo);
            litPrograma.Text = string.IsNullOrEmpty(u.Programa) ? "No aplica" : Server.HtmlEncode(u.Programa);
            litEstado.Text = u.Activo ? "<span class=\"estado estado-activa\">Cuenta activa</span>" : "<span class=\"estado estado-cancelada\">Cuenta inactiva</span>";
        }

        public void MostrarEstadisticas(int inscripcionesActivas, int resenasPublicadas)
        {
            litInscripciones.Text = inscripcionesActivas.ToString();
            litResenas.Text = resenasPublicadas.ToString();
        }

        public void MostrarError(string mensaje)
        {
            MostrarToast(litToast, false, mensaje, null);
        }
    }
}
