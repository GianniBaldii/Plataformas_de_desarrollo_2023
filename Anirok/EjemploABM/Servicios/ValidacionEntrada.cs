using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace EjemploABM.Servicios
{
    internal static class ValidacionEntrada
    {
        public static string Texto(string valor)
        {
            return (valor ?? string.Empty).Trim();
        }

        public static bool Requerido(string valor)
        {
            return !string.IsNullOrWhiteSpace(valor);
        }

        public static bool Nombre(string valor)
        {
            string texto = Texto(valor);
            return texto.Length >= 2 && texto.Length <= 100 && Regex.IsMatch(texto, @"^[\p{L}\p{N} .,'-]+$");
        }

        public static bool Email(string valor)
        {
            try
            {
                string texto = Texto(valor);
                if (texto.Length > 150) return false;
                MailAddress direccion = new MailAddress(texto);
                return string.Equals(direccion.Address, texto, StringComparison.OrdinalIgnoreCase);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool SoloDigitos(string valor, int minimo, int maximo)
        {
            string texto = Texto(valor);
            return texto.Length >= minimo && texto.Length <= maximo && texto.All(char.IsDigit);
        }

        public static bool DecimalPositivo(string valor, out decimal resultado)
        {
            return decimal.TryParse(Texto(valor), NumberStyles.Number, CultureInfo.CurrentCulture, out resultado) && resultado > 0;
        }

        public static bool EnteroNoNegativo(string valor, out int resultado)
        {
            return int.TryParse(Texto(valor), NumberStyles.Integer, CultureInfo.CurrentCulture, out resultado) && resultado >= 0;
        }

        public static string LimpiarError(string detalle)
        {
            if (string.IsNullOrWhiteSpace(detalle)) return "Ocurrió un error inesperado.";
            return detalle.Length > 300 ? detalle.Substring(0, 300) + "..." : detalle;
        }
    }
}
