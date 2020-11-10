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
    public class CompetencyController : Controller
    {
        // GET: Competency
        public ActionResult Index()
        {
            ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("Competency", null);
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
                retval = new CompetencyMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Competency"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateCompetency(CompetencyMasterVM CompetencyVM)
        {
            try
            {
                var category = new CompetencyMastersRepository().AddOrUpdate(CompetencyVM);
                return new ReplyFormat().Success(Messages.SUCCESS, category);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        public ActionResult Create()
        {

            CompetencyMasterVM CompetencyMasterVM = new CompetencyMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageCompetencyMaster", CompetencyMasterVM);
            
        }

        public ActionResult Edit(int id)
        {
            try
            {
                CompetencyMasterVM CompetencyVM = new CompetencyMastersRepository().Get(id);
                ViewBag.PageType = "Edit";
                return View("ManageCompetencyMaster", CompetencyVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
           
        }
    }
}