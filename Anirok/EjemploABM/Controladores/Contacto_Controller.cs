using EjemploABM.Modelo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EjemploABM.Controladores
{
    internal static class Contacto_Controller
    {
        public static List<Contacto> ObtenerTodos()
        {
            var resultado = new List<Contacto>();
            if (DB_Controller.connection == null) DB_Controller.initialize();
            const string query = "SELECT id, asunto, email, mensaje, fecha, estado FROM dbo.contacto ORDER BY fecha DESC";
            using (var command = new SqlCommand(query, DB_Controller.connection))
            {
                try
                {
                    DB_Controller.connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultado.Add(new Contacto
                            {
                                Id = reader.GetInt32(0), Asunto = reader.GetString(1), Email = reader.GetString(2),
                                Mensaje = reader.GetString(3), Fecha = reader.GetDateTime(4), Estado = reader.GetString(5)
                            });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("No se pudieron cargar las consultas. Verificá que exista la tabla contacto.", ex);
                }
                finally { if (DB_Controller.connection.State != System.Data.ConnectionState.Closed) DB_Controller.connection.Close(); }
            }
            return resultado;
        }
    }
}
