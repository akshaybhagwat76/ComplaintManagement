using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using Newtonsoft.Json;
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
            ViewBag.lstRole = new RoleMasterRepoitory().GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("Role", null);
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
                retval = new RoleMasterRepoitory().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Role"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateRole1(RoleMasterVM data)
        {
            try
            {
                var Role = new RoleMasterRepoitory().AddOrUpdate(data);
                return new ReplyFormat().Success(Messages.SUCCESS, Role);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateRole(string roleParams)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<RoleMasterVM>(roleParams);
                var Role = new RoleMasterRepoitory().AddOrUpdate(data);
                return new ReplyFormat().Success(Messages.SUCCESS, Role);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            RoleMasterVM RoleMasterVM = new RoleMasterVM();
            ViewBag.PageType = "Create";
            ViewBag.lstUser = new UserMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSBU = new SBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
            ViewBag.lstLOS = new LOSMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();

            return View("ManageRoleMaster", RoleMasterVM);
           
        }

        public ActionResult Edit(int id)
        {
            try
            {
                RoleMasterVM RoleVM = new RoleMasterRepoitory().Get(id);
                ViewBag.PageType = "Edit";
                ViewBag.PageType = "Create";
                ViewBag.lstUser = new UserMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLOS = new LOSMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();

                return View("ManageRoleMaster", RoleVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
            
        }
    }
}