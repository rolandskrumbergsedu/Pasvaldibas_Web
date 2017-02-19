using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pasvaldibas.Web.Controllers
{
    public class CilveksController : Controller
    {
        // GET: Cilveks
        public ActionResult Index()
        {
            return View();
        }

        // GET: Cilveks/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Cilveks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cilveks/Create
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

        // GET: Cilveks/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Cilveks/Edit/5
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

        // GET: Cilveks/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Cilveks/Delete/5
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
