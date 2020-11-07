using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class SBUController : Controller
    {
        // GET: SBU
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageSBUMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageSBUMaster");
        }
    }
}