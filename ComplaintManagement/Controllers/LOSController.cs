using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
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
            ViewBag.lstLOS = new LOSMasterRepository().GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("LOS", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
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