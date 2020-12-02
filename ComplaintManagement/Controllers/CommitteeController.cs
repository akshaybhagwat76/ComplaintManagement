using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    [Authorize]
    public class CommitteeController : Controller
    {
        // GET: Committee
        public ActionResult Index()
        {
            var lst = GetAll(1);

            var DataTableDetail = new HomeController().getDataTableDetail("Committee", null);
            ViewBag.lstCommittee = JsonConvert.SerializeObject(lst);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        [HttpGet]
        public ActionResult HistoryIndex(int id)
        {
            var lst = GetAllHistory(1,id);
            ViewBag.name = id.ToString();

            var DataTableDetail = new HomeController().getDataTableDetail("Committee", null);
            ViewBag.lstHistoryCommittee = JsonConvert.SerializeObject(lst);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }

        public ActionResult SearchCommittee(string search)
        {
            var originalList = GetAll(0);
            dynamic output = new List<dynamic>();

            foreach (var item in originalList)
            {
                var itemIndex = (IDictionary<string, object>)item;
                foreach (var kvp in itemIndex)
                {
                    if (kvp.Value != null)
                    {


                        if (kvp.Value.ToString().ToLower().Contains(search.ToLower()))
                        {

                            output.Add(item);
                        }
                        if (kvp.Key.ToString() == Messages.Status.ToString() && (search.ToLower() == Messages.Active.ToLower() || search.ToLower() == Messages.Inactive.ToLower()))
                        {
                            bool Status = Convert.ToBoolean(kvp.Value);
                            if (Status && search.ToLower() == Messages.Active.ToLower())
                            {
                                output.Add(item);
                            }
                            if (!Status && search.ToLower() == Messages.Inactive.ToLower())
                            {
                                output.Add(item);
                            }
                        }
                    }
                }
            }
            List<dynamic> data = output;
            ViewBag.lstCommittee = JsonConvert.SerializeObject(data.Distinct().ToList());
            return View("Index");
        }
        [HttpGet]
        public ActionResult LoadHistoryCommittee(int currentPageIndex, int id)
        {
            ViewBag.lstHistoryCommittee = JsonConvert.SerializeObject(GetAllHistory(currentPageIndex, id));
            //if (!string.IsNullOrEmpty(range) && range.Contains(","))
            //{
            //    ViewBag.startDate = range.Split(',')[0];
            //    ViewBag.toDate = range.Split(',')[1];
            //}
            return View("HistoryIndex");
        }

        [HttpGet]
        public ActionResult LoadCommittee(int currentPageIndex, string range = "")
        {
            ViewBag.lstCommittee = JsonConvert.SerializeObject(GetAll(currentPageIndex, range));
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }

        [HttpPost]
        public JsonResult CheckIfExist(CommitteeMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.CommitteeName))
                {
                    var Committee = false;
                    if (data.Id == 0)
                    {
                        Committee = new CommitteeMastersRepository().IsExist(data.CommitteeName);
                    }
                    else
                    {
                        Committee = new CommitteeMastersRepository().IsExist(data.CommitteeName, data.Id);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, Committee);
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
        public dynamic GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var lst = new CommitteeMastersRepository().GetAll();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Committee in lst
                       where Committee.CreatedDate.Date >= fromDate.Date && Committee.CreatedDate.Date <= toDate.Date
                       select Committee).ToList();

                lstCount = lst.Count;
                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                dynamic output = new List<dynamic>();
                List<UserMasterVM> lstUserMaster = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();
                if (lst != null && lst.Count > 0)
                {
                    foreach (CommitteeMasterVM com in lst)
                    {
                        dynamic row = new ExpandoObject();
                       
                        if (!string.IsNullOrEmpty(com.UserId))
                        {
                            if (com.UserId.Contains(","))
                            {
                                string[] array = com.UserId.Split(',');
                                List<string> users = new List<string>();
                                foreach (string UserItem in array)
                                {
                                    if (lstUserMaster.Where(x => x.Id == Convert.ToInt32(UserItem)).FirstOrDefault() != null)
                                    {
                                        users.Add(lstUserMaster.Where(x => x.Id == Convert.ToInt32(UserItem)).FirstOrDefault().EmployeeName);
                                    }
                                }
                                row.User = string.Join(",", users);
                            }
                            else
                            {
                                row.User = lstUserMaster.Where(x => x.Id == Convert.ToInt32(com.UserId)).FirstOrDefault() != null ? lstUserMaster.Where(x => x.Id == Convert.ToInt32(com.UserId)).FirstOrDefault().EmployeeName : string.Empty;
                            }
                        }
                        row.Id = com.Id;
                        row.CommitteeName = com.CommitteeName;
                        row.UpdatedByName = com.UpdatedByName;
                        row.CreatedByName = com.CreatedByName;
                        row.ModifiedBy = com.ModifiedBy;
                        row.CreatedBy = com.CreatedBy;
                        row.UpdatedDate = com.UpdatedDate;
                        row.CreatedDate = com.CreatedDate;

                        row.Status = com.Status;

                        output.Add(row);
                    }
                }

                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return output;
            }
            else
            {
                dynamic output = new List<dynamic>();

                lst = (from Committee in lst
                       select Committee)
           .OrderByDescending(user => user.Id)
           .Skip((currentPage - 1) * maxRows)
           .Take(maxRows).ToList();
                List<UserMasterVM> lstUserMaster = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();



                if (lst != null && lst.Count > 0)
                {
                    foreach (CommitteeMasterVM com in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (!string.IsNullOrEmpty(com.UserId))
                        {
                            if (com.UserId.Contains(","))
                            {
                                string[] array = com.UserId.Split(',');
                                List<string> users = new List<string>();
                                foreach (string UserItem in array)
                                {
                                    if (lstUserMaster.Where(x => x.Id == Convert.ToInt32(UserItem)).FirstOrDefault() != null)
                                    {
                                        users.Add(lstUserMaster.Where(x => x.Id == Convert.ToInt32(UserItem)).FirstOrDefault().EmployeeName);
                                    }
                                }
                                row.User = string.Join(",", users);
                            }
                            else
                            {
                                row.User = lstUserMaster.Where(x => x.Id == Convert.ToInt32(com.UserId)).FirstOrDefault() != null ? lstUserMaster.Where(x => x.Id == Convert.ToInt32(com.UserId)).FirstOrDefault().EmployeeName : string.Empty;
                            }
                        }
                        row.Id = com.Id;
                        row.CommitteeName = com.CommitteeName;
                        row.Status = com.Status;
                        row.UpdatedByName = com.UpdatedByName;
                        row.CreatedByName = com.CreatedByName;
                        row.ModifiedBy = com.ModifiedBy;
                        row.CreatedBy = com.CreatedBy;
                        row.UpdatedDate = com.UpdatedDate;
                        row.CreatedDate = com.CreatedDate;
                        output.Add(row);
                    }
                }
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;

                return output;

            }
        }

        public dynamic GetAllHistory(int currentPage, int id)
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var lst = new CommitteeMastersRepository().GetAllHistory();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(id.ToString()))
            {
                //string[] dates = range.Split(',');
                //DateTime fromDate = Convert.ToDateTime(dates[0]);
                //DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Committee in lst
                       where Committee.CommitteeId == id
                       select Committee).ToList();

                lstCount = lst.Count;
                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                dynamic output = new List<dynamic>();
                List<UserMasterVM> lstUserMaster = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();
                if (lst != null && lst.Count > 0)
                {
                    foreach (CommitteeMasterHistoryVM com in lst)
                    {
                        dynamic row = new ExpandoObject();

                        if (!string.IsNullOrEmpty(com.UserId))
                        {
                            if (com.UserId.Contains(","))
                            {
                                string[] array = com.UserId.Split(',');
                                List<string> users = new List<string>();
                                foreach (string UserItem in array)
                                {
                                    if (lstUserMaster.Where(x => x.Id == Convert.ToInt32(UserItem)).FirstOrDefault() != null)
                                    {
                                        users.Add(lstUserMaster.Where(x => x.Id == Convert.ToInt32(UserItem)).FirstOrDefault().EmployeeName);
                                    }
                                }
                                row.User = string.Join(",", users);
                            }
                            else
                            {
                                row.User = lstUserMaster.Where(x => x.Id == Convert.ToInt32(com.UserId)).FirstOrDefault() != null ? lstUserMaster.Where(x => x.Id == Convert.ToInt32(com.UserId)).FirstOrDefault().EmployeeName : string.Empty;
                            }
                        }
                        row.Id = com.Id;
                        row.CommitteeName = com.CommitteeName;
                        row.UpdatedByName = com.UpdatedByName;
                        row.CreatedByName = com.CreatedByName;
                        row.ModifiedBy = com.ModifiedBy;
                        row.CreatedBy = com.CreatedBy;
                        row.UpdatedDate = com.UpdatedDate;
                        row.CreatedDate = com.CreatedDate;
                        row.EntityState = com.EntityState;
                        row.Status = com.Status;

                        output.Add(row);
                    }
                }

                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return output;
            }
            else
            {
                dynamic output = new List<dynamic>();

                lst = (from Committee in lst
                       select Committee)
           .OrderByDescending(user => user.Id)
           .Skip((currentPage - 1) * maxRows)
           .Take(maxRows).ToList();
                List<UserMasterVM> lstUserMaster = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();



                //if (lst != null && lst.Count > 0)
                //{
                //    foreach (CommitteeMasterVM com in lst)
                //    {
                //        dynamic row = new ExpandoObject();
                //        if (!string.IsNullOrEmpty(com.UserId))
                //        {
                //            if (com.UserId.Contains(","))
                //            {
                //                string[] array = com.UserId.Split(',');
                //                List<string> users = new List<string>();
                //                foreach (string UserItem in array)
                //                {
                //                    if (lstUserMaster.Where(x => x.Id == Convert.ToInt32(UserItem)).FirstOrDefault() != null)
                //                    {
                //                        users.Add(lstUserMaster.Where(x => x.Id == Convert.ToInt32(UserItem)).FirstOrDefault().EmployeeName);
                //                    }
                //                }
                //                row.User = string.Join(",", users);
                //            }
                //            else
                //            {
                //                row.User = lstUserMaster.Where(x => x.Id == Convert.ToInt32(com.UserId)).FirstOrDefault() != null ? lstUserMaster.Where(x => x.Id == Convert.ToInt32(com.UserId)).FirstOrDefault().EmployeeName : string.Empty;
                //            }
                //        }
                //        row.Id = com.Id;
                //        row.CommitteeName = com.CommitteeName;
                //        row.Status = com.Status;
                //        row.UpdatedByName = com.UpdatedByName;
                //        row.CreatedByName = com.CreatedByName;
                //        row.ModifiedBy = com.ModifiedBy;
                //        row.CreatedBy = com.CreatedBy;
                //        row.UpdatedDate = com.UpdatedDate;
                //        row.CreatedDate = com.CreatedDate;
                //        output.Add(row);
                //    }
                //}
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;

                return output;

            }
        }

        [HttpGet]
        public ActionResult GetCommittee(string range, int currentPage)
        {
            ViewBag.lstCommittee = JsonConvert.SerializeObject(GetAll(currentPage, range));
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
                retval = new CommitteeMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Committee"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateCommittee(CommitteeMasterVM CommitteeVM)
        {
            try
            {
                var Committee = new CommitteeMastersRepository().AddOrUpdate(CommitteeVM);
                return new ReplyFormat().Success(Messages.SUCCESS, Committee);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        public ActionResult Create()
        {
            CommitteeMasterVM CommitteeMasterVM = new CommitteeMasterVM();
            ViewBag.PageType = "Create";
            ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();

            return View("ManageCommitteeMaster", CommitteeMasterVM);

        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                CommitteeMasterVM CommitteeVM = new CommitteeMastersRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();


                return View("ManageCommitteeMaster", CommitteeVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();

        }

        [HttpPost]
        public ActionResult ImportCommitties(string file)
        {
            try
            {
                string retval = new CommitteeMastersRepository().UploadImportCommitties(file);
                if (!string.IsNullOrEmpty(retval))
                {
                    int count = new CommitteeMastersRepository().ImportImportCommitties(retval);
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


                var ws = package.Workbook.Worksheets.Add(Messages.Committee);
                //Headers
                ws.Cells["A1"].Value = Messages.UserName;
                ws.Cells["B1"].Value = Messages.Committee;
                ws.Cells["C1"].Value = Messages.CreatedDate;
                ws.Cells["D1"].Value = Messages.CreatedBy;
                ws.Cells["E1"].Value = Messages.ModifiedDate;
                ws.Cells["F1"].Value = Messages.ModifiedBy;
                ws.Cells["G1"].Value = Messages.Status;


                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.UserName;

                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.Committee;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.CreatedDate;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.ModifiedDate;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.ModifiedBy;

                ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                ws.Cells[rowNumber, 7].Value = Messages.Status;
                foreach (var log in GetAll(0))
                {
                    rowNumber++;

                    ws.Cells[rowNumber, 1].Value = log.User;
                    ws.Cells[rowNumber, 2].Value = log.CommitteeName;
                    ws.Cells[rowNumber, 3].Value = log.CreatedDate != null ? log.CreatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 4].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 5].Value = log.UpdatedDate != null ? log.UpdatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 6].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;
                    ws.Cells[rowNumber, 7].Value = log.Status ? Messages.Active : Messages.Inactive;

                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.Committee + Messages.XLSX;
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


                var ws = package.Workbook.Worksheets.Add(Messages.CommitteeHistory);
                //Headers
                ws.Cells["A1"].Value = Messages.UserName;
                ws.Cells["B1"].Value = Messages.Committee;
                ws.Cells["C1"].Value = Messages.CreatedDate;
                ws.Cells["D1"].Value = Messages.CreatedBy;
                ws.Cells["E1"].Value = Messages.ModifiedDate;
                ws.Cells["F1"].Value = Messages.ModifiedBy;
                ws.Cells["G1"].Value = Messages.Status;
                ws.Cells["H1"].Value = Messages.EntityState;


                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.UserName;

                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.Committee;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.CreatedDate;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.ModifiedDate;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.ModifiedBy;

                ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                ws.Cells[rowNumber, 7].Value = Messages.Status;

                ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                ws.Cells[rowNumber, 8].Value = Messages.EntityState;
                var lst = GetAllHistory(0, id);
                foreach (var log in lst)
                {
                    rowNumber++;

                    ws.Cells[rowNumber, 1].Value = log.User;
                    ws.Cells[rowNumber, 2].Value = log.CommitteeName;
                    ws.Cells[rowNumber, 3].Value = log.CreatedDate != null ? log.CreatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 4].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 5].Value = log.UpdatedDate != null ? log.UpdatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 6].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;
                    ws.Cells[rowNumber, 7].Value = log.Status ? Messages.Active : Messages.Inactive;
                    ws.Cells[rowNumber, 8].Value = log.EntityState;
                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.CommitteeHistory + Messages.XLSX;
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