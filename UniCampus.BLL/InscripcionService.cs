using System;
using System.Collections.Generic;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    public class InscripcionService
    {
        private readonly InscripcionRepository _inscripciones;
        private readonly ActividadRepository _actividades;
        private readonly UsuarioRepository _usuarios;

        public InscripcionService()
        {
            _inscripciones = new InscripcionRepository();
            _actividades = new ActividadRepository();
            _usuarios = new UsuarioRepository();
        }

        public List<Inscripcion> ListarPorActividad(int actividadId)
        {
            return _inscripciones.ListarPorActividad(actividadId);
        }

        public List<Inscripcion> ListarPorUsuario(int usuarioId)
        {
            return _inscripciones.ListarPorUsuario(usuarioId);
        }

        public List<Usuario> ListarEstudiantesDisponibles(int actividadId)
        {
            return _usuarios.ListarEstudiantesNoInscritos(actividadId);
        }

        public bool EstaInscrito(int actividadId, int usuarioId)
        {
            return _inscripciones.EstaInscrito(actividadId, usuarioId);
        }

        public static bool AdmiteInscripciones(Actividad actividad)
        {
            return actividad != null
                && (actividad.Estado == EstadosActividad.Activa || actividad.Estado == EstadosActividad.Proxima)
                && actividad.FechaFin.Date >= DateTime.Today;
        }

        public bool PuedeInscribirse(Actividad actividad, Usuario usuario)
        {
            return usuario != null
                && usuario.EsEstudiante
                && AdmiteInscripciones(actividad)
                && actividad.CuposDisponibles > 0
                && !_inscripciones.EstaInscrito(actividad.ActividadId, usuario.UsuarioId);
        }

        public ResultadoOperacion Inscribir(int actividadId, int estudianteId, Usuario usuarioActual)
        {
            if (usuarioActual == null)
                return ResultadoOperacion.Error("Debe iniciar sesión para inscribirse.");

            if (!usuarioActual.EsAdministrador && estudianteId != usuarioActual.UsuarioId)
                return ResultadoOperacion.Error("No tiene permisos para inscribir a otro usuario.");

            if (estudianteId <= 0)
                return ResultadoOperacion.Error("Seleccione el estudiante que desea inscribir.");

            var estudiante = _usuarios.ObtenerPorId(estudianteId);
            if (estudiante == null || !estudiante.Activo || !estudiante.EsEstudiante)
                return ResultadoOperacion.Error("Solo se pueden inscribir estudiantes activos.");

            var actividad = _actividades.ObtenerPorId(actividadId);
            if (actividad == null)
                return ResultadoOperacion.Error("La actividad seleccionada no existe.");

            if (!AdmiteInscripciones(actividad))
                return ResultadoOperacion.Error("La actividad \"" + actividad.Titulo + "\" no admite inscripciones (estado: "
                                                + actividad.Estado + ", finaliza el " + actividad.FechaFin.ToString("dd/MM/yyyy") + ").");

            switch (_inscripciones.Inscribir(actividadId, estudianteId))
            {
                case ResultadoInscripcion.Exitosa:
                    return ResultadoOperacion.Ok(estudiante.Nombre + " quedó inscrito(a) en \"" + actividad.Titulo + "\".");
                case ResultadoInscripcion.YaInscrito:
                    return ResultadoOperacion.Error(estudiante.Nombre + " ya está inscrito(a) en esta actividad.");
                case ResultadoInscripcion.SinCupos:
                    return ResultadoOperacion.Error("No hay cupos disponibles en \"" + actividad.Titulo + "\".");
                default:
                    return ResultadoOperacion.Error("La actividad seleccionada no existe.");
            }
        }

        public ResultadoOperacion Cancelar(int inscripcionId, Usuario usuarioActual)
        {
            if (usuarioActual == null)
                return ResultadoOperacion.Error("Debe iniciar sesión.");

            var inscripcion = _inscripciones.ObtenerPorId(inscripcionId);
            if (inscripcion == null || inscripcion.Estado != EstadosInscripcion.Activa)
                return ResultadoOperacion.Error("La inscripción no existe o ya fue cancelada.");

            if (!usuarioActual.EsAdministrador && inscripcion.UsuarioId != usuarioActual.UsuarioId)
                return ResultadoOperacion.Error("Solo puede cancelar sus propias inscripciones.");

            if (inscripcion.EstadoActividad == EstadosActividad.Finalizada)
                return ResultadoOperacion.Error("No se puede cancelar la inscripción de una actividad finalizada.");

            _inscripciones.Cancelar(inscripcionId);
            return ResultadoOperacion.Ok("Se canceló la inscripción de " + inscripcion.NombreEstudiante
                                         + " en \"" + inscripcion.TituloActividad + "\".");
        }
    }
}
