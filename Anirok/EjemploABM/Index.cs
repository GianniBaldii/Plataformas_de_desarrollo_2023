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
using EjemploABM.Servicios;
using Guna.UI2.WinForms;

namespace EjemploABM
{
    public partial class Index : Form
    {
        private bool userScrolled = false;
        private readonly Guna2Button btnProveedores = new Guna2Button();
        public Index()
        {
            InitializeComponent();
            EstiloAplicacion.Aplicar(this);
            Text = "Anirok - Administración";
            panel1.MinimumSize = Size.Empty;
            panel1.MaximumSize = Size.Empty;
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Size = new Size(ClientSize.Width - panel1.Left - 32, ClientSize.Height - panel1.Top - 28);
            Resize += (sender, args) => AcomodarMenu();

            EstiloAplicacion.BotonPrimario(guna2Button8);
            EstiloAplicacion.BotonSecundario(btn_prods);
            EstiloAplicacion.BotonSecundario(btn_cats);
            EstiloAplicacion.BotonSecundario(guna2Button5);
            EstiloAplicacion.BotonSecundario(guna2Button9);
            EstiloAplicacion.BotonSecundario(btn_users);
            EstiloAplicacion.BotonPeligro(cerrar__sesion_btn);
            AcomodarMenu();

            cerrar__sesion_btn.Text = "CERRAR SESIÓN";
            guna2Button7.Text = "PRECIOS Y COSTOS";
            guna2Button7.Click += guna2Button7_Click;
            btnProveedores.Text = "VER PROVEEDORES";
            Controls.Add(btnProveedores);
            EstiloAplicacion.BotonSecundario(btnProveedores);
            btnProveedores.Click += (s, e) => addUserControl(new Proveedores_UC());
            AcomodarMenu();

            
            Productos_UC prodsUC = new Productos_UC();
            addUserControl(prodsUC);
            
            lb_nombre_logueado.Text = Program.logueado == null ? "Usuario" : Program.logueado.Nombre;
            if (Program.logueado == null || Program.logueado.Rol != 1)
            {
                btn_users.Hide();
            }


        }

        private void AcomodarMenu()
        {
            int ancho = ClientSize.Width;
            int margen = 32;
            int botonAncho = 190;
            int botonAlto = 42;
            int separacion = 14;
            int cantidadColumnas = 4;
            int grupoAncho = cantidadColumnas * botonAncho + (cantidadColumnas - 1) * separacion;
            int inicioX = Math.Max(margen, (ancho - grupoAncho) / 2);

            cerrar__sesion_btn.SetBounds(margen, 24, 150, 52);
            cerrar__sesion_btn.ImageSize = new Size(30, 30);

            guna2CirclePictureBox1.SetBounds(Math.Max(margen, (ancho - 160) / 2), 12, 160, 108);
            label1.Location = new Point(Math.Max(margen, ancho - 300), 34);
            lb_nombre_logueado.Location = new Point(Math.Max(margen, ancho - 150), 32);

            guna2Button8.SetBounds(inicioX, 150, botonAncho, botonAlto);
            btn_prods.SetBounds(inicioX + botonAncho + separacion, 150, botonAncho, botonAlto);
            btn_cats.SetBounds(inicioX + (botonAncho + separacion) * 2, 150, botonAncho, botonAlto);
            guna2Button5.SetBounds(inicioX + (botonAncho + separacion) * 3, 150, botonAncho, botonAlto);

            guna2Button9.SetBounds(inicioX + botonAncho + separacion, 202, botonAncho, botonAlto);
            guna2Button7.SetBounds(inicioX, 202, botonAncho, botonAlto);
            btn_users.SetBounds(inicioX + (botonAncho + separacion) * 2, 202, botonAncho, botonAlto);
            btnProveedores.SetBounds(inicioX + (botonAncho + separacion) * 3, 202, botonAncho, botonAlto);

            panel1.SetBounds(margen, 270, Math.Max(500, ancho - margen * 2), Math.Max(250, ClientSize.Height - 292));
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
            EstiloAplicacion.AplicarAControl(uc);
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
            using (FormVenta form = new FormVenta())
                form.ShowDialog(this);
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            Ventas_UC venta = new Ventas_UC();
            addUserControl(venta);
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            addUserControl(new PreciosCostos_UC());
        }

    }
}
