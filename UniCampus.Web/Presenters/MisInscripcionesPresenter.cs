using System;
using System.Collections.Generic;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class MisInscripcionesPresenter
    {
        private readonly IMisInscripcionesView _view;
        private readonly InscripcionService _inscripciones;
        private readonly ResenaService _resenas;

        public MisInscripcionesPresenter(IMisInscripcionesView view)
        {
            _view = view;
            _inscripciones = new InscripcionService();
            _resenas = new ResenaService();
        }

        public void Cargar()
        {
            Ejecutar("cargar sus inscripciones", CargarDatos);
        }

        public void Cancelar(int inscripcionId)
        {
            Ejecutar("cancelar la inscripción", () => Mostrar(_inscripciones.Cancelar(inscripcionId, _view.UsuarioActual)));
        }

        public void EliminarResena(int resenaId)
        {
            Ejecutar("eliminar la reseña", () => Mostrar(_resenas.Eliminar(resenaId, _view.UsuarioActual)));
        }

        private void Mostrar(ResultadoOperacion resultado)
        {
            if (resultado.Exitoso) _view.MostrarExito(resultado.Mensaje);
            else _view.MostrarErrores(resultado.Mensaje, resultado.Errores);
            CargarDatos();
        }

        private void CargarDatos()
        {
            var usuario = _view.UsuarioActual;
            if (!usuario.EsEstudiante)
            {
                _view.MostrarSoloEstudiantes();
                return;
            }
            _view.MostrarInscripciones(_inscripciones.ListarPorUsuario(usuario.UsuarioId));
            _view.MostrarResenas(_resenas.ListarPorUsuario(usuario.UsuarioId));
        }

        private void Ejecutar(string accion, Action operacion)
        {
            try
            {
                operacion();
            }
            catch (Exception ex)
            {
                RegistroErrores.Registrar(ex, "MisInscripcionesPresenter: " + accion);
                _view.MostrarErrores("No fue posible " + accion + ".",
                    new List<string> { "Ocurrió un error inesperado. Intente nuevamente." });
            }
        }
    }
}
