using System;
using System.Collections.Generic;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    /// <summary>
    /// Reglas de reseñas: solo estudiantes inscritos, una reseña por actividad,
    /// calificación 1-5 y comentario con longitud controlada.
    /// </summary>
    public class ResenaService
    {
        public const int ComentarioMinimo = 10;
        public const int ComentarioMaximo = 500;

        private readonly ResenaRepository _resenas;
        private readonly InscripcionRepository _inscripciones;
        private readonly ActividadRepository _actividades;

        public ResenaService()
        {
            _resenas = new ResenaRepository();
            _inscripciones = new InscripcionRepository();
            _actividades = new ActividadRepository();
        }

        public List<Resena> ListarPorActividad(int actividadId)
        {
            return _resenas.ListarPorActividad(actividadId);
        }

        public List<Resena> ListarPorUsuario(int usuarioId)
        {
            return _resenas.ListarPorUsuario(usuarioId);
        }

        /// <summary>
        /// Devuelve null si el usuario puede reseñar la actividad; si no, el motivo.
        /// </summary>
        public string MotivoNoPuedeResenar(Actividad actividad, Usuario usuario)
        {
            if (usuario == null) return "Debe iniciar sesión para reseñar.";
            if (!usuario.EsEstudiante) return "Solo los estudiantes pueden publicar reseñas.";
            if (actividad == null) return "La actividad no existe.";
            if (actividad.Estado == EstadosActividad.Cancelada) return "No se pueden reseñar actividades canceladas.";
            if (actividad.FechaInicio.Date > DateTime.Today) return "Podrá reseñar esta actividad cuando haya iniciado.";
            if (!_inscripciones.EstaInscrito(actividad.ActividadId, usuario.UsuarioId))
                return "Solo los estudiantes inscritos pueden reseñar esta actividad.";
            if (_resenas.Existe(actividad.ActividadId, usuario.UsuarioId))
                return "Ya publicó una reseña para esta actividad.";
            return null;
        }

        public ResultadoOperacion Publicar(int actividadId, int calificacion, string comentario, Usuario usuarioActual)
        {
            comentario = (comentario ?? "").Trim();

            var errores = new List<string>();
            if (calificacion < 1 || calificacion > 5)
                errores.Add("La calificación debe ser un valor entre 1 y 5 estrellas.");
            if (comentario.Length < ComentarioMinimo || comentario.Length > ComentarioMaximo)
                errores.Add("El comentario debe tener entre " + ComentarioMinimo + " y " + ComentarioMaximo + " caracteres.");
            if (errores.Count > 0)
                return ResultadoOperacion.ErrorValidacion(errores);

            var actividad = _actividades.ObtenerPorId(actividadId);
            string motivo = MotivoNoPuedeResenar(actividad, usuarioActual);
            if (motivo != null)
                return ResultadoOperacion.Error(motivo);

            _resenas.Insertar(new Resena
            {
                ActividadId = actividadId,
                UsuarioId = usuarioActual.UsuarioId,
                Calificacion = calificacion,
                Comentario = comentario
            });
            return ResultadoOperacion.Ok("¡Gracias! Su reseña de \"" + actividad.Titulo + "\" fue publicada.");
        }

        public ResultadoOperacion Eliminar(int resenaId, Usuario usuarioActual)
        {
            if (usuarioActual == null)
                return ResultadoOperacion.Error("Debe iniciar sesión.");

            var resena = _resenas.ObtenerPorId(resenaId);
            if (resena == null)
                return ResultadoOperacion.Error("La reseña no existe o ya fue eliminada.");

            if (!usuarioActual.EsAdministrador && resena.UsuarioId != usuarioActual.UsuarioId)
                return ResultadoOperacion.Error("Solo puede eliminar sus propias reseñas.");

            _resenas.Eliminar(resenaId);
            return ResultadoOperacion.Ok("La reseña fue eliminada.");
        }
    }
}
