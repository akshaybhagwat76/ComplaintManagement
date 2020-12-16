using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using Ionic.Zip;
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
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            ViewBag.lstEmployeeComplaint = GetAll(1);
            ViewBag.lstCategories = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSubCategories = new SubCategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubCategoryName, Value = d.Id.ToString() }).ToList(); ;

            var DataTableDetail = new HomeController().getDataTableDetail("EmployeeCompliant", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult searchEmployeeCompliant(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (search.ToLower() == Messages.Inactive.ToLower())
                {
                    ViewBag.lstEmployeeComplaint = GetAll(0).ToList().Where(x => !x.Status).ToList();
                }
                if (search.ToLower() == Messages.Active.ToLower())
                {
                    ViewBag.lstEmployeeComplaint = GetAll(0).ToList().Where(x => x.Status).ToList();
                }
                if (search.ToLower() != Messages.Active.ToLower() && search.ToLower() != Messages.Inactive.ToLower())
                {
                    ViewBag.lstEmployeeComplaint = GetAll(0).ToList().Where(x => x.CategoryName.Contains(search) || x.SubCategoryName.Contains(search) || x.Remark.Contains(search)).ToList();
                }
                var DataTableDetail = new HomeController().getDataTableDetail("EmployeeComplaint", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
            }
            ViewBag.lstCategories = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSubCategories = new SubCategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubCategoryName, Value = d.Id.ToString() }).ToList(); ;

            return View("Index");
        }
        public List<EmployeeCompliantMasterVM> GetAll(int currentPage, string range = "")

        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var lst = new EmployeeComplaintMastersRepository().GetAll();
            lstCount = lst.Count;

            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from EmployeeComplain in lst
                       where EmployeeComplain.CreatedDate.Date >= fromDate.Date && EmployeeComplain.CreatedDate.Date <= toDate.Date
                       select EmployeeComplain).ToList();
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
                lst = (from EmployeeComplain in lst
                       select EmployeeComplain)
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
        public JsonResult LoadEmployeeComplaints(CompliantSearchVM data)
        {
            try
            {
                int maxRows = 10; int lstCount = 0;
                if (data.currentPage == 0)
                {
                    maxRows = 2147483647;
                }

                var lst = new EmployeeComplaintMastersRepository().GetAll();
                lstCount = lst.Count;

                if (!string.IsNullOrEmpty(data.FromDate) && !string.IsNullOrEmpty(data.ToDate))
                {
                    DateTime fromDate = Convert.ToDateTime(data.FromDate);
                    DateTime toDate = Convert.ToDateTime(data.ToDate);
                    lst = (from EmployeeComplain in lst
                           where EmployeeComplain.CreatedDate.Date >= fromDate && EmployeeComplain.CreatedDate.Date <= toDate
                           select EmployeeComplain).ToList();

                }
                if (data.CategoryId > 0)
                {
                    lst = (from EmployeeComplain in lst
                           where EmployeeComplain.CategoryId == data.CategoryId
                           select EmployeeComplain).ToList();
                }
                if (data.SubCategoryId > 0)
                {
                    lst = (from EmployeeComplain in lst
                           where EmployeeComplain.SubCategoryId == data.SubCategoryId
                           select EmployeeComplain).ToList();
                }
                lstCount = lst.Count;
                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((data.currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = data.currentPage;
                ViewBag.lstEmployeeComplaint = lst;
                return new ReplyFormat().Success(Messages.SUCCESS, new { Url = PartialView("_EmployeeListView").RenderToString(), fromDate = data.FromDate, toDate = data.ToDate, categoryId = data.CategoryId, subCategoryId = data.SubCategoryId });
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpGet]
        public ActionResult GetEmployeeCompliant(string range, int currentPage)
        {
            ViewBag.lstCategories = GetAll(currentPage, range);
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("EmployeeCompliant", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View("Index");
        }


        [HttpGet]
        public ActionResult DownloadAttachments(string files)
        {
            if (!string.IsNullOrEmpty(files))
            {
                using (ZipFile zip = new ZipFile())
                {

                    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                    zip.AddDirectoryByName("Files");
                    string[] docfileArray = Directory.GetFiles(Server.MapPath("~/Documents/"));
                    string[] filesArray = files.Split(new string[] { "," }, StringSplitOptions.None);

                    foreach (string file in filesArray)
                    {
                        if (file != null && !string.IsNullOrEmpty(file) && file.Length > 5)
                        {
                            if (docfileArray.FirstOrDefault(x => x.Contains(file)) != null)
                            {
                                string indexed = docfileArray.FirstOrDefault(x => x.Contains(file));


                                if (System.IO.File.Exists(indexed))
                                {
                                    zip.AddFile(indexed, "Files");
                                }
                                else { return RedirectToAction("Index"); }
                            }

                        }
                    }

                    var order_id = "attachments";
                    string zipName = string.Format("" + order_id + "_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                    if (zip.Entries.Count > 1)
                    {
                        using (MemoryStream memoryStram = new MemoryStream())
                        {
                            zip.Save(memoryStram);
                            return File(memoryStram.ToArray(), "application/zip", zipName);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult ImportEmployeeCompliant(string file)
        {
            try
            {
                string retval = new EmployeeComplaintMastersRepository().UploadImportEmployeeCompliant(file);
                if (!string.IsNullOrEmpty(retval))
                {
                    int count = new EmployeeComplaintMastersRepository().ImportEmployeeCompliant(retval);
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


                var ws = package.Workbook.Worksheets.Add(Messages.Category);
                //Headers
                ws.Cells["A1"].Value = Messages.EmployeeName;
                ws.Cells["B1"].Value = Messages.Category;
                ws.Cells["C1"].Value = Messages.SubCategory;
                ws.Cells["D1"].Value = Messages.CreatedDate;
                ws.Cells["E1"].Value = Messages.CreatedBy;
                ws.Cells["F1"].Value = Messages.PendingWith;
                ws.Cells["G1"].Value = Messages.Remark;
                ws.Cells["H1"].Value = Messages.Status;


                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.EmployeeName;

                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.Category;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.SubCategory;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.CreatedDate;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.PendingWith;

                ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                ws.Cells[rowNumber, 7].Value = Messages.Remark;

                ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                ws.Cells[rowNumber, 8].Value = Messages.Status;
                foreach (var log in GetAll(1))
                {
                    rowNumber++;
                    ws.Cells[rowNumber, 1].Value = log.EmployeeName;
                    ws.Cells[rowNumber, 2].Value = log.CategoryName;
                    ws.Cells[rowNumber, 3].Value = log.SubCategoryName;
                    ws.Cells[rowNumber, 4].Value = log.CreatedDate.ToString("dd/MM/yyyy");
                    ws.Cells[rowNumber, 5].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 6].Value = log.LastPerformedBy;
                    ws.Cells[rowNumber, 7].Value = log.Remark;

                    var Status = string.Empty;
                    if (log.ComplaintStatus == Messages.SUBMITTED || log.ComplaintStatus == Messages.COMMITTEE)
                    {
                        Status = Messages.InProgress;
                    }
                    else { Status = log.ComplaintStatus; }

                    ws.Cells[rowNumber, 8].Value = Status;

                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.ComplaintList + Messages.XLSX;
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
