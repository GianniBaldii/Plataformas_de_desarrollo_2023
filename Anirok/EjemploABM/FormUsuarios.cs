using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EjemploABM.Controladores;
using EjemploABM.Modelo;
using EjemploABM.Servicios;

namespace EjemploABM
{
    public partial class FormUsuarios : Form
    {
        string situacion;
        int id_editar;

        public FormUsuarios()
        {
            InitializeComponent();

            combo_tipo.Items.Clear();

            combo_tipo.Items.Add("Admin");
            combo_tipo.Items.Add("Vendedor");

            situacion = "creacion";
        }


        // SOBRECARGAR EL CONSTRUCTOR PARA INICIAR EL FORM CON LA INFO CARGADA, PARA EDITAR
        public FormUsuarios(Usuario usr)
        {
            InitializeComponent();

            id_editar = usr.Id;


            txt_nombre.Text = usr.Nombre.ToString();
            txt_apellido.Text = usr.Apellido.ToString();
            txt_mail.Text = usr.Mail.ToString();
            txt_telefono.Text = usr.Telefono.ToString();
            txt_direccion.Text = usr.Direccion.ToString();
            txt_contraseña.Text = usr.Contraseña.ToString();
            txt_dni.Text = usr.Dni.ToString();

            combo_tipo.Items.Clear();
            combo_tipo.Items.Add("Admin");
            combo_tipo.Items.Add("Vendedor");

            if (usr.Rol == 1)
            {
                combo_tipo.SelectedIndex = 0;
            }
            else
            {
                combo_tipo.SelectedIndex = 1;
            }

            situacion = "edicion";

            label2.Text = "Editar Usuario";
            btn_crear.Text = "Editar";


        }

        private void btn_crear_Click_1(object sender, EventArgs e)
        {
            if (situacion == "creacion")
            {
                crear();
            }
            if (situacion == "edicion")
            {
                editar();
            }

        }


        private void crear()
        {
            if (!ValidarCampos())
            {
                MessageBox.Show("Por favor, complete todos los campos y seleccione un rol antes de crear un usuario.", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            int tipo = 2;
            if (combo_tipo.SelectedItem.ToString() == "Admin")
            {
                tipo = 1;
            }

            // Verificar si la contraseña ya está en uso
            if (Usuario_Controller.existeContraseña(txt_contraseña.Text))
            {
                MessageBox.Show("La contraseña ingresada ya está en uso. Por favor, elija otra contraseña.", "Contraseña repetida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Usuario usr = new Usuario(0, txt_nombre.Text, txt_apellido.Text, txt_mail.Text, txt_telefono.Text, txt_direccion.Text, txt_dni.Text, txt_contraseña.Text, tipo);

            EjecutarOperacion(() => Usuario_Controller.crearUsuario(usr), "crear el usuario");
        }

        private void editar()
        {
            if (!ValidarCampos())
            {
                MessageBox.Show("Por favor, complete todos los campos y seleccione un rol antes de crear un usuario.", "Campos faltantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int tipo = 2;
            if (combo_tipo.SelectedItem.ToString() == "Admin")
            {
                tipo = 1;
            }

            Usuario usr = new Usuario(id_editar, txt_nombre.Text, txt_apellido.Text, txt_mail.Text, txt_telefono.Text , txt_direccion.Text , txt_dni.Text, txt_contraseña.Text, tipo);

            EjecutarOperacion(() => Usuario_Controller.editarUsuario(usr), "editar el usuario");
        }

        private bool ValidarCampos()
        {
            if (!ValidacionEntrada.Nombre(txt_nombre.Text) || !ValidacionEntrada.Nombre(txt_apellido.Text))
                return MostrarValidacion("Nombre y apellido deben tener entre 2 y 100 caracteres.");
            if (!ValidacionEntrada.Email(txt_mail.Text))
                return MostrarValidacion("Ingresá un correo electrónico válido.");
            if (!ValidacionEntrada.SoloDigitos(txt_telefono.Text, 6, 20))
                return MostrarValidacion("El teléfono debe contener entre 6 y 20 dígitos.");
            if (!ValidacionEntrada.Requerido(txt_direccion.Text) || txt_direccion.Text.Trim().Length > 150)
                return MostrarValidacion("Ingresá una dirección válida.");
            if (!ValidacionEntrada.SoloDigitos(txt_dni.Text, 7, 12))
                return MostrarValidacion("El DNI debe contener entre 7 y 12 dígitos.");
            if (txt_contraseña.Text.Trim().Length < 6 || txt_contraseña.Text.Length > 100)
                return MostrarValidacion("La contraseña debe tener entre 6 y 100 caracteres.");
            if (combo_tipo.SelectedItem == null)
                return MostrarValidacion("Seleccioná un rol.");
            return true;
        }

        private bool MostrarValidacion(string mensaje)
        {
            MessageBox.Show(mensaje, "Datos no válidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private void EjecutarOperacion(Func<bool> operacion, string accion)
        {
            try
            {
                if (operacion()) DialogResult = DialogResult.OK;
                else MessageBox.Show("No se pudo " + accion + ".", "Operación no realizada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo " + accion + ":\n" + ValidacionEntrada.LimpiarError(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //CIERRA VENTANA FORMUSUARIOS
        private void btnCerrarVentana_Click_1(object sender, EventArgs e)
        {
            this.Hide();
        }

      

        private void txt_nombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void combo_tipo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txt_direccion_TextChanged(object sender, EventArgs e)
        {

        }

        private void combo_tipo_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}
