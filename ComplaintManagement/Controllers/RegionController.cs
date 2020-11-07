using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class RegionController : Controller
    {
        // GET: Region
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageRegionMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageRegionMaster");
        }
    }
}