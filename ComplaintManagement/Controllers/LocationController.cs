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
    public class LocationController : Controller
    {
        // GET: Location
        public ActionResult Index()
        {
            ViewBag.lstLocation = GetAll(1);
            var DataTableDetail = new HomeController().getDataTableDetail("Location", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult HistoryIndex(int id)
        {
            ViewBag.lstLocationHistory = GetAllHistories(1, id).ToList();
            ViewBag.name = id.ToString();
            var DataTableDetail = new HomeController().getDataTableDetail("Location", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult SearchLocation(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (search.ToLower() == Messages.Inactive.ToLower())
                {
                    ViewBag.lstLocation = GetAll(0).ToList().Where(x => !x.Status).ToList();
                }
                if (search.ToLower() == Messages.Active.ToLower())
                {
                    ViewBag.lstLocation = GetAll(0).ToList().Where(x => x.Status).ToList();
                }
                if (search.ToLower() != Messages.Active.ToLower() && search.ToLower() != Messages.Inactive.ToLower())
                {
                    ViewBag.lstLocation = GetAll(0).ToList().Where(x => x.LocationName.Contains(search)).ToList();
                }

                var DataTableDetail = new HomeController().getDataTableDetail("Location", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult LoadLocation(int currentPageIndex, string range = "")
        {
            ViewBag.lstLocation = GetAll(currentPageIndex, range);
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult LoadHistoryLocation(int currentPageIndex, int range =0 )
        {
            ViewBag.name = range;
            ViewBag.lstLocationHistory = GetAllHistories(currentPageIndex, range);
            //if (!string.IsNullOrEmpty(range))
            //{
            //    ViewBag.startDate = range.Split(',')[0];
            //    ViewBag.toDate = range.Split(',')[1];
            //}
            return View("Index");
        }
        public List<LocationMasterVM> GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }
            
            var lst = new LocationMastersRepository().GetAll();
            lstCount = lst.Count;


            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Location in lst
                       where Location.CreatedDate.Date >= fromDate.Date && Location.CreatedDate.Date <= toDate.Date
                       select Location).ToList();
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
                lst = (from Location in lst
                       select Location)
             .OrderByDescending(customer => customer.Id)
             .Skip((currentPage - 1) * maxRows)
             .Take(maxRows).ToList();
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return lst;
            }
        }
        public List<LocationMasterHistoryVM> GetAllHistories(int currentPage, int range = 0)
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var lst = new LocationMastersRepository().GetAllHistory();
            lstCount = lst.Count;


            if (!string.IsNullOrEmpty(range.ToString()))
            {
                //string[] dates = range.Split(',');
                //DateTime fromDate = Convert.ToDateTime(dates[0]);
                //DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Location in lst
                       where Location.LocationId == range
                       select Location).ToList();
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
                lst = (from Location in lst
                       select Location)
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
        public ActionResult GetLocations(string range, int currentPage)
        {
            ViewBag.lstLocation = GetAll(currentPage,range);
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
                retval = new LocationMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Location"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateLocation(LocationMasterVM LocationVM)
        {
            try
            {
                var Location = new LocationMastersRepository().AddOrUpdate(LocationVM);
                return new ReplyFormat().Success(Messages.SUCCESS, Location);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            LocationMasterVM LocationMasterVM = new LocationMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageLocationMaster", LocationMasterVM);
            
        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                LocationMasterVM LocationVM = new LocationMastersRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                return View("ManageLocationMaster", LocationVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
           
        }
        [HttpPost]
        public JsonResult CheckIfExist(LocationMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.LocationName))
                {
                    var Location = false;
                    if (data.Id == 0)
                    {
                        Location = new LocationMastersRepository().IsExist(data.LocationName);
                    }
                    else
                    {
                        Location = new LocationMastersRepository().IsExist(data.LocationName, data.Id);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, Location);
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
        [HttpPost]
        public ActionResult ImportLocation(string file)
        {
            try
            {
                string retval = new LocationMastersRepository().UploadImportLocation(file);
                if (!string.IsNullOrEmpty(retval))
                {
                    int count = new LocationMastersRepository().ImportLocation(retval);
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


                var ws = package.Workbook.Worksheets.Add(Messages.Location);
                //Headers
                ws.Cells["A1"].Value = Messages.Location;
                ws.Cells["B1"].Value = Messages.CreatedDate;
                ws.Cells["C1"].Value = Messages.CreatedBy;
                ws.Cells["D1"].Value = Messages.ModifiedDate;
                ws.Cells["E1"].Value = Messages.ModifiedBy;
                ws.Cells["F1"].Value = Messages.Status;


                var rowNumber = 1;

                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.Location;

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

                foreach (var log in new LocationMastersRepository().GetAll())
                {
                    rowNumber++;

                    ws.Cells[rowNumber, 1].Value = log.LocationName;
                    ws.Cells[rowNumber, 2].Value = log.CreatedDate.ToString("dd/MM/yyyy");
                    ws.Cells[rowNumber, 3].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 4].Value = log.UpdatedDate.HasValue ? log.UpdatedDate.Value.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 5].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;
                    ws.Cells[rowNumber, 6].Value = log.Status ? Messages.Active : Messages.Inactive;
                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.Location + Messages.XLSX;
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


                var ws = package.Workbook.Worksheets.Add(Messages.LocationHistory);
                //Headers
                ws.Cells["A1"].Value = Messages.Location;
                ws.Cells["B1"].Value = Messages.CreatedDate;
                ws.Cells["C1"].Value = Messages.EntityState;
                ws.Cells["D1"].Value = Messages.CreatedBy;
                ws.Cells["E1"].Value = Messages.ModifiedDate;
                ws.Cells["F1"].Value = Messages.ModifiedBy;
                ws.Cells["G1"].Value = Messages.Status;

                List<LocationMasterHistoryVM> list = new LocationMastersRepository().GetAllHistory().Where(x => x.LocationId == id).ToList();

                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.Location;

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
                        ws.Cells[rowNumber, 1].Value = log.LocationName;
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

                string fileName = Messages.LocationHistory + Messages.XLSX;
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