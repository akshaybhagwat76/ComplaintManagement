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
    public class EntityController : Controller
    {
        // GET: Entity
        public ActionResult Index()
        {
            ViewBag.lstEntity = new EntityMasterRepository().GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("Entity", null);
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
                retval = new EntityMasterRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Entity"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateEntity(EntityMasterVM EntityVM)
        {
            try
            {
                var Entity = new EntityMasterRepository().AddOrUpdate(EntityVM);
                return new ReplyFormat().Success(Messages.SUCCESS, Entity);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            EntityMasterVM EntityMasterVM = new EntityMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageEntityMaster", EntityMasterVM);
        }

        public ActionResult Edit(int id)
        {
            try
            {
                EntityMasterVM EntityVM = new EntityMasterRepository().Get(id);
                ViewBag.PageType = "Edit";
                return View("ManageEntityMaster", EntityVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
        }
    }
}