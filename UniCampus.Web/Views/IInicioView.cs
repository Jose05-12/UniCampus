using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IInicioView
    {
        Usuario UsuarioActual { get; }

        void MostrarResumen(ResumenActividades resumen);
        void MostrarProximas(IList<Actividad> actividades);
        void MostrarMisInscripciones(IList<Inscripcion> inscripciones);
        void MostrarError(string mensaje);
    }
}
