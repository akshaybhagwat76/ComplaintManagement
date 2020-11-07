using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class DesignationController : Controller
    {
        // GET: Designation
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageDesignationMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageDesignationMaster");
        }
    }
}