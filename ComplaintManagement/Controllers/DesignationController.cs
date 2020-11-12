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
    public class DesignationController : Controller
    {
        // GET: Designation
        public ActionResult Index()
        {
            ViewBag.lstDesignatio = new DesignationMasterRepository().GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("Categories", null);
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
                retval = new DesignationMasterRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Designation"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateDesignation(DesignationMasterVM DesignationVM)
        {
            try
            {
                var Designation = new DesignationMasterRepository().AddOrUpdate(DesignationVM);
                return new ReplyFormat().Success(Messages.SUCCESS, Designation);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            DesignationMasterVM DesignationMasterVM = new DesignationMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageDesignationMaster", DesignationMasterVM);
            
        }

        public ActionResult Edit(int Id)
        {
            try
            {
                DesignationMasterVM DesignationVM= new DesignationMasterRepository().Get(Id);
                ViewBag.PageType = "Edit";
                return View("ManageDesignationMaster", DesignationVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
            
        }
    }
}