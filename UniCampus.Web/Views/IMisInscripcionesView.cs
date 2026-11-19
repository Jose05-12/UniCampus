using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IMisInscripcionesView
    {
        Usuario UsuarioActual { get; }

        void MostrarInscripciones(IList<Inscripcion> inscripciones);
        void MostrarResenas(IList<Resena> resenas);
        void MostrarSoloEstudiantes();
        void MostrarExito(string mensaje);
        void MostrarErrores(string titulo, IList<string> errores);
    }
}
