using EjemploABM.Modelo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Windows.Forms;

namespace EjemploABM.Controladores
{
    class Usuario_Controller
    {

        // GET ONE

        public static bool autenticar(string usr, string pass, bool hasheado)
        {
            Usuario user = null;

            // Consulta para autenticar al usuario
            string query = "SELECT * FROM dbo.usuario WHERE (nombre = @usr OR mail = @usr) AND contraseña = @pass;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@usr", usr);

            if (hasheado)
            {
                cmd.Parameters.AddWithValue("@pass", pass);
            }
            else
            {
                // cmd.Parameters.AddWithValue("@pass", hc.PassHash(pass));
            }

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Trace.WriteLine("Usr encontrado, nombre: " + reader.GetString(1));
                    user = new Usuario(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4),
                        reader.GetString(5),
                        reader.GetString(6),
                        reader.GetString(7),
                        reader.GetInt32(8)
                    );
                }

                reader.Close();

                if (user != null)
                {
                    Program.logueado = user;

                    // Consulta para obtener la URL de la imagen del usuario
                    string imgQuery = "SELECT urlimg FROM dbo.usuario_urls WHERE usuario_id = @userId;";
                    SqlCommand imgCmd = new SqlCommand(imgQuery, DB_Controller.connection);
                    imgCmd.Parameters.AddWithValue("@userId", user.Id);

                    SqlDataReader imgReader = imgCmd.ExecuteReader();

                    if (imgReader.Read())
                    {
                        Program.URLimg = imgReader.IsDBNull(0) ? null : imgReader.GetString(0);
                    }

                    imgReader.Close();

                    // Consulta para obtener la URL del PDF del usuario
                    string pdfQuery = "SELECT urlpdf FROM dbo.usuario_urls WHERE usuario_id = @userId;";
                    SqlCommand pdfCmd = new SqlCommand(pdfQuery, DB_Controller.connection);
                    pdfCmd.Parameters.AddWithValue("@userId", user.Id);

                    SqlDataReader pdfReader = pdfCmd.ExecuteReader();

                    if (pdfReader.Read())
                    {
                        Program.URLpdf = pdfReader.IsDBNull(0) ? null : pdfReader.GetString(0);
                    }

                    pdfReader.Close();
                    DB_Controller.connection.Close();

                    // Verifica si alguna de las URLs no está configurada
                    if (string.IsNullOrEmpty(Program.URLimg) || string.IsNullOrEmpty(Program.URLpdf))
                    {
                        MessageBox.Show("Una o ambas URLs no están configuradas. Por favor, configúrelas utilizando el botón 'Setear URL'.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    return true;
                }
                else
                {
                    DB_Controller.connection.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }
        }



        // POST

        public static bool crearUsuario(Usuario usr)
        {
            //Darlo de alta en la BBDD

            string query = "insert into dbo.usuario values" +
               "(@id, " +
               "@nombre, " +
               "@apellido, " +
               "@mail, " +
               "@telefono, " +
               "@direccion, " +
               "@dni, " +
               "@contraseña, " +
               "@rol);";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", obtenerMaxId() + 1);
            cmd.Parameters.AddWithValue("@nombre", usr.Nombre);
            cmd.Parameters.AddWithValue("@apellido", usr.Apellido);
            cmd.Parameters.AddWithValue("@mail", usr.Mail);
            cmd.Parameters.AddWithValue("@telefono", usr.Telefono);
            cmd.Parameters.AddWithValue("@direccion", usr.Direccion);
            cmd.Parameters.AddWithValue("@dni", usr.Dni);
            cmd.Parameters.AddWithValue("@contraseña", usr.Contraseña);
            cmd.Parameters.AddWithValue("@rol", usr.Rol);



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
            string query = "select max(id) from dbo.usuario;";

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

        public static List<Usuario> obtenerTodos()
        {
            List<Usuario> list = new List<Usuario>();
            string query = "select * from dbo.usuario;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Usuario(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetInt32(8)));
         
                    Trace.WriteLine("Usr encontrado, nombre: " + reader.GetString(1));
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

        public static Usuario obtenerPorId(int id)
        {
            Usuario usr = new Usuario();
            string query = "select * from dbo.usuario where id = @id;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                DB_Controller.connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    usr = new Usuario(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetInt32(8));
                    Trace.WriteLine("Usr encontrado, nombre: " + reader.GetString(1));
                }

                reader.Close();
                DB_Controller.connection.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }

            return usr;
        }



        // EDIT / PUT

        public static bool editarUsuario(Usuario usr)
        {
            //Darlo de alta en la BBDD

            string query = "UPDATE dbo.usuario SET " +
                "nombre = @nombre, " +
                "apellido = @apellido, " +
                "mail = @mail, " +
                "telefono = @telefono, " +
                "direccion = @direccion," +
                " dni = @dni, " +
                "contraseña = @contraseña," +
                " rol = @rol " +
                "WHERE id = @id;";

            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@id", usr.Id);
            cmd.Parameters.AddWithValue("@nombre", usr.Nombre);
            cmd.Parameters.AddWithValue("@apellido", usr.Apellido);
            cmd.Parameters.AddWithValue("@mail", usr.Mail);
            cmd.Parameters.AddWithValue("@telefono", usr.Telefono);
            cmd.Parameters.AddWithValue("@direccion", usr.Direccion);
            cmd.Parameters.AddWithValue("@dni", usr.Dni);
            cmd.Parameters.AddWithValue("@contraseña", usr.Contraseña);
            cmd.Parameters.AddWithValue("@rol", usr.Rol);

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


        public static bool eliminarUsuario(int id)
        {
            //Darlo de alta en la BBDD

            string query = "DELETE FROM usuario WHERE id = @id_eliminar;";

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


        // validar repeticion de la contraseña


        
public static bool existeContraseña(string contraseña)
        {
            string query = "SELECT COUNT(*) FROM dbo.usuario WHERE contraseña = @contraseña;";
            SqlCommand cmd = new SqlCommand(query, DB_Controller.connection);
            cmd.Parameters.AddWithValue("@contraseña", contraseña);

            try
            {
                DB_Controller.connection.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0; // Si count > 0, significa que la contraseña ya está en uso
            }
            catch (Exception ex)
            {
                throw new Exception("Hay un error en la query: " + ex.Message);
            }
            finally
            {
                DB_Controller.connection.Close();
            }
        }

        public static bool ActualizarUrls(int usuarioId, string urlImg, string urlPdf)
        {
            // Validar URLs
            if (!IsValidUrl(urlImg) || !IsValidUrl(urlPdf))
            {
                throw new ArgumentException("Las URLs ingresadas no son válidas.");
            }

            string updateQuery = @"
            IF EXISTS (SELECT 1 FROM dbo.usuario_urls WHERE usuario_id = @userId)
            BEGIN
                UPDATE dbo.usuario_urls
                SET urlimg = @urlImg, urlpdf = @urlPdf
                WHERE usuario_id = @userId;
            END
            ELSE
            BEGIN
                INSERT INTO dbo.usuario_urls (urlimg, urlpdf, usuario_id)
                VALUES (@urlImg, @urlPdf, @userId);
            END"
            ;



            SqlCommand cmd = new SqlCommand(updateQuery, DB_Controller.connection);
            {
                    cmd.Parameters.AddWithValue("@userId", usuarioId);
                    cmd.Parameters.AddWithValue("@urlImg", string.IsNullOrEmpty(urlImg) ? (object)DBNull.Value : urlImg);
                    cmd.Parameters.AddWithValue("@urlPdf", string.IsNullOrEmpty(urlPdf) ? (object)DBNull.Value : urlPdf);

                    try
                    {
                    DB_Controller.connection.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error al guardar las URLs: " + ex.Message);
                    }
                }
        }



        // Función para validar si una URL es válida
        private static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

       
        return true;
    }
}

}

