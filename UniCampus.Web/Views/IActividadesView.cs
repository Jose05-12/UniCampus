using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IActividadesView
    {
        Usuario UsuarioActual { get; }

        int ActividadEnEdicionId { get; set; }
        string Titulo { get; set; }
        string TipoId { get; set; }
        string Descripcion { get; set; }
        string FechaInicio { get; set; }
        string FechaFin { get; set; }
        string HoraInicio { get; set; }
        string HoraFin { get; set; }
        string Modalidad { get; set; }
        string Lugar { get; set; }
        string Cupos { get; set; }
        string Estado { get; set; }

        string FiltroTexto { get; }
        string FiltroTipoId { get; }
        string FiltroEstado { get; }
        string FiltroFecha { get; }

        int ActividadSeleccionadaId { get; set; }
        string EstudianteAInscribirId { get; }
        string ResenaCalificacion { get; }
        string ResenaComentario { get; }

        void CargarCatalogos(IList<TipoActividad> tipos, IList<string> modalidades, IList<string> estados);
        void ConfigurarPermisos(bool esAdministrador);
        void MostrarModoEdicion(bool editando);
        void MostrarFormulario(bool visible);

        void MostrarResumen(ResumenActividades resumen);
        void MostrarActividades(IList<Actividad> actividades);
        void MostrarDetalle(Actividad actividad);
        void MostrarInscritos(IList<Inscripcion> inscritos, bool puedeGestionar);
        void MostrarEstudiantesDisponibles(IList<Usuario> estudiantes);
        void MostrarVistaEstudiante(Actividad actividad, bool puedeInscribirse, string estadoInscripcion);
        void MostrarResenas(IList<Resena> resenas, Actividad actividad, string motivoNoPuedeResenar, bool puedeModerar);
        void LimpiarResena();

        void MostrarExito(string mensaje);
        void MostrarErrores(string titulo, IList<string> errores);
    }
}
