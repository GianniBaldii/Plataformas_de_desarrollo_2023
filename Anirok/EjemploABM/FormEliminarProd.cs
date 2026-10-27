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
    public partial class FormEliminarProd : Form
    {
        int id_eliminar;
        string imagenEliminar;





        private Producto prod;
        public FormEliminarProd()
        {
            InitializeComponent();
        }


        public FormEliminarProd(Producto prod_eliminar)
        {
            InitializeComponent();
            id_eliminar = prod_eliminar.Id;
            imagenEliminar = prod_eliminar.Img;
            txt_id_prod.Text = id_eliminar.ToString();
            pictureBoxEliminar.Image = ImagenProductoService.Cargar(imagenEliminar);
        }

        private void btn_sieliminar_Click(object sender, EventArgs e)
        {
            ConfirmarEliminacion();
        }

        private void btn_noeliminar_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnCerrarVentana_Click(object sender, EventArgs e)
        {
            this.Hide();
        }



        private void ConfirmarEliminacion()
        {
            // Realiza la eliminación de la categoría aquí...
            if (Producto_Controller.eliminarProducto(id_eliminar))
            {
                if (Producto_Controller.ContarProductosConImagen(imagenEliminar) == 0)
                    ImagenProductoService.Eliminar(imagenEliminar);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

    }
}
