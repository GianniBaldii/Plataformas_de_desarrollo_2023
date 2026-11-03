using AnirokWeb.Models;
using AnirokWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AnirokWeb.Controllers
{
    public class CarritoController : Controller
    {
        private const string CarritoKey = "Carrito";
        private readonly AnirokDbContext _context = new AnirokDbContext();

        public ActionResult Index()
        {
            return View("Index", ArmarCarrito());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AgregarAlCarrito(int productoId, int cantidad = 1)
        {
            var producto = _context.Productos.FirstOrDefault(p => p.Id == productoId);
            if (producto == null) return HttpNotFound();
            if (producto.Stock <= 0)
            {
                if (Request.IsAjaxRequest()) return Json(new { ok = false, message = "Este producto está agotado." });
                TempData["ErrorMessage"] = "El producto seleccionado no tiene stock.";
                return RedirectToAction("Index");
            }
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(x => x.ProductoId == productoId);
            var cantidadSolicitada = Math.Max(1, cantidad);
            var cantidadActual = item == null ? 0 : item.Cantidad;
            if (cantidadActual + cantidadSolicitada > producto.Stock)
            {
                if (Request.IsAjaxRequest()) return Json(new { ok = false, message = "Sólo quedan " + producto.Stock + " unidades de " + producto.Nombre + "." });
                TempData["ErrorMessage"] = "Sólo quedan " + producto.Stock + " unidades de " + producto.Nombre + ".";
                return RedirectToAction("Index");
            }
            if (item == null)
            {
                carrito.Add(new CarritoItem { ProductoId = productoId, Cantidad = cantidadSolicitada });
            }
            else
            {
                item.Cantidad += cantidadSolicitada;
            }
            Session[CarritoKey] = carrito;
            if (Request.IsAjaxRequest()) return Json(new { ok = true, message = producto.Nombre + " agregado al carrito.", count = carrito.Sum(x => x.Cantidad) });
            TempData["SuccessMessage"] = "Producto añadido al carrito.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Actualizar(int productoId, int cantidad)
        {
            var carrito = ObtenerCarrito();
            var producto = _context.Productos.FirstOrDefault(p => p.Id == productoId);
            if (producto == null) return HttpNotFound();
            if (cantidad > producto.Stock)
            {
                TempData["ErrorMessage"] = "Sólo quedan " + producto.Stock + " unidades de " + producto.Nombre + ".";
                return RedirectToAction("Index");
            }
            carrito.RemoveAll(x => x.ProductoId == productoId);
            if (cantidad > 0) carrito.Add(new CarritoItem { ProductoId = productoId, Cantidad = Math.Min(cantidad, 99) });
            Session[CarritoKey] = carrito;
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EliminarDelCarrito(int productoId)
        {
            var carrito = ObtenerCarrito();
            carrito.RemoveAll(x => x.ProductoId == productoId);
            Session[CarritoKey] = carrito;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Comprar()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Cuenta", new { returnUrl = Url.Action("Comprar", "Carrito") });
            if (!RolesWeb.EsCliente(Session["UsuarioRol"]))
            {
                TempData["ErrorMessage"] = "Sólo las cuentas de cliente pueden realizar compras desde la Web.";
                return RedirectToAction("Index", "Home");
            }
            var carrito = ArmarCarrito();
            if (!carrito.Lineas.Any()) return RedirectToAction("Index");
            return View(new CompraViewModel { Carrito = carrito });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Comprar(CompraViewModel model)
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", "Cuenta", new { returnUrl = Url.Action("Comprar", "Carrito") });
            if (!RolesWeb.EsCliente(Session["UsuarioRol"]))
            {
                TempData["ErrorMessage"] = "Sólo las cuentas de cliente pueden realizar compras desde la Web.";
                return RedirectToAction("Index", "Home");
            }
            model.Carrito = ArmarCarrito();
            if (!ModelState.IsValid) return View(model);
            try
            {
                var service = new CompraService(AnirokDbContext.ResolveConnectionString());
                var usuarioId = Convert.ToInt32(Session["UsuarioId"]);
                var ventaId = service.CrearVenta(usuarioId, ObtenerCarrito(), model.MetodoPago);
                Session.Remove(CarritoKey);
                try
                {
                    var comprobante = service.ObtenerComprobante(ventaId, usuarioId);
                    service.GuardarPdf(comprobante);
                    if (!string.IsNullOrWhiteSpace(comprobante.Email))
                        EmailService.EnviarComprobante(comprobante.Email, ventaId, service.CrearPdf(comprobante));
                }
                catch (Exception)
                {
                    TempData["WarningMessage"] = "La compra está confirmada, pero el comprobante no pudo enviarse por correo. Podés descargarlo ahora y reintentarlo desde el historial de envíos.";
                }
                return RedirectToAction("CompraExitosa", new { id = ventaId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public ActionResult CompraExitosa(int id)
        {
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Cuenta");
            if (!RolesWeb.EsCliente(Session["UsuarioRol"])) return new HttpStatusCodeResult(403);
            var comprobante = new CompraService(AnirokDbContext.ResolveConnectionString()).ObtenerComprobante(id, Convert.ToInt32(Session["UsuarioId"]));
            if (comprobante == null) return HttpNotFound();
            return View(comprobante);
        }

        public ActionResult Comprobante(int id)
        {
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Cuenta");
            if (!RolesWeb.EsCliente(Session["UsuarioRol"])) return new HttpStatusCodeResult(403);
            var service = new CompraService(AnirokDbContext.ResolveConnectionString());
            var comprobante = service.ObtenerComprobante(id, Convert.ToInt32(Session["UsuarioId"]));
            if (comprobante == null) return HttpNotFound();
            return File(service.ObtenerOCrearPdf(comprobante), "application/pdf", "Anirok-venta-" + id + ".pdf");
        }

        private List<CarritoItem> ObtenerCarrito()
        {
            return Session[CarritoKey] as List<CarritoItem> ?? new List<CarritoItem>();
        }

        private CarritoViewModel ArmarCarrito()
        {
            var carrito = ObtenerCarrito();
            var ids = carrito.Select(x => x.ProductoId).ToList();
            var productos = _context.Productos.Where(p => ids.Contains(p.Id)).ToList();
            return new CarritoViewModel
            {
                Lineas = carrito.Join(productos, item => item.ProductoId, producto => producto.Id, (item, producto) => new CarritoLineaViewModel { Producto = producto, Cantidad = item.Cantidad }).ToList()
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}


