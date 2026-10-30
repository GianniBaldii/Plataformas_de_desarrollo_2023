using EjemploABM.Controladores;
using EjemploABM.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace EjemploABM.Servicios
{
    internal static class VentaService
    {
        public static int RegistrarVenta(Venta venta, IList<DetalleVenta> detalles)
        {
            if (venta == null) throw new ArgumentNullException("venta");
            if (detalles == null || detalles.Count == 0)
                throw new InvalidOperationException("La venta debe tener al menos un producto.");
            if (detalles.Any(d => d.Cantidad <= 0))
                throw new InvalidOperationException("Las cantidades de la venta deben ser mayores a cero.");
            if (detalles.GroupBy(d => d.ProductoId).Any(g => g.Count() > 1))
                throw new InvalidOperationException("No se puede repetir un producto dentro de la misma venta.");

            SqlConnection connection = DB_Controller.connection;
            if (connection == null) throw new InvalidOperationException("La conexión a la base no fue inicializada.");

            if (connection.State != ConnectionState.Closed)
                connection.Close();

            connection.Open();
            using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    int ventaId = ObtenerSiguienteId(connection, transaction, "venta");

                    using (SqlCommand command = new SqlCommand(
                        "INSERT INTO dbo.venta (id, precio_total, fecha, metodo_de_pago, descuento, cliente_id, usuario_id) " +
                        "VALUES (@id, @precioTotal, @fecha, @metodoPago, @descuento, @clienteId, @usuarioId);",
                        connection, transaction))
                    {
                        command.Parameters.Add("@id", SqlDbType.Int).Value = ventaId;
                        command.Parameters.Add("@precioTotal", SqlDbType.Real).Value = venta.PrecioTotal;
                        command.Parameters.Add("@fecha", SqlDbType.DateTime).Value = venta.Fecha;
                        command.Parameters.Add("@metodoPago", SqlDbType.VarChar, 45).Value = venta.MetodoDePago;
                        command.Parameters.Add("@descuento", SqlDbType.Real).Value = venta.Descuento;
                        command.Parameters.Add("@clienteId", SqlDbType.Int).Value = venta.ClienteId;
                        command.Parameters.Add("@usuarioId", SqlDbType.Int).Value = venta.UsuarioId;
                        command.ExecuteNonQuery();
                    }

                    foreach (DetalleVenta detalle in detalles)
                    {
                        int stock = ObtenerStock(connection, transaction, detalle.ProductoId);
                        if (stock < detalle.Cantidad)
                            throw new InvalidOperationException("No hay stock suficiente para el producto " + detalle.ProductoId + ".");

                        int detalleId = ObtenerSiguienteId(connection, transaction, "detalle_venta");
                        using (SqlCommand command = new SqlCommand(
                            "INSERT INTO dbo.detalle_venta (id, cantidad, venta_id, producto_id, precio_unitario) " +
                            "VALUES (@id, @cantidad, @ventaId, @productoId, @precioUnitario);",
                            connection, transaction))
                        {
                            command.Parameters.Add("@id", SqlDbType.Int).Value = detalleId;
                            command.Parameters.Add("@cantidad", SqlDbType.Int).Value = detalle.Cantidad;
                            command.Parameters.Add("@ventaId", SqlDbType.Int).Value = ventaId;
                            command.Parameters.Add("@productoId", SqlDbType.Int).Value = detalle.ProductoId;
                            command.Parameters.Add("@precioUnitario", SqlDbType.Real).Value = (float)detalle.PrecioUnitario;
                            command.ExecuteNonQuery();
                        }

                        using (SqlCommand command = new SqlCommand(
                            "UPDATE dbo.producto SET stock = stock - @cantidad " +
                            "WHERE id = @productoId AND stock >= @cantidad;",
                            connection, transaction))
                        {
                            command.Parameters.Add("@cantidad", SqlDbType.Int).Value = detalle.Cantidad;
                            command.Parameters.Add("@productoId", SqlDbType.Int).Value = detalle.ProductoId;
                            if (command.ExecuteNonQuery() != 1)
                                throw new InvalidOperationException("No se pudo actualizar el stock del producto " + detalle.ProductoId + ".");
                        }
                    }

                    transaction.Commit();
                    return ventaId;
                }
                catch
                {
                    try { transaction.Rollback(); } catch { }
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }
            }
        }

        private static int ObtenerStock(SqlConnection connection, SqlTransaction transaction, int productoId)
        {
            using (SqlCommand command = new SqlCommand(
                "SELECT stock FROM dbo.producto WITH (UPDLOCK, HOLDLOCK) WHERE id = @productoId;",
                connection, transaction))
            {
                command.Parameters.Add("@productoId", SqlDbType.Int).Value = productoId;
                object value = command.ExecuteScalar();
                if (value == null || value == DBNull.Value)
                    throw new InvalidOperationException("El producto " + productoId + " no existe.");
                return Convert.ToInt32(value);
            }
        }

        private static int ObtenerSiguienteId(SqlConnection connection, SqlTransaction transaction, string tabla)
        {
            using (SqlCommand command = new SqlCommand(
                "SELECT ISNULL(MAX(id), 0) + 1 FROM dbo." + tabla + " WITH (TABLOCKX, HOLDLOCK);",
                connection, transaction))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
