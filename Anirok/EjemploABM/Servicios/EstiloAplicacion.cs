using Guna.UI2.WinForms;
using System.Drawing;
using System.Windows.Forms;

namespace EjemploABM.Servicios
{
    internal static class EstiloAplicacion
    {
        public static readonly Color Rojo = Color.FromArgb(198, 0, 0);
        public static readonly Color RojoHover = Color.FromArgb(156, 0, 0);
        public static readonly Color Oscuro = Color.FromArgb(28, 28, 28);
        public static readonly Color OscuroHover = Color.FromArgb(48, 48, 48);
        public static readonly Color Fondo = Color.FromArgb(247, 247, 248);
        public static readonly Color Borde = Color.FromArgb(224, 224, 224);

        public static void Aplicar(Form formulario)
        {
            formulario.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            formulario.AutoScaleMode = AutoScaleMode.Dpi;
            formulario.Opacity = 1D;
            formulario.KeyPreview = true;
            AplicarAControles(formulario.Controls);
        }

        public static void AplicarAControl(Control control)
        {
            control.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            AplicarAControles(control.Controls);
        }

        public static void BotonPrimario(Guna2Button boton)
        {
            PrepararBoton(boton);
            boton.FillColor = Rojo;
            boton.HoverState.FillColor = RojoHover;
        }

        public static void BotonSecundario(Guna2Button boton)
        {
            PrepararBoton(boton);
            boton.FillColor = Oscuro;
            boton.HoverState.FillColor = OscuroHover;
        }

        public static void BotonPeligro(Guna2Button boton)
        {
            PrepararBoton(boton);
            boton.FillColor = Color.FromArgb(116, 25, 25);
            boton.HoverState.FillColor = Color.FromArgb(92, 18, 18);
        }

        public static void PrepararGrilla(DataGridView grilla)
        {
            grilla.BackgroundColor = Color.White;
            grilla.BorderStyle = BorderStyle.None;
            grilla.RowHeadersVisible = false;
            grilla.EnableHeadersVisualStyles = false;
            grilla.ColumnHeadersHeight = 38;
            grilla.ColumnHeadersDefaultCellStyle.BackColor = Oscuro;
            grilla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grilla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grilla.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            grilla.DefaultCellStyle.SelectionBackColor = Color.FromArgb(244, 222, 222);
            grilla.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);
            grilla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            grilla.RowTemplate.Height = 34;
            grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grilla.MultiSelect = false;
        }

        private static void AplicarAControles(Control.ControlCollection controles)
        {
            foreach (Control control in controles)
            {
                var etiqueta = control as Label;
                if (etiqueta != null)
                    etiqueta.Font = new Font("Segoe UI", etiqueta.Font.Size, etiqueta.Font.Style);

                var cuadroTexto = control as TextBoxBase;
                if (cuadroTexto != null)
                    cuadroTexto.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

                var cuadroGuna = control as Guna2TextBox;
                if (cuadroGuna != null)
                {
                    cuadroGuna.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                    cuadroGuna.BorderRadius = 6;
                    cuadroGuna.BorderColor = Borde;
                    cuadroGuna.FocusedState.BorderColor = Rojo;
                }

                var comboGuna = control as Guna2ComboBox;
                if (comboGuna != null)
                {
                    comboGuna.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                    comboGuna.BorderRadius = 6;
                    comboGuna.BorderColor = Borde;
                    comboGuna.FocusedState.BorderColor = Rojo;
                }

                var boton = control as Guna2Button;
                if (boton != null)
                    PrepararBoton(boton);

                var grilla = control as DataGridView;
                if (grilla != null)
                    PrepararGrilla(grilla);

                if (control.HasChildren)
                    AplicarAControles(control.Controls);
            }
        }

        private static void PrepararBoton(Guna2Button boton)
        {
            boton.BorderRadius = 8;
            boton.Cursor = Cursors.Hand;
            boton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            boton.ForeColor = Color.White;
        }
    }
}
