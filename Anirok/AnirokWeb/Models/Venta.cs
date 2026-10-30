using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnirokWeb.Models
{
    [Table("venta")]
    public class Venta
    {
        public int Id { get; set; }

        [Column("precio_total")]
        public double PrecioTotal { get; set; }

        public DateTime Fecha { get; set; }

        [Column("metodo_de_pago")]
        public string MetodoPago { get; set; }

        public double Descuento { get; set; }

        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        public virtual ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
