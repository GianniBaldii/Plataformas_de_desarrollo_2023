using System;
using System.Security.Cryptography;
using System.Text;

namespace AnirokWeb.Services
{
    /// <summary>Compatibilidad con las contraseñas hashadas por la aplicación Desktop.</summary>
    internal static class PasswordService
    {
        private const string Prefix = "sha256$";

        public static string Hash(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? string.Empty));
                return Prefix + BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored)) return false;
            // Se mantiene compatibilidad temporal con registros antiguos en texto plano.
            return stored.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase)
                ? string.Equals(Hash(password), stored, StringComparison.OrdinalIgnoreCase)
                : string.Equals(password, stored, StringComparison.Ordinal);
        }
    }
}
