using EjemploABM.Controladores;
using EjemploABM.ControlesDeUsuario;
using EjemploABM.Modelo;
using MaterialSkin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO; // Asegúrate de tener esta línea
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Resources.ResXFileRef;
using EjemploABM.Servicios;

namespace EjemploABM
{
    public partial class FormProducto : Form
    {
        Image File;

        string situacion;
        int id_editar;
        string nombrefoto;
        string rutaImagenSeleccionada;
        string imagenAnterior;

        public FormProducto()
        {
            InitializeComponent();

            comboBoxTalle.Items.Clear();

            comboBoxTalle.Items.Add("S");
            comboBoxTalle.Items.Add("M");
            comboBoxTalle.Items.Add("L");
            comboBoxTalle.Items.Add("XL");
            comboBoxTalle.Items.Add("No tiene");
            CargarCategoriasEnComboBoxCrear();
            comboBoxCat.SelectedIndexChanged += ComboBoxCat_SelectedIndexChanged;
            situacion = "creacion";
            guna2PictureBox1.Image = ImagenProductoService.Cargar(null);
        }

        public FormProducto(Producto prod)
        {
            InitializeComponent();

            id_editar = prod.Id;
            btn_cargar_img.Show();

            // Asigna los valores del producto a los controles correspondientes
            txt_nombre.Text = prod.Nombre;
            txt_descripcion.Text = prod.Descripcion;
            txt_precio.Text = prod.Precio.ToString();
            txt_codigo.Text = prod.codigo;
            txt_cantidad.Text = prod.Stock.ToString();
            txt_proveedor.Text = prod.Proveedor;
            nombrefoto = prod.Img;
            imagenAnterior = prod.Img;

            // Selecciona la categoría y subcategoría correspondientes en los ComboBox
            comboBoxTalle.Items.Clear();

            comboBoxTalle.Items.Add("S");
            comboBoxTalle.Items.Add("M");
            comboBoxTalle.Items.Add("L");
            comboBoxTalle.Items.Add("XL");
            comboBoxTalle.Items.Add("No tiene");
            CargarCategoriasEnComboBoxCrear();
            comboBoxCat.SelectedIndexChanged += ComboBoxCat_SelectedIndexChanged;

            comboBoxCat.SelectedValue = prod.CategoriaId;
            comboBoxSub.SelectedValue = prod.SubcategoriaId;

            // Selecciona el talle correspondiente en el ComboBox
            comboBoxTalle.SelectedItem = prod.Talle;

            guna2PictureBox1.Image = ImagenProductoService.Cargar(nombrefoto);

            label2.Text = "Editar Producto";

            situacion = "edicion";
        }

        // SOBRECARGAR EL CONSTRUCTOR PARA INICIAR EL FORM CON LA INFO CARGADA, PARA EDITAR

        private void btn_cargar_img_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Liberar recursos de la imagen actual antes de cargar la nueva
                if (guna2PictureBox1.Image != null)
                {
                    guna2PictureBox1.Image.Dispose();
                }

