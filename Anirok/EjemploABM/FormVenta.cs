using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using EjemploABM.Controladores;
using EjemploABM.Modelo;
using EjemploABM.Servicios;


namespace EjemploABM
{
    public partial class FormVenta : Form
    {
        private List<Producto> productosDisponibles;
        private List<Producto> productosEnCarrito;
        private int elementosPorPagina = 7;
        private int paginaActual = 1;
        private int totalDePaginas;
        private decimal subtotal = 0;
        private decimal descuento = 0;
        private decimal total = 0;
        private bool clienteExistente = false;

        public FormVenta()
        {
            InitializeComponent();
            CargarProductos();
            txtBusqueda.TextChanged += TxtBusqueda_TextChanged;
            productosEnCarrito = new List<Producto>();
            dataGridViewCarrito.CellContentClick += dataGridViewCarrito_CellClick;
            dataGridViewDisponibles.CellContentClick += dataGridViewDisponibles_CellContentClick_1;
            txt_descuento.TextChanged += txtDescuento_TextChanged;

            // Asegúrate de que estos controles estén creados en tu formulario
            // Actualizar el label del total final
            txt_total.Text = $"Total: {total:N2}";

            // Actualizar el label del descuento
            txt_descuento.Text = $"{descuento:N2}";
        }

        private void CargarMetodosDePago()
        {
            combobox_metododepago.Items.Add("Efectivo");
            combobox_metododepago.Items.Add("Tarjeta");
            combobox_metododepago.Items.Add("Transferencia");
            combobox_metododepago.SelectedIndex = 0; // Selecciona la primera opción por defecto
        }

        private void TxtBusqueda_TextChanged(object sender, EventArgs e)
        {
            FiltrarProductosPorNombre(txtBusqueda.Text);
        }

        private void FiltrarProductosPorNombre(string nombre)
        {
            if (productosDisponibles != null)
            {
                List<Producto> productosFiltrados = productosDisponibles
                    .Where(prod => prod.Nombre.ToLower().Contains(nombre.ToLower()))
                    .ToList();

                int totalDePaginasFiltradas = (int)Math.Ceiling((double)productosFiltrados.Count / elementosPorPagina);
                int inicio = (paginaActual - 1) * elementosPorPagina;
                int fin = Math.Min(inicio + elementosPorPagina, productosFiltrados.Count);

                dataGridViewDisponibles.Rows.Clear();

                for (int i = inicio; i < fin; i++)
                {
                    Producto prod = productosFiltrados[i];
                    int rowIndex = dataGridViewDisponibles.Rows.Add();

                    dataGridViewDisponibles.Rows[rowIndex].Cells[0].Value = prod.Id.ToString();
                    dataGridViewDisponibles.Rows[rowIndex].Cells[1].Value = prod.Nombre.ToString();
                    dataGridViewDisponibles.Rows[rowIndex].Cells[2].Value = prod.Precio.ToString();
                    dataGridViewDisponibles.Rows[rowIndex].Cells[3].Value = "Ver";
                    dataGridViewDisponibles.Rows[rowIndex].Cells[4].Value = "Agregar";


                }

                lblPaginaActual.Text = $"Página {paginaActual} de {totalDePaginasFiltradas}";
            }
        }

