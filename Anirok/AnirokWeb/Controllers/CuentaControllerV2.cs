using AnirokWeb.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace AnirokWeb.Controllers
{
    public class CuentaController : Controller
    {
        private readonly AnirokDbContext _context = new AnirokDbContext();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(string usuarioOEmail, string contrasena)
        {
            var identificador = (usuarioOEmail ?? "").Trim();
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Mail == identificador || u.Nombre == identificador);
            if (usuario == null || usuario.Contrasena != contrasena)
            {
                TempData["ErrorMessage"] = "Usuario/email o contraseña incorrectos.";
                return View();
            }

            Session["UsuarioId"] = usuario.Id;
            Session["UsuarioNombre"] = usuario.Nombre;
            TempData["LoginSuccessMessage"] = "Inicio de sesión exitoso.";
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

            var siguienteId = _context.Usuarios.Select(u => (int?)u.Id).Max() ?? 0;
            var usuario = new Usuario
            {
                Id = siguienteId + 1, Nombre = nombre, Apellido = model.Apellido.Trim(), Mail = mail,
                Telefono = model.Telefono.Trim(), Direccion = model.Direccion.Trim(), Dni = model.Dni.Trim(),
                Contrasena = model.Contrasena, Rol = 3
            };
            try
            {
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "No se pudo crear la cuenta. Revisá que el usuario, email y DNI no estén repetidos.");
                return View(model);
            }

            Session["UsuarioId"] = usuario.Id;
            Session["UsuarioNombre"] = usuario.Nombre;
            TempData["LoginSuccessMessage"] = "Cuenta creada correctamente.";
            return RedirectToAction("Index", "Home");
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
