using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AnirokWeb.Models
{
    [Table("producto")]
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        [Column("descripción")]
        public string Descripcion { get; set; }
        public double Precio { get; set; }
        public string Img { get; set; }

        [Column("categoria_id"), ForeignKey("Categoria")]
        public int CategoriaId { get; set; }  // Clave foránea a la tabla de Categoría

        // Virtual para uso de lazy loading
        public virtual Categoria Categoria { get; set; } 
    }

}
