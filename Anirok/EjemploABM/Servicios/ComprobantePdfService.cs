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
            var connection = DB_Controller.connection;
            if (connection == null)
                throw new InvalidOperationException("La conexión a la base no fue inicializada.");

            if (connection.State != System.Data.ConnectionState.Closed)
                connection.Close();

            connection.Open();
            try
            {
                DateTime fecha;
                string cliente;
                string metodoPago;
                double total;
                double descuento;

                using (var command = new SqlCommand(
                    "SELECT v.fecha, c.nombre + ' ' + c.apellido, v.metodo_de_pago, v.precio_total, v.descuento " +
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
                        descuento = Convert.ToDouble(reader.GetValue(4));
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
                    BaseColor rojoAnirok = new BaseColor(198, 0, 0);
                    BaseColor grisClaro = new BaseColor(245, 245, 245);
                    Font fuenteNormal = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.DARK_GRAY);
                    Font fuenteEtiqueta = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.DARK_GRAY);
                    Font fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.WHITE);

                    var encabezado = new PdfPTable(new float[] { 1.1f, 3f }) { WidthPercentage = 100 };
                    encabezado.SetWidths(new float[] { 1.1f, 3f });
                    string logo = ResolverLogo();
                    if (File.Exists(logo))
                    {
                        var imagenLogo = iTextSharp.text.Image.GetInstance(logo);
                        imagenLogo.ScaleToFit(120f, 65f);
                        var celdaLogo = new PdfPCell(imagenLogo) { Border = Rectangle.NO_BORDER, Padding = 8, HorizontalAlignment = Element.ALIGN_LEFT };
                        encabezado.AddCell(celdaLogo);
                    }
                    else
                        encabezado.AddCell(new PdfPCell(new Phrase("ANIROK", fuenteTitulo)) { Border = Rectangle.NO_BORDER, Padding = 12 });
                    var bloqueTitulo = new PdfPCell(new Phrase("COMPROBANTE DE VENTA\nVenta #" + ventaId, fuenteTitulo))
                    {
                        BackgroundColor = rojoAnirok, Border = Rectangle.NO_BORDER, Padding = 14, VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    encabezado.AddCell(bloqueTitulo);
                    document.Add(encabezado);
                    document.Add(new Paragraph(" "));

                    var datos = new PdfPTable(2) { WidthPercentage = 100 };
                    datos.AddCell(CeldaDato("Fecha", fecha.ToString("dd/MM/yyyy HH:mm"), fuenteEtiqueta, fuenteNormal, grisClaro));
                    datos.AddCell(CeldaDato("Método de pago", metodoPago, fuenteEtiqueta, fuenteNormal, grisClaro));
                    datos.AddCell(CeldaDato("Cliente", cliente, fuenteEtiqueta, fuenteNormal, BaseColor.WHITE));
                    datos.AddCell(CeldaDato("N° de comprobante", ventaId.ToString("000000"), fuenteEtiqueta, fuenteNormal, BaseColor.WHITE));
                    document.Add(datos);
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
                                AgregarCelda(tabla, reader.GetString(0), Element.ALIGN_LEFT, fuenteNormal);
                                AgregarCelda(tabla, cantidad.ToString(), Element.ALIGN_CENTER, fuenteNormal);
                                AgregarCelda(tabla, precio.ToString("C", Cultura), Element.ALIGN_RIGHT, fuenteNormal);
                                AgregarCelda(tabla, (cantidad * precio).ToString("C", Cultura), Element.ALIGN_RIGHT, fuenteNormal);
                            }
                        }
                    }

                    document.Add(tabla);
                    document.Add(new Paragraph(" "));
                    var resumen = new PdfPTable(new float[] { 2.5f, 1.5f }) { WidthPercentage = 42, HorizontalAlignment = Element.ALIGN_RIGHT };
                    AgregarResumen(resumen, "Subtotal", (total + descuento).ToString("C", Cultura), fuenteNormal, grisClaro);
                    AgregarResumen(resumen, "Descuento", descuento.ToString("C", Cultura), fuenteNormal, BaseColor.WHITE);
                    AgregarResumen(resumen, "TOTAL", total.ToString("C", Cultura), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, rojoAnirok), new BaseColor(255, 235, 235));
                    document.Add(resumen);
                    document.Add(new Paragraph(" "));
                    var pie = new Paragraph("Gracias por tu compra · Anirok Mangas & More", FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 9, BaseColor.GRAY));
                    pie.Alignment = Element.ALIGN_CENTER;
                    document.Add(pie);
                    document.Close();
                }

                return ruta;
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    connection.Close();
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

        private static PdfPCell CeldaDato(string etiqueta, string valor, Font fuenteEtiqueta, Font fuenteNormal, BaseColor fondo)
        {
            var celda = new PdfPCell { BackgroundColor = fondo, BorderColor = new BaseColor(225, 225, 225), Padding = 7 };
            celda.AddElement(new Phrase(etiqueta + ": ", fuenteEtiqueta));
            celda.AddElement(new Phrase(valor, fuenteNormal));
            return celda;
        }

        private static void AgregarCelda(PdfPTable tabla, string texto, int alineacion, Font fuente)
        {
            tabla.AddCell(new PdfPCell(new Phrase(texto, fuente))
            {
                HorizontalAlignment = alineacion,
                Padding = 7,
                BorderColor = new BaseColor(225, 225, 225)
            });
        }

        private static void AgregarResumen(PdfPTable tabla, string etiqueta, string valor, Font fuente, BaseColor fondo)
        {
            tabla.AddCell(new PdfPCell(new Phrase(etiqueta, fuente)) { BackgroundColor = fondo, BorderColor = BaseColor.WHITE, Padding = 7, HorizontalAlignment = Element.ALIGN_RIGHT });
            tabla.AddCell(new PdfPCell(new Phrase(valor, fuente)) { BackgroundColor = fondo, BorderColor = BaseColor.WHITE, Padding = 7, HorizontalAlignment = Element.ALIGN_RIGHT });
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

        private static string ResolverLogo()
        {
            string[] candidatos =
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos", "logo.png"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Recursos", "logo.png")
            };

            foreach (string candidato in candidatos)
                if (File.Exists(candidato)) return Path.GetFullPath(candidato);

            foreach (string inicio in new[] { AppDomain.CurrentDomain.BaseDirectory, Environment.CurrentDirectory })
            {
                var carpeta = new DirectoryInfo(inicio);
                while (carpeta != null)
                {
                    string candidato = Path.Combine(carpeta.FullName, "Anirok", "EjemploABM", "Recursos", "logo.png");
                    if (File.Exists(candidato)) return candidato;
                    carpeta = carpeta.Parent;
                }
            }
            return string.Empty;
        }
    }
}
