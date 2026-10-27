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
                        var total = 0d;
                        var productos = new List<Tuple<int, int, double>>();

                        foreach (var item in carrito)
                        {
                            using (var command = new SqlCommand("SELECT nombre, precio, stock FROM producto WITH (UPDLOCK, ROWLOCK) WHERE id = @id", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", item.ProductoId);
                                using (var reader = command.ExecuteReader())
                                {
                                    if (!reader.Read()) throw new InvalidOperationException("Uno de los productos ya no existe.");
                                    var nombre = reader.GetString(0);
                                    var precio = Convert.ToDouble(reader.GetValue(1));
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
                            using (var command = new SqlCommand("INSERT INTO detalle_venta (id, cantidad, venta_id, producto_id) VALUES (@id, @cantidad, @venta, @producto); UPDATE producto SET stock = stock - @cantidad WHERE id = @producto", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", detalleId++);
                                command.Parameters.AddWithValue("@cantidad", producto.Item2);
                                command.Parameters.AddWithValue("@venta", ventaId);
                                command.Parameters.AddWithValue("@producto", producto.Item1);
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
                using (var command = new SqlCommand("SELECT v.id, v.fecha, c.nombre + ' ' + c.apellido, v.metodo_de_pago, v.precio_total FROM venta v INNER JOIN cliente c ON c.id = v.cliente_id WHERE v.id = @venta AND v.usuario_id = @usuario", connection))
                {
                    command.Parameters.AddWithValue("@venta", ventaId);
                    command.Parameters.AddWithValue("@usuario", usuarioId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        comprobante = new ComprobanteViewModel { VentaId = reader.GetInt32(0), Fecha = reader.GetDateTime(1), Cliente = reader.GetString(2), MetodoPago = reader.GetString(3), Total = Convert.ToDouble(reader.GetValue(4)) };
                    }
                }
                using (var command = new SqlCommand("SELECT p.nombre, d.cantidad, p.precio FROM detalle_venta d INNER JOIN producto p ON p.id = d.producto_id WHERE d.venta_id = @venta ORDER BY d.id", connection))
                {
                    command.Parameters.AddWithValue("@venta", ventaId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read()) comprobante.Lineas.Add(new ComprobanteLineaViewModel { Producto = reader.GetString(0), Cantidad = reader.GetInt32(1), PrecioUnitario = Convert.ToDouble(reader.GetValue(2)) });
                    }
                }
                return comprobante;
            }
        }

        public byte[] CrearPdf(ComprobanteViewModel comprobante)
        {
            using (var output = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter.GetInstance(document, output);
                document.Open();
                document.Add(new Paragraph("ANIROK - COMPROBANTE DE COMPRA", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18)));
                document.Add(new Paragraph("Venta #" + comprobante.VentaId));
                document.Add(new Paragraph("Fecha: " + comprobante.Fecha.ToString("dd/MM/yyyy")));
                document.Add(new Paragraph("Cliente: " + comprobante.Cliente));
                document.Add(new Paragraph("Método de pago: " + comprobante.MetodoPago));
                document.Add(new Paragraph(" "));
                foreach (var linea in comprobante.Lineas) document.Add(new Paragraph(linea.Cantidad + " x " + linea.Producto + " - " + linea.Subtotal.ToString("C", CultureInfo.GetCultureInfo("es-AR"))));
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("TOTAL: " + comprobante.Total.ToString("C", CultureInfo.GetCultureInfo("es-AR")), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14)));
                document.Close();
                return output.ToArray();
            }
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
