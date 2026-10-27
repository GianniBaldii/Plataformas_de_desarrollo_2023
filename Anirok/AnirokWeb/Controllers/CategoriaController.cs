using AnirokWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics; // Importar para usar Debug.WriteLine()
using Antlr.Runtime.Misc;

namespace AnirokWeb.Controllers
{
    public class CategoriaController : Controller
    {
        private AnirokDbContext _context = new AnirokDbContext();

        // Método para obtener todas las categorías
        public List<Categoria> ObtenerTodasLasCategorias()
        {
            try
            {
                var categorias = _context.Categorias.ToList(); // Obtiene todas las categorías de la base de datos
                return categorias;
            }
            catch (Exception ex)
            {
                // Capturar y mostrar la inner exception
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
                Debug.WriteLine("Error al obtener categorías: " + ex.Message);
                throw; // Lanzar la excepción para depuración adicional
            }
        }






        // Acción que usa el método para mostrar en la vista
 
        public ActionResult Index()
        {
            var categorias = ObtenerTodasLasCategorias();
            return View(categorias);
        }

    }
}
