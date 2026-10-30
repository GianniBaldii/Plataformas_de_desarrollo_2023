using EjemploABM.Controladores;
using EjemploABM.Modelo;
using EjemploABM.Servicios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace EjemploABM.ControlesDeUsuario
{
    public class Proveedores_UC : UserControl
    {
        private readonly DataGridView grilla = new DataGridView();
        private readonly Guna2TextBox buscar = new Guna2TextBox();
        private readonly Button agregar = new Button();
        private List<Proveedor> proveedores = new List<Proveedor>();
        public Proveedores_UC()
        {
            Dock = DockStyle.Fill; BackColor = Color.White; Font = new Font("Segoe UI", 9F);
            Controls.Add(new Label { Text = "PROVEEDORES", Font = new Font("Segoe UI", 18F, FontStyle.Bold), AutoSize = true, Location = new Point(24, 20) });
            Controls.Add(new Label { Text = "Alta, edición y baja lógica de proveedores", ForeColor = Color.Gray, AutoSize = true, Location = new Point(26, 53) });
            buscar.PlaceholderText = "Buscar por nombre o CUIT..."; buscar.Location = new Point(24, 88); buscar.Size = new Size(300, 28); buscar.TextChanged += (s,e) => Cargar(); Controls.Add(buscar);
            agregar.Text = "Agregar proveedor"; agregar.Location = new Point(340, 86); agregar.Size = new Size(150, 32); agregar.BackColor = Color.FromArgb(0, 100, 35); agregar.ForeColor = Color.White; agregar.FlatStyle = FlatStyle.Flat; agregar.Click += Agregar_Click; Controls.Add(agregar);
            grilla.Location = new Point(24, 135); grilla.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; grilla.Size = new Size(Math.Max(600, Width - 48), Math.Max(250, Height - 160)); grilla.ReadOnly = true; grilla.AllowUserToAddRows = false; grilla.AutoGenerateColumns = false; grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; EstiloAplicacion.PrepararGrilla(grilla);
            string[] encabezados = { "Id", "Nombre", "Contacto", "Teléfono", "Email", "CUIT", "Acciones" };
            string[] nombres = { "Id", "Nombre", "Contacto", "Telefono", "Email", "Cuit", "Acciones" };
            for (int i=0;i<6;i++) grilla.Columns.Add(new DataGridViewTextBoxColumn { Name=nombres[i], HeaderText=encabezados[i], DataPropertyName=nombres[i], FillWeight=i==1?150:100 });
            grilla.Columns.Add(new DataGridViewButtonColumn { Name="Editar", HeaderText="Editar", Text="Editar", UseColumnTextForButtonValue=true, FillWeight=75 });
            grilla.Columns.Add(new DataGridViewButtonColumn { Name="Desactivar", HeaderText="Estado", Text="Desactivar", UseColumnTextForButtonValue=true, FillWeight=85 });
            grilla.CellContentClick += Grilla_CellContentClick; Controls.Add(grilla); Resize += (s,e) => grilla.Size = new Size(Math.Max(600, Width - 48), Math.Max(250, Height - 160)); Cargar();
        }
        private void Cargar() { try { proveedores = Proveedor_Controller.ObtenerTodos(buscar.Text.Trim()); grilla.DataSource = null; grilla.DataSource = proveedores; } catch (Exception ex) { MessageBox.Show("No se pudieron cargar los proveedores.\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } }
        private void Agregar_Click(object sender, EventArgs e) { using (var f = new EditarProveedorForm()) if (f.ShowDialog(FindForm()) == DialogResult.OK) { try { Proveedor_Controller.Guardar(f.Proveedor); Cargar(); } catch (Exception ex) { MostrarError(ex); } } }
        private void Grilla_CellContentClick(object sender, DataGridViewCellEventArgs e) { if (e.RowIndex < 0 || e.ColumnIndex < 0) return; var p = proveedores[e.RowIndex]; string accion = grilla.Columns[e.ColumnIndex].Name; if (accion == "Editar") { using (var f = new EditarProveedorForm(p)) if (f.ShowDialog(FindForm()) == DialogResult.OK) { try { f.Proveedor.Id = p.Id; Proveedor_Controller.Guardar(f.Proveedor); Cargar(); } catch (Exception ex) { MostrarError(ex); } } } else if (accion == "Desactivar" && MessageBox.Show("¿Desactivar el proveedor '" + p.Nombre + "'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) { try { Proveedor_Controller.Desactivar(p.Id); Cargar(); } catch (Exception ex) { MostrarError(ex); } } }
        private void MostrarError(Exception ex) { MessageBox.Show(ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
