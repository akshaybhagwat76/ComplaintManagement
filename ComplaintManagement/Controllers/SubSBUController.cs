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
    public class SubSBUController : Controller
    {
        // GET: SubSBU
        public ActionResult Index()
        {
            ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("SubSBU", null);
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
                retval = new SubSBUMasterRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "SubSBU"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateSubSBU(SubSBUMasterVM SubSBUVM)
        {
            try
            {
                var SBU = new SubSBUMasterRepository().AddOrUpdate(SubSBUVM);
                return new ReplyFormat().Success(Messages.SUCCESS, SBU);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            SubSBUMasterVM SubSBUMasterVM = new SubSBUMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageSubSBUMaster", SubSBUMasterVM);
            
        }

        public ActionResult Edit(int id)
        {
            try
            {
                SubSBUMasterVM SubSBUVM = new SubSBUMasterRepository().Get(id);
                ViewBag.PageType = "Edit";
                return View("ManageSubSBUMaster", SubSBUVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
           
        }
    }
}