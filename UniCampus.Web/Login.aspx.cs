using System;
using System.Web.UI;
using UniCampus.Entities;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    public partial class Login : Page, ILoginView
    {
        private LoginPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new LoginPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && SesionUsuario.Obtener() != null)
            {
                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            _presenter.IniciarSesion();
        }

        public string Correo { get { return txtCorreo.Text; } }
        public string Password { get { return txtPassword.Text; } }

        public void MostrarError(string mensaje)
        {
            pnlError.Visible = true;
            litError.Text = Server.HtmlEncode(mensaje);
        }

        public void IniciarSesionUsuario(Usuario usuario)
        {
            SesionUsuario.Guardar(usuario);
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
