using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnirokWeb.Models
{
    [Table("categoria")]
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Esta_activo { get; set; }

        // Colección de productos en esta categoría
        public virtual ICollection<Producto> Productos { get; set; }
    }
}
