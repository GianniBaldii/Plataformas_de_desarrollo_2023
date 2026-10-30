using EjemploABM.Controladores;
using EjemploABM.Modelo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using EjemploABM.Servicios;

namespace EjemploABM.ControlesDeUsuario
{
    public partial class Categorias_UC : UserControl
    {
        private const int ElementosPorPagina = 8;
        private int paginaActual = 1;
        private List<Categoria> categorias;

        public Categorias_UC()
        {
            InitializeComponent();
            ListadoVisual.Aplicar(this, guna2DataGridView1, "CATEGORÍAS", "Organización del catálogo por familias de productos", btn_add_cat, lblPaginaActual, btn_anterior, btn_siguiente, label1);
            CargarCategorias();
        }

        private void CargarCategorias()
        {
            categorias = Categoria_Controller.ObtenerCategorias();
            ActualizarVista();
        }

        private void ActualizarVista()
        {
            guna2DataGridView1.Rows.Clear();

            int inicio = (paginaActual - 1) * ElementosPorPagina;
            int fin = Math.Min(inicio + ElementosPorPagina, categorias.Count);

            for (int i = inicio; i < fin; i++)
            {
                int rowIndex = guna2DataGridView1.Rows.Add();

                guna2DataGridView1.Rows[rowIndex].Cells[0].Value = categorias[i].Id.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[1].Value = categorias[i].Nombre.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[2].Value = categorias[i].IsActive.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[3].Value = "Editar";
                guna2DataGridView1.Rows[rowIndex].Cells[4].Value = "Eliminar";
            }

            int totalDePaginasFiltradas = CalcularTotalDePaginasFiltradas(); // Reemplaza con tu lógica para calcular el total de páginas filtradas

            lblPaginaActual.Text = $"Página {paginaActual} de {totalDePaginasFiltradas}";
        }

        private int CalcularTotalDePaginasFiltradas()
        {
            // Aquí debes incluir la lógica para calcular el total de páginas
            // considerando los filtros aplicados a tus datos.
            // Puedes adaptar esta lógica según tus necesidades.

            int totalDePaginasFiltradas = (int)Math.Ceiling((double)categorias.Count / ElementosPorPagina);

            return totalDePaginasFiltradas;
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Editar")
            {
                //EDITAMOS
                Debug.WriteLine("Valor de la celda: " + guna2DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                int id = int.Parse(guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                Trace.WriteLine("el id es: " + id);

                Categoria cat_editar = Categoria_Controller.obtenerPorId(id);

                FormCategorias frmCat = new FormCategorias(cat_editar);

                DialogResult dr = frmCat.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    Trace.WriteLine("OK - se edito");
                    //ACTUALIZAR LA LISTA
                    CargarCategorias();
                }
            }
            else if (senderGrid.Columns[e.ColumnIndex].Name == "Eliminar")
            {
                Debug.WriteLine("Valor de la celda: " + guna2DataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                int id = int.Parse(guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                Trace.WriteLine("el id es: " + id);
                Categoria cat_eliminar = Categoria_Controller.obtenerPorId(id);

                DialogResult eliminar;
                if (Program.logueado.Rol == 1 || Program.logueado.Rol == 2)
                {
                    FormEliminarAdminCat formeliminarcat = new FormEliminarAdminCat(cat_eliminar);
                    eliminar = formeliminarcat.ShowDialog();
                }
                else
                {
                    // En caso de otro valor de logueado.Rol, puedes manejarlo según tus necesidades.
                    eliminar = DialogResult.Cancel;
                }

                if (eliminar == DialogResult.OK)
                {
                    Trace.WriteLine("OK - se creó el formulario para eliminar");
                    CargarCategorias();
                }
            }
        }

        private void btn_add_cat_Click(object sender, EventArgs e)
        {
            FormCategorias frmCat = new FormCategorias();
            DialogResult dr = frmCat.ShowDialog();

            if (dr == DialogResult.OK)
            {
                Trace.WriteLine("OK - se creo");
                CargarCategorias();
            }
        }

        private void Categorias_UC_Load(object sender, EventArgs e)
        {
            // Código de carga, si es necesario
        }

        private void btn_siguiente_Click(object sender, EventArgs e)
        {
            int totalPaginas = (int)Math.Ceiling((double)categorias.Count / ElementosPorPagina);
            Debug.WriteLine($"Página actual: {paginaActual}, Total de páginas: {totalPaginas}");

            if (paginaActual < totalPaginas)
            {
                paginaActual++;
                ActualizarVista();
            }
        }

        private void btn_anterior_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"Página actual: {paginaActual}");

            if (paginaActual > 1)
            {
                paginaActual--;
                ActualizarVista();
            }
        }
    }
}
