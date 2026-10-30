using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace EjemploABM
{
    public class EditarCostoForm : Form
    {
        private readonly TextBox txtCosto = new TextBox();
        private readonly ComboBox cmbMoneda = new ComboBox();
        private readonly TextBox txtCotizacion = new TextBox();

        public decimal Costo { get; private set; }
        public string Moneda { get; private set; }
        public decimal Cotizacion { get; private set; }

        public EditarCostoForm(string producto, decimal costo, string moneda, decimal cotizacion)
        {
            Text = "Editar costo";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(410, 245);
            Font = new Font("Segoe UI", 9F);

            var titulo = new Label { Text = "Costo de " + producto, AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold), Location = new Point(24, 20) };
            Controls.Add(titulo);
            AgregarEtiqueta("Costo:", 24, 65);
            txtCosto.Location = new Point(150, 61); txtCosto.Size = new Size(220, 27); txtCosto.Text = costo.ToString("0.##", CultureInfo.CurrentCulture);
            Controls.Add(txtCosto);
            AgregarEtiqueta("Moneda:", 24, 105);
            cmbMoneda.Location = new Point(150, 101); cmbMoneda.Size = new Size(220, 27); cmbMoneda.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMoneda.Items.AddRange(new object[] { "ARS", "USD" }); cmbMoneda.SelectedItem = moneda == "USD" ? "USD" : "ARS";
            cmbMoneda.SelectedIndexChanged += (s, e) => ActualizarEstadoCotizacion(); Controls.Add(cmbMoneda);
            AgregarEtiqueta("Cotización dólar:", 24, 145);
            txtCotizacion.Location = new Point(150, 141); txtCotizacion.Size = new Size(220, 27); txtCotizacion.Text = cotizacion > 0 ? cotizacion.ToString("0.##", CultureInfo.CurrentCulture) : string.Empty;
            Controls.Add(txtCotizacion);
            var btnGuardar = new Button { Text = "Guardar", DialogResult = DialogResult.None, Location = new Point(190, 190), Size = new Size(85, 30) };
            btnGuardar.Click += Guardar_Click; Controls.Add(btnGuardar);
            var btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(285, 190), Size = new Size(85, 30) };
            Controls.Add(btnCancelar);
            AcceptButton = btnGuardar; CancelButton = btnCancelar;
            ActualizarEstadoCotizacion();
        }

        private void AgregarEtiqueta(string texto, int x, int y) { Controls.Add(new Label { Text = texto, AutoSize = true, Location = new Point(x, y + 4) }); }

        private void ActualizarEstadoCotizacion()
        {
            bool usd = Convert.ToString(cmbMoneda.SelectedItem) == "USD";
            txtCotizacion.Enabled = usd;
            if (!usd) txtCotizacion.Text = string.Empty;
        }

        private void Guardar_Click(object sender, EventArgs e)
        {
            decimal costo;
            if (!decimal.TryParse(txtCosto.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out costo) || costo < 0)
            { MessageBox.Show("Ingresá un costo válido mayor o igual a cero.", "Dato no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtCosto.Focus(); return; }
            string moneda = Convert.ToString(cmbMoneda.SelectedItem) == "USD" ? "USD" : "ARS";
            decimal cotizacion = 0;
            if (moneda == "USD" && (!decimal.TryParse(txtCotizacion.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out cotizacion) || cotizacion <= 0))
            { MessageBox.Show("Ingresá una cotización válida para costos en USD.", "Dato no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtCotizacion.Focus(); return; }
            Costo = costo; Moneda = moneda; Cotizacion = cotizacion; DialogResult = DialogResult.OK; Close();
        }
    }
}
