using AnirokWeb.Models;
using AnirokWeb.Services;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AnirokWeb.Controllers
{
    public class CuentaController : Controller
    {
        private readonly AnirokDbContext _context = new AnirokDbContext();

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(string usuarioOEmail, string contrasena, string returnUrl)
        {
            var identificador = (usuarioOEmail ?? "").Trim();
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Mail == identificador || u.Nombre == identificador);
            if (usuario == null || !PasswordService.Verify(contrasena, usuario.Contrasena))
            {
                TempData["ErrorMessage"] = "Usuario/email o contraseña incorrectos.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            Session["UsuarioId"] = usuario.Id;
            Session["UsuarioNombre"] = usuario.Nombre;
            Session["UsuarioRol"] = usuario.Rol;
            // Migra automáticamente las cuentas antiguas que todavía estuvieran en texto plano.
            if (!string.IsNullOrEmpty(usuario.Contrasena) && !usuario.Contrasena.StartsWith("sha256$", StringComparison.OrdinalIgnoreCase))
            {
                usuario.Contrasena = PasswordService.Hash(contrasena);
                _context.SaveChanges();
            }
            TempData["LoginSuccessMessage"] = "Inicio de sesión exitoso.";
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Registro()
        {
            return View(new RegistroViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Registro(RegistroViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var nombre = model.Nombre.Trim();
            var mail = model.Mail.Trim();
            if (_context.Usuarios.Any(u => u.Nombre == nombre || u.Mail == mail))
            {
                ModelState.AddModelError("", "El nombre de usuario o el email ya están registrados.");
                return View(model);
            }

            try
            {
                var usuarioId = CuentaService.Registrar(model, AnirokDbContext.ResolveConnectionString());
                Session["UsuarioId"] = usuarioId;
                Session["UsuarioNombre"] = nombre;
                Session["UsuarioRol"] = 3;
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            TempData["LoginSuccessMessage"] = "Cuenta creada correctamente.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Compras()
        {
            if (Session["UsuarioId"] == null)
                return RedirectToAction("Login", new { returnUrl = Url.Action("Compras", "Cuenta") });
            if (!RolesWeb.EsCliente(Session["UsuarioRol"]))
            {
                TempData["ErrorMessage"] = "El historial de compras está disponible para cuentas de cliente.";
                return RedirectToAction("Index", "Home");
            }

            var usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            var compras = _context.Ventas
                .Where(v => v.UsuarioId == usuarioId)
                .OrderByDescending(v => v.Fecha)
                .ThenByDescending(v => v.Id)
                .Select(v => new CompraResumenViewModel
                {
                    Id = v.Id,
                    Fecha = v.Fecha,
                    Total = v.PrecioTotal,
                    MetodoPago = v.MetodoPago,
                    Descuento = v.Descuento
                }).ToList();
            return View(compras);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
