using System;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class LoginPresenter
    {
        private readonly ILoginView _view;
        private readonly UsuarioService _usuarios;

        public LoginPresenter(ILoginView view)
        {
            _view = view;
            _usuarios = new UsuarioService();
        }

        public void IniciarSesion()
        {
            try
            {
                Usuario usuario;
                var resultado = _usuarios.IniciarSesion(_view.Correo, _view.Password, out usuario);
                if (!resultado.Exitoso)
                {
                    _view.MostrarError(resultado.Mensaje);
                    return;
                }
                _view.IniciarSesionUsuario(usuario);
            }
            catch (Exception ex)
            {
                RegistroErrores.Registrar(ex, "LoginPresenter.IniciarSesion");
                _view.MostrarError("No fue posible conectarse con el sistema. Verifique que la base de datos esté disponible.");
            }
        }
    }
}
