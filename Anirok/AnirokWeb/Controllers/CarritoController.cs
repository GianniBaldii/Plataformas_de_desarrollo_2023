using AnirokWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnirokWeb.Controllers
{
    public class CarritoController : Controller
    {
        private AnirokDbContext _context = new AnirokDbContext();
        // GET: Carrito
        public ActionResult Index()
        {
            // Verifica si el usuario está logueado
            if (Session["UsuarioId"] == null)
            {
                // Si no está logueado, redirigir a la vista de login
                return RedirectToAction("Login", "Cuenta");
            }

            // Recupera el carrito de la sesión o inicializa una lista vacía si no existe
            List<Producto> carrito = Session["Carrito"] as List<Producto> ?? new List<Producto>();

            // Si está logueado, mostrar la vista del carrito

            return View(carrito);
        }

        // Acción para añadir un producto al carrito
        public ActionResult AgregarAlCarrito(int productoId)
        {
            //Verificar que el usuario este logueado para añadir al carrito, sino redirige al login

            if (Session["UsuarioId"] == null)
            {
                // Si no está logueado, redirige al login
                TempData["ErrorMessage"] = "Debes iniciar sesión para añadir productos al carrito.";
                return RedirectToAction("Login", "Cuenta");
            }

            // Obtiene el producto por su ID
            var producto = _context.Productos.FirstOrDefault(p => p.Id == productoId);

            if (producto != null)
            {
                // Recupera el carrito de la sesión (o crea uno nuevo si no existe)
                List<Producto> carrito = Session["Carrito"] as List<Producto> ?? new List<Producto>();

                // Agrega el producto al carrito
                carrito.Add(producto);

                // Guarda el carrito de vuelta en la sesión
                Session["Carrito"] = carrito;

                TempData["SuccessMessage"] = "Producto añadido al carrito!";
                Debug.WriteLine($"Producto con ID {productoId} añadido al carrito por el usuario {Session["UsuarioNombre"]}");
            }

            return RedirectToAction("Index", "Home");
        }

        //GET: Carrito | Ver carrito
        // Acción para mostrar la vista del carrito
        public ActionResult VerCarrito()
        {
            // Recupera el carrito de la sesión
            List<Producto> carrito = Session["Carrito"] as List<Producto> ?? new List<Producto>();

            return View(carrito);
        }

        // POST: borrar producto del carrito
        public ActionResult EliminarDelCarrito(int productoId)
        {
            // Recupera el carrito de la sesión
            List<Producto> carrito = Session["Carrito"] as List<Producto> ?? new List<Producto>();

            //Traer producto de la lista del carrito
            var producto = carrito.FirstOrDefault(p => p.Id == productoId);
            if (producto != null)
            {
                carrito.Remove(producto);
            }

            //Guardar en la sesión la lista actualizada
            Session["Carrito"] = carrito;

            //Redirigir a la vista del carrito
            return RedirectToAction("Index");
        }


        // POST: Carrito/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Carrito/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Carrito/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Carrito/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Carrito/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
