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
            connectionString = Environment.GetEnvironmentVariable("ANIROK_DB_CONNECTION");

            if (string.IsNullOrWhiteSpace(connectionString))
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
