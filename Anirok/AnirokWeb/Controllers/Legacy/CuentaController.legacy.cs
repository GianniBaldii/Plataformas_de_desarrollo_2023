using AnirokWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnirokWeb.Controllers
{
    public class CuentaController : Controller
    {
        private AnirokDbContext _context = new AnirokDbContext(); // Acceso al contexto de la BD


        // GET: /Cuenta/Login
        public ActionResult Login()
        {
            
            return View();
        }

        // POST: /Cuenta/Login
        [HttpPost]
        public ActionResult Login(string email, string contrasena)
        {
            // Busca el usuario en la base de datos por email
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Mail == email);

            if (usuario != null)
            {
                // Verifica si la contraseña ingresada coincide con la de la base de datos
                if (usuario.Contrasena == contrasena)
                {

                    Session["UsuarioId"] = usuario.Id;
                    Session["UsuarioNombre"] = usuario.Nombre;

                    TempData["LoginSuccessMessage"] = "Inicio de sesión exitoso.";
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    // Contraseña incorrecta
                    TempData["ErrorMessage"] = "Contraseña incorrecta.";
                }
            }
            else
            {
                // Usuario no encontrado
                TempData["ErrorMessage"] = "El email ingresado no está registrado.";
            }

            // Retorna la vista de login para que el usuario pueda intentar nuevamente
            return View();
        }


        // POST: /Cuenta/Logout
        public ActionResult Logout() 
        {
            // Limpiar las variables de sesión
            Session.Clear(); // Elimina todos los elementos de la sesión actual
            Session.Abandon(); // Cierra la sesión

            

            // Redirigir al login
            return RedirectToAction("Login", "Cuenta");

        }
    }
}