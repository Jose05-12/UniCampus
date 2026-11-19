using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IPerfilView
    {
        Usuario UsuarioActual { get; }

        void MostrarUsuario(Usuario usuario);
        void MostrarEstadisticas(int inscripcionesActivas, int resenasPublicadas);
        void MostrarError(string mensaje);
    }
}
