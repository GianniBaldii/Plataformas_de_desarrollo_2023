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
    public partial class FormCategorias : Form
    {
        string situacion;
        int id_editar;

        public FormCategorias()
        {
            InitializeComponent();

            combo_tipo.Items.Clear();

            combo_tipo.Items.Add("Activa");
            combo_tipo.Items.Add("Desactivada");

            situacion = "creacion";

        }

        // SOBRECARGAR EL CONSTRUCTOR PARA INICIAR EL FORM CON LA INFO CARGADA, PARA EDITAR
        public FormCategorias(Categoria cat)
        {
            InitializeComponent();

            id_editar = cat.Id;


            txt_nombre.Text = cat.Nombre.ToString();

            combo_tipo.Items.Clear();
            combo_tipo.Items.Add("Activa");
            combo_tipo.Items.Add("Desactivada");

            if (cat.IsActive == "Activa")
            {
                combo_tipo.SelectedIndex = 0;
            }
            else
            {
                combo_tipo.SelectedIndex = 1;
            }

            situacion = "edicion";

            label2.Text = "Editar Categoria";
            btn_crear.Text = "Editar";
        }

        private void btn_crear_Click(object sender, EventArgs e)
        {

            if (situacion == "creacion")
            {
                crear();
            }

            if (situacion == "edicion")
            {
                editar();
            }


        }


        private void crear()
        {
            if (!ValidarCampos())
            {
                MessageBox.Show("Por favor, complete todos los campos y seleccione un rol antes de crear un usuario.", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tipo = "";
            if (combo_tipo.SelectedItem.ToString() == "Activa")
            {
                tipo = "Activa";
            }
            else if (combo_tipo.SelectedItem.ToString() == "Desactivada")
            {
                tipo = "Desactivada";
            }

            Categoria cat = new Categoria(0, txt_nombre.Text, tipo.ToString());

            EjecutarOperacion(() => Categoria_Controller.crearCategoria(cat), "crear la categoría");
        }
        private void editar()
        {
            if (!ValidarCampos())
            {
                MessageBox.Show("Por favor, complete todos los campos y seleccione un rol antes de crear un usuario.", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            string tipo = "";
            if (combo_tipo.SelectedItem.ToString() == "Activa")
            {
                tipo = "Activa";
            }
            else if (combo_tipo.SelectedItem.ToString() == "Desactivada")
            {
                tipo = "Desactivada";
            }

            Categoria cat = new Categoria(id_editar, txt_nombre.Text, tipo.ToString());

            EjecutarOperacion(() => Categoria_Controller.editarCategoria(cat), "editar la categoría");
        }

        private bool ValidarCampos()
        {
            if (!ValidacionEntrada.Nombre(txt_nombre.Text) || txt_nombre.Text.Trim().Length > 80)
            {
                MessageBox.Show("El nombre debe tener entre 2 y 80 caracteres.", "Datos no válidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (combo_tipo.SelectedItem == null)
            {
                MessageBox.Show("Seleccioná el estado de la categoría.", "Datos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void EjecutarOperacion(Func<bool> operacion, string accion)
        {
            try
            {
                if (operacion()) DialogResult = DialogResult.OK;
                else MessageBox.Show("No se pudo " + accion + ".", "Operación no realizada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo " + accion + ":\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnCerrarVentana_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

       


        private void combo_tipo_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
