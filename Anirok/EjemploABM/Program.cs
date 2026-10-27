using EjemploABM.Controladores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EjemploABM.Modelo;
using EjemploABM.Servicios;

namespace EjemploABM
{
    static class Program
    {

        public static Usuario logueado;
        public static string URLimg;
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DB_Controller.initialize();
            URLimg = ImagenProductoService.RutaBase;

            if (validateConnection())
            {
                Trace.WriteLine("Conexion a la base de datos establecida con exito");
            }


            Application.Run(new Form1());
        }

        public static bool validateConnection()
        {
            try
            {
                DB_Controller.connection.Open();
                DB_Controller.connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error al conectar a la base de datos: " + ex.Message);
                return false;
            }
        }

    }
}