                try
                {
                    rutaImagenSeleccionada = ofd.FileName;
                    File = ImagenProductoService.CargarDesdeRuta(rutaImagenSeleccionada);
                    guna2PictureBox1.Image = File;
                }
                catch (Exception ex)
                {
                    rutaImagenSeleccionada = null;
                    MessageBox.Show(ex.Message, "Imagen no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void editar()
        {
            
            if (id_editar <= 0)
            {
                MessageBox.Show("No se ha seleccionado ningún producto para editar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_nombre.Text) ||
                string.IsNullOrWhiteSpace(txt_descripcion.Text) ||
                string.IsNullOrWhiteSpace(txt_precio.Text) ||
                string.IsNullOrWhiteSpace(txt_codigo.Text) ||
                string.IsNullOrWhiteSpace(txt_cantidad.Text) ||
                string.IsNullOrWhiteSpace(txt_proveedor.Text) ||
                comboBoxTalle.SelectedItem == null)
            {
                MessageBox.Show("Por favor, complete todos los campos y seleccione un rol antes de editar el producto.", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string talle = comboBoxTalle.SelectedItem?.ToString() ?? "No tiene";

            int catId;
            if (!int.TryParse(comboBoxCat.SelectedValue?.ToString(), out catId) || catId == 0)
            {
                MessageBox.Show("ID categoría no válido.", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int subId;
            if (!int.TryParse(comboBoxSub.SelectedValue?.ToString(), out subId))
            {
                MessageBox.Show("ID subcategoría nulo debido a que no existen subcategorias de esa categoria", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            float precio;
            if (float.TryParse(txt_precio.Text, out precio))
            {
                int cantidad;
                if (int.TryParse(txt_cantidad.Text, out cantidad))
                {
                    if (precio <= 0 || cantidad < 0)
                    {
                        MessageBox.Show("El precio debe ser mayor a cero y el stock no puede ser negativo.", "Valores no válidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string imagenImportada = null;
                    bool actualizacionGuardada = false;
                    try
                    {
                        string nuevoNombreFoto = nombrefoto;
                        bool imagenNueva = !string.IsNullOrWhiteSpace(rutaImagenSeleccionada);
                        if (imagenNueva)
                        {
                            nuevoNombreFoto = ImagenProductoService.Importar(rutaImagenSeleccionada, id_editar);
                            imagenImportada = nuevoNombreFoto;
                        }

                        Producto productoActualizado = new Producto(id_editar, txt_nombre.Text, txt_descripcion.Text, precio, txt_codigo.Text, nuevoNombreFoto, txt_proveedor.Text, subId, catId, talle, cantidad);

                        // Intentar actualizar el producto
                        if (Producto_Controller.editarProducto(productoActualizado))
                        {
                            actualizacionGuardada = true;
                            if (imagenNueva && !string.Equals(imagenAnterior, nuevoNombreFoto, StringComparison.OrdinalIgnoreCase) && Producto_Controller.ContarProductosConImagen(imagenAnterior) == 0)
                                ImagenProductoService.Eliminar(imagenAnterior);
                            this.DialogResult = DialogResult.OK;
                            MessageBox.Show("Producto actualizado con éxito.");
                        }
                        else
                        {
                            if (imagenNueva) ImagenProductoService.Eliminar(nuevoNombreFoto);
                            MessageBox.Show("Error al actualizar el producto.");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!actualizacionGuardada && !string.IsNullOrWhiteSpace(imagenImportada))
                            ImagenProductoService.Eliminar(imagenImportada);
                        MessageBox.Show($"Error al actualizar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("El valor de la cantidad no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("El valor del precio no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void crear()
        {
            if (string.IsNullOrWhiteSpace(txt_nombre.Text) ||
                string.IsNullOrWhiteSpace(txt_descripcion.Text) ||
                string.IsNullOrWhiteSpace(txt_precio.Text) ||
                string.IsNullOrWhiteSpace(txt_codigo.Text) ||
                string.IsNullOrWhiteSpace(txt_cantidad.Text) ||
                string.IsNullOrWhiteSpace(txt_proveedor.Text) ||
                comboBoxTalle.SelectedItem == null)
            {
                MessageBox.Show("Por favor, complete todos los campos y seleccione un rol antes de crear un usuario.", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string talle = comboBoxTalle.SelectedItem?.ToString() ?? "No tiene";

            int catId;
            if (!int.TryParse(comboBoxCat.SelectedValue?.ToString(), out catId) || catId == 0)
            {
                MessageBox.Show("ID categoría no válido.", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int subId;
            if (!int.TryParse(comboBoxSub.SelectedValue?.ToString(), out subId))
            {
                MessageBox.Show("ID subcategoría nulo debido a que no existen subcategorias de esa categoria", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            float precio;
            if (float.TryParse(txt_precio.Text, out precio))
            {
                int cantidad;
                if (int.TryParse(txt_cantidad.Text, out cantidad))
                {
                    if (precio <= 0 || cantidad < 0)
                    {
                        MessageBox.Show("El precio debe ser mayor a cero y el stock no puede ser negativo.", "Valores no válidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string imagenImportada = null;
                    bool productoCreado = false;
                    try
                    {
                        if (string.IsNullOrWhiteSpace(rutaImagenSeleccionada))
                        {
                            MessageBox.Show("Seleccioná una imagen para el producto.", "Imagen requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int nuevoProductoId = Producto_Controller.obtenerMaxId() + 1;
                        string nombrefoto = ImagenProductoService.Importar(rutaImagenSeleccionada, nuevoProductoId);
                        imagenImportada = nombrefoto;

                        // Intentar crear el producto
                        if (Producto_Controller.crearProducto(new Producto(0, txt_nombre.Text, txt_descripcion.Text, precio, txt_codigo.Text, nombrefoto, txt_proveedor.Text, subId, catId, talle, cantidad)))
                        {
                            productoCreado = true;
                            this.DialogResult = DialogResult.OK;
                            MessageBox.Show("Producto y imagen guardados con éxito.");
                        }
                        else
                        {
                            ImagenProductoService.Eliminar(nombrefoto);
                            MessageBox.Show("Error al crear el producto.");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!productoCreado && !string.IsNullOrWhiteSpace(imagenImportada))
                            ImagenProductoService.Eliminar(imagenImportada);
                        MessageBox.Show($"Error al crear el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("El valor de la cantidad no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("El valor del precio no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarDetallesProducto(Producto producto)
        {
            MessageBox.Show(
                "Nombre: " + producto.Nombre +
                "\nDescripción: " + producto.Descripcion +
                "\nPrecio: " + producto.Precio +
                "\nCódigo: " + producto.codigo +
                "\nCantidad: " + producto.Stock +
                "\nFoto: " + producto.Img +
                "\nTalle: " + producto.Talle +
                "\nProveedor: " + producto.Proveedor +
                "\nCategoría ID: " + producto.CategoriaId +
                "\nSubcategoría ID: " + producto.SubcategoriaId
            );
        }

        public void CargarCategoriasEnComboBoxCrear()
        {
            comboBoxCat.DisplayMember = "Nombre"; // Establece la propiedad que se mostrará en el ComboBox
            comboBoxCat.ValueMember = "Id"; // Establece la propiedad que se usará como valor interno
            comboBoxCat.DataSource = Categoria_Controller.ObtenerCategoriasActivas(); // Asigna la lista de categorías al ComboBox
            comboBoxCat.SelectedIndex = -1; // Establece la selección actual en vacío (ningún elemento seleccionado)
        }

        private void ComboBoxCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCat.SelectedIndex != -1)
            {
                int categoriaId = (int)comboBoxCat.SelectedValue; // Obtenemos el Id de la categoría seleccionada
                CargarSubcategoriasEnComboBoxCrear(categoriaId);
                comboBoxSub.Visible = true;
            }
            else
            {
                comboBoxSub.Visible = false; // Ocultar el ComboBox de subcategorías si no hay categoría seleccionada
            }
        }

        public void CargarSubcategoriasEnComboBoxCrear(int categoriaId)
        {
            comboBoxSub.DisplayMember = "Nombre"; // Establece la propiedad que se mostrará en el ComboBox
            comboBoxSub.ValueMember = "Id"; // Establece la propiedad que se usará como valor interno
            comboBoxSub.DataSource = Subcategoria_Controller.ObtenerSubcategoriasPorCategoria(categoriaId); // Asigna las subcategorías filtradas por categoría al ComboBox
            comboBoxSub.SelectedIndex = -1; // Establece la selección actual en vacío (ningún elemento seleccionado)
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void btn_confirmar_Click(object sender, EventArgs e)
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

        private void lbl_nombre_Click(object sender, EventArgs e)
        {

        }

        private void txt_nombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_descripcion_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_precio_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_cantidad_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxCat_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}



        //public void CargarSubcategoriasEnComboBoxCrear()
        //{


        //comboBoxSub.DisplayMember = "Nombre"; // Establece la propiedad que se mostrará en el ComboBox
        //comboBoxSub.ValueMember = "Id"; // Establece la propiedad que se usará como valor interno
        //comboBoxSub.DataSource = (); // Asigna la lista de subcategorías al ComboBox
        //comboBoxCat.SelectedIndex = -1; // Establece la selección actual en vacío (ningún elemento seleccionado)
        //}

