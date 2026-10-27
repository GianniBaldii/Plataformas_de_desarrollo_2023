using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AnirokWeb.Models;

namespace AnirokWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AnirokDbContext _context = new AnirokDbContext();

        public ActionResult Index()
        {
            if (TempData["LoginSuccessMessage"] != null)
            {
                Debug.WriteLine("LoginSuccessMessage is set.");
            }

            var productos = _context.Productos.OrderBy(p => p.Id).Take(8).ToList();
            return View(productos);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Message = "Your login page.";

            return View();
        }


        public ActionResult Producto()
        {
            ViewBag.Message = "Your prodct page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
