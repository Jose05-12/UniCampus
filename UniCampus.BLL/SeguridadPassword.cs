using System.Security.Cryptography;

namespace UniCampus.BLL
{
    public static class SeguridadPassword
    {
        private const int Iteraciones = 10000;
        private const int TamanoHash = 32;
        private const int TamanoSalt = 16;

        public static byte[] GenerarSalt()
        {
            var salt = new byte[TamanoSalt];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static byte[] CalcularHash(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iteraciones, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(TamanoHash);
            }
        }

        public static bool Verificar(string password, byte[] salt, byte[] hashEsperado)
        {
            if (string.IsNullOrEmpty(password) || salt == null || hashEsperado == null) return false;

            byte[] calculado = CalcularHash(password, salt);
            if (calculado.Length != hashEsperado.Length) return false;

            int diferencia = 0;
            for (int i = 0; i < calculado.Length; i++)
            {
                diferencia |= calculado[i] ^ hashEsperado[i];
            }
            return diferencia == 0;
        }
    }
}
