using System;

namespace UniCampus.Entities
{
    public class FiltroActividad
    {
        public string Texto { get; set; }
        public int? TipoId { get; set; }
        public string Estado { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
