using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EjemploABM.Modelo;


namespace EjemploABM.Controladores
{
    internal class Venta_Controller
    {
        public static bool CrearVenta(Venta venta)
        {
            string query = "INSERT INTO dbo.venta (id, precio_total, fecha, metodo_de_pago, descuento, cliente_id, usuario_id) " +
                           "VALUES (@id, @precioTotal, @fecha, @metodoDePago, @descuento, @clienteId, @usuarioId);";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", obtenerMaxId()+1);
            cmd.Parameters.AddWithValue("@precioTotal", venta.PrecioTotal);
            cmd.Parameters.AddWithValue("@fecha", venta.Fecha);
            cmd.Parameters.AddWithValue("@metodoDePago", venta.MetodoDePago);
            cmd.Parameters.AddWithValue("@descuento", venta.Descuento);
            cmd.Parameters.AddWithValue("@clienteId", venta.ClienteId);
            cmd.Parameters.AddWithValue("@usuarioId", venta.UsuarioId);

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
        public static int obtenerMaxId()
        {
            int MaxId = 0;
            string query = "select isnull(max(id), 0) from dbo.venta;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    MaxId = Convert.ToInt32(reader.GetValue(0));
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

        public static List<Venta> ObtenerVentas()
        {
            List<Venta> listaVentas = new List<Venta>();
            string query = "SELECT id, precio_total, fecha, metodo_de_pago, descuento, cliente_id, usuario_id FROM dbo.venta;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Venta venta = CrearVentaDesdeReader(reader);

                    listaVentas.Add(venta);

                    Trace.WriteLine("Venta encontrada, ID: " + venta.Id);
                }

                reader.Close();
                DB_Controller.connection.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }

            return listaVentas;
        }

        public static Venta ObtenerVentaPorId(int id)
        {
            Venta venta = new Venta();
            string query = "SELECT id, precio_total, fecha, metodo_de_pago, descuento, cliente_id, usuario_id FROM dbo.venta WHERE id = @id;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    venta = CrearVentaDesdeReader(reader);

                    Trace.WriteLine("Venta encontrada, ID: " + venta.Id);
                }

                reader.Close();
                DB_Controller.connection.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }

            return venta;
        }

        private static Venta CrearVentaDesdeReader(SqlDataReader reader)
        {
            return new Venta
            {
                Id = Entero(reader.GetValue(0)),
                PrecioTotal = DecimalAFloat(reader.GetValue(1)),
                Fecha = reader.IsDBNull(2) ? DateTime.MinValue : Convert.ToDateTime(reader.GetValue(2)),
                MetodoDePago = Texto(reader.GetValue(3)),
                Descuento = DecimalAFloat(reader.GetValue(4)),
                ClienteId = Entero(reader.GetValue(5)),
                UsuarioId = Entero(reader.GetValue(6))
            };
        }

        private static string Texto(object valor)
        {
            return valor == null || valor == DBNull.Value ? string.Empty : Convert.ToString(valor);
        }

        private static int Entero(object valor)
        {
            return valor == null || valor == DBNull.Value ? 0 : Convert.ToInt32(valor);
        }

        private static float DecimalAFloat(object valor)
        {
            return valor == null || valor == DBNull.Value ? 0f : Convert.ToSingle(valor);
        }

        public static bool EditarVenta(Venta venta)
        {
            string query = "UPDATE dbo.venta SET " +
                           "precio_total = @precioTotal, " +
                           "fecha = @fecha, " +
                           "metodo_de_pago = @metodoDePago, " +
                           "descuento = @descuento, " +
                           "cliente_id = @clienteId, " +
                           "usuario_id = @usuarioId " +
                           "WHERE id = @id;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", venta.Id);
            cmd.Parameters.AddWithValue("@precioTotal", venta.PrecioTotal);
            cmd.Parameters.AddWithValue("@fecha", venta.Fecha);
            cmd.Parameters.AddWithValue("@metodoDePago", venta.MetodoDePago);
            cmd.Parameters.AddWithValue("@descuento", venta.Descuento);
            cmd.Parameters.AddWithValue("@clienteId", venta.ClienteId);
            cmd.Parameters.AddWithValue("@usuarioId", venta.UsuarioId);

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

        public static bool EliminarVenta(int id)
        {
            string query = "DELETE FROM dbo.venta WHERE id = @id;";

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

