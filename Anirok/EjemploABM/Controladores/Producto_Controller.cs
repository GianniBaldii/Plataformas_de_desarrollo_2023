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
    public static class Producto_Controller
    {
        public static bool crearProducto(Producto prod)
        {
            //Darlo de alta en la BBDD

            string query = "INSERT INTO dbo.producto " +
                "(id, nombre, descripción, precio, codigo, img, proovedor, subcategoria_id, categoria_id, talle, stock) " +
                "VALUES (@id, @nombre, @descripcion, @precio, @codigo, @img, @proovedor, @subcategoria_id, @categoria_id, @talle, @stock);";


            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", obtenerMaxId() + 1);
            cmd.Parameters.AddWithValue("@nombre", prod.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", prod.Descripcion);
            cmd.Parameters.AddWithValue("@precio", prod.Precio);
            cmd.Parameters.AddWithValue("@codigo", prod.codigo);
            cmd.Parameters.AddWithValue("@img", prod.Img);
            cmd.Parameters.AddWithValue("@proovedor", prod.Proveedor);
            cmd.Parameters.AddWithValue("@subcategoria_id", prod.SubcategoriaId);
            cmd.Parameters.AddWithValue("@categoria_id", prod.CategoriaId);
            cmd.Parameters.AddWithValue("@talle", prod.Talle);
            cmd.Parameters.AddWithValue("@stock", prod.Stock);






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

        // OBTENER EL MAX ID

        public static int obtenerMaxId()
        {
            int MaxId = 0;
            string query = "select max(id) from dbo.producto;";

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


        // GET ALL

        public static List<Producto> obtenerProductos()
        {
            List<Producto> list = new List<Producto>();
            string query = "SELECT id, nombre, descripción, precio, codigo, img, proovedor, subcategoria_id, categoria_id, talle, stock  FROM dbo.producto;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Los campos monetarios fueron migrados a DECIMAL y algunos
                    // datos históricos pueden venir NULL. GetDouble/GetString
                    // fallan cuando el tipo SQL no coincide exactamente.
                    var producto = CrearProductoDesdeReader(reader);
                    list.Add(producto);
                    Trace.WriteLine("Producto encontrado, nombre: " + producto.Nombre);
                }

                reader.Close();
                DB_Controller.connection.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }

            return list;
        }


        // GET ONE BY ID

        public static Producto obtenerPorId(int id)
        {
            Producto sub = new Producto();
            // Mantener el mismo orden y conjunto de columnas que el listado.
            // Evita que nuevas columnas (costo, moneda, cotización) desplacen
            // los índices cuando se usa SELECT *.
            string query = "SELECT id, nombre, descripción, precio, codigo, img, proovedor, subcategoria_id, categoria_id, talle, stock FROM dbo.producto WHERE id = @id;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    sub = CrearProductoDesdeReader(reader);
                    Trace.WriteLine("Producto encontrado, nombre: " + sub.Nombre);
                }

                reader.Close();
                DB_Controller.connection.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }

            return sub;
        }

        private static Producto CrearProductoDesdeReader(SqlDataReader reader)
        {
            return new Producto(
                Convert.ToInt32(reader.GetValue(0)),
                Texto(reader.GetValue(1)),
                Texto(reader.GetValue(2)),
                Numero(reader.GetValue(3)),
                Texto(reader.GetValue(4)),
                Texto(reader.GetValue(5)),
                Texto(reader.GetValue(6)),
                Entero(reader.GetValue(7)),
                Entero(reader.GetValue(8)),
                Texto(reader.GetValue(9)),
                Entero(reader.GetValue(10)));
        }

        private static string Texto(object valor)
        {
            return valor == null || valor == DBNull.Value ? string.Empty : Convert.ToString(valor);
        }

        private static double Numero(object valor)
        {
            return valor == null || valor == DBNull.Value ? 0d : Convert.ToDouble(valor);
        }

        private static int Entero(object valor)
        {
            return valor == null || valor == DBNull.Value ? 0 : Convert.ToInt32(valor);
        }


        // EDIT / PUT

        public static bool editarProducto(Producto prod)
        {
            //Darlo de alta en la BBDD

            string query = "UPDATE dbo.producto SET " +
                               "nombre = @nombre, " +
                               "descripción = @descripcion, " +
                               "precio = @precio, " +
                               "codigo = @codigo, " +
                               "img = @img, " +
                               "proovedor = @proovedor, " +
                               "subcategoria_id = @subcategoria_id, " +
                               "categoria_id = @categoria_id, " +
                               "talle = @talle, " +
                               "stock = @stock " +
                               "WHERE id = @id;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", prod.Id);
            cmd.Parameters.AddWithValue("@nombre", prod.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", prod.Descripcion);
            cmd.Parameters.AddWithValue("@precio", prod.Precio);
            cmd.Parameters.AddWithValue("@codigo", prod.codigo);
            cmd.Parameters.AddWithValue("@stock", prod.Stock);
            cmd.Parameters.AddWithValue("@img", prod.Img);
            cmd.Parameters.AddWithValue("@talle", prod.Talle);
            cmd.Parameters.AddWithValue("@proovedor", prod.Proveedor);
            cmd.Parameters.AddWithValue("@categoria_id", prod.CategoriaId);
            cmd.Parameters.AddWithValue("@subcategoria_id", prod.SubcategoriaId);
            ;


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

        public static bool eliminarProducto(int id)
        {
            //Darlo de alta en la BBDD

            string query = "DELETE FROM producto WHERE id = @id_eliminar;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id_eliminar", id);


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

        public static int ContarProductosConImagen(string nombreImagen)
        {
            const string query = "SELECT COUNT(*) FROM dbo.producto WHERE img = @img;";
            using (var cmd = new SqlCommand(query, DB_Controller.connection))
            {
                cmd.Parameters.AddWithValue("@img", nombreImagen);
                try
                {
                    DB_Controller.connection.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
                finally
                {
                    if (DB_Controller.connection.State != System.Data.ConnectionState.Closed)
                        DB_Controller.connection.Close();
                }
            }
        }


        public static bool CambiarCategoriaDeProductos(int idAnteriorCategoria, int idCategoriaPosterior)
        {
            // Utiliza una consulta SQL de actualización para cambiar el id_categoria de los productos.
            string query = "UPDATE producto SET categoria_id = @idCategoriaPosterior WHERE categoria_id = @idAnteriorCategoria;";

            using (SqlConnection connection = new SqlConnection("TuCadenaDeConexion"))
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                // Define los parámetros para la consulta.
                cmd.Parameters.AddWithValue("@idAnteriorCategoria", idAnteriorCategoria);
                cmd.Parameters.AddWithValue("@idCategoriaPosterior", idCategoriaPosterior);

                try
                {
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Si se actualiza al menos una fila, se considera un éxito.
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al cambiar la categoría de los productos: " + ex.Message);
                }
            }
        }

        public static bool ActualizarStock(int productoId, int cantidadVendida)
        {
            string query = "UPDATE dbo.producto SET stock = stock - @cantidadVendida WHERE id = @productoId;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@productoId", productoId);
            cmd.Parameters.AddWithValue("@cantidadVendida", cantidadVendida);

            try
            {
                DB_Controller.connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                DB_Controller.connection.Close();
                return rowsAffected > 0; // Retorna true si se actualizó al menos una fila
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el stock: " + ex.Message);
            }
        }


    }
}
