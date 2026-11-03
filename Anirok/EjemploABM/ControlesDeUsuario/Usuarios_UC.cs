using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EjemploABM.Controladores;
using EjemploABM.Modelo;
using EjemploABM.Servicios;
using Guna.UI2.WinForms;
using System.Linq;

namespace EjemploABM.ControlesDeUsuario
{
    public partial class Usuarios_UC : UserControl
    {

        List<Usuario> users;
        private readonly Guna2TextBox txtBuscar = new Guna2TextBox();

        public Usuarios_UC()
        {
            InitializeComponent();
            ListadoVisual.Aplicar(this, guna2DataGridView1, "USUARIOS", "Administración de usuarios y permisos del sistema", btn_add, null, null, null, label1);
            if (guna2DataGridView1.Columns.Contains("Contraseña")) guna2DataGridView1.Columns["Contraseña"].Visible = false;
            if (!guna2DataGridView1.Columns.Contains("Resetear"))
            {
                guna2DataGridView1.Columns.Add(new DataGridViewButtonColumn { Name = "Resetear", HeaderText = "Seguridad", Text = "Resetear clave", UseColumnTextForButtonValue = true, ReadOnly = true });
                guna2DataGridView1.Columns["Resetear"].ToolTipText = "Genera una contraseña temporal. Luego podrá enviarse por correo al usuario.";
            }
            ListadoVisual.AplicarAcciones(guna2DataGridView1);
            txtBuscar.PlaceholderText = "Buscar usuario..."; txtBuscar.Location = new System.Drawing.Point(24, 84); txtBuscar.Size = new System.Drawing.Size(300, 30); txtBuscar.TextChanged += (s,e) => cargarUsuarios(); Controls.Add(txtBuscar); EstiloAplicacion.AplicarAControl(txtBuscar);
            var btnConsultas = new Guna2Button { Text = "CONSULTAS WEB", Location = new System.Drawing.Point(340, 84), Size = new System.Drawing.Size(170, 32), BorderRadius = 8 };
            btnConsultas.Click += (s, e) =>
            {
                try
                {
                    using (var form = new ConsultasContactoForm()) form.ShowDialog(FindForm());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudieron abrir las consultas Web.\n" + ValidacionEntrada.LimpiarError(ex.Message), "Consultas Web", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            Controls.Add(btnConsultas); EstiloAplicacion.AplicarAControl(btnConsultas);
            var btnMails = new Guna2Button { Text = "ENVIOS DE MAIL", Location = new System.Drawing.Point(520, 84), Size = new System.Drawing.Size(170, 32), BorderRadius = 8 };
            btnMails.Click += (s, e) => { using (var form = new EnviosMailForm()) form.ShowDialog(FindForm()); };
            Controls.Add(btnMails); EstiloAplicacion.AplicarAControl(btnMails);
            cargarUsuarios();
            
                
            //dataGridView1.Font = new Font("Roboto", 10, FontStyle.Regular); // Cambia la fuente del DataGridView
            
            
        }

        private void cargarUsuarios()
        {
            users = Usuario_Controller.obtenerTodos();
            guna2DataGridView1.Rows.Clear();
            var filtrados = users.Where(usr => string.IsNullOrWhiteSpace(txtBuscar.Text) || (usr.Nombre + " " + usr.Apellido + " " + usr.Mail + " " + usr.Dni).IndexOf(txtBuscar.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            foreach(Usuario usr in filtrados)
            {
                int rowIndex = guna2DataGridView1.Rows.Add();

                guna2DataGridView1.Rows[rowIndex].Cells[0].Value = usr.Id.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[1].Value = usr.Nombre.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[2].Value = usr.Apellido.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[3].Value = usr.Mail.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[4].Value = usr.Telefono.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[5].Value = usr.Direccion.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[6].Value = usr.Dni.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[7].Value = usr.Contraseña.ToString();
                    guna2DataGridView1.Rows[rowIndex].Cells[8].Value = usr.Rol == 1 ? "Administrador" : (usr.Rol == 2 ? "Vendedor" : "Cliente");


                guna2DataGridView1.Rows[rowIndex].Cells[9].Value = "Editar";
                guna2DataGridView1.Rows[rowIndex].Cells[10].Value = "Eliminar";

            }
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            Trace.WriteLine("estoy andando");
            Debug.WriteLine("Celda seleccionada: " + e.ColumnIndex + ", " + e.RowIndex);

            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Resetear")
            {
                int idReset;
                if (!int.TryParse(Convert.ToString(guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value), out idReset)) return;
                string nombre = Convert.ToString(guna2DataGridView1.Rows[e.RowIndex].Cells[1].Value);
                using (var dialogo = new CambiarContraseñaForm(nombre))
                if (dialogo.ShowDialog(FindForm()) == DialogResult.OK)
                try
                {
                    string temporal = dialogo.GenerarTemporal ? Usuario_Controller.ResetearContraseña(idReset) : dialogo.Contraseña;
                    if (!dialogo.GenerarTemporal) Usuario_Controller.CambiarContraseña(idReset, dialogo.Contraseña);
                    if (dialogo.GenerarTemporal)
                    {
                        var usuarioReset = Usuario_Controller.obtenerPorId(idReset);
                        if (!string.IsNullOrWhiteSpace(usuarioReset.Mail))
                        {
                            try
                            {
                                EmailService.EnviarContraseñaTemporal(usuarioReset.Mail, usuarioReset.Nombre, temporal);
                                MessageBox.Show("Contraseña temporal generada y enviada a " + usuarioReset.Mail + ".", "Contraseña actualizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch
                            {
                                MessageBox.Show("Contraseña temporal generada:\n\n" + temporal + "\n\nNo se pudo enviar el mail. Quedó registrado para reintentar desde Envíos de mail.", "Contraseña actualizada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else MessageBox.Show("Contraseña temporal generada:\n\n" + temporal + "\n\nEl usuario no tiene mail cargado.", "Contraseña actualizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else MessageBox.Show("La contraseña elegida fue guardada correctamente.", "Contraseña actualizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo resetear la contraseña.\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (senderGrid.Columns[e.ColumnIndex].Name == "Editar")
            {
                //EDITAMOS
                Debug.WriteLine("Valor de la celda: " + guna2DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                int id = int.Parse(guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                Trace.WriteLine("el id es: " + id);

                Usuario user_editar = Usuario_Controller.obtenerPorId(id);

                FormUsuarios frmUser = new FormUsuarios(user_editar);

                DialogResult dr = frmUser.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    Trace.WriteLine("OK - se edito");
                    //ACTUALIZAR LA LISTA
                    cargarUsuarios();

                }
            } else if (senderGrid.Columns[e.ColumnIndex].Name == "Eliminar")
            {
                Debug.WriteLine("Valor de la celda: " + guna2DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                int id = int.Parse(guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                Trace.WriteLine("el id es: " + id);
                Usuario user_eliminar = Usuario_Controller.obtenerPorId(id);
                FormEliminar formeliminar= new FormEliminar(user_eliminar);
                DialogResult eliminar = formeliminar.ShowDialog();
                if (eliminar == DialogResult.OK)
                {
                    Trace.WriteLine("OK - se creo form eliminar");
                    cargarUsuarios();

                }
            }

        }

        //CREAR USUARIO
        private void btn_add_Click(object sender, EventArgs e)
        {
            FormUsuarios frmUser = new FormUsuarios();
            DialogResult dr = frmUser.ShowDialog();

            if (dr == DialogResult.OK)
            {
                Trace.WriteLine("OK - se creo");
                //ACTUALIZAR LA LISTA
                cargarUsuarios();

            }

        }


        


        private void label1_Click(object sender, EventArgs e)
        {

        }


    }
}
