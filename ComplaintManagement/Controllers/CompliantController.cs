﻿using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();
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
        public ActionResult Edit(string id, bool isView, string isRedirect)
        {
            EmployeeCompliantMasterVM EmployeeCompliant_oneVM = new EmployeeCompliantMasterVM();
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);
                    int ids = Convert.ToInt32(id);
                    EmployeeCompliant_oneVM = new EmployeeComplaintMastersRepository().Get(Convert.ToInt32(id));
                    //EmployeeCompliant_oneVM = new EmployeeComplaintMastersRepository().Get(ids);
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
                        userMasterVM.Attachments = EmployeeCompliant_oneVM.Attachments;
                        userMasterVM.ComplaintId = ids;
                        ViewBag.lstSubCategories = new SubCategoryMastersRepository().GetAll().Where(c => c.Status && c.CategoryId == EmployeeCompliant_oneVM.CategoryId).ToList().Select(d => new SelectListItem { Text = d.SubCategoryName, Value = d.Id.ToString() }).ToList(); 
                    }
                    ViewBag.Entity = userMasterVM.Company > 0 ? new EntityMasterRepository().Get(userMasterVM.Company) != null ? new EntityMasterRepository().Get(userMasterVM.Company).EntityName : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.SBU = userMasterVM.SBUId > 0 ? new SBUMasterRepository().Get(userMasterVM.SBUId) != null ? new SBUMasterRepository().Get(userMasterVM.SBUId).SBU : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.SubSBU = userMasterVM.SubSBUId > 0 ? new SubSBUMasterRepository().Get(userMasterVM.SubSBUId) != null ? new SubSBUMasterRepository().Get(userMasterVM.SubSBUId).SubSBU : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.LOS = userMasterVM.LOSId > 0 ? new LOSMasterRepository().Get(userMasterVM.LOSId) != null ? new LOSMasterRepository().Get(userMasterVM.LOSId).LOSName : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.Location = userMasterVM.LocationId > 0 ? new LocationMastersRepository().Get(userMasterVM.LocationId) != null ? new LocationMastersRepository().Get(userMasterVM.LocationId).LocationName : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.Region = userMasterVM.RegionId > 0 ? new RegionMasterRepository().Get(userMasterVM.RegionId) != null ? new RegionMasterRepository().Get(userMasterVM.RegionId).Region : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.ManagementLevel = userMasterVM.BusinessTitle > 0 ? new DesignationMasterRepository().Get(userMasterVM.BusinessTitle) != null ? new DesignationMasterRepository().Get(userMasterVM.BusinessTitle).Designation : Messages.NotAvailable : Messages.NotAvailable;

                    ViewBag.lstCategories = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
                    

                    ViewBag.CategoryName = new CategoryMastersRepository().Get(Convert.ToInt32(EmployeeCompliant_oneVM.CategoryId)).CategoryName;
                    ViewBag.SubCategoryName = new SubCategoryMastersRepository().Get(Convert.ToInt32(EmployeeCompliant_oneVM.SubCategoryId)).SubCategoryName;

                    ViewBag.CashType = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
                    ViewBag.InvolvedUsers = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();

                    ViewBag.lstComplaintHistory = new EmployeeComplaintHistoryRepository().GetAll().Where(x => x.ComplaintId == Convert.ToInt32(EmployeeCompliant_oneVM.Id)).ToList();

                    if (!string.IsNullOrEmpty(userMasterVM.ImagePath))
                    {
                        if (!new Common().GetFilePathExist(userMasterVM.ImagePath))
                        {
                            userMasterVM.ImagePath = string.Empty;
                        }
                    }


                    ViewBag.ViewState = isView;
                    ViewBag.PageType = !isView ? "Edit" : "View";
                    if (isRedirect == "2")
                    {
                        ViewBag.lstComplaintHistory = new EmployeeComplaintHistoryRepository().GetAll().Where(x => x.ComplaintId == ids).ToList();
                        ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();

                        int loginuserId = Convert.ToInt32(sid);

                        var HrRole = db.HR_Role.FirstOrDefault(i => i.ComplentId == (ids) && i.HRUserId == loginuserId);
                        if (HrRole != null)
                        {
                            userMasterVM.Ramarked = HrRole.Remark;
                            userMasterVM.Attachments1 = HrRole.Attachement;
                            userMasterVM.CaseType = HrRole.CaseType;
                            userMasterVM.InvolvedUsersId = HrRole.InvolvedUsersId;

                            if (HrRole.CommitteeUSerId != null)
                            {
                                var CommitteeData = db.CommitteeRoles.FirstOrDefault(i => i.Id == HrRole.CommitteeUSerId);
                                var CommitteeUserData = new UserMastersRepository().Get(Convert.ToInt32(CommitteeData.CommitteeUserId));
                                ViewBag.CommitteeUserData = CommitteeUserData;
                                ViewBag.CommitteeData = CommitteeData;
                                ViewBag.CommitteeEntity = userMasterVM.Company > 0 ? new EntityMasterRepository().Get(CommitteeUserData.Company) != null ? new EntityMasterRepository().Get(CommitteeUserData.Company).EntityName : Messages.NotAvailable : Messages.NotAvailable;
                                ViewBag.CommitteeSBU = userMasterVM.SBUId > 0 ? new SBUMasterRepository().Get(CommitteeUserData.SBUId) != null ? new SBUMasterRepository().Get(CommitteeUserData.SBUId).SBU : Messages.NotAvailable : Messages.NotAvailable;
                                ViewBag.CommitteeSubSBU = userMasterVM.SubSBUId > 0 ? new SubSBUMasterRepository().Get(CommitteeUserData.SubSBUId) != null ? new SubSBUMasterRepository().Get(CommitteeUserData.SubSBUId).SubSBU : Messages.NotAvailable : Messages.NotAvailable;
                                ViewBag.CommitteeLOS = userMasterVM.LOSId > 0 ? new LOSMasterRepository().Get(CommitteeUserData.LOSId) != null ? new LOSMasterRepository().Get(CommitteeUserData.LOSId).LOSName : Messages.NotAvailable : Messages.NotAvailable;
                                ViewBag.CommitteeLocation = userMasterVM.LocationId > 0 ? new LocationMastersRepository().Get(CommitteeUserData.LocationId) != null ? new LocationMastersRepository().Get(CommitteeUserData.LocationId).LocationName : Messages.NotAvailable : Messages.NotAvailable;
                                ViewBag.CommitteeRegion = userMasterVM.RegionId > 0 ? new RegionMasterRepository().Get(CommitteeUserData.RegionId) != null ? new RegionMasterRepository().Get(CommitteeUserData.RegionId).Region : Messages.NotAvailable : Messages.NotAvailable;
                                ViewBag.CommitteeManagementLevel = userMasterVM.BusinessTitle > 0 ? new DesignationMasterRepository().Get(CommitteeUserData.BusinessTitle) != null ? new DesignationMasterRepository().Get(CommitteeUserData.BusinessTitle).Designation : Messages.NotAvailable : Messages.NotAvailable;
                                ViewBag.CommitteeCompetency = CommitteeUserData.CompentencyId > 0 ? new CompetencyMastersRepository().Get(CommitteeUserData.CompentencyId) != null ? new CompetencyMastersRepository().Get(CommitteeUserData.CompentencyId).CompetencyName : Messages.NotAvailable : Messages.NotAvailable;
                                ViewBag.CommitteeInvolvedUsers = GetCommaSeparatedUser(CommitteeData.InvolvedUsersId);
                                ViewBag.CommitteeCaseType = CommitteeData.CashTypeId;


                            }
                        }



                        return View("Compliant_two", userMasterVM);
                    }
                    else if (isRedirect == "3")
                    {
                        return View("Compliant_three", userMasterVM);
                    }
                    else
                    {
                        return View("Compliant_one", userMasterVM);

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            if (isRedirect == "2")
            {
                return View("Compliant_two", EmployeeCompliant_oneVM);
            }
            else if (isRedirect == "3")
            {
                return View("Compliant_three", EmployeeCompliant_oneVM);
            }
            else
            {
                return View("Compliant_one", EmployeeCompliant_oneVM);
            }
        }

        [HttpPost]
        public ActionResult AddOrUpdateEmployeeCompliant(string EmpCompliantParams, string flag)
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
                    var User = new EmployeeComplaintMastersRepository().AddOrUpdate(EmployeeComplaintDto, flag);
                    if (flag == "B")
                    {
                        flag = null;
                        return RedirectToAction("SubmitComplaint", new { id = CryptoEngineUtils.Encrypt(Convert.ToString(User.EmployeeComplaintMasterId), true) });
                    }
                    else
                    {
                        return new ReplyFormat().Success(Messages.SUCCESS, User);
                    }
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
            ViewBag.lstSubCategories = new SubCategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubCategoryName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstComplaintHistory = new EmployeeComplaintHistoryRepository().GetAll().Where(x => x.ComplaintId == Convert.ToInt32(sid)).ToList();
            ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
            if (!string.IsNullOrEmpty(UserVM.ImagePath))
            {
                if (!new Common().GetFilePathExist(UserVM.ImagePath))
                {
                    UserVM.ImagePath = string.Empty;
                }
            }

            //var HrRole = db.HR_Role.FirstOrDefault(i => i.ComplentId == (complaintId));
            //ViewBag.HrRole = HrRole;
            //var hrRoleData = new UserMastersRepository().Get(Convert.ToInt32(HrRole.HRUserId));
            //ViewBag.HrRoleData = hrRoleData;

            ViewBag.NavbarTitle = "Complaint Information";
            UserVM.ComplaintStatus = Messages.Opened; UserVM.Id = UserVM.CompentencyId = 0; UserVM.DateOfJoining = DateTime.UtcNow.AddDays(5);
            return View(UserVM);
        }
        public ActionResult Compliant_three(string id, bool isView)
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
               .Select(c => c.Value).SingleOrDefault();
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
                        userMasterVM.Attachments = EmployeeCompliant_oneVM.Attachments;
                        userMasterVM.ComplaintId = EmployeeCompliant_oneVM.Id;
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

                    ViewBag.CategoryName = new CategoryMastersRepository().Get(Convert.ToInt32(EmployeeCompliant_oneVM.CategoryId)).CategoryName;
                    ViewBag.SubCategoryName = new SubCategoryMastersRepository().Get(Convert.ToInt32(EmployeeCompliant_oneVM.SubCategoryId)).SubCategoryName;

                    //ViewBag.CashType = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
                    ViewBag.InvolvedUsers = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();

                    ViewBag.lstComplaintHistory = new EmployeeComplaintHistoryRepository().GetAll().Where(x => x.ComplaintId == Convert.ToInt32(EmployeeCompliant_oneVM.Id)).ToList();

                    //HR Role data
                    int complaintId = Convert.ToInt32(id);

                    var HrRole = db.HR_Role.FirstOrDefault(i => i.ComplentId == (complaintId) && i.Status == Messages.COMMITTEE);
                    ViewBag.HrRole = HrRole;
                    var hrRoleData = new UserMastersRepository().Get(Convert.ToInt32(HrRole.HRUserId));
                    ViewBag.HrRoleData = hrRoleData;
                    ViewBag.HrRoleEntity = userMasterVM.Company > 0 ? new EntityMasterRepository().Get(hrRoleData.Company) != null ? new EntityMasterRepository().Get(hrRoleData.Company).EntityName : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.HrRoleSBU = userMasterVM.SBUId > 0 ? new SBUMasterRepository().Get(hrRoleData.SBUId) != null ? new SBUMasterRepository().Get(hrRoleData.SBUId).SBU : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.HrRoleSubSBU = userMasterVM.SubSBUId > 0 ? new SubSBUMasterRepository().Get(hrRoleData.SubSBUId) != null ? new SubSBUMasterRepository().Get(hrRoleData.SubSBUId).SubSBU : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.HrRoleLOS = userMasterVM.LOSId > 0 ? new LOSMasterRepository().Get(hrRoleData.LOSId) != null ? new LOSMasterRepository().Get(hrRoleData.LOSId).LOSName : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.HrRoleLocation = userMasterVM.LocationId > 0 ? new LocationMastersRepository().Get(hrRoleData.LocationId) != null ? new LocationMastersRepository().Get(hrRoleData.LocationId).LocationName : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.HrRoleRegion = userMasterVM.RegionId > 0 ? new RegionMasterRepository().Get(hrRoleData.RegionId) != null ? new RegionMasterRepository().Get(hrRoleData.RegionId).Region : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.HrRoleManagementLevel = userMasterVM.BusinessTitle > 0 ? new DesignationMasterRepository().Get(hrRoleData.BusinessTitle) != null ? new DesignationMasterRepository().Get(hrRoleData.BusinessTitle).Designation : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.Competency = hrRoleData.CompentencyId > 0 ? new CompetencyMastersRepository().Get(hrRoleData.CompentencyId) != null ? new CompetencyMastersRepository().Get(hrRoleData.CompentencyId).CompetencyName : Messages.NotAvailable : Messages.NotAvailable;
                    ViewBag.HrRoleInvolvedUsers = GetCommaSeparatedUser(HrRole.InvolvedUsersId);
                    ViewBag.HrRoleCaseType = HrRole.CaseType;

                    int loginUserId = Convert.ToInt32(sid);
                    var CommitteeRoleData = db.CommitteeRoles.FirstOrDefault(i => i.ComplaintId == (complaintId) && i.CommitteeUserId == loginUserId);
                    if (CommitteeRoleData != null)
                    {
                        userMasterVM.AttachmentsCommittee = CommitteeRoleData.Attachment;
                        userMasterVM.RemarkCommittee = CommitteeRoleData.Remark;
                        userMasterVM.CashTypeId = CommitteeRoleData.CashTypeId;
                        //string[] involveuser= (CommitteeRoleData.InvolvedUsersId).Split(',').ToArray();
                        userMasterVM.InvolvedUsersId = CommitteeRoleData.InvolvedUsersId;
                    }

                    if (!string.IsNullOrEmpty(userMasterVM.ImagePath))
                    {
                        if (!new Common().GetFilePathExist(userMasterVM.ImagePath))
                        {
                            userMasterVM.ImagePath = string.Empty;
                        }
                    }
                    return View(userMasterVM);
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View(EmployeeCompliant_oneVM);

        }
        [HttpPost]
        public ActionResult SaveEmployeeCommitteeCompliant(string EmpCompliantParams, string UserInvolved)
        {
            List<string> filesName = new List<string>();
            try
            {
                var CommitteUserid = Session["id"];

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



                    if (!string.IsNullOrEmpty(data.ComplaintId))
                    {
                        data.ComplaintId = CryptoEngineUtils.Decrypt(data.ComplaintId.Replace(" ", "+"), true);
                        JObject jsonObj = (JObject)JsonConvert.DeserializeObject(EmpCompliantParams);
                        jsonObj.Property("ComplaintId").Value = data.ComplaintId;
                        EmpCompliantParams = JsonConvert.SerializeObject(jsonObj);
                    }
                    var EmployeeComplaintDto = JsonConvert.DeserializeObject<UserMasterVM>(EmpCompliantParams);
                    EmployeeComplaintDto.AttachmentsCommittee = (string.Join(",", filesName.Select(x => x.ToString()).ToArray()));
                    var User = new EmployeeComplaintMastersRepository().AddOrUpdateSaveCommittee(EmployeeComplaintDto, UserInvolved);
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
        public ActionResult BackToBUHCEmployeeCommitteeCompliant(string EmpCompliantParams, string UserInvolved, string HrRoleId)
        {
            List<string> filesName = new List<string>();
            try
            {
                var CommitteUserid = Session["id"];

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



                    if (!string.IsNullOrEmpty(data.ComplaintId))
                    {
                        data.ComplaintId = CryptoEngineUtils.Decrypt(data.ComplaintId.Replace(" ", "+"), true);
                        JObject jsonObj = (JObject)JsonConvert.DeserializeObject(EmpCompliantParams);
                        jsonObj.Property("ComplaintId").Value = data.ComplaintId;
                        EmpCompliantParams = JsonConvert.SerializeObject(jsonObj);
                    }
                    var EmployeeComplaintDto = JsonConvert.DeserializeObject<UserMasterVM>(EmpCompliantParams);
                    EmployeeComplaintDto.AttachmentsCommittee = (string.Join(",", filesName.Select(x => x.ToString()).ToArray()));
                    var User = new EmployeeComplaintMastersRepository().AddOrUpdateBackToBUHCCommittee(EmployeeComplaintDto, UserInvolved, HrRoleId);
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

        public string GetCommaSeparatedUser(string UserIds)
        {
            try
            {
                string Userdata = "";
                string[] arrIds = UserIds.Split(',');

                foreach (var Ids in arrIds)
                {
                    Userdata += new UserMastersRepository().Get(Convert.ToInt32(Ids)).EmployeeName + ", ";
                }
                return Userdata;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return "";
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
        public ActionResult RemoveCommitteefile(string fileName, string ComplaintId)
        {
            try
            {
                ComplaintId = CryptoEngineUtils.Decrypt(ComplaintId.Replace(" ", "+"), true);
                new CompliantMastersRepository().RemoveCommitteefile(fileName, ComplaintId);
                return new ReplyFormat().Success(Messages.DELETE_MESSAGE_FILE, fileName);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult RemoveHRfile(string fileName, string ComplaintId)
        {
            try
            {
                //ComplaintId = CryptoEngineUtils.Decrypt(ComplaintId.Replace(" ", "+"), true);
                new CompliantMastersRepository().RemoveHRfile(fileName, ComplaintId);
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
        public ActionResult GetHistoryByComplaintId(string ComplaintId)
        {
            try
            {
                if (!string.IsNullOrEmpty(ComplaintId))
                {
                    ComplaintId = CryptoEngineUtils.Decrypt(ComplaintId.Replace(" ", "+"), true);
                    ViewBag.lstComplaintHistory = new EmployeeComplaintHistoryRepository().GetAll().Where(x => x.ComplaintId == Convert.ToInt32(ComplaintId) && (x.ActionType.ToLower() != Messages.PushToCommittee.ToLower() && x.ActionType.ToLower() != Messages.BackToBUHC.ToLower() ) && x.UserType.ToLower() != Messages.COMMITTEE.ToLower()).ToList();
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


        [HttpGet]
        public ActionResult ComplaintThree_Index()
        {
            int currentPage = 1;
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }
            var complaintcommittee = new EmployeeComplaintHistoryRepository().GetAllComplaintsThree();
            var complaintcomm = complaintcommittee.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();
            ViewBag.lstEmployeeComplaint = complaintcomm.ToList();
            lstCount = complaintcommittee.Count;
            double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
            ViewBag.PageCount = (int)Math.Ceiling(pageCount);

            ViewBag.CurrentPageIndex = currentPage;


            ViewBag.lstCategories = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSubCategories = new SubCategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubCategoryName, Value = d.Id.ToString() }).ToList(); ;

            //var DataTableDetail = new HomeController().getDataTableDetail("EmployeeCompliant", null);
            //ViewBag.Page = DataTableDetail.Item1;
            //ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult ComplaintThreecommittee_Index(int currentPage)
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var complaintcommittee = new EmployeeComplaintHistoryRepository().GetAllComplaintsThree();
            var complaintcomm = complaintcommittee.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();
            ViewBag.lstEmployeeComplaint = complaintcomm.ToList();
            lstCount = complaintcommittee.Count;

            double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
            ViewBag.PageCount = (int)Math.Ceiling(pageCount);

            ViewBag.CurrentPageIndex = currentPage;

            ViewBag.lstCategories = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSubCategories = new SubCategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubCategoryName, Value = d.Id.ToString() }).ToList(); ;

            //var DataTableDetail = new HomeController().getDataTableDetail("EmployeeCompliant", null);
            //ViewBag.Page = DataTableDetail.Item1;
            //ViewBag.PageIndex = DataTableDetail.Item2;


            return View("ComplaintThree_Index");
        }


        [HttpGet]
        public ActionResult ComplaintTwo_Index()
        {
            int currentPage = 1;
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }
            var complaintHR = new EmployeeComplaintHistoryRepository().GetAllComplaintsTwo();
            var comphr = complaintHR.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();
            ViewBag.lstEmployeeComplaint = comphr.ToList();
            lstCount = complaintHR.Count;
            double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
            ViewBag.PageCount = (int)Math.Ceiling(pageCount);

            ViewBag.CurrentPageIndex = currentPage;
            ViewBag.lstCategories = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSubCategories = new SubCategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubCategoryName, Value = d.Id.ToString() }).ToList(); ;

            var DataTableDetail = new HomeController().getDataTableDetail("EmployeeCompliant", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult ComplaintTwoHR_Index(int currentPage)
        {

            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }
            var complaintHR = new EmployeeComplaintHistoryRepository().GetAllComplaintsTwo();
            var comphr = complaintHR.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();
            ViewBag.lstEmployeeComplaint = comphr.ToList();
            lstCount = complaintHR.Count;
            double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
            ViewBag.PageCount = (int)Math.Ceiling(pageCount);

            ViewBag.CurrentPageIndex = currentPage;
            ViewBag.lstCategories = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSubCategories = new SubCategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubCategoryName, Value = d.Id.ToString() }).ToList(); ;

            var DataTableDetail = new HomeController().getDataTableDetail("EmployeeCompliant", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View("ComplaintTwo_Index");
        }

        //Aman work
        [HttpPost]

        public ActionResult AddOrEmployeeCompliantHR(string EmpCompliantParams, String Id, string UserInvolved, int Status)

        {
            List<string> filesName = new List<string>();
            try
            {
                string ids = CryptoEngineUtils.Decrypt(Id.Replace(" ", "+"), true);
                //e id--user master(hr)
                var Hrid = Session["id"];
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
                    var EmployeeComplaintDto = JsonConvert.DeserializeObject<EmployeeCompliantMasterVM>(EmpCompliantParams);
                    EmployeeComplaintDto.Attachments1 = (string.Join(",", filesName.Select(x => x.ToString()).ToArray()));
                    var User = new EmployeeComplaintMastersRepository().SaveHRComplaint(EmployeeComplaintDto, ids, Convert.ToInt32(Hrid), UserInvolved, Status);
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

        //14/12/2020
        public ActionResult ComplaintTwo_Close(string Id, string UserId, string Remark)
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();

                string id = CryptoEngineUtils.Decrypt(Id.Replace(" ", "+"), true);
                int ids = Convert.ToInt32(id);
                var WorkFlow = db.EmployeeComplaintWorkFlows.FirstOrDefault(p => p.ComplaintId == ids);
                if (WorkFlow != null)
                {
                    WorkFlow.ActionType = Messages.COMPLETED;
                    WorkFlow.UpdatedDate = DateTime.UtcNow;
                    db.Entry(WorkFlow).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var ComplaintMaster = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == ids);
                if (ComplaintMaster != null)
                {
                    ComplaintMaster.ComplaintStatus = Messages.COMPLETED;
                    ComplaintMaster.UpdatedDate = DateTime.UtcNow;
                    ComplaintMaster.LastPerformedBy = null;
                    db.Entry(ComplaintMaster).State = EntityState.Modified;
                    db.SaveChanges();
                }
                new EmployeeComplaintHistoryRepository().AddComplaintHistory(Remark, ids, Messages.COMPLETED, db);
                //Notification Work
                var LOSName = string.Empty; var CategoryName = string.Empty; var SubCategoryName = string.Empty;
                string NotificationContent = string.Empty;
                List<string> mailTo = new List<string>();
                List<string> mailBody = new List<string>();
                CategoryName = new CategoryMastersRepository().Get(Convert.ToInt32(ComplaintMaster.CategoryId)).CategoryName;
                SubCategoryName = new SubCategoryMastersRepository().Get(Convert.ToInt32(ComplaintMaster.SubCategoryId)).SubCategoryName;
                var userData = new UserMastersRepository().Get(Convert.ToInt32(sid));
                LOSName = new LOSMasterRepository().Get(WorkFlow.LOSId).LOSName;
                NotificationContent = WorkFlow.ComplaintNo + " (" + LOSName + "-" + CategoryName + "-" + SubCategoryName + ") has been closed on " + DateTime.UtcNow.ToString("dd/MM/yyyy") + " for " + LOSName  + " by " + userData.EmployeeName + ".";

                new NotificationAlertRepository().AddNotificatioAlert(NotificationContent, Convert.ToInt32(ComplaintMaster.CreatedBy));
                mailTo.Add(userData.WorkEmail);
                mailBody.Add(@"<html><body><p>Subject:Compliant Completion" + ",</p></br><p>Receipt-HR and Compliant Owner" + ",</p></br><p>Subject:Compliant Completion" + ",</p></br><p>Receipt-HR and Compliant Owner" + ",</p></br>Dear " + userData.EmployeeName + ",</p></br><p>" + NotificationContent + "</p><p>Thank You.</br></br>Employee Assistance Portal</p></body></html>");
                MailSend.SendEmailWithDifferentBody(mailTo, "Compliant Completion", mailBody, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("ComplaintTwo_Index");
        }

        [HttpGet]
        public ActionResult NotificationAlertCount()
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
                List<NotificationAlert> NotificationAlert = new NotificationAlertRepository().Get(Convert.ToInt32(sid));

                var count = NotificationAlert.Where(x => x.Status == true).Count();
                return new ReplyFormat().Success(Messages.SUCCESS, count);
            }
            catch (Exception ex)
            {
                return new ReplyFormat().Error(Messages.BAD_DATA);
            }

        }
        [HttpGet]
        public ActionResult NotificationAlertList()
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
                var NotificationAlert = new NotificationAlertRepository().Get(Convert.ToInt32(sid)).Select(x => new { NotificationContent = x.NotificationContent, CreatedDate = x.CreatedDate.ToString("MMMM dd yyyy"), OrderByDate = x.CreatedDate }).OrderByDescending(x => Convert.ToDateTime(x.OrderByDate)).Take(5).ToList();
               

                return new ReplyFormat().Success(Messages.SUCCESS, NotificationAlert);
            }
            catch (Exception ex)
            {
                return new ReplyFormat().Error(Messages.BAD_DATA);
            }

        }

        [HttpPost]
        public ActionResult NotificationAlertRead()
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
                new NotificationAlertRepository().UpdateNotificatioAlert();
                return new ReplyFormat().Success(Messages.SUCCESS);
            }
            catch (Exception ex)
            {
                return new ReplyFormat().Error(Messages.BAD_DATA);
            }

        }

        public ActionResult NotificationList()
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
                ViewBag.NotificationAlert = new NotificationAlertRepository().Get(Convert.ToInt32(sid)).OrderByDescending(x => x.CreatedDate).ToList();
                return View();
            }
            catch (Exception ex)
            {
                return new ReplyFormat().Error(Messages.BAD_DATA);
            }

        }

        [HttpGet]
        public ActionResult DownloadAttachments(string files, string Id, string redirectTo)
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
                                else
                                {
                                    if (redirectTo == "1")
                                    {
                                        return RedirectToAction("Edit", new { id = CryptoEngineUtils.Encrypt(Convert.ToString(Id), true), isView = false, isRedirect = 2 });
                                    }
                                    else
                                    {
                                        return RedirectToAction("Compliant_three", new { id = CryptoEngineUtils.Encrypt(Convert.ToString(Id), true), isView = false });
                                    }

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

            }
            if (redirectTo == "1")
            {
                return RedirectToAction("Edit", new { id = CryptoEngineUtils.Encrypt(Convert.ToString(Id), true), isView = false, isRedirect = 2 });
            }
            else
            {
                return RedirectToAction("Compliant_three", new { id = CryptoEngineUtils.Encrypt(Convert.ToString(Id), true), isView = false });
            }
        }
        
        public JsonResult CategoryWiseSubCategory(int CategoryId)
        {
            try
            {
                List<SubCategoryMasterVM> Subcategory = new List<SubCategoryMasterVM>();
                if (!string.IsNullOrEmpty(CategoryId.ToString()))
                {
                    Subcategory = new SubCategoryMastersRepository().CategoryWiseSubCategory(CategoryId);

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

        public JsonResult SubCategoryWiseCategory(int SubCategoryId)
        {
            try
            {
                SubCategoryMasterVM Subcategory = new SubCategoryMasterVM();
                if (!string.IsNullOrEmpty(SubCategoryId.ToString()))
                {
                    Subcategory = new SubCategoryMastersRepository().Get(SubCategoryId);

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

     
    }
}
