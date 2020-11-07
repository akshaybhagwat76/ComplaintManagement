using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class LOSController : Controller
    {
        // GET: LOS
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageLOSMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageLOSMaster");
        }
    }
}