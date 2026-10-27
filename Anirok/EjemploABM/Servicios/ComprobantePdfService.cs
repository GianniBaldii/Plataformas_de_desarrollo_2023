using EjemploABM.Controladores;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace EjemploABM.Servicios
{
    public static class ComprobantePdfService
    {
        private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("es-AR");
        private static readonly string rutaBase = ResolverRutaBase();

        public static string RutaBase
        {
            get
            {
                Directory.CreateDirectory(rutaBase);
                return rutaBase;
            }
        }

        public static string GenerarDesdeVenta(int ventaId)
        {
            using (var connection = new SqlConnection(DB_Controller.connection.ConnectionString))
            {
                connection.Open();
                DateTime fecha;
                string cliente;
                string metodoPago;
                double total;

                using (var command = new SqlCommand(
                    "SELECT v.fecha, c.nombre + ' ' + c.apellido, v.metodo_de_pago, v.precio_total " +
                    "FROM dbo.venta v INNER JOIN dbo.cliente c ON c.id = v.cliente_id WHERE v.id = @venta", connection))
                {
                    command.Parameters.AddWithValue("@venta", ventaId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read()) throw new InvalidOperationException("No se encontró la venta para generar el comprobante.");
                        fecha = reader.GetDateTime(0);
                        cliente = reader.GetString(1);
                        metodoPago = reader.GetString(2);
                        total = Convert.ToDouble(reader.GetValue(3));
                    }
                }

                string carpeta = Path.Combine(RutaBase, fecha.ToString("yyyy"), fecha.ToString("MM"));
                Directory.CreateDirectory(carpeta);
                string ruta = Path.Combine(carpeta, "venta-" + ventaId + ".pdf");

                using (var stream = new FileStream(ruta, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var document = new Document(PageSize.A4, 45, 45, 45, 45))
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    document.Add(new Paragraph("ANIROK - COMPROBANTE DE COMPRA", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18)));
                    document.Add(new Paragraph("Venta #" + ventaId));
                    document.Add(new Paragraph("Fecha: " + fecha.ToString("dd/MM/yyyy HH:mm")));
                    document.Add(new Paragraph("Cliente: " + cliente));
                    document.Add(new Paragraph("Método de pago: " + metodoPago));
                    document.Add(new Paragraph(" "));

                    var tabla = new PdfPTable(new float[] { 4, 1, 2, 2 }) { WidthPercentage = 100 };
                    AgregarEncabezado(tabla, "Producto");
                    AgregarEncabezado(tabla, "Cant.");
                    AgregarEncabezado(tabla, "Precio unitario");
                    AgregarEncabezado(tabla, "Subtotal");

                    using (var command = new SqlCommand(
                        "SELECT p.nombre, d.cantidad, d.precio_unitario " +
                        "FROM dbo.detalle_venta d INNER JOIN dbo.producto p ON p.id = d.producto_id " +
                        "WHERE d.venta_id = @venta ORDER BY d.id", connection))
                    {
                        command.Parameters.AddWithValue("@venta", ventaId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int cantidad = reader.GetInt32(1);
                                double precio = Convert.ToDouble(reader.GetValue(2));
                                tabla.AddCell(reader.GetString(0));
                                tabla.AddCell(cantidad.ToString());
                                tabla.AddCell(precio.ToString("C", Cultura));
                                tabla.AddCell((cantidad * precio).ToString("C", Cultura));
                            }
                        }
                    }

                    document.Add(tabla);
                    document.Add(new Paragraph(" "));
                    document.Add(new Paragraph("TOTAL: " + total.ToString("C", Cultura), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14)));
                    document.Close();
                }

                return ruta;
            }
        }

        private static void AgregarEncabezado(PdfPTable tabla, string texto)
        {
            tabla.AddCell(new PdfPCell(new Phrase(texto, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
            {
                BackgroundColor = new BaseColor(225, 225, 225),
                Padding = 6
            });
        }

        private static string ResolverRutaBase()
        {
            string configurada = Environment.GetEnvironmentVariable("ANIROK_COMPROBANTES_PATH");
            if (!string.IsNullOrWhiteSpace(configurada)) return Path.GetFullPath(configurada);

            foreach (string inicio in new[] { AppDomain.CurrentDomain.BaseDirectory, Environment.CurrentDirectory })
            {
                var carpeta = new DirectoryInfo(inicio);
                while (carpeta != null)
                {
                    if (File.Exists(Path.Combine(carpeta.FullName, "docker-compose.yml")))
                        return Path.Combine(carpeta.FullName, "storage", "comprobantes");
                    carpeta = carpeta.Parent;
                }
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Anirok", "storage", "comprobantes");
        }
    }
}
