using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Selection()
        {
            ViewBag.Message = "量化选股";

            return View();
        }

        public ActionResult Strategy()
        {
            ViewBag.Message = "量化策略";

            return View();
        }
    }
}