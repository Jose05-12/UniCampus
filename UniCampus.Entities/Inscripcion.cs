using System;

namespace UniCampus.Entities
{
    public class Inscripcion
    {
        public int InscripcionId { get; set; }
        public int ActividadId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public string Estado { get; set; }

        // Datos de apoyo para mostrar en pantalla (vienen de JOIN).
        public string NombreEstudiante { get; set; }
        public string Programa { get; set; }
        public string TituloActividad { get; set; }
        public string TipoActividad { get; set; }
        public DateTime FechaInicioActividad { get; set; }
        public DateTime FechaFinActividad { get; set; }
        public TimeSpan HoraInicioActividad { get; set; }
        public TimeSpan HoraFinActividad { get; set; }
        public string LugarActividad { get; set; }
        public string EstadoActividad { get; set; }
    }
}
