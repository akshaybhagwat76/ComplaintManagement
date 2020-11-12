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
    public class SubCategoryController : Controller
    {
        // GET: SubCategory
        public ActionResult Index()
        {
            ViewBag.lstSubCategory = new SubCategoryMastersRepository().GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("Category", null);
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

        public ActionResult Edit(int id)
        {
            try
            {
                SubCategoryMasterVM SubcategoryVM = new SubCategoryMastersRepository().Get(id);
                ViewBag.PageType = "Edit";
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