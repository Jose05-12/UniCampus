namespace UniCampus.Entities
{
    /// <summary>
    /// Usuario + datos de autenticación. Solo se usa entre DAL y BLL
    /// para verificar la contraseña; nunca se envía a la vista.
    /// </summary>
    public class UsuarioCredencial
    {
        public Usuario Usuario { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
