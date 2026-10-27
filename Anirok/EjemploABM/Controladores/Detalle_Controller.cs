using EjemploABM.Modelo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploABM.Controladores
{
    internal class Detalle_Controller
    {
        public static bool CrearDetalleVenta(DetalleVenta detalleVenta)
        {
            string query = "INSERT INTO dbo.detalle_venta (id, cantidad, venta_id, producto_id, precio_unitario) " +
                           "VALUES (@id, @cantidad, @ventaId, @productoId, @precioUnitario);";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", obtenerMaxId()+1);
            cmd.Parameters.AddWithValue("@cantidad", detalleVenta.Cantidad);
            cmd.Parameters.AddWithValue("@ventaId", detalleVenta.VentaId);
            cmd.Parameters.AddWithValue("@productoId", detalleVenta.ProductoId);
            cmd.Parameters.AddWithValue("@precioUnitario", detalleVenta.PrecioUnitario);

            try
            {
                DB_Controller.connection.Open();
                cmd.ExecuteNonQuery();
                DB_Controller.connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }
        }

        public static List<DetalleVenta> ObtenerDetallesVenta()
        {
            List<DetalleVenta> listaDetallesVenta = new List<DetalleVenta>();
            string query = "SELECT id, cantidad, venta_id, producto_id, precio_unitario FROM dbo.detalle_venta;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DetalleVenta detalleVenta = new DetalleVenta
                    {
                        Id = reader.GetInt32(0),
                        Cantidad = reader.GetInt32(1),
                        VentaId = reader.GetInt32(2),
                        ProductoId = reader.GetInt32(3),
                        PrecioUnitario = Convert.ToDouble(reader.GetValue(4))
                    };

                    listaDetallesVenta.Add(detalleVenta);

                    Trace.WriteLine("DetalleVenta encontrada, ID: " + reader.GetInt32(0));
                }

                reader.Close();
                DB_Controller.connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }

            return listaDetallesVenta;
        }
        public static int obtenerMaxId()
        {
            int MaxId = 0;
            string query = "select max(id) from dbo.detalle_venta;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    MaxId = reader.GetInt32(0);
                }

                reader.Close();
                DB_Controller.connection.Close();
                return MaxId;
            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }
        }

        public static DetalleVenta ObtenerDetalleVentaPorId(int id)
        {
            DetalleVenta detalleVenta = new DetalleVenta();
            string query = "SELECT id, cantidad, venta_id, producto_id, precio_unitario FROM dbo.detalle_venta WHERE id = @id;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    detalleVenta = new DetalleVenta
                    {
                        Id = reader.GetInt32(0),
                        Cantidad = reader.GetInt32(1),
                        VentaId = reader.GetInt32(2),
                        ProductoId = reader.GetInt32(3),
                        PrecioUnitario = Convert.ToDouble(reader.GetValue(4))
                    };

                    Trace.WriteLine("DetalleVenta encontrada, ID: " + reader.GetInt32(0));
                }

                reader.Close();
                DB_Controller.connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }

            return detalleVenta;
        }

        public static bool EditarDetalleVenta(DetalleVenta detalleVenta)
        {
            string query = "UPDATE dbo.detalle_venta SET " +
                           "cantidad = @cantidad, " +
                           "venta_id = @ventaId, " +
                           "producto_id = @productoId, " +
                           "precio_unitario = @precioUnitario " +
                           "WHERE id = @id;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", detalleVenta.Id);
            cmd.Parameters.AddWithValue("@cantidad", detalleVenta.Cantidad);
            cmd.Parameters.AddWithValue("@ventaId", detalleVenta.VentaId);
            cmd.Parameters.AddWithValue("@productoId", detalleVenta.ProductoId);
            cmd.Parameters.AddWithValue("@precioUnitario", detalleVenta.PrecioUnitario);

            try
            {
                DB_Controller.connection.Open();
                cmd.ExecuteNonQuery();
                DB_Controller.connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }
        }

        public static bool EliminarDetalleVenta(int id)
        {
            string query = "DELETE FROM detalle_venta WHERE id = @id;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                DB_Controller.connection.Open();
                cmd.ExecuteNonQuery();
                DB_Controller.connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }
        }
    }
}

