namespace UniCampus.Entities
{
    public class UsuarioCredencial
    {
        public Usuario Usuario { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