        private void CargarProductos()
        {
            productosDisponibles = Producto_Controller.obtenerProductos();

            if (productosDisponibles != null)
            {
                totalDePaginas = (int)Math.Ceiling((double)productosDisponibles.Count / elementosPorPagina);
                int inicio = (paginaActual - 1) * elementosPorPagina;
                int fin = Math.Min(inicio + elementosPorPagina, productosDisponibles.Count);

                dataGridViewDisponibles.Rows.Clear();

                for (int i = inicio; i < fin; i++)
                {
                    Producto prod = productosDisponibles[i];
                    int rowIndex = dataGridViewDisponibles.Rows.Add();

                    dataGridViewDisponibles.Rows[rowIndex].Cells[0].Value = prod.Id.ToString();
                    dataGridViewDisponibles.Rows[rowIndex].Cells[1].Value = prod.Nombre.ToString();
                    dataGridViewDisponibles.Rows[rowIndex].Cells[2].Value = prod.Precio.ToString();
                    dataGridViewDisponibles.Rows[rowIndex].Cells[3].Value = "Ver";
                    dataGridViewDisponibles.Rows[rowIndex].Cells[4].Value = "Agregar";
                }

                lblPaginaActual.Text = $"Página {paginaActual} de {totalDePaginas}";
            }
            else
            {
                MessageBox.Show("No se pudieron cargar los productos. La lista es nula.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_anterior_Click_1(object sender, EventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                CargarProductos();
            }
        }

        private void btn_siguiente_Click_1(object sender, EventArgs e)
        {
            if ((paginaActual * elementosPorPagina) < productosDisponibles.Count)
            {
                paginaActual++;
                CargarProductos();
            }
        }

        private void dataGridViewDisponibles_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridViewDisponibles.Columns["Agregar"].Index && e.RowIndex >= 0)
            {
                int id2 = Convert.ToInt32(dataGridViewDisponibles.Rows[e.RowIndex].Cells[0].Value);
                Producto prod_elim = Producto_Controller.obtenerPorId(id2);

                // Verificar si el producto ya está en el carrito
                if (!productosEnCarrito.Any(p => p.Id == prod_elim.Id))
                {
                    // Agregar el producto al carrito
                    AgregarProductoAlCarrito(prod_elim);

                    // Actualizar la vista de productos disponibles
                    CargarProductos();
                }
            }
            else if (e.ColumnIndex == dataGridViewDisponibles.Columns["Ver"].Index && e.RowIndex >= 0)
            {
                int id2 = Convert.ToInt32(dataGridViewDisponibles.Rows[e.RowIndex].Cells[0].Value);
                Trace.WriteLine("el id es: " + id2);
                Producto prod_elim = Producto_Controller.obtenerPorId(id2);
                FormVerProd formverprod = new FormVerProd(prod_elim);
                DialogResult ver = formverprod.ShowDialog();
            }
        }

        private void dataGridViewCarrito_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Verifica que se haya hecho clic en una celda válida
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // Obtiene el valor de la celda "Id"
                    int id = Convert.ToInt32(dataGridViewCarrito.Rows[e.RowIndex].Cells[0].Value);

                    // Busca el producto en la lista del carrito
                    Producto producto = productosEnCarrito.FirstOrDefault(p => p.Id == id);

