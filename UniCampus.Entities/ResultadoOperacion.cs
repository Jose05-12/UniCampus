using System.Collections.Generic;

namespace UniCampus.Entities
{
    /// <summary>
    /// Respuesta que la capa de negocio devuelve a la presentación:
    /// indica si la operación fue exitosa y qué mensajes mostrar.
    /// </summary>
    public class ResultadoOperacion
    {
        public bool Exitoso { get; private set; }
        public string Mensaje { get; private set; }
        public List<string> Errores { get; private set; }
        public int IdGenerado { get; set; }

        private ResultadoOperacion()
        {
            Errores = new List<string>();
        }

        public static ResultadoOperacion Ok(string mensaje)
        {
            return new ResultadoOperacion { Exitoso = true, Mensaje = mensaje };
        }

        public static ResultadoOperacion Error(string mensaje)
        {
            var r = new ResultadoOperacion { Exitoso = false, Mensaje = mensaje };
            r.Errores.Add(mensaje);
            return r;
        }

        public static ResultadoOperacion ErrorValidacion(List<string> errores)
        {
            var r = new ResultadoOperacion
            {
                Exitoso = false,
                Mensaje = "Revise los datos del formulario."
            };
            r.Errores.AddRange(errores);
            return r;
        }
    }
}
