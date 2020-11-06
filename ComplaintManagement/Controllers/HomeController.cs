using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index1()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Index2()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}