                    if (producto != null)
                    {
                        // Verifica si se hizo clic en el botón "Quitar"
                        if (dataGridViewCarrito.Columns[e.ColumnIndex].Name == "Quitar")
                        {
                            // Quitar el producto del carrito
                            QuitarProductoDelCarrito(producto);
                        }
                        

                        // Actualiza el DataGridView del carrito
                        CargarProductosEnCarrito();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al intentar quitar o ver el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void QuitarProductoDelCarrito(Producto producto)
        {
            // Crea una copia de la lista actual para evitar modificaciones concurrentes
            List<Producto> productosAEliminar = new List<Producto>(productosEnCarrito);

            // Encuentra y elimina el producto de la lista
            Producto productoAEliminar = productosAEliminar.FirstOrDefault(p => p.Id == producto.Id);

            if (productoAEliminar != null)
            {
                productosEnCarrito.Remove(productoAEliminar);

                // Actualiza el botón en el DataGridView de productos disponibles
                ActualizarBotonEnProductosDisponibles(producto.Id, "Agregar");

                // Actualiza el subtotal
                subtotal -= Convert.ToDecimal(producto.Precio);
                txt_subtotal.Text = $"Subtotal: {subtotal:C}";

                // Actualiza el total final teniendo en cuenta el descuento
                ActualizarTotalFinal();

                // Actualiza el DataGridView del carrito
                CargarProductosEnCarrito();
            }
        }

        private void CargarProductosEnCarrito()
        {
            // Limpia el DataGridView del carrito
            dataGridViewCarrito.Rows.Clear();

            // Agrega los productos actuales en el carrito al DataGridView
            foreach (Producto producto in productosEnCarrito)
            {
                int rowIndex = dataGridViewCarrito.Rows.Add();
                dataGridViewCarrito.Rows[rowIndex].Cells[0].Value = producto.Id.ToString();
                dataGridViewCarrito.Rows[rowIndex].Cells[1].Value = producto.Nombre.ToString();
                dataGridViewCarrito.Rows[rowIndex].Cells[2].Value = producto.Precio.ToString();
                dataGridViewCarrito.Rows[rowIndex].Cells[3].Value = "Ver";
                dataGridViewCarrito.Rows[rowIndex].Cells[4].Value = "Quitar";
            }
        }


        private void AgregarProductoAlCarrito(Producto producto)
        {
            if (producto.Stock <= 0)
            {
                MessageBox.Show("No hay suficiente stock disponible para este producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Abrir el formulario FormVentaCantidad pasando el stock del producto
            using (FormVentaCantidad formCantidad = new FormVentaCantidad(producto.Stock))
            {
                // Mostrar el formulario como un cuadro de diálogo modal
                DialogResult result = formCantidad.ShowDialog();

                // Verificar si el usuario hizo clic en Confirmar en el formulario FormVentaCantidad
                if (result == DialogResult.OK)
                {
                    // Obtener la cantidad ingresada por el usuario
                    int cantidadSeleccionada = formCantidad.CantidadSeleccionada;

                    // Verificar si la cantidad seleccionada supera el stock disponible
                    if (cantidadSeleccionada <= 0 || cantidadSeleccionada > producto.Stock)
                    {
                        MessageBox.Show("La cantidad seleccionada no es válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Resto del código para agregar el producto al carrito con la cantidad seleccionada
                    producto.CantidadSeleccionada = cantidadSeleccionada;

                    // Agregar el producto al carrito
                    productosEnCarrito.Add(producto);

                    int rowIndex = dataGridViewCarrito.Rows.Add();

                    dataGridViewCarrito.Rows[rowIndex].Cells[0].Value = producto.Id.ToString();
                    dataGridViewCarrito.Rows[rowIndex].Cells[1].Value = producto.Nombre.ToString();
                    dataGridViewCarrito.Rows[rowIndex].Cells[2].Value = producto.Precio.ToString();

                    // Agregar la cantidad al DataGridView
                    dataGridViewCarrito.Rows[rowIndex].Cells[3].Value = cantidadSeleccionada.ToString();

                    // Actualizar el botón en el DataGridView de productos disponibles
                    ActualizarBotonEnProductosDisponibles(producto.Id, "Quitar");

                    // Calcular el subtotal con la cantidad ingresada
                    subtotal += Convert.ToDecimal(producto.Precio) * cantidadSeleccionada;
                    txt_subtotal.Text = $"Subtotal: {subtotal:C}";

                    // Actualizar el total final teniendo en cuenta el descuento
                    ActualizarTotalFinal();
                }
            }
        }



        private void ActualizarBotonEnProductosDisponibles(int productId, string buttonText)
        {
            var row = dataGridViewDisponibles.Rows.Cast<DataGridViewRow>()
                .FirstOrDefault(r => Convert.ToInt32(r.Cells["Id"].Value) == productId);

            if (row != null)
            {
                row.Cells["Agregar"].Value = buttonText;
            }
        }



        private void MostrarClienteEncontrado(Cliente cliente)
        {
            // Limpia los campos antes de mostrar el nuevo cliente
            LimpiarCampos();

            if (cliente != null)
            {
                // Actualiza los TextBox con la información del cliente
                txt_nombre.Text = cliente.Nombre;
                txt_apellido.Text = cliente.Apellido;
                txt_mail.Text = cliente.Mail;
                txt_telefono.Text = cliente.Telefono;
                txt_direccion.Text = cliente.Direccion;
                
            }
            else
            {
                // Muestra un mensaje si el cliente no fue encontrado
                MessageBox.Show("No se encontró un cliente con ese DNI en la base de datos.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LimpiarCampos()
        {
            // Limpia todos los TextBox
            txt_nombre.Text = string.Empty;
            txt_apellido.Text = string.Empty;
            txt_mail.Text = string.Empty;
            txt_telefono.Text = string.Empty;
            txt_direccion.Text = string.Empty;
            
        }

        private void txtDescuento_TextChanged(object sender, EventArgs e)
        {
            // Manejador de eventos para el cambio en el descuento
            // Validar que el descuento ingresado sea un valor numérico
            if (decimal.TryParse(txt_descuento.Text, out decimal nuevoDescuento))
            {
                // Actualizar el descuento
                descuento = nuevoDescuento;

                // Actualizar el total final teniendo en cuenta el nuevo descuento
                ActualizarTotalFinal();
            }
            else
            {
                // Mostrar un mensaje de error si el descuento no es un valor numérico
                MessageBox.Show("Por favor, ingrese un valor numérico válido para el descuento.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarTotalFinal()
        {
            // Calcular el total final restando el descuento del subtotal
            decimal totalFinal = subtotal - descuento;

            // Actualizar el label del total final
            txt_total.Text = $"Total: {totalFinal:C}";
        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtén la información ingresada por el usuario
                string nombre = txt_nombre.Text;
                string apellido = txt_apellido.Text;
                string mail = txt_mail.Text;
                string telefono = txt_telefono.Text;
                string direccion = txt_direccion.Text;
                string dni = txtDni.Text;

                // Validar que la información no sea nula o vacía
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || string.IsNullOrEmpty(mail) ||
                    string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(direccion) || string.IsNullOrEmpty(dni))
                {
                    MessageBox.Show("Por favor, completa todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Sale del método si hay campos vacíos
                }

                int clienteId;

                // Si el cliente no existe, lo creamos
                if (!clienteExistente)
                {
                    if (Cliente_Controller.ExisteClienteConDNI(dni))
                    {
                        MessageBox.Show("Ya existe un cliente con el mismo DNI.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Sale del método si ya existe un cliente con el mismo DNI
                    }

                    Cliente nuevoCliente = new Cliente(0, nombre, apellido, mail, telefono, direccion, dni);
                    Cliente_Controller.crearCliente(nuevoCliente);
                    clienteId = Cliente_Controller.buscarPorDni(dni).Id;

                    MessageBox.Show("Cliente creado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    clienteId = Cliente_Controller.buscarPorDni(dni).Id;
                }

                // Datos de la venta
                float precioTotal = float.Parse(txt_total.Text.Replace("Total: $", "").Trim()); // Convertir a decimal
                DateTime fecha = DateTime.Now; // Fecha actual
                string metodoDePago = combobox_metododepago.SelectedItem.ToString(); // Obtener método de pago
                float descuento = float.Parse(txt_descuento.Text); // Convertir a decimal

                int usuarioId = Program.logueado.Id;

                Venta nuevaVenta = new Venta
                {
                    PrecioTotal = precioTotal,
                    Fecha = fecha,
                    MetodoDePago = metodoDePago,
                    Descuento = descuento,
                    ClienteId = clienteId,
                    UsuarioId = usuarioId
                };

                Venta_Controller.CrearVenta(nuevaVenta);
                int ventaId = Venta_Controller.obtenerMaxId();
                foreach (Producto producto in productosEnCarrito)
                {
                    DetalleVenta detalleVenta = new DetalleVenta
                    {
                        Cantidad = producto.CantidadSeleccionada,
                        VentaId = ventaId,
                        ProductoId = producto.Id,
                        PrecioUnitario = producto.Precio
                    };

                    Detalle_Controller.CrearDetalleVenta(detalleVenta);

                    // Actualizar el stock del producto
                    Producto_Controller.ActualizarStock(producto.Id, producto.CantidadSeleccionada);
                }
                //falla porque falta el campo ID en la BD

                ComprobantePdfService.GenerarDesdeVenta(ventaId);
                MessageBox.Show("La venta se ha generado y el PDF se ha creado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




#if false // Implementación histórica reemplazada por ComprobantePdfService.
        private static void GenerarPDFCliente(Cliente cliente, List<Producto> productosEnCarrito)
        {
            try
            {
                string folderPath = @Program.URLpdf;

                // Verifica si la carpeta existe, créala si no existe
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, $"Cliente_{Cliente_Controller.obtenerMaxId()}.pdf");

                // Ruta del archivo del logo
                string logoPath = @"C:\Users\Usuario\Documents\GitHub\Plataformas_de_desarrollo_2023\Anirok\EjemploABM\Recursos\logo.png";

                // Utiliza bloques using para garantizar la liberación adecuada de recursos
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    // Creamos un nuevo documento PDF
                    Document pdfDoc = new Document();

                    // Creamos el escritor PDF y asociamos el documento con el stream
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);

                    // Abrimos el documento para escritura
                    pdfDoc.Open();

                    // Agregamos contenido al documento

                    // Agregamos una tabla para organizar la información
                    PdfPTable table = new PdfPTable(2);
                    table.WidthPercentage = 80;
                    table.SetWidths(new float[] { 1, 3 });

                    // Agregamos una celda para la imagen del logo
                    PdfPCell cellLogo = new PdfPCell();
                    cellLogo.Colspan = 1;
                    cellLogo.HorizontalAlignment = Element.ALIGN_CENTER;

                    // Agregar la imagen del logo al PDF
                    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                    logo.ScaleToFit(100f, 100f); // Ajusta el tamaño según tus necesidades
                    cellLogo.AddElement(logo);

                    table.AddCell(cellLogo);

                    // Agregamos una celda para la información del cliente
                    PdfPCell cellInfo = new PdfPCell();
                    cellInfo.Colspan = 1;
                    cellInfo.AddElement(new Paragraph($"Factura para: {cliente.Nombre} {cliente.Apellido}"));
                    cellInfo.AddElement(new Paragraph($"ID del Cliente: {cliente.Id}"));
                    cellInfo.AddElement(new Paragraph($"DNI: {cliente.Dni}"));
                    cellInfo.AddElement(new Paragraph($"Fecha: {DateTime.Now.ToString("dd/MM/yyyy")}"));
                    cellInfo.AddElement(new Paragraph($"Teléfono: {cliente.Telefono}"));
                    cellInfo.AddElement(new Paragraph($"Dirección: {cliente.Direccion}"));
                    cellInfo.AddElement(new Paragraph($"Mail: {cliente.Mail}"));
                    cellInfo.HorizontalAlignment = 0; // Alineación a la izquierda
                    table.AddCell(cellInfo);

                    // Agregamos la tabla al documento
                    pdfDoc.Add(table);

                    // Añadimos un espacio en blanco
                    pdfDoc.Add(new Paragraph(" "));

                    // Añadimos una tabla para los detalles (puedes personalizar según tus necesidades)
                    PdfPTable detallesTable = new PdfPTable(5);
                    detallesTable.WidthPercentage = 80;
                    detallesTable.SetWidths(new float[] { 1, 3, 1, 1, 1 });

                    // Encabezado de la tabla de detalles
                    detallesTable.AddCell("Cantidad");
                    detallesTable.AddCell("Descripción");
                    detallesTable.AddCell("Precio Unitario");
                    detallesTable.AddCell("Importe por Cantidad");
                    detallesTable.AddCell("Importe Total");

                    // Agregamos los productos del carrito a la tabla de detalles
                    foreach (Producto producto in productosEnCarrito)
                    {
                        double precioTotalPorCantidad = producto.Precio * producto.CantidadSeleccionada;
                        double precioTotal = precioTotalPorCantidad;

                        detallesTable.AddCell(producto.CantidadSeleccionada.ToString()); // Cantidad
                        detallesTable.AddCell(producto.Nombre); // Descripción
                        detallesTable.AddCell($"${producto.Precio:F2}"); // Precio Unitario
                        detallesTable.AddCell($"${precioTotalPorCantidad:F2}"); // Importe por Cantidad
                        detallesTable.AddCell($"${precioTotal:F2}"); // Importe Total
                    }

                    // Agregamos la tabla de detalles al documento
                    pdfDoc.Add(detallesTable);

                    // Añadimos un espacio en blanco
                    pdfDoc.Add(new Paragraph(" "));

                    // Calculamos el total sumando los precios de los productos del carrito
                    double total = productosEnCarrito.Sum(p => p.Precio * p.CantidadSeleccionada);

                    // Añadimos el total al documento
                    pdfDoc.Add(new Paragraph($"Total: ${total:F2}"));

                    // Cerramos el documento
                    pdfDoc.Close();

                    Console.WriteLine($"PDF generado: {filePath}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar el PDF del cliente: " + ex.Message);
            }
        }

#endif
        private void btn_noeliminar_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Obtén el DNI ingresado por el usuario
                string dni = txtDni.Text;

                // Validar que el campo DNI no esté vacío
                if (string.IsNullOrWhiteSpace(dni))
                {
                    MessageBox.Show("Por favor, ingrese un DNI válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Busca el cliente por DNI
                Cliente clienteEncontrado = Cliente_Controller.buscarPorDni(dni);

                // Verifica si se encontró un cliente
                if (clienteEncontrado != null)
                {
                    // Actualiza los controles de interfaz con la información del cliente encontrado
                    MostrarClienteEncontrado(clienteEncontrado);
                    clienteExistente = true; // Indica que el cliente ya existe
                }
                else
                {
                    // Muestra un mensaje indicando que no se encontró ningún cliente
                    MessageBox.Show("No se encontró ningún cliente con el DNI proporcionado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Puedes agregar lógica adicional aquí, como limpiar los controles de interfaz, etc.
                    clienteExistente = false; // El cliente no existe, se deberá crear
                }
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error si algo sale mal
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void btnCerrarVentana_Click(object sender, EventArgs e)
        {
            this.Close();
        }

     

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}

