using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            ViewBag.Entity = new EntityMasterRepository().Get(UserVM.Company).EntityName;
            ViewBag.SBU = new SBUMasterRepository().Get(UserVM.SBUId).SBU;
            ViewBag.SubSBU = new SubSBUMasterRepository().Get(UserVM.SubSBUId).SubSBU;
            ViewBag.LOS = new LOSMasterRepository().Get(UserVM.LOSId).LOSName;
            ViewBag.Competency = new CompetencyMastersRepository().Get(UserVM.CompentencyId).CompetencyName;
            ViewBag.Location = new LocationMastersRepository().Get(UserVM.LocationId).LocationName;
            ViewBag.Region = new RegionMasterRepository().Get(UserVM.RegionId).Region;
            ViewBag.ManagementLevel = new DesignationMasterRepository().Get(UserVM.BusinessTitle).Designation;
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
            return View(UserVM);
        }
        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                EmployeeCompliantMasterVM EmployeeCompliant_oneVM = new EmployeeComplaintMastersRepository().Get(id);
                UserMasterVM userMasterVM = new UserMastersRepository().Get(EmployeeCompliant_oneVM.UserId);
                if (EmployeeCompliant_oneVM != null)
                {
                    userMasterVM.CategoryId = EmployeeCompliant_oneVM.CategoryId;
                    userMasterVM.SubCategoryId = EmployeeCompliant_oneVM.SubCategoryId;
                    userMasterVM.Remark = EmployeeCompliant_oneVM.Remark;
                    userMasterVM.Attachments = EmployeeCompliant_oneVM.Attachments;
                    userMasterVM.Id = EmployeeCompliant_oneVM.UserId;

                    userMasterVM.CompentencyId = EmployeeCompliant_oneVM.Id;

                }
                ViewBag.Entity = new EntityMasterRepository().Get(userMasterVM.Company) != null ? new EntityMasterRepository().Get(userMasterVM.Company).EntityName : Messages.NotAvailable;
                ViewBag.SBU = new SBUMasterRepository().Get(userMasterVM.SBUId)!= null? new SBUMasterRepository().Get(userMasterVM.SBUId).SBU:Messages.NotAvailable;
                ViewBag.SubSBU = new SubSBUMasterRepository().Get(userMasterVM.SubSBUId)!= null? new SubSBUMasterRepository().Get(userMasterVM.SubSBUId).SubSBU:Messages.NotAvailable;
                ViewBag.LOS = new LOSMasterRepository().Get(userMasterVM.LOSId)!= null ? new LOSMasterRepository().Get(userMasterVM.LOSId).LOSName:Messages.NotAvailable;
                ViewBag.Competency = new CompetencyMastersRepository().Get(userMasterVM.CompentencyId)!=null?new CompetencyMastersRepository().Get(userMasterVM.CompentencyId).CompetencyName:Messages.NotAvailable;
                ViewBag.Location = new LocationMastersRepository().Get(userMasterVM.LocationId)!= null ? new LocationMastersRepository().Get(userMasterVM.LocationId).LocationName:Messages.NotAvailable;
                ViewBag.Region = new RegionMasterRepository().Get(userMasterVM.RegionId)!= null? new RegionMasterRepository().Get(userMasterVM.RegionId).Region:Messages.Region;
                ViewBag.ManagementLevel = new DesignationMasterRepository().Get(userMasterVM.BusinessTitle).Designation;
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
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View("Compliant_one");
        }

        [HttpPost]
        public ActionResult AddOrUpdateEmployeeCompliant(string EmpCompliantParams)
        {
            try
            {
                List<string> filesName = new List<string>();
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
                            var InputFileName = new Common().UniqueFileName();
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
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            bool retval = true;
            try
            {
                retval = new EmployeeComplaintMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "EmployeeComplaint"), null);
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
        public ActionResult RemoveFIle(string fileName)
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
    }
}
