using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AnirokWeb.Services
{
    public static class ProductoImagenService
    {
        private static readonly HashSet<string> ExtensionesPermitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png"
        };

        private static readonly string rutaBase = ResolverRutaBase();

        public static bool IntentarObtener(string nombreArchivo, out string ruta, out string tipoContenido)
        {
            ruta = null;
            tipoContenido = null;
            if (string.IsNullOrWhiteSpace(nombreArchivo)) return false;

            string nombreSeguro = Path.GetFileName(nombreArchivo);
            string extension = Path.GetExtension(nombreSeguro);
            if (!string.Equals(nombreSeguro, nombreArchivo, StringComparison.Ordinal) || !ExtensionesPermitidas.Contains(extension))
                return false;

            string candidata = Path.Combine(rutaBase, nombreSeguro);
            if (!File.Exists(candidata)) return false;

            ruta = candidata;
            tipoContenido = extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" : "image/jpeg";
            return true;
        }

        public static byte[] CrearPlaceholder()
        {
            const string svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"640\" height=\"640\" viewBox=\"0 0 640 640\"><rect width=\"640\" height=\"640\" fill=\"#eeeeee\"/><text x=\"320\" y=\"320\" dominant-baseline=\"middle\" text-anchor=\"middle\" font-family=\"Arial,sans-serif\" font-size=\"28\" font-weight=\"bold\" fill=\"#666666\">Imagen no disponible</text></svg>";
            return Encoding.UTF8.GetBytes(svg);
        }

        private static string ResolverRutaBase()
        {
            string configurada = Environment.GetEnvironmentVariable("ANIROK_STORAGE_PATH");
            if (!string.IsNullOrWhiteSpace(configurada)) return Path.GetFullPath(configurada);

            foreach (string inicio in new[] { AppDomain.CurrentDomain.BaseDirectory, Environment.CurrentDirectory })
            {
                var carpeta = new DirectoryInfo(inicio);
                while (carpeta != null)
                {
                    if (File.Exists(Path.Combine(carpeta.FullName, "docker-compose.yml")))
                        return Path.Combine(carpeta.FullName, "storage", "productos");
                    carpeta = carpeta.Parent;
                }
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Anirok", "storage", "productos");
        }
    }
}
