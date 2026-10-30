using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploABM.Modelo
{
    public class Producto
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double Precio { get; set; }
        public double Costo { get; set; }
        public string CostoMoneda { get; set; }
        public double CotizacionDolar { get; set; }
        public double CostoPesos { get; set; }
        public string codigo { get; set; }
        public int Stock { get; set; }
        public string Img { get; set; }

        public string Talle { get; set; }

        public string Proveedor { get; set; }

        public int CategoriaId { get; set; }

        public int SubcategoriaId { get; set; }

        public int CantidadSeleccionada { get; set; }




        public Producto(int id, string name, string desc, double precio, string cod, string img, string proveedor, int subcategoriaid ,int categoriaid, string talle, int stock)
        {
            Id = id;
            Nombre = name;
            Descripcion = desc;
            Precio = precio;
            codigo = cod;
            Img = img;
            Proveedor = proveedor;
            SubcategoriaId = subcategoriaid;
            CategoriaId = categoriaid;
            Talle = talle;
            Stock = stock;

        }

        public Producto()
        {
        }


    }
}
