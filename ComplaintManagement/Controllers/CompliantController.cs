using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    [Authorize]
    public class CompliantController : Controller
    {
        // GET: Compliant
        public ActionResult Compliant_one()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
               .Select(c => c.Value).SingleOrDefault();
            UserMasterVM UserVM = new UserMastersRepository().Get(Convert.ToInt32(sid));

            ViewBag.Entity = UserVM.Company > 0 ? new EntityMasterRepository().Get(UserVM.Company) != null ? new EntityMasterRepository().Get(UserVM.Company).EntityName : Messages.NotAvailable : Messages.NotAvailable;
            ViewBag.SBU = UserVM.SBUId > 0 ? new SBUMasterRepository().Get(UserVM.SBUId) != null ? new SBUMasterRepository().Get(UserVM.SBUId).SBU : Messages.NotAvailable : Messages.NotAvailable;
            ViewBag.SubSBU = UserVM.SubSBUId > 0 ? new SubSBUMasterRepository().Get(UserVM.SubSBUId) != null ? new SubSBUMasterRepository().Get(UserVM.SubSBUId).SubSBU : Messages.NotAvailable : Messages.NotAvailable;
            ViewBag.LOS = UserVM.LOSId > 0 ? new LOSMasterRepository().Get(UserVM.LOSId) != null ? new LOSMasterRepository().Get(UserVM.LOSId).LOSName : Messages.NotAvailable : Messages.NotAvailable;
            ViewBag.Competency = UserVM.CompentencyId > 0 ? new CompetencyMastersRepository().Get(UserVM.CompentencyId) != null ? new CompetencyMastersRepository().Get(UserVM.CompentencyId).CompetencyName : Messages.NotAvailable : Messages.NotAvailable;
            ViewBag.Location = UserVM.LocationId > 0 ? new LocationMastersRepository().Get(UserVM.LocationId) != null ? new LocationMastersRepository().Get(UserVM.LocationId).LocationName : Messages.NotAvailable : Messages.NotAvailable;
            ViewBag.Region = UserVM.RegionId > 0 ? new RegionMasterRepository().Get(UserVM.RegionId) != null ? new RegionMasterRepository().Get(UserVM.RegionId).Region : Messages.NotAvailable : Messages.NotAvailable;
            ViewBag.ManagementLevel = UserVM.BusinessTitle > 0 ? new DesignationMasterRepository().Get(UserVM.BusinessTitle) != null ? new DesignationMasterRepository().Get(UserVM.BusinessTitle).Designation : Messages.NotAvailable : Messages.NotAvailable;

            ViewBag.lstCategories = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSubCategories = new SubCategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubCategoryName, Value = d.Id.ToString() }).ToList(); ;

            if (!string.IsNullOrEmpty(UserVM.ImagePath))
            {
                if (!new Common().GetFilePathExist(UserVM.ImagePath))
                {
                    UserVM.ImagePath = string.Empty;
                }
            }
            ViewBag.NavbarTitle = "Complaint Information";
            UserVM.ComplaintStatus = Messages.Opened; UserVM.Id = UserVM.CompentencyId = 0; UserVM.DateOfJoining = DateTime.UtcNow.AddDays(5);
            return View(UserVM);
        }
        public ActionResult Edit(string id, bool isView)
        {
            EmployeeCompliantMasterVM EmployeeCompliant_oneVM = new EmployeeCompliantMasterVM();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);

                    EmployeeCompliant_oneVM = new EmployeeComplaintMastersRepository().Get(Convert.ToInt32(id));
                    UserMasterVM userMasterVM = new UserMastersRepository().Get(EmployeeCompliant_oneVM.UserId);
                    ViewBag.Competency = userMasterVM.CompentencyId > 0 ? new CompetencyMastersRepository().Get(userMasterVM.CompentencyId) != null ? new CompetencyMastersRepository().Get(userMasterVM.CompentencyId).CompetencyName : Messages.NotAvailable : Messages.NotAvailable;
                    if (EmployeeCompliant_oneVM != null)
                    {
                        userMasterVM.CategoryId = EmployeeCompliant_oneVM.CategoryId;
                        userMasterVM.SubCategoryId = EmployeeCompliant_oneVM.SubCategoryId;
                        userMasterVM.Remark = EmployeeCompliant_oneVM.Remark;
                        userMasterVM.Attachments = EmployeeCompliant_oneVM.Attachments;
                        userMasterVM.Id = EmployeeCompliant_oneVM.Id;
                        userMasterVM.ComplaintStatus = EmployeeCompliant_oneVM.ComplaintStatus;
                        userMasterVM.CompentencyId = EmployeeCompliant_oneVM.UserId;
                        userMasterVM.DateOfJoining = EmployeeCompliant_oneVM.DueDate;
                    }
                    ViewBag.Entity = userMasterVM.Company > 0 ? new EntityMasterRepository().Get(userMasterVM.Company) != null ? new EntityMasterRepository().Get(userMasterVM.Company).EntityName : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.SBU = userMasterVM.SBUId > 0 ? new SBUMasterRepository().Get(userMasterVM.SBUId) != null ? new SBUMasterRepository().Get(userMasterVM.SBUId).SBU : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.SubSBU = userMasterVM.SubSBUId > 0 ? new SubSBUMasterRepository().Get(userMasterVM.SubSBUId) != null ? new SubSBUMasterRepository().Get(userMasterVM.SubSBUId).SubSBU : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.LOS = userMasterVM.LOSId > 0 ? new LOSMasterRepository().Get(userMasterVM.LOSId) != null ? new LOSMasterRepository().Get(userMasterVM.LOSId).LOSName : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.Location = userMasterVM.LocationId > 0 ? new LocationMastersRepository().Get(userMasterVM.LocationId) != null ? new LocationMastersRepository().Get(userMasterVM.LocationId).LocationName : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.Region = userMasterVM.RegionId > 0 ? new RegionMasterRepository().Get(userMasterVM.RegionId) != null ? new RegionMasterRepository().Get(userMasterVM.RegionId).Region : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.ManagementLevel = userMasterVM.BusinessTitle > 0 ? new DesignationMasterRepository().Get(userMasterVM.BusinessTitle) != null ? new DesignationMasterRepository().Get(userMasterVM.BusinessTitle).Designation : Messages.NotAvailable : Messages.NotAvailable;

                    ViewBag.lstCategories = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstSubCategories = new SubCategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubCategoryName, Value = d.Id.ToString() }).ToList(); ;

                    if (!string.IsNullOrEmpty(userMasterVM.ImagePath))
                    {
                        if (!new Common().GetFilePathExist(userMasterVM.ImagePath))
                        {
                            userMasterVM.ImagePath = string.Empty;
                        }
                    }
                    ViewBag.ViewState = isView;
                    ViewBag.PageType = !isView ? "Edit" : "View";
                    return View("Compliant_one", userMasterVM);
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View("Compliant_one", EmployeeCompliant_oneVM);
        }

        [HttpPost]
        public ActionResult AddOrUpdateEmployeeCompliant(string EmpCompliantParams)
        {
            List<string> filesName = new List<string>();
            try
            {
                if (Request.Files.Count > 0)
                {
                    var files = Request.Files;

                    //iterating through multiple file collection   
                    foreach (string str in files)
                    {
                        HttpPostedFileBase file = Request.Files[str] as HttpPostedFileBase;
                        //Checking file is available to save.  
                        if (file != null)
                        {
                            var InputFileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + new Common().UniqueFileName();
                            string strpath = InputFileName += System.IO.Path.GetExtension(file.FileName);

                            String path = Server.MapPath("~/Documents/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            var ServerSavePath = Path.Combine(Server.MapPath("~/Documents/") + strpath);
                            //Save file to server folder  
                            file.SaveAs(ServerSavePath);
                            filesName.Add(strpath);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(EmpCompliantParams))
                {
                    var converter = new ExpandoObjectConverter();
                    dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(EmpCompliantParams, converter);
                    if (data.Id != null && !string.IsNullOrEmpty(Convert.ToString(data.Id)))
                    {
                        string Id = Convert.ToString(data.Id);
                        if (Id.Length > 5)
                        {
                            Id = CryptoEngineUtils.Decrypt(Id.Replace(" ", "+"), true);
                            EmpCompliantParams = new Common().UpdateTokenValue(EmpCompliantParams, Messages.Id, Convert.ToInt32(Id).ToString());
                        }
                    }
                    var EmployeeComplaintDto = JsonConvert.DeserializeObject<EmployeeCompliantMasterVM>(EmpCompliantParams);
                    EmployeeComplaintDto.Attachments = (string.Join(",", filesName.Select(x => x.ToString()).ToArray()));
                    var User = new EmployeeComplaintMastersRepository().AddOrUpdate(EmployeeComplaintDto);
                    return new ReplyFormat().Success(Messages.SUCCESS, User);
                }
                else
                {
                    return new ReplyFormat().Error(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (filesName.Count > 0)
                {
                    foreach (string file in filesName)
                    {
                        if (!string.IsNullOrEmpty(file))
                        {
                            string filePath = "~/Documents/" + file;

                            if (System.IO.File.Exists(Server.MapPath(filePath)))
                            {
                                System.IO.File.Delete(Server.MapPath(filePath));
                            }
                        }
                    }
                }

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            bool retval = true;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);

                    retval = new EmployeeComplaintMastersRepository().Delete(Convert.ToInt32(id));
                    return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "EmployeeComplaint"), null);
                }
                return new ReplyFormat().Error(Messages.BAD_DATA);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
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

        [HttpGet]
        public ActionResult LoadEmployeeComplain(int currentPageIndex, string range = "")
        {
            ViewBag.lstEmployeeComplain = GetAll(currentPageIndex, range);
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        public ActionResult Compliant_two()
        {
            ViewBag.NavbarTitle = "BHU Approval";
            return View();
        }
        public ActionResult Compliant_three()
        {
            ViewBag.NavbarTitle = "Committee";
            return View();
        }
        public ActionResult RemoveFile(string fileName)
        {
            try
            {
                new CompliantMastersRepository().Removefile(fileName);
                return new ReplyFormat().Success(Messages.DELETE_MESSAGE_FILE, fileName);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpGet]
        public ActionResult SubmitComplaint(string id)
        {
            bool retval = true;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);

                    retval = new EmployeeComplaintMastersRepository().SubmitComplaint(Convert.ToInt32(id));
                    return new ReplyFormat().Success(Messages.SUCCESS, Messages.ComplaintSubmitted);
                }
                //return RedirectToAction("Edit", new { id = id, isView = false });
                return new ReplyFormat().Error(Messages.RoleMasterComplaintCriteriaNotFound);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpPost]
        public ActionResult WithdrawComplaint(string withdrawData)
        {
            try
            {
                if (!string.IsNullOrEmpty(withdrawData))
                {
                    int id = 0; string remarks = string.Empty;
                    var converter = new ExpandoObjectConverter();
                    dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(withdrawData, converter);
                    if (!string.IsNullOrEmpty(data.Id))
                    {
                        id = Convert.ToInt32(CryptoEngineUtils.Decrypt(data.Id.Replace(" ", "+"), true));
                    }

                    new EmployeeComplaintMastersRepository().WithdrawComplaint(id, data.Remark);
                    return new ReplyFormat().Success(Messages.SUCCESS);
                }
                return new ReplyFormat().Error(Messages.BAD_DATA);

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpGet]
        public ActionResult GetHistoryByComplaint(string ComplaintId)
        {
            try
            {
                if (!string.IsNullOrEmpty(ComplaintId))
                {
                    ComplaintId = CryptoEngineUtils.Decrypt(ComplaintId.Replace(" ", "+"), true);
                    ViewBag.lstComplaintHistory = new EmployeeComplaintHistoryRepository().GetAll().Where(x => x.ComplaintId == Convert.ToInt32(ComplaintId)).ToList();
                    return PartialView("_ComplaintHistoryContent");
                }
                return new ReplyFormat().Error(Messages.BAD_DATA);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpGet]
        public ActionResult GetAwaitingComplaints()
        {
            try
            {
                ViewBag.lstComplaintAwaiting = new EmployeeComplaintHistoryRepository().GetAllAwaitingComplaints();
                return PartialView("_ComplaintAwaitingContent");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpGet]
        public ActionResult GetDueComplaints()
        {
            try
            {
                ViewBag.lstComplaintDue = new EmployeeComplaintHistoryRepository().GetAllDueComplaints();
                return PartialView("_ComplaintDueContent");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpGet]
        public ActionResult GetOverDueComplaints()
        {
            try
            {
                ViewBag.lstComplaintOverDue = new EmployeeComplaintHistoryRepository().GetAllOverDueComplaints();
                return PartialView("_ComplaintOverDueContent");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
    }
}
