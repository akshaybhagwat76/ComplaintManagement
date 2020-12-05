using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using Ionic.Zip;
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

                    foreach (var file in filesArray)
                    {
                        if (file != null)
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
    }
}
