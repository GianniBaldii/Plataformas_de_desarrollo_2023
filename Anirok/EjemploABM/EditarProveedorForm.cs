using System;
using System.Drawing;
using System.Windows.Forms;
using EjemploABM.Modelo;

namespace EjemploABM
{
    public class EditarProveedorForm : Form
    {
        private readonly TextBox[] campos = new TextBox[6];
        private readonly string[] etiquetas = { "Nombre", "Contacto", "Teléfono", "Email", "Dirección", "CUIT" };
        public Proveedor Proveedor { get; private set; }

        public EditarProveedorForm(Proveedor proveedor = null)
        {
            Text = proveedor == null ? "Nuevo proveedor" : "Editar proveedor";
            StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog; MinimizeBox = false; MaximizeBox = false; ShowInTaskbar = false; ClientSize = new Size(430, 330); Font = new Font("Segoe UI", 9F);
            for (int i = 0; i < etiquetas.Length; i++)
            {
                Controls.Add(new Label { Text = etiquetas[i] + ":", AutoSize = true, Location = new Point(24, 20 + i * 38) });
                campos[i] = new TextBox { Location = new Point(125, 16 + i * 38), Size = new Size(275, 27), MaxLength = i == 4 ? 160 : 120 };
                Controls.Add(campos[i]);
            }
            if (proveedor != null) { campos[0].Text = proveedor.Nombre; campos[1].Text = proveedor.Contacto; campos[2].Text = proveedor.Telefono; campos[3].Text = proveedor.Email; campos[4].Text = proveedor.Direccion; campos[5].Text = proveedor.Cuit; }
            var guardar = new Button { Text = "Guardar", Location = new Point(220, 270), Size = new Size(85, 30) }; guardar.Click += Guardar_Click; Controls.Add(guardar);
            var cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(315, 270), Size = new Size(85, 30) }; Controls.Add(cancelar);
            AcceptButton = guardar; CancelButton = cancelar;
        }

        private void Guardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(campos[0].Text) || campos[0].Text.Trim().Length > 100) { MessageBox.Show("Ingresá un nombre de proveedor válido.", "Dato no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning); campos[0].Focus(); return; }
            Proveedor = new Proveedor { Nombre = campos[0].Text.Trim(), Contacto = campos[1].Text.Trim(), Telefono = campos[2].Text.Trim(), Email = campos[3].Text.Trim(), Direccion = campos[4].Text.Trim(), Cuit = campos[5].Text.Trim() };
            DialogResult = DialogResult.OK; Close();
        }
    }
}
