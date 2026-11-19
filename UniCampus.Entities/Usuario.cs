namespace UniCampus.Entities
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Programa { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }

        public bool EsAdministrador
        {
            get { return Rol == Roles.Administrador; }
        }

        public bool EsEstudiante
        {
            get { return Rol == Roles.Estudiante; }
        }
    }
}
