﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pasvaldibas.Web.Controllers
{
    public class PasvaldibaController : Controller
    {
        // GET: Pasvaldiba
        public ActionResult Index(string id)
        {
            return View();
        }
    }
}
