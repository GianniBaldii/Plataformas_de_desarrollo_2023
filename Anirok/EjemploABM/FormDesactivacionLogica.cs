using EjemploABM.Controladores;
using EjemploABM.Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EjemploABM.Servicios;

namespace EjemploABM
{
    public partial class FormDesactivacionLogica : Form
    {
        Categoria cat;
        int id_editar;
        public FormDesactivacionLogica(Categoria cat)
        {
            InitializeComponent();

            id_editar = cat.Id;
            CargarCategoriasEnComboBoxCrear();
        }

        private void btnCerrarVentana_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_confirmar_Click(object sender, EventArgs e)
        {
            try
            {
                int catId;
                if (!int.TryParse(Convert.ToString(ComboBoxCat.SelectedValue), out catId) || catId <= 0)
                {
                    MessageBox.Show("Seleccioná una categoría de destino válida.", "Datos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Categoria_Controller.DesactivarCategoria(id_editar);
                Producto_Controller.CambiarCategoriaDeProductos(id_editar, catId);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo desactivar la categoría y mover sus productos:\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void CargarCategoriasEnComboBoxCrear()
        {
            List<Categoria> categorias = Categoria_Controller.ObtenerCategoriasActivas();

            ComboBoxCat.DisplayMember = "Nombre"; // Establece la propiedad que se mostrará en el ComboBox
            ComboBoxCat.ValueMember = "Id"; // Establece la propiedad que se usará como valor interno
            ComboBoxCat.DataSource = categorias; // Asigna la lista de categorías al ComboBox
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void ComboBoxCat_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
