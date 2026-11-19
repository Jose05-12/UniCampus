using System;
using UniCampus.BLL;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class PerfilPresenter
    {
        private readonly IPerfilView _view;
        private readonly UsuarioService _usuarios;
        private readonly InscripcionService _inscripciones;
        private readonly ResenaService _resenas;

        public PerfilPresenter(IPerfilView view)
        {
            _view = view;
            _usuarios = new UsuarioService();
            _inscripciones = new InscripcionService();
            _resenas = new ResenaService();
        }

        public void Cargar()
        {
            try
            {
                int id = _view.UsuarioActual.UsuarioId;
                var usuario = _usuarios.ObtenerPorId(id);
                if (usuario == null)
                {
                    _view.MostrarError("No se encontró la información del usuario.");
                    return;
                }
                _view.MostrarUsuario(usuario);
                _view.MostrarEstadisticas(_inscripciones.ListarPorUsuario(id).Count, _resenas.ListarPorUsuario(id).Count);
            }
            catch (Exception ex)
            {
                RegistroErrores.Registrar(ex, "PerfilPresenter.Cargar");
                _view.MostrarError("No fue posible cargar el perfil. Intente nuevamente.");
            }
        }
    }
}
