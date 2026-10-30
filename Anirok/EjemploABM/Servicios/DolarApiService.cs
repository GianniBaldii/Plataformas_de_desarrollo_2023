using EjemploABM.Modelo;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EjemploABM.Servicios
{
    internal static class DolarApiService
    {
        private const string Endpoint = "https://dolarapi.com/v1/dolares/oficial";

        public static CotizacionDolar ObtenerOficial()
        {
            using (var cliente = new HttpClient())
            {
                cliente.Timeout = TimeSpan.FromSeconds(5);
                string json = cliente.GetStringAsync(Endpoint).GetAwaiter().GetResult();
                return Parsear(json);
            }
        }

        private static CotizacionDolar Parsear(string json)
        {
            decimal compra = LeerNumero(json, "compra", "buy");
            decimal venta = LeerNumero(json, "venta", "sell");
            if (venta <= 0) throw new InvalidOperationException("La API no devolvió una cotización de venta válida.");
            return new CotizacionDolar
            {
                Compra = compra,
                Venta = venta,
                Actualizada = DateTime.Now,
                Fuente = Endpoint
            };
        }

        private static decimal LeerNumero(string json, string propiedad, string propiedadAlternativa)
        {
            string patron = "\\\"(?:" + propiedad + "|" + propiedadAlternativa + ")\\\"\\s*:\\s*([0-9]+(?:\\.[0-9]+)?)";
            Match coincidencia = Regex.Match(json ?? string.Empty, patron, RegexOptions.IgnoreCase);
            decimal valor;
            if (!coincidencia.Success || !decimal.TryParse(coincidencia.Groups[1].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valor))
                return 0;
            return valor;
        }
    }
}
