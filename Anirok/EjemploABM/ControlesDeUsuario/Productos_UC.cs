using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using EjemploABM.Controladores;
using EjemploABM.Modelo;
using EjemploABM.Servicios;

namespace EjemploABM.ControlesDeUsuario
{
    public partial class Productos_UC : UserControl
    {
        List<Producto> productos;
        int elementosPorPagina = 8;
        int paginaActual = 1;
        int totalDePaginas;

        public Productos_UC()
        {
            InitializeComponent();
            ListadoVisual.Aplicar(this, guna2DataGridView1, "PRODUCTOS", "Catálogo y administración de productos", btn_add_cat, lblPaginaActual, btn_anterior, btn_siguiente, label2);
            txtBusqueda.PlaceholderText = "Buscar producto...";
            label1.Location = new System.Drawing.Point(24, 91);
            txtBusqueda.Location = new System.Drawing.Point(145, 85);
            cargarProductos();
            txtBusqueda.TextChanged += TxtBusqueda_TextChanged;
        }

        private void TxtBusqueda_TextChanged(object sender, EventArgs e)
        {
            paginaActual = 1;
            FiltrarProductosPorNombre(txtBusqueda.Text);
        }

        private void FiltrarProductosPorNombre(string nombre)
        {
            if (productos != null)
            {
                List<Producto> productosFiltrados = productos
                    .Where(prod => prod.Nombre.ToLower().Contains(nombre.ToLower()))
                    .ToList();

                int totalDePaginasFiltradas = (int)Math.Ceiling((double)productosFiltrados.Count / elementosPorPagina);
                int inicio = (paginaActual - 1) * elementosPorPagina;
                int fin = Math.Min(inicio + elementosPorPagina, productosFiltrados.Count);

                guna2DataGridView1.Rows.Clear();

                for (int i = inicio; i < fin; i++)
                {
                    Producto prod = productosFiltrados[i];
                    int rowIndex = guna2DataGridView1.Rows.Add();

                    guna2DataGridView1.Rows[rowIndex].Cells[0].Value = prod.Id.ToString();
                    guna2DataGridView1.Rows[rowIndex].Cells[1].Value = prod.Nombre.ToString();
                    guna2DataGridView1.Rows[rowIndex].Cells[2].Value = prod.Precio.ToString("C0");


                    guna2DataGridView1.Rows[rowIndex].Cells[3].Value = "Ver";
                    guna2DataGridView1.Rows[rowIndex].Cells[4].Value = "Editar";
                    guna2DataGridView1.Rows[rowIndex].Cells[5].Value = "Eliminar";
                }

                lblPaginaActual.Text = totalDePaginasFiltradas == 0 ? "Sin resultados" : $"Página {paginaActual} de {totalDePaginasFiltradas}";
            }
        }

        private void cargarProductos()
        {
            productos = Producto_Controller.obtenerProductos();

            totalDePaginas = (int)Math.Ceiling((double)productos.Count / elementosPorPagina);
            int inicio = (paginaActual - 1) * elementosPorPagina;
            int fin = Math.Min(inicio + elementosPorPagina, productos.Count);

            guna2DataGridView1.Rows.Clear();

            for (int i = inicio; i < fin; i++)
            {
                Producto prod = productos[i];
                int rowIndex = guna2DataGridView1.Rows.Add();

                guna2DataGridView1.Rows[rowIndex].Cells[0].Value = prod.Id.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[1].Value = prod.Nombre.ToString();
                guna2DataGridView1.Rows[rowIndex].Cells[2].Value = prod.Precio.ToString("C0");
              

                guna2DataGridView1.Rows[rowIndex].Cells[3].Value = "Ver";
                guna2DataGridView1.Rows[rowIndex].Cells[4].Value = "Editar";
                guna2DataGridView1.Rows[rowIndex].Cells[5].Value = "Eliminar";
            }

            lblPaginaActual.Text = totalDePaginas == 0 ? "Sin productos" : $"Página {paginaActual} de {totalDePaginas}";
        }

        private void btn_anterior_Click(object sender, EventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                cargarProductos();
            }
        }

        private void btn_siguiente_Click(object sender, EventArgs e)
        {
            if ((paginaActual * elementosPorPagina) < productos.Count)
            {
                paginaActual++;
                cargarProductos();
            }
        }

        private void btn_add_cat_Click(object sender, EventArgs e)
        {
            FormProducto frmProd = new FormProducto();
            DialogResult dr = frmProd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                Trace.WriteLine("OK - se creo");
                cargarProductos();
            }
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            Trace.WriteLine("estoy andando");
            Debug.WriteLine("Celda seleccionada: " + e.ColumnIndex + ", " + e.RowIndex);

            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex].Name == "Editar")
            {
                int id = int.Parse(guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                Trace.WriteLine("el id es: " + id);

                Producto prod_editar = Producto_Controller.obtenerPorId(id);

                FormProducto formprod = new FormProducto(prod_editar);

                DialogResult dr = formprod.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    Trace.WriteLine("OK - se edito");
                    cargarProductos();
                }
            }
            else if (senderGrid.Columns[e.ColumnIndex].Name == "Eliminar")
            {
                int id = int.Parse(guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                Trace.WriteLine("el id es: " + id);
                Producto prod_elim = Producto_Controller.obtenerPorId(id);
                FormEliminarProd formeliminarprod = new FormEliminarProd(prod_elim);
                DialogResult eliminar = formeliminarprod.ShowDialog();
                if (eliminar == DialogResult.OK)
                {
                    Trace.WriteLine("OK - se creo form eliminar");
                    cargarProductos();
                }
            }
            else if (senderGrid.Columns[e.ColumnIndex].Name == "Ver")
            {
                int id = int.Parse(guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                Trace.WriteLine("el id es: " + id);
                Producto prod_elim = Producto_Controller.obtenerPorId(id);
                FormVerProd formverprod = new FormVerProd(prod_elim);
                DialogResult ver = formverprod.ShowDialog();
                if (ver == DialogResult.OK)
                {
                    Trace.WriteLine("OK - se creo form eliminar");
                    cargarProductos();
                }
            }
        }

        
    }
}


