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
            ViewBag.lstSubSBU = GetAll(1);
            var DataTableDetail = new HomeController().getDataTableDetail("SubSBU", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        [HttpGet]
        public ActionResult LoadSubSBU(int currentPageIndex, string range = "")
        {
            ViewBag.lstSubSBU = GetAll(currentPageIndex, range);
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        public List<SubSBUMasterVM> GetAll(int currentPage,string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            var lst = new SubSBUMasterRepository().GetAll();
            lstCount = lst.Count;


            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from SubSBU in lst
                       where SubSBU.CreatedDate >= fromDate && SubSBU.CreatedDate <= toDate
                       select SubSBU).ToList();
                lstCount = lst.Count;
                lst = (lst)
                        .OrderBy(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return lst;
            }
            else
            {
                lst = (from SubSBU in lst
                       select SubSBU)
             .OrderBy(customer => customer.Id)
             .Skip((currentPage - 1) * maxRows)
             .Take(maxRows).ToList();
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return lst;
            }
        }

        [HttpGet]
        public ActionResult GetSubSBU(string range, int currentPage)
        {
            ViewBag.lstSubSBU = GetAll(currentPage,range);
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("SubCategory", null);
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

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                SubSBUMasterVM SubSBUVM = new SubSBUMasterRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
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