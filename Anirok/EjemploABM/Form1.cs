using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EjemploABM.Controladores;
using MaterialSkin;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using EjemploABM.Servicios;

namespace EjemploABM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            EstiloAplicacion.Aplicar(this);
            Text = "Anirok - Iniciar sesión";
            AcceptButton = btn_log;
            labelError.Visible = false;
        }


        private void btn_login_Click(object sender, EventArgs e)
        {
            IntentarIniciarSesion();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Configura el SkinManager para tu formulario
            MaterialSkinManager manager = MaterialSkinManager.Instance;


            // Cambia el color de fondo del botón
            manager.ColorScheme = new ColorScheme(
                Primary.Red900, // Cambiar el color de fondo del botón
                Primary.Red500, // Cambiar el color de acento del botón
                Primary.Red500, // Cambiar el color de la barra de título del formulario
                Accent.Orange200,
                TextShade.WHITE
            );

        }

        
        

        private void btn_log_Click(object sender, EventArgs e)
        {
            IntentarIniciarSesion();
        }

        private void IntentarIniciarSesion()
        {
            string usuario = textBox1.Text.Trim();
            string contrasena = textBox2.Text;

            if (usuario == "Usuario" || contrasena == "Contraseña" ||
                string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                labelError.Text = "Ingresá tu usuario y contraseña.";
                labelError.Visible = true;
                return;
            }

            if (Usuario_Controller.autenticar(usuario, contrasena, true))
            {
                Index index = new Index();
                index.Show();
                Hide();
                return;
            }

            labelError.Text = "Usuario o contraseña incorrectos. Intentá nuevamente.";
            labelError.Visible = true;
            textBox2.Clear();
            textBox2.UseSystemPasswordChar = true;
            textBox2.Focus();
        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if(textBox1.Text == "Usuario")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.LightGray;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "") {
                textBox1.Text = "Usuario";
                textBox1.ForeColor = Color.LightGray;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Contraseña")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.LightGray;
                //para que se oculten los caracteres
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "Contraseña";
                textBox2.ForeColor = Color.LightGray;
                
                textBox2.UseSystemPasswordChar = false;

            }
        }

        //CERRAR Y MINIMIZAR 

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
