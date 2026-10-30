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
            string query = "INSERT INTO dbo.venta VALUES" +
                           "(@id, " +
                           "@precioTotal, " +
                           "@fecha, " +
                           "@metodoDePago, " +
                           "@descuento, " +
                           "@clienteId, " +
                           "@usuarioId" +
                           ");";

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
            string query = "SELECT * FROM dbo.venta;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Venta venta = new Venta
                    {
                        Id = reader.GetInt32(0),
                        PrecioTotal = reader.GetFloat(1),
                        Fecha = reader.GetDateTime(2),
                        MetodoDePago = reader.GetString(3),
                        Descuento = reader.GetFloat(4),
                        ClienteId = reader.GetInt32(5),
                        UsuarioId = reader.GetInt32(6)
                    };

                    listaVentas.Add(venta);

                    Trace.WriteLine("Venta encontrada, ID: " + reader.GetInt32(0));
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
            string query = "SELECT * FROM dbo.venta WHERE id = @id;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    venta = new Venta
                    {
                        Id = reader.GetInt32(0),
                        PrecioTotal = reader.GetFloat(1),
                        Fecha = reader.GetDateTime(2),
                        MetodoDePago = reader.GetString(3),
                        Descuento = reader.GetFloat(4),
                        ClienteId = reader.GetInt32(5),
                        UsuarioId = reader.GetInt32(6)
                    };

                    Trace.WriteLine("Venta encontrada, ID: " + reader.GetInt32(0));
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

        public static bool EditarVenta(Venta venta)
        {
            string query = "UPDATE dbo.venta SET " +
                           "precioTotal = @precioTotal, " +
                           "fecha = @fecha, " +
                           "metodoDePago = @metodoDePago, " +
                           "descuento = @descuento, " +
                           "clienteId = @clienteId, " +
                           "usuarioId = @usuarioId " +
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

