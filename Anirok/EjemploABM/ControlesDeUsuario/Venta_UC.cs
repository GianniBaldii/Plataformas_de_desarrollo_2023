using EjemploABM.Controladores;
using EjemploABM.Modelo;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using EjemploABM.Servicios;
using Guna.UI2.WinForms;
using System.Linq;

namespace EjemploABM.ControlesDeUsuario
{
    public partial class Ventas_UC : UserControl
    {
        private const int ElementosPorPagina = 8;
        private int paginaActual = 1;
        private List<Venta> ventas;
        private readonly Guna2TextBox txtBuscar = new Guna2TextBox();

        public Ventas_UC()
        {
            InitializeComponent();
            ListadoVisual.Aplicar(this, guna2DataGridView1, "VENTAS", "Historial de operaciones y comprobantes emitidos", btn_add_cat, lblPaginaActual, btn_anterior, btn_siguiente);
            ListadoVisual.AplicarAcciones(guna2DataGridView1);
            txtBuscar.PlaceholderText = "Buscar venta por método o número..."; txtBuscar.Location = new System.Drawing.Point(24, 84); txtBuscar.Size = new System.Drawing.Size(300, 30); txtBuscar.TextChanged += (s,e) => { paginaActual = 1; ActualizarVista(); }; Controls.Add(txtBuscar); EstiloAplicacion.AplicarAControl(txtBuscar);
            EstiloAplicacion.AplicarAControl(this);
            EstiloAplicacion.BotonSecundario(btn_siguiente);
            EstiloAplicacion.BotonSecundario(btn_anterior);
            EstiloAplicacion.BotonPrimario(btn_add_cat);
            btn_siguiente.Click += btnSiguiente_Click;
            btn_anterior.Click += btnAnterior_Click;
            btn_add_cat.Click += btnAgregarVenta_Click;
            CargarVentas();
        }

        private void CargarVentas()
        {
            try
            {
                ventas = Venta_Controller.ObtenerVentas() ?? new List<Venta>();
                int totalPaginas = CalcularTotalDePaginasFiltradas();
                if (paginaActual > totalPaginas && totalPaginas > 0) paginaActual = totalPaginas;
                ActualizarVista();
            }
            catch (Exception ex)
            {
                ventas = new List<Venta>();
                ActualizarVista();
                MessageBox.Show("No se pudieron cargar las ventas:\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarVista()
        {
            guna2DataGridView1.Rows.Clear();

            if (ventas == null) ventas = new List<Venta>();
            var filtradas = ventas.Where(v => string.IsNullOrWhiteSpace(txtBuscar.Text) || v.Id.ToString().Contains(txtBuscar.Text.Trim()) || v.MetodoDePago.IndexOf(txtBuscar.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            int inicio = (paginaActual - 1) * ElementosPorPagina;
            int fin = Math.Min(inicio + ElementosPorPagina, filtradas.Count);

            for (int i = inicio; i < fin; i++)
            {
                int rowIndex = guna2DataGridView1.Rows.Add();

                guna2DataGridView1.Rows[rowIndex].Cells[0].Value = filtradas[i].Id.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[1].Value = filtradas[i].PrecioTotal.ToString("C0");
                guna2DataGridView1.Rows[rowIndex].Cells[2].Value = filtradas[i].Fecha.ToString("dd/MM/yyyy");
                guna2DataGridView1.Rows[rowIndex].Cells[3].Value = filtradas[i].MetodoDePago.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[4].Value = filtradas[i].Descuento.ToString("C0");
                
                guna2DataGridView1.Rows[rowIndex].Cells[5].Value = "Ver detalle";
            }

            int totalDePaginasFiltradas = filtradas.Count == 0 ? 0 : (int)Math.Ceiling((double)filtradas.Count / ElementosPorPagina);
            lblPaginaActual.Text = totalDePaginasFiltradas == 0 ? "Sin ventas" : $"Página {paginaActual} de {totalDePaginasFiltradas}";
        }

        private int CalcularTotalDePaginasFiltradas()
        {
            return ventas == null || ventas.Count == 0 ? 0 : (int)Math.Ceiling((double)ventas.Count / ElementosPorPagina);
        }

        private void guna2DataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex >= guna2DataGridView1.Columns.Count) return;
            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Detalle")
            {
                int id;
                if (!int.TryParse(Convert.ToString(guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value), out id)) return;
                try
                {
                    Venta venta = Venta_Controller.ObtenerVentaPorId(id);
                    if (venta == null)
                    {
                        MessageBox.Show("No se encontró la venta seleccionada.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string rutaFactura = ComprobantePdfService.GenerarDesdeVenta(venta.Id);
                        DialogResult abrir = MessageBox.Show("Venta #" + venta.Id + "\nTotal: " + venta.PrecioTotal.ToString("C0") +
                            "\nFecha: " + venta.Fecha.ToString("dd/MM/yyyy") +
                            "\nMétodo de pago: " + venta.MetodoDePago +
                            "\nDescuento: " + venta.Descuento.ToString("C0") +
                            "\n\n¿Querés abrir la factura PDF?",
                            "Factura de venta", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (abrir == DialogResult.Yes)
                            Process.Start(new ProcessStartInfo(rutaFactura) { UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo consultar el detalle:\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAgregarVenta_Click(object sender, EventArgs e)
        {
            using (FormVenta form = new FormVenta())
                form.ShowDialog(FindForm());
            CargarVentas();
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            int totalPaginas = CalcularTotalDePaginasFiltradas();
            if (paginaActual < totalPaginas)
            {
                paginaActual++;
                ActualizarVista();
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                ActualizarVista();
            }
        }

        
    }
}
