using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploABM.Modelo
{
    internal class DetalleVenta
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public double PrecioUnitario { get; set; }

        public Venta Venta { get; set; }
        
        public Producto Producto { get; set; }

        public DetalleVenta()
        {
            // Constructor vacío
        }

        public DetalleVenta(int id, int cantidad, int ventaId, int productoId, double precioUnitario)
        {
            Id = id;
            Cantidad = cantidad;
            VentaId = ventaId;
            ProductoId = productoId;
            PrecioUnitario = precioUnitario;
        }
    }
}
