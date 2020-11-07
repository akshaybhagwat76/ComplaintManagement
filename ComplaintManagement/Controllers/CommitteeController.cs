using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class CommitteeController : Controller
    {
        // GET: Committee
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View("ManageCommitteeMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageCommitteeMaster");
        }
    }
}