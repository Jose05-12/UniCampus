using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    public class UsuarioService
    {
        private const string MensajeCredencialesInvalidas = "Correo o contraseña incorrectos.";

        private readonly UsuarioRepository _usuarios;

        public UsuarioService()
        {
            _usuarios = new UsuarioRepository();
        }

        /// <summary>
        /// Verifica las credenciales. Devuelve el mismo mensaje para correo inexistente o
        /// contraseña errónea, así no se revela qué correos están registrados.
        /// </summary>
        public ResultadoOperacion IniciarSesion(string correo, string password, out Usuario usuario)
        {
            usuario = null;
            correo = (correo ?? "").Trim().ToLowerInvariant();

            if (correo.Length == 0 || string.IsNullOrEmpty(password))
                return ResultadoOperacion.Error("Ingrese su correo y su contraseña.");

            if (correo.Length > 150 || !correo.Contains("@"))
                return ResultadoOperacion.Error("Ingrese un correo válido.");

            var credencial = _usuarios.ObtenerCredencialPorCorreo(correo);
            if (credencial == null || !SeguridadPassword.Verificar(password, credencial.PasswordSalt, credencial.PasswordHash))
                return ResultadoOperacion.Error(MensajeCredencialesInvalidas);

            if (!credencial.Usuario.Activo)
                return ResultadoOperacion.Error("Su usuario está inactivo. Comuníquese con la coordinación.");

            usuario = credencial.Usuario;
            return ResultadoOperacion.Ok("Bienvenido(a), " + usuario.Nombre + ".");
        }

        public Usuario ObtenerPorId(int usuarioId)
        {
            return _usuarios.ObtenerPorId(usuarioId);
        }
    }
}
