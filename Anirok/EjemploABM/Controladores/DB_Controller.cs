using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploABM.Controladores
{
    public static class DB_Controller
    {

        private static string connectionString = "";
        public static SqlConnection connection;

        public static void initialize()
        {
            var environmentConnection = Environment.GetEnvironmentVariable("ANIROK_DB_CONNECTION");
            // En desarrollo local solo aceptamos la cadena Docker vigente. Si
            // Visual Studio heredó una contraseña antigua, la descartamos para
            // evitar el error "Login failed for user 'sa'".
            if (!string.IsNullOrWhiteSpace(environmentConnection)
                && environmentConnection.IndexOf("localhost,1433", StringComparison.OrdinalIgnoreCase) >= 0
                && environmentConnection.IndexOf("Anirok_dev_2026!", StringComparison.Ordinal) >= 0)
            {
                connectionString = environmentConnection;
            }
            else
            {
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = "localhost,1433",
                    InitialCatalog = "anirok2",
                    UserID = "sa",
                    Password = "Anirok_dev_2026!",
                    TrustServerCertificate = true
                };

                connectionString = builder.ToString();
            }

            connection = new SqlConnection(connectionString);

            Trace.WriteLine("Conexion a la DB: " + connection);

        }

    }
}
