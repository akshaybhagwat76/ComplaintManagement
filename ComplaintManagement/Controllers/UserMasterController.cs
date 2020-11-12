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
    public class UserMasterController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            ViewBag.lstUser = new UserMastersRepository().GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("User", null);
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
                retval = new UserMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "User"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateUser(UserMasterVM UserVM)
        {
            try
            {
                var User = new UserMastersRepository().AddOrUpdate(UserVM);
                return new ReplyFormat().Success(Messages.SUCCESS, User);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            UserMasterVM UserVM = new UserMasterVM();
            ViewBag.PageType = "Create";
            try
            {
                ViewBag.lstDesignation = new DesignationMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.Designation, Value = d.Id.ToString() }).ToList();
                ViewBag.lstEntity = new EntityMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.EntityName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLOS = new LOSMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLocation = new LocationMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LocationName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstRegion = new RegionMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.Region, Value = d.Id.ToString() }).ToList();


            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View("ManageUserMaster", UserVM);
            
        }

        public ActionResult Edit(int id)
        {
            try
            {

                ViewBag.lstDesignation = new DesignationMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.Designation, Value = d.Id.ToString() }).ToList();
                ViewBag.lstEntity = new EntityMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.EntityName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLOS = new LOSMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLocation = new LocationMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LocationName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstRegion = new RegionMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.Region, Value = d.Id.ToString() }).ToList();

                UserMasterVM UserVM = new UserMastersRepository().Get(id);
                ViewBag.PageType = "Edit";
                return View("ManageUserMaster", UserVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
            
        }
    }
}