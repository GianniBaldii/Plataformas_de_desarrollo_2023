using EjemploABM.Controladores;
using EjemploABM.Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EjemploABM.Servicios;


namespace EjemploABM
{
    public partial class FormVerProd : Form
    {

        int id_ver;
        public FormVerProd()
        {
            InitializeComponent();

        }

        public FormVerProd(Producto prod)
        {
            InitializeComponent();
            txt_id.Text = prod.Id.ToString();   
            txt_nombre.Text= prod.Nombre.ToString();
            txt_descripcion.Text= prod.Descripcion.ToString();
            txt_precio.Text= prod.Precio.ToString();
            txt_codigo.Text= prod.codigo.ToString();
            txt_proveedor.Text = prod.Proveedor.ToString();
            txt_subcategoria.Text = Subcategoria_Controller.obtenerSubcategoriaPorId(prod.SubcategoriaId);
            txt_categoria.Text= Categoria_Controller.obtenerCategoriaPorId(prod.CategoriaId);
            txt_talle.Text= prod.Talle.ToString();
            txt_stock.Text= prod.Stock.ToString();

            pictureBoxVer.Image = ImagenProductoService.Cargar(prod.Img);

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void btnCerrarVentana_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void txt_precio_Click(object sender, EventArgs e)
        {

        }
    }
}
