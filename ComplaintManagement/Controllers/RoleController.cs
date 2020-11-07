using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class RoleController : Controller
    {
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageRoleMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageRoleMaster");
        }
    }
}