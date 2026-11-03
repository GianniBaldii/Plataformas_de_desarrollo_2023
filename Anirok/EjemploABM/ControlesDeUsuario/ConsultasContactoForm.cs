using EjemploABM.Controladores;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EjemploABM.ControlesDeUsuario
{
    public class ConsultasContactoForm : Form
    {
        private readonly DataGridView grilla = new DataGridView();
        private readonly TextBox buscar = new TextBox();

        public ConsultasContactoForm()
        {
            Text = "Consultas recibidas desde la Web"; StartPosition = FormStartPosition.CenterParent; Size = new Size(1050, 600); MinimumSize = new Size(800, 450);
            buscar.Text = ""; buscar.Dock = DockStyle.Top; buscar.Height = 32; buscar.Margin = new Padding(12); buscar.TextChanged += (s, e) => Cargar();
            grilla.Dock = DockStyle.Fill; grilla.ReadOnly = true; grilla.AllowUserToAddRows = false; grilla.AutoGenerateColumns = true; grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Controls.Add(grilla); Controls.Add(buscar); Cargar();
        }

        private void Cargar()
        {
            try
            {
                var texto = (buscar.Text ?? "").Trim();
                var datos = Contacto_Controller.ObtenerTodos().Where(c => string.IsNullOrWhiteSpace(texto) || (c.Asunto + " " + c.Email + " " + c.Estado).IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                grilla.DataSource = datos;
                if (grilla.Columns.Contains("Mensaje")) grilla.Columns["Mensaje"].Width = 420;
                if (grilla.Columns.Contains("Id")) grilla.Columns["Id"].HeaderText = "N°";
                if (grilla.Columns.Contains("Fecha")) grilla.Columns["Fecha"].HeaderText = "Recibida";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Consultas", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }
}
