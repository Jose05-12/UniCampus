using System;

namespace UniCampus.Entities
{
    /// <summary>
    /// Representa una actividad académica. Solo transporta datos entre capas:
    /// no contiene controles de interfaz ni ejecuta SQL.
    /// </summary>
    public class Actividad
    {
        public int ActividadId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int TipoId { get; set; }
        public string TipoNombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Modalidad { get; set; }
        public string Lugar { get; set; }
        public int Cupos { get; set; }
        public string Estado { get; set; }

        // Datos calculados por la consulta (no se guardan en la tabla).
        public int Inscritos { get; set; }
        public decimal? PromedioCalificacion { get; set; }
        public int TotalResenas { get; set; }

        public int CuposDisponibles
        {
            get { return Math.Max(0, Cupos - Inscritos); }
        }

        public bool EsNueva
        {
            get { return ActividadId == 0; }
        }

        public string FechasTexto
        {
            get
            {
                return FechaInicio.Date == FechaFin.Date
                    ? FechaInicio.ToString("dd/MM/yyyy")
                    : FechaInicio.ToString("dd/MM/yyyy") + " - " + FechaFin.ToString("dd/MM/yyyy");
            }
        }

        public string HorarioTexto
        {
            get { return HoraInicio.ToString(@"hh\:mm") + " - " + HoraFin.ToString(@"hh\:mm"); }
        }

        public int PorcentajeOcupacion
        {
            get { return Cupos <= 0 ? 0 : (int)Math.Round(Inscritos * 100.0 / Cupos); }
        }
    }
}
