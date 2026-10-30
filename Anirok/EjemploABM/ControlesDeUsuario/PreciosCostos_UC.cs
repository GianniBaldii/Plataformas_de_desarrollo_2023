using EjemploABM.Controladores;
using EjemploABM.Modelo;
using EjemploABM.Servicios;
using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EjemploABM.ControlesDeUsuario
{
    public class PreciosCostos_UC : UserControl
    {
        private readonly DataGridView grilla = new DataGridView();
        private readonly Guna2TextBox txtBuscar = new Guna2TextBox();
        private readonly Guna2TextBox txtPorcentaje = new Guna2TextBox();
        private readonly Label lblCotizacion = new Label();
        private readonly Label lblEstado = new Label();
        private readonly Button btnCotizacion = new Button();
        private readonly Button btnAplicar = new Button();
        private readonly ComboBox cmbCategoria = new ComboBox();
        private readonly Button btnAplicarCategoria = new Button();
        private readonly Button btnRecargar = new Button();
        private readonly Button btnAnterior = new Button();
        private readonly Button btnSiguiente = new Button();
        private readonly Label lblPagina = new Label();
        private DataTable tablaProductos;
        private int paginaActual = 1;
        private const int ElementosPorPagina = 8;
        private CotizacionDolar cotizacionActual;

        public PreciosCostos_UC()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9F);
            ConstruirInterfaz();
            Resize += (s, e) => AcomodarZonaListado();
            CargarProductos();
        }

        private void ConstruirInterfaz()
        {
            var titulo = new Label { Text = "PRECIOS Y COSTOS", Font = new Font("Segoe UI", 18F, FontStyle.Bold), AutoSize = true, Location = new Point(24, 20) };
            var subtitulo = new Label { Text = "Control de costos, margen y actualización de precios", ForeColor = Color.Gray, AutoSize = true, Location = new Point(26, 53) };
            Controls.Add(titulo);
            Controls.Add(subtitulo);

            txtBuscar.PlaceholderText = "Buscar producto...";
            txtBuscar.Location = new Point(24, 88);
            txtBuscar.Size = new Size(230, 28);
            txtBuscar.TextChanged += (s, e) => FiltrarGrilla();
            Controls.Add(txtBuscar);
            EstiloAplicacion.AplicarAControl(txtBuscar);

            lblCotizacion.Text = "Dólar oficial: sin consultar";
            lblCotizacion.AutoSize = false;
            lblCotizacion.Location = new Point(275, 94);
            lblCotizacion.Size = new Size(215, 24);
            lblCotizacion.AutoEllipsis = true;
            lblCotizacion.ForeColor = Color.FromArgb(80, 80, 80);
            Controls.Add(lblCotizacion);

            btnCotizacion.Text = "Actualizar dólar";
            btnCotizacion.Location = new Point(500, 86);
            btnCotizacion.Size = new Size(140, 32);
            btnCotizacion.Click += ConsultarCotizacion;
            Controls.Add(btnCotizacion);

            btnRecargar.Text = "Recargar";
            btnRecargar.Location = new Point(650, 86);
            btnRecargar.Size = new Size(100, 32);
            btnRecargar.Click += (s, e) => CargarProductos();
            Controls.Add(btnRecargar);

            var lblAumento = new Label { Text = "Aumento %:", AutoSize = true, Location = new Point(780, 94) };
            Controls.Add(lblAumento);
            txtPorcentaje.Text = "0";
            txtPorcentaje.Location = new Point(850, 88);
            txtPorcentaje.Size = new Size(70, 28);
            Controls.Add(txtPorcentaje);
            EstiloAplicacion.AplicarAControl(txtPorcentaje);

            btnAplicar.Text = "Aplicar a todos";
            btnAplicar.Location = new Point(930, 86);
            btnAplicar.Size = new Size(140, 32);
            btnAplicar.BackColor = EstiloAplicacion.Rojo;
            btnAplicar.ForeColor = Color.White;
            btnAplicar.FlatStyle = FlatStyle.Flat;
            btnAplicar.Click += AplicarAumento;
            Controls.Add(btnAplicar);

            var lblCategoria = new Label { Text = "Categoría:", AutoSize = true, Location = new Point(24, 130) };
            Controls.Add(lblCategoria);
            cmbCategoria.Location = new Point(90, 124);
            cmbCategoria.Size = new Size(250, 28);
            cmbCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(cmbCategoria);
            btnAplicarCategoria.Text = "Aplicar a categoría";
            btnAplicarCategoria.Location = new Point(350, 122);
            btnAplicarCategoria.Size = new Size(160, 32);
            btnAplicarCategoria.BackColor = Color.FromArgb(35, 120, 65);
            btnAplicarCategoria.ForeColor = Color.White;
            btnAplicarCategoria.FlatStyle = FlatStyle.Flat;
            btnAplicarCategoria.Click += AplicarAumentoCategoria;
            Controls.Add(btnAplicarCategoria);

            grilla.Location = new Point(24, 165);
            grilla.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grilla.Size = new Size(Math.Max(600, Width - 48), Math.Max(250, Height - 210));
            grilla.ReadOnly = true;
            grilla.AllowUserToAddRows = false;
            grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            grilla.ScrollBars = ScrollBars.Vertical;
            EstiloAplicacion.PrepararGrilla(grilla);
            grilla.CellFormatting += Grilla_CellFormatting;
            grilla.CellContentClick += Grilla_CellContentClick;
            Controls.Add(grilla);

            btnAnterior.Text = "Anterior";
            btnAnterior.Location = new Point(24, Height - 55);
            btnAnterior.Size = new Size(100, 28);
            btnAnterior.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAnterior.Click += (s, e) => CambiarPagina(-1);
            Controls.Add(btnAnterior);

            btnSiguiente.Text = "Siguiente";
            btnSiguiente.Location = new Point(130, Height - 55);
            btnSiguiente.Size = new Size(100, 28);
            btnSiguiente.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSiguiente.Click += (s, e) => CambiarPagina(1);
            Controls.Add(btnSiguiente);

            lblPagina.AutoSize = true;
            lblPagina.Location = new Point(245, Height - 48);
            lblPagina.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Controls.Add(lblPagina);

            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(24, Height - 32);
            lblEstado.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblEstado.ForeColor = Color.Gray;
            Controls.Add(lblEstado);
            AcomodarZonaListado();
        }

        private void AcomodarZonaListado()
        {
            int alto = Math.Max(250, Height - 210);
            grilla.SetBounds(24, 165, Math.Max(600, Width - 48), alto);
            int y = Math.Max(165, Height - 38);
            btnAnterior.SetBounds(24, y, 100, 28);
            btnSiguiente.SetBounds(130, y, 100, 28);
            lblPagina.Location = new Point(245, y + 7);
            lblEstado.Location = new Point(24, y + 36);
        }

        private void Grilla_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (grilla.Columns[e.ColumnIndex].Name == "cotizacion_dolar" && (e.Value == null || e.Value == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(e.Value))))
                e.Value = "—";
            if (grilla.Columns[e.ColumnIndex].Name != "Analisis") return;
            var fila = grilla.Rows[e.RowIndex].DataBoundItem as DataRowView;
            decimal costoPesos = fila == null || fila["costo_pesos"] == DBNull.Value ? 0 : Convert.ToDecimal(fila["costo_pesos"]);
            decimal precio = fila == null || fila["precio"] == DBNull.Value ? 0 : Convert.ToDecimal(fila["precio"]);
            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (costoPesos <= 0)
            {
                e.Value = "Sin costo";
                e.CellStyle.BackColor = Color.FromArgb(235, 235, 235);
                e.CellStyle.ForeColor = Color.FromArgb(100, 100, 100);
                return;
            }
            decimal margen = (precio - costoPesos) / costoPesos * 100;
            e.Value = margen == 0 ? "Venta al costo" : (margen > 0 ? "Ganancia " : "Pérdida ") + margen.ToString("0.##") + "%";
            if (margen > 0)
            {
                e.CellStyle.BackColor = Color.FromArgb(220, 245, 225);
                e.CellStyle.ForeColor = Color.FromArgb(24, 120, 55);
            }
            else if (margen < 0)
            {
                e.CellStyle.BackColor = Color.FromArgb(252, 222, 222);
                e.CellStyle.ForeColor = Color.FromArgb(180, 25, 25);
            }
            else
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 244, 190);
                e.CellStyle.ForeColor = Color.FromArgb(130, 95, 0);
            }
        }

        private void CargarProductos()
        {
            try
            {
                using (var command = new SqlCommand("SELECT id, nombre, costo, costo_moneda, cotizacion_dolar, costo_pesos, precio FROM dbo.producto ORDER BY nombre", DB_Controller.connection))
                using (var adapter = new SqlDataAdapter(command))
                {
                    tablaProductos = new DataTable();
                    adapter.Fill(tablaProductos);
                    paginaActual = 1;
                    lblEstado.Text = tablaProductos.Rows.Count + " productos cargados";
                    CargarCategorias();
                    ActualizarPagina();
                }
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Ejecutá la migración 002 antes de usar este módulo.";
                MessageBox.Show("No se pudieron cargar costos y precios.\n" + ValidacionEntrada.LimpiarError(ex.Message), "Migración requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CargarCategorias()
        {
            try
            {
                var categorias = new DataTable();
                using (var command = new SqlCommand("SELECT id, nombre FROM dbo.categoria WHERE esta_activo = 'Activa' ORDER BY nombre", DB_Controller.connection))
                using (var adapter = new SqlDataAdapter(command)) adapter.Fill(categorias);
                cmbCategoria.DataSource = categorias;
                cmbCategoria.DisplayMember = "nombre";
                cmbCategoria.ValueMember = "id";
                cmbCategoria.SelectedIndex = -1;
            }
            catch
            {
                cmbCategoria.DataSource = null;
            }
        }

        private void FiltrarGrilla()
        {
            paginaActual = 1;
            ActualizarPagina();
        }

        private void ActualizarPagina()
        {
            if (tablaProductos == null) return;
            string filtro = (txtBuscar.Text ?? string.Empty).Trim().Replace("'", "''");
            tablaProductos.DefaultView.RowFilter = string.IsNullOrWhiteSpace(filtro) ? string.Empty : "nombre LIKE '%" + filtro + "%'";
            DataTable filtrada = tablaProductos.DefaultView.ToTable();
            int totalPaginas = filtrada.Rows.Count == 0 ? 0 : (int)Math.Ceiling((double)filtrada.Rows.Count / ElementosPorPagina);
            if (totalPaginas > 0 && paginaActual > totalPaginas) paginaActual = totalPaginas;
            DataTable pagina = filtrada.Clone();
            int inicio = (paginaActual - 1) * ElementosPorPagina;
            int fin = Math.Min(inicio + ElementosPorPagina, filtrada.Rows.Count);
            for (int i = inicio; i < fin; i++) pagina.ImportRow(filtrada.Rows[i]);
            grilla.DataSource = pagina;
            if (!grilla.Columns.Contains("EditarCosto"))
            {
                grilla.Columns.Add(new DataGridViewButtonColumn { Name = "EditarCosto", HeaderText = "Acción", Text = "Editar costo", UseColumnTextForButtonValue = true, Width = 115, FlatStyle = FlatStyle.Flat });
            }
            if (!grilla.Columns.Contains("Analisis"))
            {
                grilla.Columns.Add(new DataGridViewTextBoxColumn { Name = "Analisis", HeaderText = "Análisis", ReadOnly = true, Width = 145 });
            }
            ConfigurarColumnasGrilla();
            lblPagina.Text = totalPaginas == 0 ? "Sin resultados" : "Página " + paginaActual + " de " + totalPaginas;
            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;
        }

        private void ConfigurarColumnasGrilla()
        {
            if (grilla.Columns.Count < 7) return;
            grilla.Columns[0].HeaderText = "Id";
            grilla.Columns[1].HeaderText = "Producto";
            grilla.Columns[2].HeaderText = "Costo";
            grilla.Columns[3].HeaderText = "Moneda";
            grilla.Columns[4].HeaderText = "Dólar";
            grilla.Columns[5].HeaderText = "Costo ARS";
            grilla.Columns[6].HeaderText = "Precio";
            grilla.Columns[2].DefaultCellStyle.Format = "C0";
            grilla.Columns[4].DefaultCellStyle.Format = "C2";
            grilla.Columns[5].DefaultCellStyle.Format = "C0";
            grilla.Columns[6].DefaultCellStyle.Format = "C0";
            grilla.Columns[0].Width = 55;
            grilla.Columns[1].Width = 250;
            grilla.Columns[2].Width = 130;
            grilla.Columns[3].Width = 90;
            grilla.Columns[4].Width = 130;
            grilla.Columns[5].Width = 140;
            grilla.Columns[6].Width = 140;
            if (grilla.Columns.Contains("EditarCosto")) grilla.Columns["EditarCosto"].Width = 115;
            if (grilla.Columns.Contains("Analisis")) grilla.Columns["Analisis"].Width = 145;
        }

        private void Grilla_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || grilla.Columns[e.ColumnIndex].Name != "EditarCosto") return;
            var fila = grilla.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (fila == null)
            {
                MessageBox.Show("No se pudo identificar el producto seleccionado. Recargá el listado e intentá nuevamente.", "Producto no disponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id;
            if (!int.TryParse(Convert.ToString(fila["id"], System.Globalization.CultureInfo.InvariantCulture), out id))
            {
                MessageBox.Show("El identificador del producto no es válido.", "Producto no disponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string nombre = Convert.ToString(fila["nombre"]);
            decimal costo = fila["costo"] == DBNull.Value ? 0 : Convert.ToDecimal(fila["costo"]);
            string moneda = Convert.ToString(fila["costo_moneda"]);
            decimal cotizacion = fila["cotizacion_dolar"] == DBNull.Value ? 0 : Convert.ToDecimal(fila["cotizacion_dolar"]);
            using (var dialogo = new EditarCostoForm(nombre, costo, moneda, cotizacion))
            {
                if (dialogo.ShowDialog(FindForm()) == DialogResult.OK) GuardarCosto(id, dialogo.Costo, dialogo.Moneda, dialogo.Cotizacion);
            }
        }

        private void GuardarCosto(int productoId, decimal costo, string moneda, decimal cotizacion)
        {
            var connection = DB_Controller.connection;
            try
            {
                if (connection.State != ConnectionState.Closed) connection.Close();
                connection.Open();
                using (var tx = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    decimal costoAnterior = 0, cotizacionAnterior = 0, precioActual = 0;
                    string monedaAnterior = "ARS";
                    using (var read = new SqlCommand("SELECT costo, costo_moneda, cotizacion_dolar, precio FROM dbo.producto WITH (UPDLOCK,HOLDLOCK) WHERE id=@id", connection, tx))
                    {
                        read.Parameters.AddWithValue("@id", productoId);
                        using (var r = read.ExecuteReader())
                        {
                            if (!r.Read()) throw new InvalidOperationException("El producto ya no existe.");
                            costoAnterior = r.IsDBNull(0) ? 0 : Convert.ToDecimal(r.GetValue(0)); monedaAnterior = r.IsDBNull(1) ? "ARS" : r.GetString(1); cotizacionAnterior = r.IsDBNull(2) ? 0 : Convert.ToDecimal(r.GetValue(2)); precioActual = r.IsDBNull(3) ? 0 : Convert.ToDecimal(r.GetValue(3));
                        }
                    }
                    decimal costoPesos = moneda == "USD" ? costo * cotizacion : costo;
                    using (var update = new SqlCommand("UPDATE dbo.producto SET costo=@costo, costo_moneda=@moneda, cotizacion_dolar=@cotizacion, costo_pesos=@pesos WHERE id=@id", connection, tx))
                    {
                        update.Parameters.AddWithValue("@costo", (double)costo); update.Parameters.AddWithValue("@moneda", moneda); update.Parameters.AddWithValue("@cotizacion", moneda == "USD" ? (object)(double)cotizacion : DBNull.Value); update.Parameters.AddWithValue("@pesos", (double)costoPesos); update.Parameters.AddWithValue("@id", productoId); update.ExecuteNonQuery();
                    }
                    string insert = "INSERT INTO dbo.producto_historial_precio (id, producto_id, costo_anterior, costo_nuevo, precio_anterior, precio_nuevo, moneda_costo, cotizacion_dolar, fecha_cambio, usuario_id, motivo, origen) SELECT ISNULL(MAX(id),0)+1,@producto,@anterior,@nuevo,@precio,@precio,@moneda,@cotizacion,GETDATE(),NULLIF(@usuario,0),'Edición individual de costo','Manual' FROM dbo.producto_historial_precio";
                    using (var history = new SqlCommand(insert, connection, tx))
                    {
                        history.Parameters.AddWithValue("@producto", productoId); history.Parameters.AddWithValue("@anterior", (double)costoAnterior); history.Parameters.AddWithValue("@nuevo", (double)costo); history.Parameters.AddWithValue("@precio", (double)precioActual); history.Parameters.AddWithValue("@moneda", moneda); history.Parameters.AddWithValue("@cotizacion", moneda == "USD" ? (object)(double)cotizacion : DBNull.Value); history.Parameters.AddWithValue("@usuario", Program.logueado == null ? 0 : Program.logueado.Id); history.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
                connection.Close();
                MessageBox.Show("Costo actualizado y registrado en el historial.", "Costo actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarProductos();
            }
            catch (Exception ex)
            {
                if (connection.State != ConnectionState.Closed) connection.Close();
                MessageBox.Show("No se pudo actualizar el costo.\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CambiarPagina(int incremento)
        {
            paginaActual += incremento;
            if (paginaActual < 1) paginaActual = 1;
            ActualizarPagina();
        }

        private void ConsultarCotizacion(object sender, EventArgs e)
        {
            try
            {
                cotizacionActual = DolarApiService.ObtenerOficial();
                lblCotizacion.Text = "Dólar oficial venta: " + cotizacionActual.Venta.ToString("C2") + " (" + cotizacionActual.Actualizada.ToString("dd/MM HH:mm") + ")";
                lblCotizacion.ForeColor = Color.FromArgb(35, 125, 70);
            }
            catch (Exception ex)
            {
                lblCotizacion.Text = "No se pudo consultar el dólar";
                lblCotizacion.ForeColor = EstiloAplicacion.Rojo;
                MessageBox.Show("No se pudo consultar la cotización.\n" + ValidacionEntrada.LimpiarError(ex.Message), "Cotización no disponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AplicarAumento(object sender, EventArgs e)
        {
            decimal porcentaje;
            if (!decimal.TryParse(txtPorcentaje.Text, out porcentaje) || porcentaje < -90 || porcentaje > 500)
            {
                MessageBox.Show("Ingresá un porcentaje entre -90 y 500.", "Aumento no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Se actualizarán los precios de todos los productos un " + porcentaje + "%. ¿Continuar?", "Confirmar actualización", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                var connection = DB_Controller.connection;
                if (connection.State != ConnectionState.Closed) connection.Close();
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    int usuarioId = Program.logueado == null ? 0 : Program.logueado.Id;
                    string update = @"
DECLARE @Cambios TABLE (producto_id INT, precio_anterior REAL, precio_nuevo REAL, costo_actual REAL, moneda VARCHAR(3), dolar REAL);
INSERT INTO @Cambios
SELECT id, precio, ROUND((precio * (1 + @porcentaje / 100.0)) / 500, 0) * 500, costo, costo_moneda, cotizacion_dolar
FROM dbo.producto WHERE precio IS NOT NULL;
UPDATE p SET precio = c.precio_nuevo FROM dbo.producto p INNER JOIN @Cambios c ON c.producto_id = p.id;
INSERT INTO dbo.producto_historial_precio
(id, producto_id, costo_anterior, costo_nuevo, precio_anterior, precio_nuevo, moneda_costo, cotizacion_dolar, fecha_cambio, usuario_id, motivo, origen)
SELECT ISNULL((SELECT MAX(id) FROM dbo.producto_historial_precio), 0) + ROW_NUMBER() OVER (ORDER BY producto_id),
       producto_id, costo_actual, costo_actual, precio_anterior, precio_nuevo, moneda, dolar, GETDATE(), NULLIF(@usuario, 0),
       'Actualización masiva de precios', 'ManualMasivo'
FROM @Cambios;";
                    using (var command = new SqlCommand(update, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@porcentaje", porcentaje);
                        command.Parameters.AddWithValue("@usuario", usuarioId);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                connection.Close();
                MessageBox.Show("Precios actualizados y registrados en el historial.", "Actualización realizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarProductos();
            }
            catch (Exception ex)
            {
                if (DB_Controller.connection.State != ConnectionState.Closed) DB_Controller.connection.Close();
                MessageBox.Show("No se pudieron actualizar los precios.\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarAumentoCategoria(object sender, EventArgs e)
        {
            if (cmbCategoria.SelectedValue == null || cmbCategoria.SelectedValue is DataRowView)
            {
                MessageBox.Show("Seleccioná una categoría.", "Categoría requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal porcentaje;
            if (!decimal.TryParse(txtPorcentaje.Text, out porcentaje) || porcentaje < -90 || porcentaje > 500)
            {
                MessageBox.Show("Ingresá un porcentaje entre -90 y 500.", "Aumento no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int categoriaId = Convert.ToInt32(cmbCategoria.SelectedValue);
            string categoriaNombre = Convert.ToString(cmbCategoria.Text);
            if (MessageBox.Show("Se actualizarán los precios de '" + categoriaNombre + "' un " + porcentaje + "%. ¿Continuar?", "Confirmar actualización", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var connection = DB_Controller.connection;
            try
            {
                if (connection.State != ConnectionState.Closed) connection.Close();
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    int usuarioId = Program.logueado == null ? 0 : Program.logueado.Id;
                    string update = @"
DECLARE @Cambios TABLE (producto_id INT, precio_anterior REAL, precio_nuevo REAL, costo_actual REAL, moneda VARCHAR(3), dolar REAL);
INSERT INTO @Cambios
SELECT id, precio, ROUND((precio * (1 + @porcentaje / 100.0)) / 500, 0) * 500, costo, costo_moneda, cotizacion_dolar
FROM dbo.producto WHERE precio IS NOT NULL AND categoria_id = @categoria;
UPDATE p SET precio = c.precio_nuevo FROM dbo.producto p INNER JOIN @Cambios c ON c.producto_id = p.id;
INSERT INTO dbo.producto_historial_precio
(id, producto_id, costo_anterior, costo_nuevo, precio_anterior, precio_nuevo, moneda_costo, cotizacion_dolar, fecha_cambio, usuario_id, motivo, origen)
SELECT ISNULL((SELECT MAX(id) FROM dbo.producto_historial_precio), 0) + ROW_NUMBER() OVER (ORDER BY producto_id), producto_id, costo_actual, costo_actual, precio_anterior, precio_nuevo, moneda, dolar, GETDATE(), NULLIF(@usuario, 0), 'Actualización de precios por categoría', 'ManualCategoria'
FROM @Cambios;
SELECT COUNT(*) FROM @Cambios;";
                    using (var command = new SqlCommand(update, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@porcentaje", porcentaje);
                        command.Parameters.AddWithValue("@categoria", categoriaId);
                        command.Parameters.AddWithValue("@usuario", usuarioId);
                        int afectados = Convert.ToInt32(command.ExecuteScalar());
                        transaction.Commit();
                        connection.Close();
                        MessageBox.Show("Se actualizaron " + afectados + " registros de la categoría seleccionada.", "Actualización realizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                CargarProductos();
            }
            catch (Exception ex)
            {
                if (connection.State != ConnectionState.Closed) connection.Close();
                MessageBox.Show("No se pudieron actualizar los precios de la categoría.\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
