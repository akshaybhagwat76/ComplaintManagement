using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace ComplaintManagement.Controllers
{
    [Authorize]
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
        [HttpGet]
        public ActionResult HistoryIndex(int id)
        {
            ViewBag.lstHistoryDesignation = GetAllHistories(1, id).ToList();
            ViewBag.name = id.ToString();
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
                    ViewBag.lstDesignation = GetAll(0).ToList().Where(x => !x.Status).ToList();
                }
                if (search.ToLower() == Messages.Active.ToLower())
                {
                    ViewBag.lstDesignation = GetAll(0).ToList().Where(x => x.Status).ToList();
                }
                if (search.ToLower() != Messages.Active.ToLower() && search.ToLower() != Messages.Inactive.ToLower())
                {
                    ViewBag.lstDesignation = GetAll(0).ToList().Where(x => x.Designation.Contains(search)).ToList();
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
            return View("HistoryIndex");
        }
        [HttpGet]
        public ActionResult LoadHistoryDesignations(int currentPageIndex, int range = 0)
        {
            ViewBag.name = range;
            ViewBag.lstDesignation = GetAllHistories(currentPageIndex, range);
            //if (!string.IsNullOrEmpty(range))
            //{
            //    ViewBag.startDate = range.Split(',')[0];
            //    ViewBag.toDate = range.Split(',')[1];
            //}
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
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var lst = new DesignationMasterRepository().GetAll();
            lstCount = lst.Count;

            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Designation in lst
                       where Designation.CreatedDate.Date >= fromDate.Date && Designation.CreatedDate.Date <= toDate.Date
                       select Designation).ToList();
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
                lst = (from Designation in lst
                       select Designation)
             .OrderByDescending(customer => customer.Id)
             .Skip((currentPage - 1) * maxRows)
             .Take(maxRows).ToList();
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return lst;
            }
        }
        public List<DesignationMasterHistoryVM> GetAllHistories(int currentPage, int range = 0)
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var lst = new DesignationMasterRepository().GetAllHistory();
            lstCount = lst.Count;

            if (!string.IsNullOrEmpty(range.ToString()))
            {
                //string[] dates = range.Split(',');
                //DateTime fromDate = Convert.ToDateTime(dates[0]);
                //DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Designation in lst
                       where Designation.DesignationId == range
                       select Designation).ToList();
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
                lst = (from Designation in lst
                       select Designation)
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
        public ActionResult GetDesignation(string range, int currentPage)
        {
            ViewBag.lstDesignation = GetAll(currentPage, range);
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

        public ActionResult Edit(int Id, bool isView)
        {
            try
            {
                DesignationMasterVM DesignationVM = new DesignationMasterRepository().Get(Id);
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
        [HttpPost]
        public ActionResult ImportDesignation(string file)
        {
            try
            {
                string retval = new DesignationMasterRepository().UploadImportDesignation(file);
                if (!string.IsNullOrEmpty(retval))
                {
                    int count = new DesignationMasterRepository().ImportDesignation(retval);
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
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage();


                var ws = package.Workbook.Worksheets.Add(Messages.Designation);
                //Headers
                ws.Cells["A1"].Value = Messages.Designation;
                ws.Cells["B1"].Value = Messages.CreatedDate;
                ws.Cells["C1"].Value = Messages.CreatedBy;
                ws.Cells["D1"].Value = Messages.ModifiedDate;
                ws.Cells["E1"].Value = Messages.ModifiedBy;
                ws.Cells["F1"].Value = Messages.Status;


                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.Designation;

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
                foreach (var log in new DesignationMasterRepository().GetAll())
                {
                    rowNumber++;

                    ws.Cells[rowNumber, 1].Value = log.Designation;
                    ws.Cells[rowNumber, 2].Value = log.CreatedDate.ToString("dd/MM/yyyy");
                    ws.Cells[rowNumber, 3].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 4].Value = log.UpdatedDate.HasValue ? log.UpdatedDate.Value.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 5].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;
                    ws.Cells[rowNumber, 6].Value = log.Status ? Messages.Active : Messages.Inactive;

                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.Designation + Messages.XLSX;
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
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage();


                var ws = package.Workbook.Worksheets.Add(Messages.DesignationHistory);
                //Headers
                ws.Cells["A1"].Value = Messages.Designation;
                ws.Cells["B1"].Value = Messages.CreatedDate;
                ws.Cells["C1"].Value = Messages.EntityState;
                ws.Cells["D1"].Value = Messages.CreatedBy;
                ws.Cells["E1"].Value = Messages.ModifiedDate;
                ws.Cells["F1"].Value = Messages.ModifiedBy;
                ws.Cells["G1"].Value = Messages.Status;

                List<DesignationMasterHistoryVM> list = new DesignationMasterRepository().GetAllHistory().Where(x => x.DesignationId == id).ToList();

                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.Designation;

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
                        ws.Cells[rowNumber, 1].Value = log.Designation;
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

                string fileName = Messages.DesignationHistory + Messages.XLSX;
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