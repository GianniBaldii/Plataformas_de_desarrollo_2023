using AnirokWeb.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace AnirokWeb.Services
{
    public class CompraService
    {
        private readonly string _connectionString;

        public CompraService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CrearVenta(int usuarioId, IList<CarritoItem> carrito, string metodoPago)
        {
            if (carrito == null || carrito.Count == 0) throw new InvalidOperationException("El carrito está vacío.");
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var usuario = ObtenerUsuario(usuarioId, connection, transaction);
                        var clienteId = ObtenerOCrearCliente(usuario, connection, transaction);
                        var ventaId = SiguienteId("venta", connection, transaction);
                        var detalleId = SiguienteId("detalle_venta", connection, transaction);
                        var total = 0m;
                        var productos = new List<Tuple<int, int, decimal>>();

                        foreach (var item in carrito)
                        {
                            using (var command = new SqlCommand("SELECT nombre, precio, stock FROM producto WITH (UPDLOCK, ROWLOCK) WHERE id = @id", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", item.ProductoId);
                                using (var reader = command.ExecuteReader())
                                {
                                    if (!reader.Read()) throw new InvalidOperationException("Uno de los productos ya no existe.");
                                    var nombre = reader.GetString(0);
                                    var precio = Convert.ToDecimal(reader.GetValue(1));
                                    var stock = reader.GetInt32(2);
                                    if (item.Cantidad < 1 || stock < item.Cantidad) throw new InvalidOperationException("No hay stock suficiente de " + nombre + ".");
                                    productos.Add(Tuple.Create(item.ProductoId, item.Cantidad, precio));
                                    total += precio * item.Cantidad;
                                }
                            }
                        }

                        using (var command = new SqlCommand("INSERT INTO venta (id, precio_total, fecha, metodo_de_pago, descuento, cliente_id, usuario_id) VALUES (@id, @total, @fecha, @pago, 0, @cliente, @usuario)", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", ventaId);
                            command.Parameters.AddWithValue("@total", total);
                            command.Parameters.AddWithValue("@fecha", DateTime.Today);
                            command.Parameters.AddWithValue("@pago", metodoPago);
                            command.Parameters.AddWithValue("@cliente", clienteId);
                            command.Parameters.AddWithValue("@usuario", usuarioId);
                            command.ExecuteNonQuery();
                        }

                        foreach (var producto in productos)
                        {
                            using (var command = new SqlCommand("INSERT INTO detalle_venta (id, cantidad, venta_id, producto_id, precio_unitario) VALUES (@id, @cantidad, @venta, @producto, @precio); UPDATE producto SET stock = stock - @cantidad WHERE id = @producto", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", detalleId++);
                                command.Parameters.AddWithValue("@cantidad", producto.Item2);
                                command.Parameters.AddWithValue("@venta", ventaId);
                                command.Parameters.AddWithValue("@producto", producto.Item1);
                                command.Parameters.AddWithValue("@precio", producto.Item3);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return ventaId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public ComprobanteViewModel ObtenerComprobante(int ventaId, int usuarioId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                ComprobanteViewModel comprobante;
                using (var command = new SqlCommand("SELECT v.id, v.fecha, c.nombre + ' ' + c.apellido, c.mail, v.metodo_de_pago, v.precio_total FROM venta v INNER JOIN cliente c ON c.id = v.cliente_id WHERE v.id = @venta AND v.usuario_id = @usuario", connection))
                {
                    command.Parameters.AddWithValue("@venta", ventaId);
                    command.Parameters.AddWithValue("@usuario", usuarioId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        comprobante = new ComprobanteViewModel { VentaId = reader.GetInt32(0), Fecha = reader.GetDateTime(1), Cliente = reader.GetString(2), Email = reader.GetString(3), MetodoPago = reader.GetString(4), Total = Convert.ToDecimal(reader.GetValue(5)) };
                    }
                }
                using (var command = new SqlCommand("SELECT p.nombre, d.cantidad, d.precio_unitario FROM detalle_venta d INNER JOIN producto p ON p.id = d.producto_id WHERE d.venta_id = @venta ORDER BY d.id", connection))
                {
                    command.Parameters.AddWithValue("@venta", ventaId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read()) comprobante.Lineas.Add(new ComprobanteLineaViewModel { Producto = reader.GetString(0), Cantidad = reader.GetInt32(1), PrecioUnitario = Convert.ToDecimal(reader.GetValue(2)) });
                    }
                }
                return comprobante;
            }
        }

        public byte[] CrearPdf(ComprobanteViewModel comprobante)
        {
            using (var output = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 42, 42, 42, 42);
                PdfWriter.GetInstance(document, output);
                document.Open();
                var rojo = new BaseColor(198, 0, 0);
                var gris = new BaseColor(245, 245, 245);
                var borde = new BaseColor(225, 225, 225);
                var normal = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.DARK_GRAY);
                var etiqueta = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.DARK_GRAY);
                var titulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 17, BaseColor.WHITE);
                var encabezado = new PdfPTable(new float[] { 1.2f, 3f }) { WidthPercentage = 100 };
                var logo = ResolverLogo();
                if (File.Exists(logo))
                {
                    var imagen = Image.GetInstance(logo);
                    imagen.ScaleToFit(115f, 62f);
                    encabezado.AddCell(new PdfPCell(imagen) { Border = Rectangle.NO_BORDER, Padding = 8, HorizontalAlignment = Element.ALIGN_LEFT });
                }
                else encabezado.AddCell(new PdfPCell(new Phrase("ANIROK", titulo)) { Border = Rectangle.NO_BORDER, Padding = 12 });
                encabezado.AddCell(new PdfPCell(new Phrase("COMPROBANTE DE VENTA\nVenta #" + comprobante.VentaId, titulo)) { BackgroundColor = rojo, Border = Rectangle.NO_BORDER, Padding = 14, VerticalAlignment = Element.ALIGN_MIDDLE });
                document.Add(encabezado);
                document.Add(new Paragraph(" "));

                var datos = new PdfPTable(2) { WidthPercentage = 100 };
                datos.AddCell(CeldaDato("Fecha", comprobante.Fecha.ToString("dd/MM/yyyy HH:mm"), etiqueta, normal, gris, borde));
                datos.AddCell(CeldaDato("Método de pago", comprobante.MetodoPago, etiqueta, normal, gris, borde));
                datos.AddCell(CeldaDato("Cliente", comprobante.Cliente, etiqueta, normal, BaseColor.WHITE, borde));
                datos.AddCell(CeldaDato("N° de comprobante", comprobante.VentaId.ToString("000000"), etiqueta, normal, BaseColor.WHITE, borde));
                document.Add(datos);
                document.Add(new Paragraph(" "));

                var tabla = new PdfPTable(new float[] { 4f, 1f, 2f, 2f }) { WidthPercentage = 100 };
                AgregarEncabezado(tabla, "Producto", rojo);
                AgregarEncabezado(tabla, "Cant.", rojo);
                AgregarEncabezado(tabla, "Precio unitario", rojo);
                AgregarEncabezado(tabla, "Subtotal", rojo);
                foreach (var linea in comprobante.Lineas)
                {
                    AgregarCelda(tabla, linea.Producto, Element.ALIGN_LEFT, normal, borde);
                    AgregarCelda(tabla, linea.Cantidad.ToString(), Element.ALIGN_CENTER, normal, borde);
                    AgregarCelda(tabla, linea.PrecioUnitario.ToString("C", CultureInfo.GetCultureInfo("es-AR")), Element.ALIGN_RIGHT, normal, borde);
                    AgregarCelda(tabla, linea.Subtotal.ToString("C", CultureInfo.GetCultureInfo("es-AR")), Element.ALIGN_RIGHT, normal, borde);
                }
                document.Add(tabla);
                document.Add(new Paragraph(" "));
                var resumen = new PdfPTable(new float[] { 2.5f, 1.5f }) { WidthPercentage = 42, HorizontalAlignment = Element.ALIGN_RIGHT };
                AgregarResumen(resumen, "Subtotal", comprobante.Total.ToString("C", CultureInfo.GetCultureInfo("es-AR")), normal, gris);
                AgregarResumen(resumen, "TOTAL", comprobante.Total.ToString("C", CultureInfo.GetCultureInfo("es-AR")), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, rojo), new BaseColor(255, 235, 235));
                document.Add(resumen);
                document.Add(new Paragraph(" "));
                var pie = new Paragraph("Gracias por tu compra · Anirok Mangas & More", FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 9, BaseColor.GRAY));
                pie.Alignment = Element.ALIGN_CENTER;
                document.Add(pie);
                document.Close();
                return output.ToArray();
            }
        }

        private static PdfPCell CeldaDato(string etiqueta, string valor, Font fuenteEtiqueta, Font fuenteNormal, BaseColor fondo, BaseColor borde)
        {
            var celda = new PdfPCell { BackgroundColor = fondo, BorderColor = borde, Padding = 7 };
            celda.AddElement(new Phrase(etiqueta + ": ", fuenteEtiqueta));
            celda.AddElement(new Phrase(valor ?? "-", fuenteNormal));
            return celda;
        }

        private static void AgregarEncabezado(PdfPTable tabla, string texto, BaseColor fondo)
        {
            tabla.AddCell(new PdfPCell(new Phrase(texto, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE))) { BackgroundColor = fondo, Padding = 7, BorderColor = fondo });
        }

        private static void AgregarCelda(PdfPTable tabla, string texto, int alineacion, Font fuente, BaseColor borde)
        {
            tabla.AddCell(new PdfPCell(new Phrase(texto ?? "", fuente)) { HorizontalAlignment = alineacion, Padding = 7, BorderColor = borde });
        }

        private static void AgregarResumen(PdfPTable tabla, string etiqueta, string valor, Font fuente, BaseColor fondo)
        {
            tabla.AddCell(new PdfPCell(new Phrase(etiqueta, fuente)) { BackgroundColor = fondo, BorderColor = BaseColor.WHITE, Padding = 7, HorizontalAlignment = Element.ALIGN_RIGHT });
            tabla.AddCell(new PdfPCell(new Phrase(valor, fuente)) { BackgroundColor = fondo, BorderColor = BaseColor.WHITE, Padding = 7, HorizontalAlignment = Element.ALIGN_RIGHT });
        }

        private static string ResolverLogo()
        {
            foreach (var inicio in new[] { AppDomain.CurrentDomain.BaseDirectory, Environment.CurrentDirectory })
            {
                var carpeta = new DirectoryInfo(inicio);
                while (carpeta != null)
                {
                    var candidato = Path.Combine(carpeta.FullName, "Anirok", "AnirokWeb", "Content", "Images", "logo_anirok.png");
                    if (File.Exists(candidato)) return candidato;
                    carpeta = carpeta.Parent;
                }
            }
            return string.Empty;
        }

        public string GuardarPdf(ComprobanteViewModel comprobante)
        {
            if (comprobante == null) throw new ArgumentNullException("comprobante");
            string carpeta = Path.Combine(ResolverRutaComprobantes(), comprobante.Fecha.ToString("yyyy"), comprobante.Fecha.ToString("MM"));
            Directory.CreateDirectory(carpeta);
            string ruta = Path.Combine(carpeta, "venta-" + comprobante.VentaId + ".pdf");
            File.WriteAllBytes(ruta, CrearPdf(comprobante));
            return ruta;
        }

        public byte[] ObtenerOCrearPdf(ComprobanteViewModel comprobante)
        {
            // Regenera el comprobante para que las ventas existentes también
            // adopten el formato visual unificado con Desktop.
            GuardarPdf(comprobante);
            string carpeta = Path.Combine(ResolverRutaComprobantes(), comprobante.Fecha.ToString("yyyy"), comprobante.Fecha.ToString("MM"));
            string ruta = Path.Combine(carpeta, "venta-" + comprobante.VentaId + ".pdf");
            return File.ReadAllBytes(ruta);
        }

        private static string ResolverRutaComprobantes()
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

        private static Dictionary<string, string> ObtenerUsuario(int id, SqlConnection connection, SqlTransaction transaction)
        {
            using (var command = new SqlCommand("SELECT nombre, apellido, mail, telefono, direccion, dni FROM usuario WHERE id = @id", connection, transaction))
            {
                command.Parameters.AddWithValue("@id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read()) throw new InvalidOperationException("No se encontró el usuario.");
                    return new Dictionary<string, string> { { "nombre", reader.GetString(0) }, { "apellido", reader.GetString(1) }, { "mail", reader.GetString(2) }, { "telefono", reader.GetString(3) }, { "direccion", reader.GetString(4) }, { "dni", reader.GetString(5) } };
                }
            }
        }

        private static int ObtenerOCrearCliente(Dictionary<string, string> usuario, SqlConnection connection, SqlTransaction transaction)
        {
            using (var command = new SqlCommand("SELECT TOP 1 id FROM cliente WITH (UPDLOCK, HOLDLOCK) WHERE dni = @dni", connection, transaction))
            {
                command.Parameters.AddWithValue("@dni", usuario["dni"]);
                var existing = command.ExecuteScalar();
                if (existing != null) return Convert.ToInt32(existing);
            }
            var id = SiguienteId("cliente", connection, transaction);
            using (var command = new SqlCommand("INSERT INTO cliente (id, nombre, apellido, mail, telefono, direccion, dni) VALUES (@id, @nombre, @apellido, @mail, @telefono, @direccion, @dni)", connection, transaction))
            {
                command.Parameters.AddWithValue("@id", id);
                foreach (var key in new[] { "nombre", "apellido", "mail", "telefono", "direccion", "dni" }) command.Parameters.AddWithValue("@" + key, usuario[key]);
                command.ExecuteNonQuery();
            }
            return id;
        }

        private static int SiguienteId(string tabla, SqlConnection connection, SqlTransaction transaction)
        {
            if (tabla != "venta" && tabla != "detalle_venta" && tabla != "cliente") throw new ArgumentOutOfRangeException("tabla");
            using (var command = new SqlCommand("SELECT ISNULL(MAX(id), 0) + 1 FROM " + tabla + " WITH (UPDLOCK, HOLDLOCK)", connection, transaction)) return Convert.ToInt32(command.ExecuteScalar());
        }
    }
}
