using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnirokWeb.Controllers
{
    public class AutenticacionController : Controller
    {
        // GET: Autenticacion
        public ActionResult Index()
        {
            return View();
        }

        // GET: Autenticacion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Autenticacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Autenticacion/Create
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

        // GET: Autenticacion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Autenticacion/Edit/5
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

        // GET: Autenticacion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Autenticacion/Delete/5
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
