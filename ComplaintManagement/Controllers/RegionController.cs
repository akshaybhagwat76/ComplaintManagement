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
    public class RegionController : Controller
    {
        // GET: Region
        public ActionResult Index()
        {
            ViewBag.lstRegion = new RegionMasterRepository().GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("Region", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            bool retval = true;
            try
            {
                retval = new RegionMasterRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Region"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateRegion(RegionMasterVM RegionVM)
        {
            try
            {
                var Region = new RegionMasterRepository().AddOrUpdate(RegionVM);
                return new ReplyFormat().Success(Messages.SUCCESS, Region);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            RegionMasterVM RegionMasterVM = new RegionMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageRegionMaster", RegionMasterVM);
        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                RegionMasterVM RegionVM = new RegionMasterRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                return View("ManageRegionMaster", RegionVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
        }
    }
}