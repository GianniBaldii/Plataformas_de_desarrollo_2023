using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnirokWeb.Models
{
    [Table("usuario")]
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Mail { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Dni { get; set; }
        [Column("contraseña")]
        public string Contrasena { get; set; }
        public int Rol { get; set; }
    }
}
