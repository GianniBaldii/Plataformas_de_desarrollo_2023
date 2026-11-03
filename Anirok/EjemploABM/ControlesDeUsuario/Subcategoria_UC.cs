using EjemploABM.Controladores;
using EjemploABM.Modelo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using Guna.UI2.WinForms;
using EjemploABM.Servicios;

namespace EjemploABM.ControlesDeUsuario
{
    public partial class Subcategoria_UC : UserControl
    {
        private const int ElementosPorPagina = 8;
        private int paginaActual = 1;
        private List<Subcategoria> subcategorias;
        private readonly Guna2TextBox txtBuscar = new Guna2TextBox();

        public Subcategoria_UC()
        {
            InitializeComponent();
            ListadoVisual.Aplicar(this, guna2DataGridView1, "SUBCATEGORÍAS", "Clasificación específica dentro de cada categoría", btn_add_cat, lblPaginaActual, btn_anterior, btn_siguiente, label1);
            ListadoVisual.AplicarAcciones(guna2DataGridView1);
            txtBuscar.PlaceholderText = "Buscar subcategoría..."; txtBuscar.Location = new System.Drawing.Point(24, 84); txtBuscar.Size = new System.Drawing.Size(300, 30); txtBuscar.TextChanged += (s,e) => { paginaActual = 1; ActualizarVista(); }; Controls.Add(txtBuscar); EstiloAplicacion.AplicarAControl(txtBuscar);
            CargarSubcategorias();
        }

        private void CargarSubcategorias()
        {
            subcategorias = Subcategoria_Controller.obtenerSubcategorias();
            ActualizarVista();
        }

        private void ActualizarVista()
        {
            guna2DataGridView1.Rows.Clear();

            var filtradas = subcategorias.Where(s => string.IsNullOrWhiteSpace(txtBuscar.Text) || s.Nombre.IndexOf(txtBuscar.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            int inicio = (paginaActual - 1) * ElementosPorPagina;
            int fin = Math.Min(inicio + ElementosPorPagina, filtradas.Count);

            for (int i = inicio; i < fin; i++)
            {
                int rowIndex = guna2DataGridView1.Rows.Add();

                guna2DataGridView1.Rows[rowIndex].Cells[0].Value = filtradas[i].Id.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[1].Value = filtradas[i].Nombre.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[2].Value = filtradas[i].categoria_id.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[3].Value = filtradas[i].IsActive.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[4].Value = "Editar";
                guna2DataGridView1.Rows[rowIndex].Cells[5].Value = "Eliminar";
            }

            int paginas = filtradas.Count == 0 ? 0 : (int)Math.Ceiling((double)filtradas.Count / ElementosPorPagina);
            lblPaginaActual.Text = paginas == 0 ? "Sin resultados" : $"Página {paginaActual} de {paginas}";
        }

        private int CalcularTotalDePaginas()
        {
            return (int)Math.Ceiling((double)subcategorias.Count / ElementosPorPagina);
        }

        private void btn_add_cat_Click_1(object sender, EventArgs e)
        {
            FormSubcategorias formsub = new FormSubcategorias();
            DialogResult dr = formsub.ShowDialog();

            if (dr == DialogResult.OK)
            {
                Trace.WriteLine("OK - se creo");
                CargarSubcategorias();
            }
        }

        private void guna2DataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Editar")
            {
                Debug.WriteLine("Valor de la celda: " + guna2DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                int id = int.Parse(guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                Trace.WriteLine("el id es: " + id);

                Subcategoria sub_editar = Subcategoria_Controller.obtenerPorId(id);

                FormSubcategorias frmCat = new FormSubcategorias(sub_editar);

                DialogResult dr = frmCat.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    Trace.WriteLine("OK - se edito");
                    CargarSubcategorias();
                }
            }
            else if (senderGrid.Columns[e.ColumnIndex].Name == "Eliminar")
            {
                Debug.WriteLine("Valor de la celda: " + guna2DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                int id = int.Parse(guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                Trace.WriteLine("el id es: " + id);
                Subcategoria sub_eliminar = Subcategoria_Controller.obtenerPorId(id);
                FormEliminarSub formeliminarsub = new FormEliminarSub(sub_eliminar);
                DialogResult eliminar = formeliminarsub.ShowDialog();
                if (eliminar == DialogResult.OK)
                {
                    Trace.WriteLine("OK - se creo form eliminar");
                    CargarSubcategorias();
                }
            }
        }

        private void btn_siguiente_Click_1(object sender, EventArgs e)
        {
            int totalPaginas = CalcularTotalDePaginas();
            if (paginaActual < totalPaginas)
            {
                paginaActual++;
                ActualizarVista();
            }
        }

        private void btn_anterior_Click_1(object sender, EventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                ActualizarVista();
            }
        }
    }
}

