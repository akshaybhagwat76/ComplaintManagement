using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageCategoryMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageCategoryMaster");
        }
    }
}
