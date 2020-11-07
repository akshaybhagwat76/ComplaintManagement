using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class LocationController : Controller
    {
        // GET: Location
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageLocationMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageLocationMaster");
        }
    }
}