using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class EntityController : Controller
    {
        // GET: Entity
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageEntityMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageEntityMaster");
        }
    }
}