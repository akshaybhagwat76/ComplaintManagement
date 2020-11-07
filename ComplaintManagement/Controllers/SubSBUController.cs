using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class SubSBUController : Controller
    {
        // GET: SubSBU
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageSubSBUMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageSubSBUMaster");
        }
    }
}