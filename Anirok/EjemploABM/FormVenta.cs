using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;
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
        private Label lblCarritoVacio;
        private Label lblEstadoCliente;
        private Guna2Button btnLimpiarCliente;
        private Guna2Button btnConsumidorFinal;

        public FormVenta()
        {
            InitializeComponent();
            EstiloAplicacion.Aplicar(this);
            EstiloAplicacion.BotonPrimario(guna2Button8);
            EstiloAplicacion.BotonSecundario(btn_noeliminar);
            EstiloAplicacion.BotonSecundario(btn_anterior);
            EstiloAplicacion.BotonSecundario(btn_siguiente);
            Text = "Anirok - Nueva venta";
            guna2Button8.Text = "CONFIRMAR VENTA";
            txtBusqueda.PlaceholderText = "Buscar producto...";
            txtDni.PlaceholderText = "DNI del cliente";
            txt_descuento.PlaceholderText = "0,00";
            txt_total.ForeColor = EstiloAplicacion.Rojo;
            txt_total.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            txt_subtotal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            PrepararColumnasVenta();
            PrepararEstadoCarrito();
            CargarMetodosDePago();
            CargarProductos();
            txtBusqueda.TextChanged += TxtBusqueda_TextChanged;
            productosEnCarrito = new List<Producto>();
            dataGridViewDisponibles.CellContentClick += dataGridViewDisponibles_CellContentClick_1;
            txt_descuento.TextChanged += txtDescuento_TextChanged;

            // Asegúrate de que estos controles estén creados en tu formulario
            // Actualizar el label del total final
            txt_total.Text = $"Total: {total:N2}";

            // Actualizar el label del descuento
            txt_descuento.Text = $"{descuento:N2}";
            txt_subtotal.Text = "Subtotal: $ 0,00";
            PrepararPanelCliente();
        }

        private void PrepararPanelCliente()
        {
            lblEstadoCliente = new Label
            {
                AutoSize = false,
                Location = new Point(btn_noeliminar.Left, txtDni.Bottom + 29),
                Size = new Size(btn_noeliminar.Width, 17),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 110, 110),
                Text = "DNI pendiente"
            };
            btnLimpiarCliente = new Guna2Button
            {
                Text = "Limpiar",
                Location = new Point(btn_noeliminar.Left, txtDni.Bottom + 2),
                Size = new Size(btn_noeliminar.Width, 26)
            };
            btnConsumidorFinal = new Guna2Button
            {
                Text = "Consumidor final",
                Location = new Point(txtDni.Left, txtDni.Bottom + 2),
                Size = new Size(txtDni.Width, 26)
            };
            Controls.Add(lblEstadoCliente);
            Controls.Add(btnLimpiarCliente);
            Controls.Add(btnConsumidorFinal);
            EstiloAplicacion.BotonSecundario(btnLimpiarCliente);
            EstiloAplicacion.BotonSecundario(btnConsumidorFinal);
            btnLimpiarCliente.Click += (sender, args) => LimpiarCliente();
            btnConsumidorFinal.Click += (sender, args) => SeleccionarConsumidorFinal();
            lblEstadoCliente.BringToFront();
            btnLimpiarCliente.BringToFront();
            btnConsumidorFinal.BringToFront();
        }

        private void PrepararColumnasVenta()
        {
            if (!dataGridViewDisponibles.Columns.Contains("Stock"))
            {
                var columnaStock = new DataGridViewTextBoxColumn
                {
                    Name = "Stock",
                    HeaderText = "Stock",
                    ReadOnly = true
                };
                dataGridViewDisponibles.Columns.Insert(3, columnaStock);
            }

            if (!dataGridViewCarrito.Columns.Contains("Quitar"))
            {
                var columnaQuitar = new DataGridViewButtonColumn
                {
                    Name = "Quitar",
                    HeaderText = "Acción",
                    Text = "Quitar",
                    UseColumnTextForButtonValue = true,
                    FlatStyle = FlatStyle.Flat,
                    ReadOnly = true
                };
                dataGridViewCarrito.Columns.Add(columnaQuitar);
            }

            dataGridViewDisponibles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCarrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewDisponibles.Columns["Id"].Visible = false;
            dataGridViewCarrito.Columns["dataGridViewTextBoxColumn1"].Visible = false;
            dataGridViewCarrito.ScrollBars = ScrollBars.Vertical;
        }

        private void PrepararEstadoCarrito()
        {
            lblCarritoVacio = new Label
            {
                AutoSize = false,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(120, 120, 120),
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                Text = "Todavía no agregaste productos al carrito.",
                TextAlign = ContentAlignment.MiddleCenter,
                Location = dataGridViewCarrito.Location,
                Size = dataGridViewCarrito.Size,
                Anchor = dataGridViewCarrito.Anchor,
                Visible = true
            };
            Controls.Add(lblCarritoVacio);
            lblCarritoVacio.BringToFront();
            ActualizarEstadoCarrito();
        }

        private void ActualizarEstadoCarrito()
        {
            if (lblCarritoVacio != null)
                lblCarritoVacio.Visible = productosEnCarrito == null || productosEnCarrito.Count == 0;
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

                    CargarFilaProductoDisponible(rowIndex, prod);


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

                    CargarFilaProductoDisponible(rowIndex, prod);
                }

                lblPaginaActual.Text = $"Página {paginaActual} de {totalDePaginas}";
            }
            else
            {
                MessageBox.Show("No se pudieron cargar los productos. La lista es nula.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarFilaProductoDisponible(int rowIndex, Producto producto)
        {
            DataGridViewRow fila = dataGridViewDisponibles.Rows[rowIndex];
            fila.Cells["Id"].Value = producto.Id;
            fila.Cells["Nombre"].Value = producto.Nombre;
            fila.Cells["Precio"].Value = producto.Precio.ToString("C0");
            fila.Cells["Stock"].Value = producto.Stock;
            fila.Cells["Ver"].Value = "Ver";
            Producto enCarrito = productosEnCarrito == null ? null : productosEnCarrito.FirstOrDefault(p => p.Id == producto.Id);
            fila.Cells["Agregar"].Value = enCarrito != null ? "Agregado · " + enCarrito.CantidadSeleccionada : (producto.Stock > 0 ? "Agregar" : "Sin stock");
            fila.DefaultCellStyle.BackColor = enCarrito != null ? Color.FromArgb(232, 247, 237) : Color.White;
            fila.DefaultCellStyle.ForeColor = enCarrito != null ? Color.FromArgb(35, 105, 55) : Color.Black;
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
                else
                {
                    MessageBox.Show("Este producto ya está en el carrito. Podés quitarlo o revisar su cantidad abajo.", "Producto ya agregado", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                subtotal -= Convert.ToDecimal(producto.Precio) * producto.CantidadSeleccionada;
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
                DataGridViewRow fila = dataGridViewCarrito.Rows[rowIndex];
                fila.Cells["dataGridViewTextBoxColumn1"].Value = producto.Id;
                fila.Cells["dataGridViewTextBoxColumn2"].Value = producto.Nombre;
                fila.Cells["dataGridViewTextBoxColumn3"].Value = producto.Precio.ToString("C0");
                fila.Cells["Cantidad"].Value = producto.CantidadSeleccionada;
                fila.Cells["Quitar"].Value = "Quitar";
            }
            ActualizarEstadoCarrito();
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

                    DataGridViewRow fila = dataGridViewCarrito.Rows[rowIndex];
                    fila.Cells["dataGridViewTextBoxColumn1"].Value = producto.Id;
                    fila.Cells["dataGridViewTextBoxColumn2"].Value = producto.Nombre;
                    fila.Cells["dataGridViewTextBoxColumn3"].Value = producto.Precio.ToString("C0");
                    fila.Cells["Cantidad"].Value = cantidadSeleccionada;
                    fila.Cells["Quitar"].Value = "Quitar";

                    // Actualizar el botón en el DataGridView de productos disponibles
                    ActualizarBotonEnProductosDisponibles(producto.Id, "Quitar");

                    // Calcular el subtotal con la cantidad ingresada
                    subtotal += Convert.ToDecimal(producto.Precio) * cantidadSeleccionada;
                    txt_subtotal.Text = $"Subtotal: {subtotal:C}";

                    // Actualizar el total final teniendo en cuenta el descuento
                    ActualizarTotalFinal();
                    ActualizarEstadoCarrito();
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
                bool agregado = buttonText.StartsWith("Agregado", StringComparison.OrdinalIgnoreCase);
                row.DefaultCellStyle.BackColor = agregado ? Color.FromArgb(232, 247, 237) : Color.White;
                row.DefaultCellStyle.ForeColor = agregado ? Color.FromArgb(35, 105, 55) : Color.Black;
            }
        }



        private void MostrarClienteEncontrado(Cliente cliente)
        {
            LimpiarCampos();

            if (cliente != null)
            {
                // Actualiza los TextBox con la información del cliente
                txt_nombre.Text = cliente.Nombre;
                txt_apellido.Text = cliente.Apellido;
                txt_mail.Text = cliente.Mail;
                txt_telefono.Text = cliente.Telefono;
                txt_direccion.Text = cliente.Direccion;
                clienteExistente = true;
                ConfigurarCamposCliente(true);
                ActualizarEstadoCliente("Encontrado", Color.FromArgb(35, 125, 70));
            }
            else
            {
                // Muestra un mensaje si el cliente no fue encontrado
                clienteExistente = false;
                ConfigurarCamposCliente(false);
                ActualizarEstadoCliente("Cliente nuevo", EstiloAplicacion.Rojo);
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
            ConfigurarCamposCliente(false);
        }

        private void ConfigurarCamposCliente(bool soloLectura)
        {
            txt_nombre.ReadOnly = soloLectura;
            txt_apellido.ReadOnly = soloLectura;
            txt_mail.ReadOnly = soloLectura;
            txt_telefono.ReadOnly = soloLectura;
            txt_direccion.ReadOnly = soloLectura;
        }

        private void LimpiarCliente()
        {
            clienteExistente = false;
            txtDni.Clear();
            LimpiarCampos();
            ActualizarEstadoCliente("DNI pendiente", Color.FromArgb(110, 110, 110));
            txtDni.Focus();
        }

        private void SeleccionarConsumidorFinal()
        {
            try
            {
                Cliente cliente = Cliente_Controller.buscarPorDni("00000000");
                if (cliente == null)
                {
                    clienteExistente = false;
                    txtDni.Text = "00000000";
                    txt_nombre.Text = "Consumidor";
                    txt_apellido.Text = "Final";
                    txt_mail.Text = "consumidor@anirok.local";
                    txt_telefono.Text = "0000000000";
                    txt_direccion.Text = "Venta en local";
                    ConfigurarCamposCliente(false);
                    ActualizarEstadoCliente("Consumidor final", Color.FromArgb(110, 110, 110));
                }
                else
                {
                    MostrarClienteEncontrado(cliente);
                    ActualizarEstadoCliente("Consumidor final", Color.FromArgb(35, 125, 70));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo seleccionar consumidor final:\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarEstadoCliente(string texto, Color color)
        {
            if (lblEstadoCliente != null)
            {
                lblEstadoCliente.Text = texto;
                lblEstadoCliente.ForeColor = color;
            }
        }

        private void txtDescuento_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_descuento.Text))
            {
                descuento = 0;
                ActualizarTotalFinal();
                return;
            }

            decimal nuevoDescuento;
            if (decimal.TryParse(txt_descuento.Text, out nuevoDescuento) && nuevoDescuento >= 0)
            {
                descuento = nuevoDescuento;
                ActualizarTotalFinal();
            }
        }

        private void ActualizarTotalFinal()
        {
            // Calcular el total final restando el descuento del subtotal
            decimal totalFinal = Math.Max(0, subtotal - descuento);
            total = totalFinal;

            // Actualizar el label del total final
            txt_total.Text = $"Total: {totalFinal:C}";
        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (productosEnCarrito.Count == 0)
                {
                    MessageBox.Show("Agregá al menos un producto antes de confirmar la venta.", "Carrito vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (descuento < 0 || descuento > subtotal)
                {
                    MessageBox.Show("El descuento no puede ser negativo ni superar el subtotal.", "Descuento no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtén la información ingresada por el usuario
                string nombre = ValidacionEntrada.Texto(txt_nombre.Text);
                string apellido = ValidacionEntrada.Texto(txt_apellido.Text);
                string mail = ValidacionEntrada.Texto(txt_mail.Text);
                string telefono = ValidacionEntrada.Texto(txt_telefono.Text);
                string direccion = ValidacionEntrada.Texto(txt_direccion.Text);
                string dni = ValidacionEntrada.Texto(txtDni.Text);

                // Validar que la información no sea nula o vacía
                if (!ValidacionEntrada.Nombre(nombre) || !ValidacionEntrada.Nombre(apellido) ||
                    !ValidacionEntrada.Email(mail) || !ValidacionEntrada.SoloDigitos(telefono, 6, 20) ||
                    !ValidacionEntrada.Requerido(direccion) || direccion.Length > 150 ||
                    !ValidacionEntrada.SoloDigitos(dni, 7, 12))
                {
                    MessageBox.Show("Revisá nombre, apellido, correo, teléfono, dirección y DNI. Hay datos incompletos o con formato inválido.", "Datos del cliente no válidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Sale del método si hay campos vacíos
                }

                if (combobox_metododepago.SelectedItem == null)
                {
                    MessageBox.Show("Seleccioná un método de pago.", "Datos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
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
                float precioTotal = (float)total;
                DateTime fecha = DateTime.Now; // Fecha actual
                string metodoDePago = combobox_metododepago.SelectedItem.ToString(); // Obtener método de pago
                float descuentoVenta = (float)descuento;

                int usuarioId = Program.logueado.Id;

                Venta nuevaVenta = new Venta
                {
                    PrecioTotal = precioTotal,
                    Fecha = fecha,
                    MetodoDePago = metodoDePago,
                    Descuento = descuentoVenta,
                    ClienteId = clienteId,
                    UsuarioId = usuarioId
                };

                List<DetalleVenta> detalles = productosEnCarrito.Select(producto => new DetalleVenta
                {
                    Cantidad = producto.CantidadSeleccionada,
                    ProductoId = producto.Id,
                    PrecioUnitario = producto.Precio
                }).ToList();

                int ventaId = VentaService.RegistrarVenta(nuevaVenta, detalles);

                try
                {
                    ComprobantePdfService.GenerarDesdeVenta(ventaId);
                    MessageBox.Show("La venta #" + ventaId + " se guardó correctamente y se generó el comprobante PDF.", "Venta confirmada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception pdfException)
                {
                    System.Diagnostics.Trace.WriteLine(pdfException.ToString());
                    MessageBox.Show("La venta #" + ventaId + " se guardó correctamente, pero no se pudo generar el PDF. " +
                        "Podés reintentarlo desde el historial de ventas.\n\nDetalle: " + pdfException.Message,
                        "Venta guardada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo completar la venta:\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void btn_noeliminar_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Obtén el DNI ingresado por el usuario
                string dni = txtDni.Text;

                // Validar que el campo DNI no esté vacío
                if (!ValidacionEntrada.SoloDigitos(dni, 7, 12))
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
                    LimpiarCampos();
                    clienteExistente = false; // El cliente no existe, se deberá crear
                    ActualizarEstadoCliente("Cliente nuevo", EstiloAplicacion.Rojo);
                }
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error si algo sale mal
                MessageBox.Show("No se pudo buscar el cliente:\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

