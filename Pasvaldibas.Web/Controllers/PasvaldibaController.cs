using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pasvaldibas.Web.Controllers
{
    public class PasvaldibaController : Controller
    {
        // GET: Pasvaldiba
        public ActionResult Index()
        {
            return View();
        }

        // GET: Pasvaldiba/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Pasvaldiba/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pasvaldiba/Create
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

        // GET: Pasvaldiba/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Pasvaldiba/Edit/5
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

        // GET: Pasvaldiba/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Pasvaldiba/Delete/5
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
