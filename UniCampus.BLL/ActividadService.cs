using System;
using System.Collections.Generic;
using System.Linq;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    /// <summary>
    /// Reglas de negocio de las actividades académicas.
    /// No manipula controles ASPX ni escribe SQL: valida y delega en los repositorios.
    /// </summary>
    public class ActividadService
    {
        public const int CuposMinimos = 1;
        public const int CuposMaximos = 500;

        private readonly ActividadRepository _actividades;
        private readonly TipoActividadRepository _tipos;

        public ActividadService()
        {
            _actividades = new ActividadRepository();
            _tipos = new TipoActividadRepository();
        }

        public List<Actividad> Listar(FiltroActividad filtro)
        {
            return _actividades.Listar(filtro);
        }

        public Actividad ObtenerPorId(int actividadId)
        {
            return actividadId <= 0 ? null : _actividades.ObtenerPorId(actividadId);
        }

        public ResumenActividades ObtenerResumen()
        {
            return _actividades.ObtenerResumen();
        }

        public List<TipoActividad> ListarTipos()
        {
            return _tipos.Listar();
        }

        public List<ReporteTipo> ReportePorTipo()
        {
            return _actividades.ReportePorTipo();
        }

        /// <summary>Actividades abiertas (Activa/Próxima) que aún no terminan, ordenadas por fecha.</summary>
        public List<Actividad> ListarProximas(int cantidad)
        {
            return _actividades.Listar(null)
                .Where(a => (a.Estado == EstadosActividad.Activa || a.Estado == EstadosActividad.Proxima)
                            && a.FechaFin >= DateTime.Today)
                .OrderBy(a => a.FechaInicio)
                .Take(cantidad)
                .ToList();
        }

        public ResultadoOperacion Guardar(Actividad actividad, Usuario usuarioActual)
        {
            if (usuarioActual == null || !usuarioActual.EsAdministrador)
            {
                return ResultadoOperacion.Error("Solo el administrador puede registrar o modificar actividades.");
            }
            if (actividad == null)
            {
                return ResultadoOperacion.Error("No se recibieron datos de la actividad.");
            }

            Normalizar(actividad);

            Actividad existente = null;
            if (!actividad.EsNueva)
            {
                existente = _actividades.ObtenerPorId(actividad.ActividadId);
                if (existente == null)
                {
                    return ResultadoOperacion.Error("La actividad que intenta modificar ya no existe.");
                }
            }

            List<string> errores = Validar(actividad, existente);
            if (errores.Count > 0)
            {
                return ResultadoOperacion.ErrorValidacion(errores);
            }

            if (actividad.EsNueva)
            {
                int id = _actividades.Insertar(actividad);
                var ok = ResultadoOperacion.Ok("La actividad \"" + actividad.Titulo + "\" se registró correctamente.");
                ok.IdGenerado = id;
                return ok;
            }

            _actividades.Actualizar(actividad);
            var actualizado = ResultadoOperacion.Ok("La actividad \"" + actividad.Titulo + "\" se actualizó correctamente.");
            actualizado.IdGenerado = actividad.ActividadId;
            return actualizado;
        }

        /// <summary>
        /// Valida campos obligatorios y reglas de negocio. Es pública para poder probarla de forma aislada.
        /// </summary>
        public List<string> Validar(Actividad a, Actividad existente)
        {
            var errores = new List<string>();
            DateTime hoy = DateTime.Today;

            // Campos obligatorios y longitudes (coinciden con el modelo de datos).
            if (string.IsNullOrWhiteSpace(a.Titulo))
                errores.Add("El título es obligatorio.");
            else if (a.Titulo.Length < 3 || a.Titulo.Length > 120)
                errores.Add("El título debe tener entre 3 y 120 caracteres.");

            if (string.IsNullOrWhiteSpace(a.Descripcion))
                errores.Add("La descripción es obligatoria.");
            else if (a.Descripcion.Length < 10 || a.Descripcion.Length > 1000)
                errores.Add("La descripción debe tener entre 10 y 1000 caracteres.");

            if (a.TipoId <= 0 || !_tipos.Existe(a.TipoId))
                errores.Add("Seleccione un tipo de actividad válido.");

            if (string.IsNullOrWhiteSpace(a.Modalidad) || !Modalidades.Todas.Contains(a.Modalidad))
                errores.Add("Seleccione una modalidad válida.");

            if (string.IsNullOrWhiteSpace(a.Lugar))
                errores.Add("El lugar es obligatorio (para actividades virtuales indique la plataforma).");
            else if (a.Lugar.Length > 150)
                errores.Add("El lugar no puede superar 150 caracteres.");

            if (string.IsNullOrWhiteSpace(a.Estado) || !EstadosActividad.Todos.Contains(a.Estado))
                errores.Add("Seleccione un estado válido.");

            // Fechas y horas.
            if (a.FechaInicio == DateTime.MinValue)
                errores.Add("La fecha de inicio es obligatoria.");
            if (a.FechaFin == DateTime.MinValue)
                errores.Add("La fecha de fin es obligatoria.");

            bool fechasCompletas = a.FechaInicio != DateTime.MinValue && a.FechaFin != DateTime.MinValue;
            if (fechasCompletas && a.FechaFin.Date < a.FechaInicio.Date)
                errores.Add("La fecha de fin no puede ser anterior a la fecha de inicio.");

            if (a.HoraInicio == TimeSpan.Zero && a.HoraFin == TimeSpan.Zero)
                errores.Add("Las horas de inicio y fin son obligatorias.");
            else if (a.HoraFin <= a.HoraInicio)
                errores.Add("La hora de fin debe ser posterior a la hora de inicio.");

            if (fechasCompletas && a.Estado == EstadosActividad.Finalizada && a.FechaFin.Date >= hoy)
                errores.Add("Solo puede marcar como 'Finalizada' una actividad cuya fecha de fin ya pasó.");

            if (fechasCompletas && existente == null && a.Estado != EstadosActividad.Finalizada
                && a.FechaInicio.Date < hoy)
                errores.Add("Una actividad nueva no puede iniciar en una fecha pasada.");

            // Cupos.
            if (a.Cupos < CuposMinimos || a.Cupos > CuposMaximos)
                errores.Add("Los cupos deben estar entre " + CuposMinimos + " y " + CuposMaximos + ".");
            else if (existente != null && a.Cupos < existente.Inscritos)
                errores.Add("No puede dejar menos cupos (" + a.Cupos + ") que estudiantes inscritos (" + existente.Inscritos + ").");

            // Duplicados.
            if (errores.Count == 0 && _actividades.ExisteDuplicado(a.Titulo, a.FechaInicio, a.ActividadId))
                errores.Add("Ya existe una actividad con el mismo título y la misma fecha de inicio.");

            return errores;
        }

        /// <summary>
        /// Si la actividad tiene inscripciones o reseñas se desactiva (estado Cancelada)
        /// para conservar el historial; si no tiene, se elimina físicamente.
        /// </summary>
        public ResultadoOperacion Eliminar(int actividadId, Usuario usuarioActual)
        {
            if (usuarioActual == null || !usuarioActual.EsAdministrador)
            {
                return ResultadoOperacion.Error("Solo el administrador puede eliminar actividades.");
            }

            var actividad = _actividades.ObtenerPorId(actividadId);
            if (actividad == null)
            {
                return ResultadoOperacion.Error("La actividad no existe o ya fue eliminada.");
            }

            if (_actividades.ContarDependencias(actividadId) > 0)
            {
                if (actividad.Estado == EstadosActividad.Cancelada)
                {
                    return ResultadoOperacion.Error("La actividad ya está cancelada y conserva inscripciones o reseñas, por eso no se elimina.");
                }
                _actividades.CambiarEstado(actividadId, EstadosActividad.Cancelada);
                return ResultadoOperacion.Ok("\"" + actividad.Titulo + "\" tiene inscripciones o reseñas: se desactivó (estado Cancelada) en lugar de eliminarse.");
            }

            _actividades.Eliminar(actividadId);
            return ResultadoOperacion.Ok("La actividad \"" + actividad.Titulo + "\" se eliminó.");
        }

        private static void Normalizar(Actividad a)
        {
            a.Titulo = (a.Titulo ?? "").Trim();
            a.Descripcion = (a.Descripcion ?? "").Trim();
            a.Lugar = (a.Lugar ?? "").Trim();
            a.Modalidad = (a.Modalidad ?? "").Trim();
            a.Estado = (a.Estado ?? "").Trim();
        }
    }
}
