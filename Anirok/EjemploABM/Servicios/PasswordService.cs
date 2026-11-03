using System;
using System.Security.Cryptography;
using System.Text;

namespace EjemploABM.Servicios
{
    internal static class PasswordService
    {
        private const string Prefijo = "sha256$";
        public static string Hash(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? string.Empty));
                return Prefijo + BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
        }
        public static bool Verificar(string password, string almacenada)
        {
            if (string.IsNullOrEmpty(almacenada)) return false;
            return almacenada.StartsWith(Prefijo, StringComparison.OrdinalIgnoreCase) ?
                string.Equals(Hash(password), almacenada, StringComparison.OrdinalIgnoreCase) :
                string.Equals(password, almacenada, StringComparison.Ordinal);
        }
        public static string GenerarTemporal()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[9]; rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes).Replace("+", "A").Replace("/", "B").Replace("=", "9").Substring(0, 12);
            }
        }
    }
}
