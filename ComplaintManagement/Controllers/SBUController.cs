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
    public class SBUController : Controller
    {
        // GET: SBU
        public ActionResult Index()
        {
            ViewBag.lstSBU = GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("SBU", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public List<SBUMasterVM> GetAll(string range = "")
        {
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                return new SBUMasterRepository().GetAll().Where(x => x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();
            }
            else
            {
                return new SBUMasterRepository().GetAll();
            }
        }

        [HttpGet]
        public ActionResult GetSBU(string range)
        {
            ViewBag.lstSBU = GetAll(range);
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("SBU", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View("Index");
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            bool retval = true;
            try
            {
                retval = new SBUMasterRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "SBU"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateSBU(SBUMasterVM SBUVM)
        {
            try
            {
                var SBU = new SBUMasterRepository().AddOrUpdate(SBUVM);
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
            SBUMasterVM SBUMasterVM = new SBUMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageSBUMaster", SBUMasterVM);
           
        }

        public ActionResult Edit(int id,bool isView)
        {
            try
            {
                SBUMasterVM SBUVM = new SBUMasterRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                return View("ManageSBUMaster", SBUVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
           
        }
    }
}