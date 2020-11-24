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
            ViewBag.lstDesignation = GetAll(1);
            var DataTableDetail = new HomeController().getDataTableDetail("Designation", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult searchDesignation(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (search.ToLower() == Messages.Inactive.ToLower())
                {
                    ViewBag.lstDesignation = GetAll(1).ToList().Where(x => !x.Status).ToList();
                }
                if (search.ToLower() == Messages.Active.ToLower())
                {
                    ViewBag.lstDesignation = GetAll(1).ToList().Where(x => x.Status).ToList();
                }
                if (search.ToLower() != Messages.Active.ToLower() && search.ToLower() != Messages.Inactive.ToLower())
                {
                    ViewBag.lstDesignation = GetAll(1).ToList().Where(x => x.Designation.Contains(search)).ToList();
                }

                var DataTableDetail = new HomeController().getDataTableDetail("Designation", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult LoadDesignations(int currentPageIndex, string range = "")
        {
            ViewBag.lstDesignation = GetAll(currentPageIndex, range);
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        [HttpPost]
        public JsonResult CheckIfExist(DesignationMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.Designation))
                {
                    var Designation = false;
                    if (data.Id == 0)
                    {
                        Designation = new DesignationMasterRepository().IsExist(data.Designation);
                    }
                    else
                    {
                        Designation = new DesignationMasterRepository().IsExist(data.Designation, data.Id);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, Designation);
                }
                else
                {
                    return new ReplyFormat().Error(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public List<DesignationMasterVM> GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            var lst = new DesignationMasterRepository().GetAll();
            lstCount = lst.Count;

            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Designation in lst
                       where Designation.CreatedDate >= fromDate && Designation.CreatedDate <= toDate
                       select Designation).ToList();
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
                lst = (from Designation in lst
                       select Designation)
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
        public ActionResult GetDesignation(string range, int currentPage)
        {
            ViewBag.lstDesignation = GetAll(currentPage,range);
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("Designatio", null);
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

        public ActionResult Edit(int Id,bool isView)
        {
            try
            {
                DesignationMasterVM DesignationVM= new DesignationMasterRepository().Get(Id);
                ViewBag.PageType = "Edit";
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
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