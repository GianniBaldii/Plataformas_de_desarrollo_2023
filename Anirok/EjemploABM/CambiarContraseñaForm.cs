using System.Drawing;
using System.Windows.Forms;

namespace EjemploABM
{
    public class CambiarContraseñaForm : Form
    {
        private readonly TextBox txtNueva = new TextBox();
        private readonly CheckBox chkGenerar = new CheckBox();
        public string Contraseña { get; private set; }
        public bool GenerarTemporal { get { return chkGenerar.Checked; } }
        public CambiarContraseñaForm(string nombre)
        {
            Text = "Cambiar contraseña"; StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog; MinimizeBox = false; MaximizeBox = false; ShowInTaskbar = false; ClientSize = new Size(430, 190); Font = new Font("Segoe UI", 9F);
            Controls.Add(new Label { Text = "Usuario: " + nombre, AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Location = new Point(24, 20) });
            chkGenerar.Text = "Generar una contraseña temporal"; chkGenerar.AutoSize = true; chkGenerar.Location = new Point(24, 62); chkGenerar.CheckedChanged += (s,e) => { txtNueva.Enabled = !chkGenerar.Checked; if (chkGenerar.Checked) txtNueva.Clear(); }; Controls.Add(chkGenerar);
            Controls.Add(new Label { Text = "Nueva contraseña:", AutoSize = true, Location = new Point(24, 103) });
            txtNueva.Location = new Point(145, 99); txtNueva.Size = new Size(250, 27); txtNueva.UseSystemPasswordChar = true; Controls.Add(txtNueva);
            var guardar = new Button { Text = "Guardar", Location = new Point(215, 145), Size = new Size(85, 30) }; guardar.Click += (s,e) => Guardar(); Controls.Add(guardar);
            var cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(310, 145), Size = new Size(85, 30) }; Controls.Add(cancelar); AcceptButton = guardar; CancelButton = cancelar;
        }
        private void Guardar()
        {
            if (!chkGenerar.Checked && (string.IsNullOrWhiteSpace(txtNueva.Text) || txtNueva.Text.Trim().Length < 6)) { MessageBox.Show("La contraseña debe tener al menos 6 caracteres.", "Dato no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtNueva.Focus(); return; }
            Contraseña = txtNueva.Text; DialogResult = DialogResult.OK; Close();
        }
    }
}
