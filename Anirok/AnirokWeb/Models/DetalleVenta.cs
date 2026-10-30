using System.ComponentModel.DataAnnotations.Schema;

namespace AnirokWeb.Models
{
    [Table("detalle_venta")]
    public class DetalleVenta
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }

        [Column("venta_id"), ForeignKey("Venta")]
        public int VentaId { get; set; }

        [Column("producto_id"), ForeignKey("Producto")]
        public int ProductoId { get; set; }

        [Column("precio_unitario")]
        public double PrecioUnitario { get; set; }

        public virtual Venta Venta { get; set; }
        public virtual Producto Producto { get; set; }

        [NotMapped]
        public double Subtotal { get { return Cantidad * PrecioUnitario; } }
    }
}
