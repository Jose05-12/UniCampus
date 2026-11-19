using System;

namespace UniCampus.Entities
{
    public class Resena
    {
        public int ResenaId { get; set; }
        public int ActividadId { get; set; }
        public int UsuarioId { get; set; }
        public int Calificacion { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaCreacion { get; set; }

        public string NombreUsuario { get; set; }

        public string Estrellas
        {
            get
            {
                int valor = Math.Max(0, Math.Min(5, Calificacion));
                return new string('★', valor) + new string('☆', 5 - valor);
            }
        }
    }
}
