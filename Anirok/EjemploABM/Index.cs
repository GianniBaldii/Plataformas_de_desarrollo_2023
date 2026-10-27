using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EjemploABM.ControlesDeUsuario;
using EjemploABM.Modelo;

namespace EjemploABM
{
    public partial class Index : Form
    {
        private bool userScrolled = false;
        public Index()
        {
            InitializeComponent();

            
            Productos_UC prodsUC = new Productos_UC();
            addUserControl(prodsUC);
            
            lb_nombre_logueado.Text = Program.logueado.Nombre;
            if(Program.logueado.Rol != 1)
            {
                btn_users.Hide();
            }


        }

        //BOTONES
        private void btn_prods_Click_1(object sender, EventArgs e)
        {
            Productos_UC prodsUC = new Productos_UC();
            addUserControl(prodsUC);
        }

        private void btn_cats_Click_1(object sender, EventArgs e)
        {
            Categorias_UC catsUC = new Categorias_UC();
            addUserControl(catsUC);
        }

        private void btn_users_Click_1(object sender, EventArgs e)
        {
            Usuarios_UC userUC = new Usuarios_UC();
            addUserControl(userUC);
        }


        private void addUserControl(UserControl uc)
        {
            uc.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(uc);
            uc.BringToFront();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
           
        }

        private void Index_Load(object sender, EventArgs e)
        {
            // Establece la posición del panel para que la barra de desplazamiento esté en la parte superior
            //panel1.AutoScrollPosition = new Point(0, 0);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {

// this.WindowState = FormWindowState.Minimized;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cerrar__sesion_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form = new Form1();
            form.Show();
            Program.logueado = null;
            Program.URLimg = EjemploABM.Servicios.ImagenProductoService.RutaBase;
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            Subcategoria_UC subcategoriaUC = new Subcategoria_UC();
            addUserControl(subcategoriaUC);
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void lb_nombre_logueado_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            FormVenta form = new FormVenta();
            form.Show();
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            Ventas_UC venta = new Ventas_UC();
            addUserControl(venta);
        }

    }
}
