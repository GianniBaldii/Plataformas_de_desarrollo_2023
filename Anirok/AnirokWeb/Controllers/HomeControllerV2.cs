using AnirokWeb.Models;
using System.Linq;
using System.Web.Mvc;

namespace AnirokWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AnirokDbContext _context = new AnirokDbContext();

        public ActionResult Index()
        {
            return View(_context.Productos.OrderBy(p => p.Id).Take(8).ToList());
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
