using AnirokWeb.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace AnirokWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AnirokDbContext _context = new AnirokDbContext();

        public ActionResult Index()
        {
            var productos = _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Stock > 0)
                .OrderBy(p => p.Id)
                .Take(8)
                .ToList();
            return View(productos);
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View("ContactV2", new ContactoViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Contact(ContactoViewModel model)
        {
            if (!ModelState.IsValid) return View("ContactV2", model);
            model.Enviado = true;
            return View("ContactV2", model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
