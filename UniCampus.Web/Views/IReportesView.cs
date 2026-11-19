using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IReportesView
    {
        Usuario UsuarioActual { get; }

        void MostrarResumen(ResumenActividades resumen);
        void MostrarActividades(IList<Actividad> actividades);
        void MostrarPorTipo(IList<ReporteTipo> tipos);
        void MostrarAccesoDenegado();
        void MostrarError(string mensaje);
    }
}
