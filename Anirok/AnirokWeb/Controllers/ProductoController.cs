using AnirokWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;  // Para usar Include

namespace AnirokWeb.Controllers
{
    public class ProductoController : Controller
    {
        private AnirokDbContext _context = new AnirokDbContext();

        // Acción para mostrar productos de cada categoria
        public ActionResult Categoria(string nombreCategoria)
        {
            // Busca la categoría en la base de datos
            var categoria = _context.Categorias.FirstOrDefault(c => c.Nombre == nombreCategoria);

            if (categoria == null)
            {
                return HttpNotFound(); // Si la categoría no existe
            }

            // Consultamos los productos directamente desde la base de datos usando Entity Framework
            var productosCategoria = _context.Productos
                                    .Where(p => p.CategoriaId == categoria.Id)
                                    .ToList()
                                    .Select(p => new Producto
                                    {
                                        Id = p.Id,
                                        Nombre = p.Nombre,
                                        Descripcion = p.Descripcion,
                                        Img = p.Img,
                                        Precio = p.Precio, 
                                        CategoriaId = p.CategoriaId
                                    }).ToList();


            // Imprimir en la consola los productos para verificar
            foreach (var producto in productosCategoria)
            {
                Debug.WriteLine($"Producto: {producto.Nombre} - Precio: {producto.Precio}");
            }


            // Si no se encuentran productos, mostrar un mensaje
            ViewBag.Categoria = nombreCategoria;
            return View("Categoria", productosCategoria);
        }


        //Devolver las categorias disponibles
        public ActionResult Categorias()
        {
            //var todasLasCategorias = ObtenerTodosLosProductos().Select(p => p.Categoria).Distinct().ToList();
            var categorias = _context.Categorias.Where(c => c.Esta_activo == "Activa").ToList();

            // Imprimir todas las categorías en la consola para verificar si trae "Cartas"
            foreach (var categoria in categorias)
            {
                Debug.WriteLine($"Categoría: {categoria.Nombre} - Activo: {categoria.Esta_activo}");
            }

            if (categorias == null || !categorias.Any())
            {
                // Puedes loguear si no hay categorías
                Debug.WriteLine("No se encontraron categorías.");
                return PartialView("_CategoriasPartial", new List<Categoria>()); // Si no hay categorías, pasa una lista vacía
            }

            return PartialView("_CategoriasPartial", categorias);
        }



        // GET: Producto
        public ActionResult Index()
        {
            // Redirige a la acción "Categoria" con la primera categoría como parámetro
            return RedirectToAction("Categoria", new { nombreCategoria = "Default" });
        }



        //Accion para ver un producto 
        public ActionResult Ver(int id)
        {
            // Obtener el producto de la base de datos usando su ID
            var producto = _context.Productos.FirstOrDefault(p => p.Id == id);

            if (producto == null)
            {
                // Manejo de error: Producto no encontrado
                return RedirectToAction("Categoria");
            }
            ViewBag.Title = producto.Nombre;
            return View("DetalleProducto", producto);
        }

        public ActionResult Buscar(string buscar)
        {
            var texto = (buscar ?? "").Trim();
            var productos = _context.Productos
                .Where(p => texto == "" || p.Nombre.Contains(texto) || p.Descripcion.Contains(texto))
                .OrderBy(p => p.Nombre)
                .ToList();
            ViewBag.Categoria = texto == "" ? "Todos los productos" : "Resultados para: " + texto;
            return View("Categoria", productos);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }

    }
}
