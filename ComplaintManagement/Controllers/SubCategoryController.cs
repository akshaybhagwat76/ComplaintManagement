using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
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
        [HttpGet]
        public ActionResult HistoryIndex(String id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);

                ViewBag.lstHistorySubCategory = GetAllHistory(1, Convert.ToInt32(id));
                ViewBag.name = id.ToString();

                var DataTableDetail = new HomeController().getDataTableDetail("SubCategory", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
                return View();
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet]
        public ActionResult LoadHistorySubCategory(int currentPageIndex, int range = 0)
        {
            ViewBag.lstHistorySubCategory = GetAllHistory(currentPageIndex, range);
            //if (!string.IsNullOrEmpty(range))
            //{
            //    ViewBag.startDate = range.Split(',')[0];
            //    ViewBag.toDate = range.Split(',')[1];
            //}
            return View("HistoryIndex");
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
        public JsonResult CheckIfCategoryExist(SubCategoryMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.CategoryId.ToString()))
                {
                    var Subcategory = false;
                    if (data.Id == 0)
                    {
                        Subcategory = new SubCategoryMastersRepository().IsCategoryExist(data.CategoryId);
                    }
                    else
                    {
                        Subcategory = new SubCategoryMastersRepository().IsCategoryExist(data.CategoryId, data.Id);
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

        public List<SubCategoryMasterHistoryVM> GetAllHistory(int currentPage, int id)
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var lst = new SubCategoryMastersRepository().GetAllHistory();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(id.ToString()))
            {
                //string[] dates = range.Split(',');
                //DateTime fromDate = Convert.ToDateTime(dates[0]);
                //DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Subcategory in lst
                       where Subcategory.SubCategoryId == id
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
        public ActionResult Delete(String id)
        {
            bool retval = true;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);

                    retval = new SubCategoryMastersRepository().Delete(Convert.ToInt32(id));
                    return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Subcategory"), null);
                }
                else
                {
                    return new ReplyFormat().Error(Messages.FAIL);
                }
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
            ViewBag.categoryList= new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
            ViewBag.PageType = "Create";

            return View("ManageSubCategoryMaster", SubcategoryMasterVM);
        }

        public ActionResult Edit(String id, bool isView)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);
                    ViewBag.categoryList = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
                    SubCategoryMasterVM SubcategoryVM = new SubCategoryMastersRepository().Get(Convert.ToInt32(id));
                    ViewBag.ViewState = isView;

                    ViewBag.PageType = !isView ? "Edit" : "View";
                    return View("ManageSubCategoryMaster", SubcategoryVM);
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();

        }

        [HttpPost]
        public ActionResult ImportSubCategory(string file)
        {
            try
            {
                string retval = new SubCategoryMastersRepository().UploadImportSubCategories(file);
                if (!string.IsNullOrEmpty(retval))
                {
                    int count = new SubCategoryMastersRepository().ImportSubCategories(retval);
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

        public ActionResult ExportData()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage();


                var ws = package.Workbook.Worksheets.Add(Messages.SubCategory);
                //Headers
                ws.Cells["A1"].Value = Messages.Category;
                ws.Cells["B1"].Value = Messages.CreatedDate;
                ws.Cells["C1"].Value = Messages.CreatedBy;
                ws.Cells["D1"].Value = Messages.ModifiedDate;
                ws.Cells["E1"].Value = Messages.ModifiedBy;
                ws.Cells["F1"].Value = Messages.Status;


                var rowNumber = 1;

                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.SubCategory;

                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.CreatedDate;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.ModifiedDate;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.ModifiedBy;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.Status;

                foreach (var log in new SubCategoryMastersRepository().GetAll())
                {
                    rowNumber++;

                    ws.Cells[rowNumber, 1].Value = log.SubCategoryName;
                    ws.Cells[rowNumber, 2].Value = log.CreatedDate.ToString("dd/MM/yyyy");
                    ws.Cells[rowNumber, 3].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 4].Value = log.UpdatedDate.HasValue ? log.UpdatedDate.Value.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 5].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;
                    ws.Cells[rowNumber, 6].Value = log.Status ? Messages.Active : Messages.Inactive;
                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.SubCategory + Messages.XLSX;
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                stream.Position = 0;
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }

        }

        public ActionResult ExportDataHistory(int id)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage();


                var ws = package.Workbook.Worksheets.Add(Messages.SubCategoryHistory);
                //Headers
                ws.Cells["A1"].Value = Messages.SubCategory;
                ws.Cells["B1"].Value = Messages.CreatedDate;
                ws.Cells["C1"].Value = Messages.EntityState;
                ws.Cells["D1"].Value = Messages.CreatedBy;
                ws.Cells["E1"].Value = Messages.ModifiedDate;
                ws.Cells["F1"].Value = Messages.ModifiedBy;
                ws.Cells["G1"].Value = Messages.Status;

                List<SubCategoryMasterHistoryVM> list = new SubCategoryMastersRepository().GetAllHistory().Where(x => x.SubCategoryId == id).ToList();

                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.Category;

                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.CreatedDate;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.EntityState;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.ModifiedDate;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.ModifiedBy;

                ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                ws.Cells[rowNumber, 7].Value = Messages.Status;

                foreach (var log in list)
                {
                    rowNumber++;
                    if (rowNumber > 1)
                    {
                        ws.Cells[rowNumber, 1].Value = log.SubCategoryName;
                        ws.Cells[rowNumber, 2].Value = log.CreatedDate.ToString("dd/MM/yyyy");
                        ws.Cells[rowNumber, 3].Value = log.EntityState;
                        ws.Cells[rowNumber, 4].Value = log.CreatedByName;
                        ws.Cells[rowNumber, 5].Value = log.UpdatedDate.HasValue ? log.UpdatedDate.Value.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                        ws.Cells[rowNumber, 6].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;
                        ws.Cells[rowNumber, 7].Value = log.Status ? Messages.Active : Messages.Inactive;
                    }
                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.SubCategoryHistory + Messages.XLSX;
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                stream.Position = 0;
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }

        }
    }
}