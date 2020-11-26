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
    [Authorize]
    public class SubCategoryController : Controller
    {
        // GET: SubCategory
        public ActionResult Index()
        {
            ViewBag.lstSubCategory = GetAll(1);
            var DataTableDetail = new HomeController().getDataTableDetail("SubCategory", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult SearchSubCategories(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (search.ToLower() == Messages.Inactive.ToLower())
                {
                    ViewBag.lstSubCategory = GetAll(0).ToList().Where(x => !x.Status).ToList();
                }
                if (search.ToLower() == Messages.Active.ToLower())
                {
                    ViewBag.lstSubCategory = GetAll(0).ToList().Where(x => x.Status).ToList();
                }
                if (search.ToLower() != Messages.Active.ToLower() && search.ToLower() != Messages.Inactive.ToLower())
                {
                    ViewBag.lstSubCategory = GetAll(0).ToList().Where(x => x.SubCategoryName.Contains(search)).ToList();
                }

                var DataTableDetail = new HomeController().getDataTableDetail("SubCategory", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult LoadSubCategories(int currentPageIndex, string range = "")
        {
            ViewBag.lstSubCategory = GetAll(currentPageIndex, range);
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        [HttpPost]
        public JsonResult CheckIfExist(SubCategoryMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.SubCategoryName))
                {
                    var Subcategory = false;
                    if (data.Id == 0)
                    {
                        Subcategory = new SubCategoryMastersRepository().IsExist(data.SubCategoryName);
                    }
                    else
                    {
                        Subcategory = new SubCategoryMastersRepository().IsExist(data.SubCategoryName, data.Id);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, Subcategory);
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
        public List<SubCategoryMasterVM> GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }
           
            var lst = new SubCategoryMastersRepository().GetAll();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Subcategory in lst
                       where Subcategory.CreatedDate.Date >= fromDate.Date && Subcategory.CreatedDate.Date <= toDate.Date
                       select Subcategory).ToList();
                lstCount = lst.Count;
                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return lst;
            }
            else
            {
                lst = (from Subcategory in lst
                       select Subcategory)
            .OrderByDescending(customer => customer.Id)
            .Skip((currentPage - 1) * maxRows)
            .Take(maxRows).ToList();
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return lst;
            }
        }


        [HttpGet]
        public ActionResult GetSubCategories(string range, int currentPage)
        {
            ViewBag.lstSubCategory = GetAll(currentPage, range);
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
                retval = new SubCategoryMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Subcategory"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateSubCategory(SubCategoryMasterVM SubcategoryVM)
        {
            try
            {
                var Subcategory = new SubCategoryMastersRepository().AddOrUpdate(SubcategoryVM);
                return new ReplyFormat().Success(Messages.SUCCESS, Subcategory);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            SubCategoryMasterVM SubcategoryMasterVM = new SubCategoryMasterVM();
            ViewBag.PageType = "Create";
          
            return View("ManageSubCategoryMaster", SubcategoryMasterVM);
        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                SubCategoryMasterVM SubcategoryVM = new SubCategoryMastersRepository().Get(id);
                ViewBag.ViewState = isView;

                ViewBag.PageType = !isView ? "Edit" : "View";
                return View("ManageSubCategoryMaster", SubcategoryVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
           
        }
    }
}