using EjemploABM.Controladores;
using EjemploABM.Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EjemploABM.Servicios;

namespace EjemploABM.ControlesDeUsuario
{
    public partial class FormSubcategorias : Form
    {
        string situacion;
        int id_editar;

        public FormSubcategorias()
        {
            InitializeComponent();

            combo_tipo.Items.Clear();

            combo_tipo.Items.Add("Activa");
            combo_tipo.Items.Add("Desactivada");

            situacion = "creacion";

            CargarCategoriasEnComboBoxCrear();

        }

        // SOBRECARGAR EL CONSTRUCTOR PARA INICIAR EL FORM CON LA INFO CARGADA, PARA EDITAR
        public FormSubcategorias(Subcategoria sub)
        {
            InitializeComponent();

            id_editar = sub.Id;


            txt_nombre.Text = sub.Nombre.ToString();

            combo_tipo.Items.Clear();
            combo_tipo.Items.Add("Activa");
            combo_tipo.Items.Add("Desactivada");

            


            if (sub.IsActive == "Activa")
            {
                combo_tipo.SelectedIndex = 0;
            }
            else
            {
                combo_tipo.SelectedIndex = 1;
            }

            situacion = "edicion";

            label2.Text = "Editar Subcategoria";
            btn_crear.Text = "Editar";
            CargarCategoriasEnComboBoxEditar(sub);
        }

        //Maneja el combobox de categorias

        
            public void CargarCategoriasEnComboBoxCrear()
            {
                ComboBoxCat.DisplayMember = "Nombre"; // Establece la propiedad que se mostrará en el ComboBox
                ComboBoxCat.ValueMember = "Id"; // Establece la propiedad que se usará como valor interno
                ComboBoxCat.DataSource = Categoria_Controller.ObtenerCategoriasActivas(); // Asigna la lista de categorías al ComboBox
                ComboBoxCat.SelectedIndex = -1; // Establece la selección actual en vacío (ningún elemento seleccionado)
            }
        

        public void CargarCategoriasEnComboBoxEditar(Subcategoria sub)
        {
            // Obtén la lista de todas las subcategorías y categorías
            List<Subcategoria> subcategorias = Subcategoria_Controller.obtenerSubcategorias();
            List<Categoria> categorias = Categoria_Controller.ObtenerCategoriasActivas();

            // Establece la propiedad que se mostrará en el ComboBox para las categorías
            ComboBoxCat.DisplayMember = "Nombre";
            // Establece la propiedad que se usará como valor interno en el ComboBox para las categorías
            ComboBoxCat.ValueMember = "Id";
            // Asigna la lista de categorías al ComboBox
            ComboBoxCat.DataSource = categorias;

            // Verifica si estamos en modo de edición
            if (situacion == "edicion")
            {
                // Obtiene la subcategoría específica que se está editando
                Subcategoria subcategoriaEditando = subcategorias.FirstOrDefault(s => s.Id == sub.Id);

                // Obtiene el id de la categoría asociada a la subcategoría
                int categoriaIdAsociada = subcategoriaEditando.categoria_id;

                // Encuentra la categoría correspondiente al id de la categoría asociada
                Categoria categoriaAsociada = categorias.FirstOrDefault(c => c.Id == categoriaIdAsociada);

                // Establece la categoría correspondiente como seleccionada en el ComboBoxCat
                ComboBoxCat.SelectedItem = categoriaAsociada;
            }
        }



        private void crear()
        {
            if (!ValidarCampos())
            {
                MessageBox.Show("Por favor, complete todos los campos y seleccione una categoria antes de crear un usuario.", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            String tipo = "";
            if (combo_tipo.SelectedItem.ToString() == "Activa")
            {
                tipo = "Activa";
            }
            if (combo_tipo.SelectedItem.ToString() == "Desactivada")
            {
                tipo = "Desactivada";
            }

            int catId;
            if (!int.TryParse(Convert.ToString(ComboBoxCat.SelectedValue), out catId) || catId <= 0) return;
            
            Subcategoria sub = new Subcategoria(0, txt_nombre.Text,catId ,tipo.ToString());

            EjecutarOperacion(() => Subcategoria_Controller.crearSubcategoria(sub), "crear la subcategoría");
        }

        private void editar()
        {
            if (!ValidarCampos())
            {
                MessageBox.Show("Por favor, complete todos los campos y seleccione una categoría antes de editar la subcategoría.", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tipo = "";
            if (combo_tipo.SelectedItem.ToString() == "Activa")
            {
                tipo = "Activa";
            }
            if (combo_tipo.SelectedItem.ToString() == "Desactivada")
            {
                tipo = "Desactivada";
            }

            // Obtén el ID de la categoría seleccionada del ComboBoxCat
            int catId;
            if (!int.TryParse(Convert.ToString(ComboBoxCat.SelectedValue), out catId) || catId <= 0)
            {
                MessageBox.Show("La categoría seleccionada no es válida.", "Datos no válidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Crea una instancia de Subcategoria con el ID correcto y las propiedades modificadas
            Subcategoria subcategoria = new Subcategoria
            {
                Id = id_editar,
                Nombre = txt_nombre.Text,
                IsActive = tipo,
                categoria_id = catId // Si `categoria_id` también se va a modificar, de lo contrario, puedes omitir esta línea.
            };

            // Llama al controlador para editar la subcategoría
            EjecutarOperacion(() => Subcategoria_Controller.editarSubcategoria(subcategoria), "editar la subcategoría");
        }

        private bool ValidarCampos()
        {
            if (!ValidacionEntrada.Nombre(txt_nombre.Text) || txt_nombre.Text.Trim().Length > 80)
            {
                MessageBox.Show("El nombre debe tener entre 2 y 80 caracteres.", "Datos no válidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (combo_tipo.SelectedItem == null || ComboBoxCat.SelectedItem == null)
            {
                MessageBox.Show("Seleccioná estado y categoría.", "Datos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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



        private void btn_crear_Click_1(object sender, EventArgs e)
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

        private void btnCerrarVentana_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

       
    }
}
