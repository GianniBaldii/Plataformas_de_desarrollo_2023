using System.Drawing;
using System;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace EjemploABM.Servicios
{
    internal static class ListadoVisual
    {
        public static void Aplicar(Control root, DataGridView grilla, string titulo, string subtitulo, Control agregar, Label pagina, Control anterior, Control siguiente, Label tituloExistente = null)
        {
            root.BackColor = Color.White;
            root.MinimumSize = Size.Empty;
            root.MaximumSize = Size.Empty;
            if (tituloExistente != null)
            {
                tituloExistente.Text = titulo;
                tituloExistente.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
                tituloExistente.Location = new Point(24, 20);
                tituloExistente.ForeColor = Color.FromArgb(25, 25, 25);
            }
            else
            {
                root.Controls.Add(new Label { Text = titulo, Font = new Font("Segoe UI", 18F, FontStyle.Bold), AutoSize = true, Location = new Point(24, 20) });
            }
            var bajada = new Label { Text = subtitulo, ForeColor = Color.Gray, AutoSize = true, Location = new Point(26, 53) };
            root.Controls.Add(bajada);
            grilla.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grilla.SetBounds(24, 150, Math.Max(600, root.Width - 48), Math.Max(250, root.Height - 205));
            grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            EstiloAplicacion.PrepararGrilla(grilla);
            if (agregar != null)
            {
                agregar.Text = "Agregar";
                agregar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                agregar.SetBounds(Math.Max(24, root.Width - 190), 80, 166, 36);
                var guna = agregar as Guna2Button;
                if (guna != null) EstiloAplicacion.BotonPrimario(guna);
                else { var boton = agregar as Button; if (boton != null) { boton.BackColor = EstiloAplicacion.Rojo; boton.ForeColor = Color.White; boton.FlatStyle = FlatStyle.Flat; } }
            }
            if (anterior != null)
            {
                anterior.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                anterior.SetBounds(24, Math.Max(155, root.Height - 42), 100, 30);
                var guna = anterior as Guna2Button;
                if (guna != null) EstiloAplicacion.BotonSecundario(guna);
            }
            if (siguiente != null)
            {
                siguiente.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                siguiente.SetBounds(130, Math.Max(155, root.Height - 42), 100, 30);
                var guna = siguiente as Guna2Button;
                if (guna != null) EstiloAplicacion.BotonSecundario(guna);
            }
            if (pagina != null) { pagina.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; pagina.Location = new Point(270, Math.Max(160, root.Height - 35)); pagina.Font = new Font("Segoe UI", 9F, FontStyle.Regular); }
            root.Resize += (s, e) =>
            {
                grilla.SetBounds(24, 150, Math.Max(600, root.Width - 48), Math.Max(250, root.Height - 205));
                if (agregar != null) agregar.Left = Math.Max(24, root.Width - 190);
                int y = Math.Max(155, root.Height - 42);
                if (anterior != null) anterior.Top = y;
                if (siguiente != null) siguiente.Top = y;
                if (pagina != null) pagina.Location = new Point(270, y + 7);
            };
        }
    }
}
