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
            PrepararPresentacion();

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
            PrepararPresentacion();

            id_editar = prod.Id;
            btn_cargar_img.Show();

            // Asigna los valores del producto a los controles correspondientes
            txt_nombre.Text = prod.Nombre;
            txt_descripcion.Text = prod.Descripcion;
            txt_precio.Text = prod.Precio.ToString();
            txt_precio.ReadOnly = true;
            txt_precio.FillColor = Color.FromArgb(242, 242, 242);
            txt_precio.BorderColor = Color.FromArgb(190, 190, 190);
            label3.Text = "Precio (consulta):";
            var ayudaPrecio = new ToolTip();
            ayudaPrecio.SetToolTip(txt_precio, "El precio se actualiza desde el módulo Precios y costos.");
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

        private void PrepararPresentacion()
        {
            EstiloAplicacion.Aplicar(this);
            EstiloAplicacion.BotonPrimario(btn_confirmar);
            EstiloAplicacion.BotonSecundario(btn_cargar_img);
            Text = "Anirok - Producto";
            AcceptButton = btn_confirmar;
            btn_confirmar.Text = "GUARDAR";
            guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
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

            if (!ValidarCamposProducto())
            {
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

            decimal precioDecimal;
            if (ValidacionEntrada.DecimalPositivo(txt_precio.Text, out precioDecimal) && precioDecimal <= 1000000000m)
            {
                float precio = (float)precioDecimal;
                int cantidad;
                if (ValidacionEntrada.EnteroNoNegativo(txt_cantidad.Text, out cantidad))
                {
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
            }
        }

        private void crear()
        {
            if (!ValidarCamposProducto())
            {
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

            decimal precioDecimal;
            if (ValidacionEntrada.DecimalPositivo(txt_precio.Text, out precioDecimal) && precioDecimal <= 1000000000m)
            {
                float precio = (float)precioDecimal;
                int cantidad;
                if (ValidacionEntrada.EnteroNoNegativo(txt_cantidad.Text, out cantidad))
                {
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
            }
        }

        private bool ValidarCamposProducto()
        {
            if (!ValidacionEntrada.Nombre(txt_nombre.Text) || txt_nombre.Text.Trim().Length > 100)
                return MostrarValidacion("El nombre debe tener entre 2 y 100 caracteres.");
            if (!ValidacionEntrada.Requerido(txt_descripcion.Text) || txt_descripcion.Text.Trim().Length > 500)
                return MostrarValidacion("La descripción es obligatoria y no puede superar 500 caracteres.");
            decimal precio;
            if (!ValidacionEntrada.DecimalPositivo(txt_precio.Text, out precio) || precio > 1000000000m)
                return MostrarValidacion("Ingresá un precio mayor a cero.");
            if (!ValidacionEntrada.Requerido(txt_codigo.Text) || txt_codigo.Text.Trim().Length > 50)
                return MostrarValidacion("Ingresá un código de producto válido.");
            int stock;
            if (!ValidacionEntrada.EnteroNoNegativo(txt_cantidad.Text, out stock))
                return MostrarValidacion("El stock debe ser un número entero igual o mayor a cero.");
            if (!ValidacionEntrada.Requerido(txt_proveedor.Text) || txt_proveedor.Text.Trim().Length > 100)
                return MostrarValidacion("Ingresá un proveedor válido.");
            if (comboBoxCat.SelectedItem == null || comboBoxSub.SelectedItem == null || comboBoxTalle.SelectedItem == null)
                return MostrarValidacion("Seleccioná categoría, subcategoría y talle.");
            return true;
        }

        private bool MostrarValidacion(string mensaje)
        {
            MessageBox.Show(mensaje, "Datos no válidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
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

