using System;
using System.Collections.Generic;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class InicioPresenter
    {
        private readonly IInicioView _view;
        private readonly ActividadService _actividades;
        private readonly InscripcionService _inscripciones;

        public InicioPresenter(IInicioView view)
        {
            _view = view;
            _actividades = new ActividadService();
            _inscripciones = new InscripcionService();
        }

        public void Cargar()
        {
            try
            {
                _view.MostrarResumen(_actividades.ObtenerResumen());
                _view.MostrarProximas(_actividades.ListarProximas(4));

                var usuario = _view.UsuarioActual;
                _view.MostrarMisInscripciones(usuario.EsEstudiante
                    ? _inscripciones.ListarPorUsuario(usuario.UsuarioId)
                    : new List<Inscripcion>());
            }
            catch (Exception ex)
            {
                RegistroErrores.Registrar(ex, "InicioPresenter.Cargar");
                _view.MostrarError("No fue posible cargar el resumen. Intente nuevamente.");
            }
        }
    }
}
