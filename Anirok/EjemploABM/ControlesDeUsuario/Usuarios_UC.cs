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

namespace EjemploABM.ControlesDeUsuario
{
    public partial class Usuarios_UC : UserControl
    {

        List<Usuario> users;

        public Usuarios_UC()
        {
            InitializeComponent();
            ListadoVisual.Aplicar(this, guna2DataGridView1, "USUARIOS", "Administración de usuarios y permisos del sistema", btn_add, null, null, null, label1);
            cargarUsuarios();
            
                
            //dataGridView1.Font = new Font("Roboto", 10, FontStyle.Regular); // Cambia la fuente del DataGridView
            
            
        }

        private void cargarUsuarios()
        {
            users = Usuario_Controller.obtenerTodos();
            guna2DataGridView1.Rows.Clear();
            foreach(Usuario usr in users)
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
                guna2DataGridView1.Rows[rowIndex].Cells[8].Value = usr.Rol.ToString();


                guna2DataGridView1.Rows[rowIndex].Cells[9].Value = "Editar";
                guna2DataGridView1.Rows[rowIndex].Cells[10].Value = "Eliminar";

            }
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Trace.WriteLine("estoy andando");
            Debug.WriteLine("Celda seleccionada: " + e.ColumnIndex + ", " + e.RowIndex);

            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Editar")
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
