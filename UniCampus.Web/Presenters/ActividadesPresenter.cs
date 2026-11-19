using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Infraestructura;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class ActividadesPresenter
    {
        private const string MensajeErrorGeneral = "Ocurrió un error inesperado. Intente nuevamente o contacte al administrador.";

        private readonly IActividadesView _view;
        private readonly ActividadService _actividades;
        private readonly InscripcionService _inscripciones;
        private readonly ResenaService _resenas;

        public ActividadesPresenter(IActividadesView view)
        {
            _view = view;
            _actividades = new ActividadService();
            _inscripciones = new InscripcionService();
            _resenas = new ResenaService();
        }

        private Usuario Usuario
        {
            get { return _view.UsuarioActual; }
        }

        public void Inicializar()
        {
            Ejecutar("cargar la página", () =>
            {
                _view.CargarCatalogos(_actividades.ListarTipos(), Modalidades.Todas, EstadosActividad.Todos);
                _view.ConfigurarPermisos(Usuario.EsAdministrador);
                LimpiarFormulario();
                _view.MostrarFormulario(false);
                CargarListado();
            });
        }

        public void Buscar()
        {
            Ejecutar("buscar actividades", CargarListado);
        }

        private void CargarListado()
        {
            _view.MostrarResumen(_actividades.ObtenerResumen());

            var actividades = _actividades.Listar(ConstruirFiltro());

            if (_view.ActividadSeleccionadaId == 0 && actividades.Count > 0)
            {
                _view.ActividadSeleccionadaId = actividades[0].ActividadId;
            }
            _view.MostrarActividades(actividades);
            CargarSeleccion();
        }

        private void CargarSeleccion()
        {
            var actividad = _actividades.ObtenerPorId(_view.ActividadSeleccionadaId);
            if (actividad == null)
            {
                _view.ActividadSeleccionadaId = 0;
            }

            _view.MostrarDetalle(actividad);

            if (actividad == null)
            {
                _view.MostrarInscritos(new List<Inscripcion>(), false);
                _view.MostrarEstudiantesDisponibles(new List<Usuario>());
                _view.MostrarVistaEstudiante(null, false, null);
                _view.MostrarResenas(new List<Resena>(), null, null, false);
                return;
            }

            bool esAdmin = Usuario.EsAdministrador;
            _view.MostrarInscritos(_inscripciones.ListarPorActividad(actividad.ActividadId), esAdmin);
            _view.MostrarEstudiantesDisponibles(esAdmin
                ? _inscripciones.ListarEstudiantesDisponibles(actividad.ActividadId)
                : new List<Usuario>());

            bool puedeInscribirse = _inscripciones.PuedeInscribirse(actividad, Usuario);
            _view.MostrarVistaEstudiante(actividad, puedeInscribirse, DescribirEstadoInscripcion(actividad, puedeInscribirse));

            _view.MostrarResenas(_resenas.ListarPorActividad(actividad.ActividadId), actividad,
                                 _resenas.MotivoNoPuedeResenar(actividad, Usuario), esAdmin);
        }

        private string DescribirEstadoInscripcion(Actividad actividad, bool puedeInscribirse)
        {
            if (puedeInscribirse) return null;
            if (Usuario.EsAdministrador) return "Vista previa: así verá el estudiante esta actividad.";
            if (_inscripciones.EstaInscrito(actividad.ActividadId, Usuario.UsuarioId)) return "Ya estás inscrito(a) en esta actividad.";
            if (!InscripcionService.AdmiteInscripciones(actividad)) return "Las inscripciones para esta actividad están cerradas.";
            if (actividad.CuposDisponibles <= 0) return "No quedan cupos disponibles.";
            return "No es posible inscribirse en esta actividad.";
        }

        public void Guardar()
        {
            Ejecutar("guardar la actividad", () =>
            {
                var conversion = new List<string>();
                var actividad = LeerFormulario(conversion);
                if (conversion.Count > 0)
                {
                    _view.MostrarErrores("Revise el formato de los datos.", conversion);
                    return;
                }

                var resultado = _actividades.Guardar(actividad, Usuario);
                if (!resultado.Exitoso)
                {
                    _view.MostrarErrores(resultado.Mensaje, resultado.Errores);
                    return;
                }

                _view.MostrarExito(resultado.Mensaje);
                _view.ActividadSeleccionadaId = resultado.IdGenerado;
                LimpiarFormulario();
                _view.MostrarFormulario(false);
                CargarListado();
            });
        }

        public void Limpiar()
        {
            int enEdicion = _view.ActividadEnEdicionId;
            LimpiarFormulario();
            _view.ActividadEnEdicionId = enEdicion;
            _view.MostrarModoEdicion(enEdicion > 0);
        }

        public void NuevaActividad()
        {
            if (!Usuario.EsAdministrador)
            {
                _view.MostrarErrores("Solo el administrador puede registrar actividades.", null);
                return;
            }
            LimpiarFormulario();
            _view.MostrarFormulario(true);
        }

        public void CerrarFormulario()
        {
            LimpiarFormulario();
            _view.MostrarFormulario(false);
        }

        public void Seleccionar(int actividadId)
        {
            Ejecutar("consultar la actividad", () =>
            {
                _view.ActividadSeleccionadaId = actividadId;
                CargarListado();
            });
        }

        public void Editar(int actividadId)
        {
            Ejecutar("cargar la actividad para edición", () =>
            {
                if (!Usuario.EsAdministrador)
                {
                    _view.MostrarErrores("Solo el administrador puede modificar actividades.", null);
                    return;
                }

                var a = _actividades.ObtenerPorId(actividadId);
                if (a == null)
                {
                    _view.MostrarErrores("La actividad ya no existe.", null);
                    CargarListado();
                    return;
                }

                _view.ActividadEnEdicionId = a.ActividadId;
                _view.Titulo = a.Titulo;
                _view.TipoId = a.TipoId.ToString(CultureInfo.InvariantCulture);
                _view.Descripcion = a.Descripcion;
                _view.FechaInicio = a.FechaInicio.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                _view.FechaFin = a.FechaFin.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                _view.HoraInicio = a.HoraInicio.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
                _view.HoraFin = a.HoraFin.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
                _view.Modalidad = a.Modalidad;
                _view.Lugar = a.Lugar;
                _view.Cupos = a.Cupos.ToString(CultureInfo.InvariantCulture);
                _view.Estado = a.Estado;
                _view.MostrarModoEdicion(true);
                _view.MostrarFormulario(true);

                _view.ActividadSeleccionadaId = a.ActividadId;
                CargarListado();
            });
        }

        public void Eliminar(int actividadId)
        {
            Ejecutar("eliminar la actividad", () =>
            {
                var resultado = _actividades.Eliminar(actividadId, Usuario);
                if (resultado.Exitoso)
                {
                    _view.MostrarExito(resultado.Mensaje);
                    if (_view.ActividadEnEdicionId == actividadId)
                    {
                        LimpiarFormulario();
                        _view.MostrarFormulario(false);
                    }
                }
                else
                {
                    _view.MostrarErrores(resultado.Mensaje, resultado.Errores);
                }
                CargarListado();
            });
        }

        public void InscribirEstudiante()
        {
            Ejecutar("inscribir al estudiante", () =>
            {
                int estudianteId;
                if (!int.TryParse(_view.EstudianteAInscribirId, out estudianteId))
                {
                    _view.MostrarErrores("Seleccione el estudiante que desea inscribir.", null);
                    return;
                }
                MostrarResultado(_inscripciones.Inscribir(_view.ActividadSeleccionadaId, estudianteId, Usuario));
            });
        }

        public void Inscribirme()
        {
            Ejecutar("realizar la inscripción", () =>
                MostrarResultado(_inscripciones.Inscribir(_view.ActividadSeleccionadaId, Usuario.UsuarioId, Usuario)));
        }

        public void CancelarInscripcion(int inscripcionId)
        {
            Ejecutar("cancelar la inscripción", () =>
                MostrarResultado(_inscripciones.Cancelar(inscripcionId, Usuario)));
        }

        public void PublicarResena()
        {
            Ejecutar("publicar la reseña", () =>
            {
                int calificacion;
                int.TryParse(_view.ResenaCalificacion, out calificacion);

                var resultado = _resenas.Publicar(_view.ActividadSeleccionadaId, calificacion, _view.ResenaComentario, Usuario);
                if (resultado.Exitoso) _view.LimpiarResena();
                MostrarResultado(resultado);
            });
        }

        public void EliminarResena(int resenaId)
        {
            Ejecutar("eliminar la reseña", () => MostrarResultado(_resenas.Eliminar(resenaId, Usuario)));
        }

        private void MostrarResultado(ResultadoOperacion resultado)
        {
            if (resultado.Exitoso)
                _view.MostrarExito(resultado.Mensaje);
            else
                _view.MostrarErrores(resultado.Mensaje, resultado.Errores);

            CargarListado();
        }

        private void LimpiarFormulario()
        {
            _view.ActividadEnEdicionId = 0;
            _view.Titulo = "";
            _view.TipoId = "";
            _view.Descripcion = "";
            _view.FechaInicio = "";
            _view.FechaFin = "";
            _view.HoraInicio = "";
            _view.HoraFin = "";
            _view.Modalidad = Modalidades.Presencial;
            _view.Lugar = "";
            _view.Cupos = "";
            _view.Estado = EstadosActividad.Proxima;
            _view.MostrarModoEdicion(false);
        }

        private Actividad LeerFormulario(List<string> errores)
        {
            var a = new Actividad
            {
                ActividadId = _view.ActividadEnEdicionId,
                Titulo = _view.Titulo,
                Descripcion = _view.Descripcion,
                Modalidad = _view.Modalidad,
                Lugar = _view.Lugar,
                Estado = _view.Estado
            };

            int numero;
            if (!string.IsNullOrWhiteSpace(_view.TipoId))
            {
                if (int.TryParse(_view.TipoId, out numero)) a.TipoId = numero;
                else errores.Add("El tipo de actividad no es válido.");
            }

            if (!string.IsNullOrWhiteSpace(_view.Cupos))
            {
                if (int.TryParse(_view.Cupos.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out numero)) a.Cupos = numero;
                else errores.Add("Los cupos deben ser un número entero.");
            }

            a.FechaInicio = LeerFecha(_view.FechaInicio, "fecha de inicio", errores);
            a.FechaFin = LeerFecha(_view.FechaFin, "fecha de fin", errores);
            a.HoraInicio = LeerHora(_view.HoraInicio, "hora de inicio", errores);
            a.HoraFin = LeerHora(_view.HoraFin, "hora de fin", errores);
            return a;
        }

        private static DateTime LeerFecha(string valor, string campo, List<string> errores)
        {
            if (string.IsNullOrWhiteSpace(valor)) return DateTime.MinValue;

            DateTime fecha;
            string[] formatos = { "yyyy-MM-dd", "dd/MM/yyyy" };
            if (DateTime.TryParseExact(valor.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                return fecha;

            errores.Add("La " + campo + " no tiene un formato válido.");
            return DateTime.MinValue;
        }

        private static TimeSpan LeerHora(string valor, string campo, List<string> errores)
        {
            if (string.IsNullOrWhiteSpace(valor)) return TimeSpan.Zero;

            TimeSpan hora;
            string[] formatos = { @"hh\:mm", @"hh\:mm\:ss", @"h\:mm" };
            if (TimeSpan.TryParseExact(valor.Trim(), formatos, CultureInfo.InvariantCulture, out hora)
                && hora >= TimeSpan.Zero && hora < TimeSpan.FromDays(1))
                return hora;

            errores.Add("La " + campo + " no tiene un formato válido (HH:mm).");
            return TimeSpan.Zero;
        }

        private FiltroActividad ConstruirFiltro()
        {
            var filtro = new FiltroActividad
            {
                Texto = string.IsNullOrWhiteSpace(_view.FiltroTexto) ? null : _view.FiltroTexto.Trim(),
                Estado = string.IsNullOrWhiteSpace(_view.FiltroEstado) ? null : _view.FiltroEstado
            };

            if (filtro.Texto != null && filtro.Texto.Length > 100)
                filtro.Texto = filtro.Texto.Substring(0, 100);

            if (filtro.Estado != null && !EstadosActividad.Todos.Contains(filtro.Estado))
                filtro.Estado = null;

            int tipoId;
            if (int.TryParse(_view.FiltroTipoId, out tipoId)) filtro.TipoId = tipoId;

            DateTime fecha;
            if (DateTime.TryParseExact(_view.FiltroFecha ?? "", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                filtro.Fecha = fecha;

            return filtro;
        }

        private void Ejecutar(string accion, Action operacion)
        {
            try
            {
                operacion();
            }
            catch (Exception ex)
            {
                RegistroErrores.Registrar(ex, "ActividadesPresenter: " + accion);
                _view.MostrarErrores("No fue posible " + accion + ".", new List<string> { MensajeErrorGeneral });
            }
        }
    }
}
