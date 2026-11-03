using AnirokWeb.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AnirokWeb.Services
{
    internal static class CuentaService
    {
        public static int Registrar(RegistroViewModel model, string connectionString)
        {
            // El registro debe usar la conexión efectiva de la aplicación y no
            // la cadena que Entity Framework pueda reconstruir desde una
            // configuración antigua de IIS Express.
            using (var connection = new SqlConnection(AnirokDbContext.ResolveConnectionString()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        using (var existe = new SqlCommand("SELECT COUNT(1) FROM dbo.usuario WHERE nombre = @nombre OR mail = @mail", connection, transaction))
                        {
                            existe.Parameters.Add("@nombre", SqlDbType.VarChar, 45).Value = model.Nombre.Trim();
                            existe.Parameters.Add("@mail", SqlDbType.VarChar, 45).Value = model.Mail.Trim();
                            if (Convert.ToInt32(existe.ExecuteScalar()) > 0)
                                throw new InvalidOperationException("El nombre de usuario o el email ya están registrados.");
                        }

                        int id;
                        using (var siguiente = new SqlCommand("SELECT ISNULL(MAX(id), 0) + 1 FROM dbo.usuario WITH (TABLOCKX, HOLDLOCK)", connection, transaction))
                        {
                            id = Convert.ToInt32(siguiente.ExecuteScalar());
                        }

                        using (var insertar = new SqlCommand(@"
                            INSERT INTO dbo.usuario
                                (id, nombre, apellido, mail, telefono, direccion, dni, [contraseña], rol)
                            VALUES
                                (@id, @nombre, @apellido, @mail, @telefono, @direccion, @dni, @contrasena, 3);", connection, transaction))
                        {
                            insertar.Parameters.Add("@id", SqlDbType.Int).Value = id;
                            insertar.Parameters.Add("@nombre", SqlDbType.VarChar, 45).Value = model.Nombre.Trim();
                            insertar.Parameters.Add("@apellido", SqlDbType.VarChar, 45).Value = model.Apellido.Trim();
                            insertar.Parameters.Add("@mail", SqlDbType.VarChar, 45).Value = model.Mail.Trim();
                            insertar.Parameters.Add("@telefono", SqlDbType.VarChar, 45).Value = model.Telefono.Trim();
                            insertar.Parameters.Add("@direccion", SqlDbType.VarChar, 45).Value = model.Direccion.Trim();
                            insertar.Parameters.Add("@dni", SqlDbType.VarChar, 45).Value = model.Dni.Trim();
                            insertar.Parameters.Add("@contrasena", SqlDbType.VarChar, 100).Value = PasswordService.Hash(model.Contrasena);
                            insertar.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return id;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
