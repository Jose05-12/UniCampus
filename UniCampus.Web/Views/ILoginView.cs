using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface ILoginView
    {
        string Correo { get; }
        string Password { get; }

        void MostrarError(string mensaje);
        void IniciarSesionUsuario(Usuario usuario);
    }
}
