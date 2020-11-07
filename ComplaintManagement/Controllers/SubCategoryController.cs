using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class SubCategoryController : Controller
    {
        // GET: SubCategory
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageSubCategoryMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageSubCategoryMaster");
        }
    }
}