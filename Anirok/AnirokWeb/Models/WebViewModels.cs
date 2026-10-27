using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AnirokWeb.Models
{
    public class RegistroViewModel
    {
        [Required, StringLength(45, MinimumLength = 2), Display(Name = "Nombre de usuario")]
        public string Nombre { get; set; }

        [Required, StringLength(45, MinimumLength = 2), Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required, EmailAddress, StringLength(45), Display(Name = "Email")]
        public string Mail { get; set; }

        [Required, RegularExpression(@"^[0-9+() -]{8,20}$", ErrorMessage = "El teléfono no es válido."), Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required, StringLength(45, MinimumLength = 4), Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required, RegularExpression(@"^\d{7,10}$", ErrorMessage = "El DNI debe tener entre 7 y 10 dígitos."), Display(Name = "DNI")]
        public string Dni { get; set; }

        [Required, StringLength(45, MinimumLength = 6), DataType(DataType.Password), Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [Required, Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden."), DataType(DataType.Password), Display(Name = "Repetir contraseña")]
        public string ConfirmarContrasena { get; set; }
    }

    public class ContactoViewModel
    {
        [Required, StringLength(80), Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; }

        [Required, StringLength(1000, MinimumLength = 10), Display(Name = "Mensaje")]
        public string Mensaje { get; set; }

        public bool Enviado { get; set; }
    }

    [Serializable]
    public class CarritoItem
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

    public class CarritoLineaViewModel
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public double Subtotal { get { return Producto.Precio * Cantidad; } }
    }

    public class CarritoViewModel
    {
        public List<CarritoLineaViewModel> Lineas { get; set; } = new List<CarritoLineaViewModel>();
        public double Total { get { return Lineas.Sum(x => x.Subtotal); } }
    }

    public class CompraViewModel
    {
        [Required, Display(Name = "Método de pago")]
        public string MetodoPago { get; set; } = "Tarjeta";
        public CarritoViewModel Carrito { get; set; }
    }

    public class ComprobanteViewModel
    {
        public int VentaId { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public string MetodoPago { get; set; }
        public double Total { get; set; }
        public List<ComprobanteLineaViewModel> Lineas { get; set; } = new List<ComprobanteLineaViewModel>();
    }

    public class ComprobanteLineaViewModel
    {
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double Subtotal { get { return PrecioUnitario * Cantidad; } }
    }
}
