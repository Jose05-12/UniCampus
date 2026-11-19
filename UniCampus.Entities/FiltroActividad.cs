using System;

namespace UniCampus.Entities
{
    /// <summary>
    /// Criterios de búsqueda del listado. Un valor null significa "sin filtro".
    /// </summary>
    public class FiltroActividad
    {
        public string Texto { get; set; }
        public int? TipoId { get; set; }
        public string Estado { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
