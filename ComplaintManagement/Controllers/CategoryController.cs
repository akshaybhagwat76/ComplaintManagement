﻿using ComplaintManagement.Helpers;
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
    [Authorize]
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            ViewBag.lstCategories = GetAll(1);
                                     
            var DataTableDetail = new HomeController().getDataTableDetail("Categories", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        [HttpGet]
        public ActionResult HistoryIndex(int id)
        {
            ViewBag.lstCategoriesHistory = GetAllHistories(1,id).ToList();
            ViewBag.name = id.ToString();
            var DataTableDetail = new HomeController().getDataTableDetail("Categories History", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }

        public ActionResult SearchCategories(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (search.ToLower() == Messages.Inactive.ToLower())
                {
                    ViewBag.lstCategories = GetAll(0).ToList().Where(x => !x.Status).ToList();
                }
                if (search.ToLower() == Messages.Active.ToLower())
                {
                    ViewBag.lstCategories = GetAll(0).ToList().Where(x => x.Status).ToList();
                }
                if (search.ToLower() != Messages.Active.ToLower() && search.ToLower() != Messages.Inactive.ToLower())
                {
                    ViewBag.lstCategories = GetAll(0).ToList().Where(x => x.CategoryName.Contains(search)).ToList();
                }

                var DataTableDetail = new HomeController().getDataTableDetail("Categories", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
            }
            return View("Index");
        }

        [HttpGet]
        public ActionResult LoadCategories(int currentPageIndex, string range = "")
        {
            ViewBag.lstCategories = GetAll(currentPageIndex, range);
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }

        [HttpGet]
        public ActionResult LoadHistoryCategories(int currentPageIndex, int range = 0)
        {
            ViewBag.name = range;
            ViewBag.lstCategoriesHistory = GetAllHistories(currentPageIndex, range);
            //if (!string.IsNullOrEmpty(range) && range.Contains(","))
            //{
            //    ViewBag.startDate = range.Split(',')[0];
            //    ViewBag.toDate = range.Split(',')[1];
            //}
            return View("HistoryIndex");
        }

        public List<CategoryMasterVM> GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }
           
            var lst = new CategoryMastersRepository().GetAll();
            lstCount = lst.Count;


            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from category in lst
                       where category.CreatedDate.Date >= fromDate.Date && category.CreatedDate.Date <= toDate.Date
                       select category).ToList();
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
                lst = (from category in lst
                       select category)
             .OrderByDescending(customer => customer.Id)
             .Skip((currentPage - 1) * maxRows)
             .Take(maxRows).ToList();
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return lst;
            }
        }

        public List<CategoryMasterHistoryVM> GetAllHistories(int currentPage, int range =0)
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var lst = new CategoryMastersRepository().GetAllHistory();
            lstCount = lst.Count;


            if (!string.IsNullOrEmpty(range.ToString()))
            {
                //string[] dates = range.Split(',');
                //DateTime fromDate = Convert.ToDateTime(dates[0]);
                //DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from category in lst
                       where category.CategoryId ==range
                       select category).ToList();
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
                lst = (from category in lst
                       select category)
             .OrderByDescending(customer => customer.Id)
             .Skip((currentPage - 1) * maxRows)
             .Take(maxRows).ToList();
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return lst;
            }
        }

        [HttpPost]
        public JsonResult CheckIfExist(CategoryMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.CategoryName))
                {
                    var category = false;
                    if (data.Id == 0)
                    {
                        category = new CategoryMastersRepository().IsExist(data.CategoryName);
                    }
                    else
                    {
                        category = new CategoryMastersRepository().IsExist(data.CategoryName, data.Id);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, category);
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

        [HttpGet]
        public ActionResult GetCategories(string range, int currentPage)
        {
            ViewBag.lstCategories = GetAll(currentPage, range);
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("Categories", null);
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
                retval = new CategoryMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "category"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpPost]
        public ActionResult AddOrUpdateCategories(CategoryMasterVM categoryVM)
        {
            try
            {
                var category = new CategoryMastersRepository().AddOrUpdate(categoryVM);
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
            CategoryMasterVM categoryMasterVM = new CategoryMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageCategoryMaster", categoryMasterVM);
        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                CategoryMasterVM categoryVM = new CategoryMastersRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                return View("ManageCategoryMaster", categoryVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
        }

        [HttpPost]
        public ActionResult ImportCategories(string file)
        {
            try
            {
                string retval = new CategoryMastersRepository().UploadImportCategories(file);
                if (!string.IsNullOrEmpty(retval))
                {
                    int count = new CategoryMastersRepository().ImportCategories(retval);
                    return new ReplyFormat().Success(count.ToString());
                }
                return new ReplyFormat().Error(Messages.BAD_DATA);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
    }
}
