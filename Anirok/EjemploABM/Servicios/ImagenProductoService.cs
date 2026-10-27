using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace EjemploABM.Servicios
{
    public static class ImagenProductoService
    {
        private const long MaximoBytes = 5 * 1024 * 1024;
        private static readonly HashSet<string> ExtensionesPermitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png"
        };

        private static readonly string rutaBase = ResolverRutaBase();

        public static string RutaBase
        {
            get
            {
                Directory.CreateDirectory(rutaBase);
                return rutaBase;
            }
        }

        public static string Importar(string rutaOrigen, int productoId)
        {
            ValidarArchivo(rutaOrigen);
            Directory.CreateDirectory(RutaBase);
            string extension = Path.GetExtension(rutaOrigen).ToLowerInvariant();
            string nombre = string.Format("producto-{0}-{1}{2}", productoId, Guid.NewGuid().ToString("N").Substring(0, 8), extension);
            File.Copy(rutaOrigen, ObtenerRuta(nombre), false);
            return nombre;
        }

        public static Image Cargar(string nombreArchivo)
        {
            string ruta = ObtenerRuta(nombreArchivo);
            if (!File.Exists(ruta)) return CrearPlaceholder();

            try
            {
                using (var temporal = Image.FromFile(ruta))
                {
                    return new Bitmap(temporal);
                }
            }
            catch
            {
                return CrearPlaceholder();
            }
        }

        public static Image CargarDesdeRuta(string ruta)
        {
            ValidarArchivo(ruta);
            using (var temporal = Image.FromFile(ruta))
            {
                return new Bitmap(temporal);
            }
        }

        public static void Eliminar(string nombreArchivo)
        {
            if (string.IsNullOrWhiteSpace(nombreArchivo)) return;
            string ruta = ObtenerRuta(nombreArchivo);
            if (File.Exists(ruta)) File.Delete(ruta);
        }

        public static string ObtenerRuta(string nombreArchivo)
        {
            if (string.IsNullOrWhiteSpace(nombreArchivo)) return Path.Combine(RutaBase, "imagen-no-disponible");
            string nombreSeguro = Path.GetFileName(nombreArchivo);
            if (!string.Equals(nombreSeguro, nombreArchivo, StringComparison.Ordinal))
                throw new InvalidOperationException("El nombre de la imagen no es válido.");
            return Path.Combine(RutaBase, nombreSeguro);
        }

        private static void ValidarArchivo(string ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta) || !File.Exists(ruta))
                throw new FileNotFoundException("No se encontró la imagen seleccionada.");

            var archivo = new FileInfo(ruta);
            if (!ExtensionesPermitidas.Contains(archivo.Extension))
                throw new InvalidOperationException("La imagen debe ser JPG, JPEG o PNG.");
            if (archivo.Length == 0 || archivo.Length > MaximoBytes)
                throw new InvalidOperationException("La imagen debe pesar menos de 5 MB.");

            try
            {
                using (var imagen = Image.FromFile(ruta))
                {
                    if (imagen.Width < 1 || imagen.Height < 1) throw new InvalidOperationException();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("El archivo seleccionado no es una imagen válida.", ex);
            }
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

        private static Image CrearPlaceholder()
        {
            var imagen = new Bitmap(640, 640);
            using (Graphics graphics = Graphics.FromImage(imagen))
            using (var fondo = new SolidBrush(Color.FromArgb(238, 238, 238)))
            using (var texto = new SolidBrush(Color.FromArgb(100, 100, 100)))
            using (var fuente = new Font("Segoe UI", 24, FontStyle.Bold))
            {
                graphics.FillRectangle(fondo, 0, 0, imagen.Width, imagen.Height);
                const string mensaje = "Imagen no disponible";
                SizeF medida = graphics.MeasureString(mensaje, fuente);
                graphics.DrawString(mensaje, fuente, texto, (imagen.Width - medida.Width) / 2, (imagen.Height - medida.Height) / 2);
            }
            return imagen;
        }
    }
}
