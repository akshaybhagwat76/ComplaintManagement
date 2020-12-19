using ComplaintManagement.Helpers;
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
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    [Authorize]
    public class UserMasterController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            var DataTableDetail = new HomeController().getDataTableDetail("User", null);
            ViewBag.lstUser = JsonConvert.SerializeObject(GetAll(1));
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

                var lst = GetAllHistory(1, Convert.ToInt32(id));
                ViewBag.name = id.ToString();
                var DataTableDetail = new HomeController().getDataTableDetail("User", null);
                ViewBag.lstUserHistory = JsonConvert.SerializeObject(lst);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
                return View();
            }
            else
            {
                return View("Index");
            }

        }
        public ActionResult SearchUser(string search)
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
            ViewBag.lstUser = JsonConvert.SerializeObject(data.Distinct().ToList());

            return View("Index");
        }

        [HttpGet]
        public ActionResult LoadUserMasters(int currentPageIndex, string range = "")
        {
            ViewBag.lstUser = JsonConvert.SerializeObject(GetAll(currentPageIndex, range));
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult LoadHistoryUserMasters(int currentPageIndex, int range = 0)
        {
            ViewBag.lstUserHistory = JsonConvert.SerializeObject(GetAllHistory(currentPageIndex, range));
            //if (!string.IsNullOrEmpty(range))
            //{
            //    ViewBag.startDate = range.Split(',')[0];
            //    ViewBag.toDate = range.Split(',')[1];
            //}
            return View("HistoryIndex");
        }

        public dynamic GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }
            var lst = new UserMastersRepository().GetAll().ToList();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from user in lst
                       where user.CreatedDate.Date >= fromDate.Date && user.CreatedDate.Date <= toDate.Date
                       select user).ToList();
                lstCount = lst.Count;
                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();


                dynamic output = new List<dynamic>();

                #region Other logics
                var lstSBU = new SBUMasterRepository().GetAll();
                var lstSubSBU = new SubSBUMasterRepository().GetAll();
                var lstLOS = new LOSMasterRepository().GetAll();
                var lstCompetency = new CompetencyMastersRepository().GetAll();
                var lstRegion = new RegionMasterRepository().GetAll();
                var lstDesignation = new DesignationMasterRepository().GetAll();
                var LstCompany = new EntityMasterRepository().GetAll();
                if (lst != null && lst.Count > 0)
                {
                    foreach (UserMasterVM com in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (com.SBUId > 0)
                        {
                            row.SBU = lstSBU.FirstOrDefault(x => x.Id == com.SBUId) != null ? lstSBU.FirstOrDefault(x => x.Id == com.SBUId).SBU : "";
                        }
                        if (com.SubSBUId > 0)
                        {

                            row.SubSBU = lstSubSBU.FirstOrDefault(x => x.Id == com.SubSBUId) != null ? lstSubSBU.FirstOrDefault(x => x.Id == com.SubSBUId).SubSBU : "";
                        }
                        if (com.LOSId > 0)
                        {
                            row.LOS = lstLOS.FirstOrDefault(x => x.Id == com.LOSId) != null ? lstLOS.FirstOrDefault(x => x.Id == com.LOSId).LOSName : "";
                        }
                        if (com.CompentencyId > 0)
                        {
                            row.Competency = lstCompetency.FirstOrDefault(x => x.Id == com.CompentencyId) != null ? lstCompetency.FirstOrDefault(x => x.Id == com.CompentencyId).CompetencyName : "";
                        }
                        if (com.RegionId > 0)
                        {
                            row.Region = lstRegion.FirstOrDefault(x => x.Id == com.RegionId) != null ? lstRegion.FirstOrDefault(x => x.Id == com.RegionId).Region : "";
                        }
                        if (com.BusinessTitle > 0)
                        {
                            row.BusinessTitle = lstDesignation.FirstOrDefault(x => x.Id == com.BusinessTitle) != null ? lstDesignation.FirstOrDefault(x => x.Id == com.BusinessTitle).Designation : "";
                        }
                        if (com.Company > 0)
                        {
                            row.Company = LstCompany.FirstOrDefault(x => x.Id == com.Company) != null ? LstCompany.FirstOrDefault(x => x.Id == com.Company).EntityName : "";
                        }
                        row.Id = com.Id;
                        row.EmployeeName = com.EmployeeName;
                        row.Status = com.Status;
                        row.TimeType = com.TimeType;
                        row.Manager = com.Manager;
                        row.CreatedByName = com.CreatedByName;
                        row.UpdatedByName = com.UpdatedByName;

                        row.UpdatedDate = com.UpdatedDate; row.CreatedDate = com.CreatedDate;

                        output.Add(row);
                    }
                }

                #endregion

                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return output;
            }
            else
            {

                dynamic output = new List<dynamic>();

                lst = (from user in lst
                       select user)
           .OrderByDescending(user => user.Id)
           .Skip((currentPage - 1) * maxRows)
           .Take(maxRows).ToList();

                #region Other joining logics
                var lstSBU = new SBUMasterRepository().GetAll();
                var lstSubSBU = new SubSBUMasterRepository().GetAll();
                var lstLOS = new LOSMasterRepository().GetAll();
                var lstCompetency = new CompetencyMastersRepository().GetAll();
                var lstRegion = new RegionMasterRepository().GetAll();
                var lstDesignation = new DesignationMasterRepository().GetAll();
                var LstCompany = new EntityMasterRepository().GetAll();
                if (lst != null && lst.Count > 0)
                {
                    foreach (UserMasterVM com in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (com.SBUId > 0)
                        {
                            row.SBU = lstSBU.FirstOrDefault(x => x.Id == com.SBUId) != null ? lstSBU.FirstOrDefault(x => x.Id == com.SBUId).SBU : "";

                        }
                        if (com.SubSBUId > 0)
                        {

                            row.SubSBU = lstSubSBU.FirstOrDefault(x => x.Id == com.SubSBUId) != null ? lstSubSBU.FirstOrDefault(x => x.Id == com.SubSBUId).SubSBU : "";
                        }
                        if (com.LOSId > 0)
                        {
                            row.LOS = lstLOS.FirstOrDefault(x => x.Id == com.LOSId) != null ? lstLOS.FirstOrDefault(x => x.Id == com.LOSId).LOSName : "";
                        }
                        if (com.CompentencyId > 0)
                        {
                            row.Competency = lstCompetency.FirstOrDefault(x => x.Id == com.CompentencyId) != null ? lstCompetency.FirstOrDefault(x => x.Id == com.CompentencyId).CompetencyName : "";
                        }
                        if (com.RegionId > 0)
                        {
                            row.Region = lstRegion.FirstOrDefault(x => x.Id == com.RegionId) != null ? lstRegion.FirstOrDefault(x => x.Id == com.RegionId).Region : "";
                        }
                        if (com.BusinessTitle > 0)
                        {
                            row.BusinessTitle = lstDesignation.FirstOrDefault(x => x.Id == com.BusinessTitle) != null ? lstDesignation.FirstOrDefault(x => x.Id == com.BusinessTitle).Designation : "";
                        }
                        if (com.Company > 0)
                        {
                            row.Company = LstCompany.FirstOrDefault(x => x.Id == com.Company) != null ? LstCompany.FirstOrDefault(x => x.Id == com.Company).EntityName : "";
                        }
                        row.Id = com.Id;
                        row.EmployeeName = com.EmployeeName;
                        row.Status = com.Status;
                        row.TimeType = com.TimeType;
                        row.Manager = com.Manager;
                        row.UpdatedDate = com.UpdatedDate; row.CreatedDate = com.CreatedDate;
                        row.CreatedByName = com.CreatedByName;
                        row.UpdatedByName = com.UpdatedByName;
                        output.Add(row);
                    }
                }
                #endregion

                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;

                return output;
            }
        }
        public dynamic GetAllHistory(int currentPage, int range = 0)
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }
            var lst = new UserMastersRepository().GetAllHistory().ToList();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range.ToString()))
            {
                //string[] dates = range.Split(',');
                //DateTime fromDate = Convert.ToDateTime(dates[0]);
                //DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from user in lst
                       where user.UserMasterId == range
                       select user).ToList();
                lstCount = lst.Count;
                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();


                dynamic output = new List<dynamic>();

                #region Other logics
                var lstSBU = new SBUMasterRepository().GetAll();
                var lstSubSBU = new SubSBUMasterRepository().GetAll();
                var lstLOS = new LOSMasterRepository().GetAll();
                var lstCompetency = new CompetencyMastersRepository().GetAll();
                var lstRegion = new RegionMasterRepository().GetAll();
                var lstDesignation = new DesignationMasterRepository().GetAll();
                var LstCompany = new EntityMasterRepository().GetAll();
                if (lst != null && lst.Count > 0)
                {
                    foreach (UserMasterHistoryVM com in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (com.SBUId > 0)
                        {
                            row.SBU = lstSBU.FirstOrDefault(x => x.Id == com.SBUId) != null ? lstSBU.FirstOrDefault(x => x.Id == com.SBUId).SBU : "";
                        }
                        if (com.SubSBUId > 0)
                        {

                            row.SubSBU = lstSubSBU.FirstOrDefault(x => x.Id == com.SubSBUId) != null ? lstSubSBU.FirstOrDefault(x => x.Id == com.SubSBUId).SubSBU : "";
                        }
                        if (com.LOSId > 0)
                        {
                            row.LOS = lstLOS.FirstOrDefault(x => x.Id == com.LOSId) != null ? lstLOS.FirstOrDefault(x => x.Id == com.LOSId).LOSName : "";
                        }
                        if (com.CompentencyId > 0)
                        {
                            row.Competency = lstCompetency.FirstOrDefault(x => x.Id == com.CompentencyId) != null ? lstCompetency.FirstOrDefault(x => x.Id == com.CompentencyId).CompetencyName : "";
                        }
                        if (com.RegionId > 0)
                        {
                            row.Region = lstRegion.FirstOrDefault(x => x.Id == com.RegionId) != null ? lstRegion.FirstOrDefault(x => x.Id == com.RegionId).Region : "";
                        }
                        if (com.BusinessTitle > 0)
                        {
                            row.BusinessTitle = lstDesignation.FirstOrDefault(x => x.Id == com.BusinessTitle) != null ? lstDesignation.FirstOrDefault(x => x.Id == com.BusinessTitle).Designation : "";
                        }
                        if (com.Company > 0)
                        {
                            row.Company = LstCompany.FirstOrDefault(x => x.Id == com.Company) != null ? LstCompany.FirstOrDefault(x => x.Id == com.Company).EntityName : "";
                        }
                        row.Id = com.Id;
                        row.EmployeeName = com.EmployeeName;
                        row.Status = com.Status;
                        row.TimeType = com.TimeType;
                        row.Manager = com.Manager;
                        row.CreatedByName = com.CreatedByName;
                        row.UpdatedByName = com.UpdatedByName;
                        row.EntityState = com.EntityState;

                        row.UpdatedDate = com.UpdatedDate; row.CreatedDate = com.CreatedDate;

                        output.Add(row);
                    }
                }

                #endregion

                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return output;
            }
            else
            {

                dynamic output = new List<dynamic>();

                lst = (from user in lst
                       select user)
           .OrderByDescending(user => user.Id)
           .Skip((currentPage - 1) * maxRows)
           .Take(maxRows).ToList();

                #region Other joining logics
                var lstSBU = new SBUMasterRepository().GetAll();
                var lstSubSBU = new SubSBUMasterRepository().GetAll();
                var lstLOS = new LOSMasterRepository().GetAll();
                var lstCompetency = new CompetencyMastersRepository().GetAll();
                var lstRegion = new RegionMasterRepository().GetAll();
                var lstDesignation = new DesignationMasterRepository().GetAll();
                var LstCompany = new EntityMasterRepository().GetAll();
                if (lst != null && lst.Count > 0)
                {
                    foreach (UserMasterHistoryVM com in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (com.SBUId > 0)
                        {
                            row.SBU = lstSBU.FirstOrDefault(x => x.Id == com.SBUId) != null ? lstSBU.FirstOrDefault(x => x.Id == com.SBUId).SBU : "";

                        }
                        if (com.SubSBUId > 0)
                        {

                            row.SubSBU = lstSubSBU.FirstOrDefault(x => x.Id == com.SubSBUId) != null ? lstSubSBU.FirstOrDefault(x => x.Id == com.SubSBUId).SubSBU : "";
                        }
                        if (com.LOSId > 0)
                        {
                            row.LOS = lstLOS.FirstOrDefault(x => x.Id == com.LOSId) != null ? lstLOS.FirstOrDefault(x => x.Id == com.LOSId).LOSName : "";
                        }
                        if (com.CompentencyId > 0)
                        {
                            row.Competency = lstCompetency.FirstOrDefault(x => x.Id == com.CompentencyId) != null ? lstCompetency.FirstOrDefault(x => x.Id == com.CompentencyId).CompetencyName : "";
                        }
                        if (com.RegionId > 0)
                        {
                            row.Region = lstRegion.FirstOrDefault(x => x.Id == com.RegionId) != null ? lstRegion.FirstOrDefault(x => x.Id == com.RegionId).Region : "";
                        }
                        if (com.BusinessTitle > 0)
                        {
                            row.BusinessTitle = lstDesignation.FirstOrDefault(x => x.Id == com.BusinessTitle) != null ? lstDesignation.FirstOrDefault(x => x.Id == com.BusinessTitle).Designation : "";
                        }
                        if (com.Company > 0)
                        {
                            row.Company = LstCompany.FirstOrDefault(x => x.Id == com.Company) != null ? LstCompany.FirstOrDefault(x => x.Id == com.Company).EntityName : "";
                        }
                        row.Id = com.Id;
                        row.EmployeeName = com.EmployeeName;
                        row.Status = com.Status;
                        row.TimeType = com.TimeType;
                        row.EntityState = com.EntityState;
                        row.Manager = com.Manager;
                        row.UpdatedDate = com.UpdatedDate; row.CreatedDate = com.CreatedDate;
                        row.CreatedByName = com.CreatedByName;
                        row.UpdatedByName = com.UpdatedByName;
                        output.Add(row);
                    }
                }
                #endregion

                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;

                return output;
            }
        }


        [HttpGet]
        public ActionResult GetUsers(string range, int currentPage)
        {
            ViewBag.lstUser = JsonConvert.SerializeObject(GetAll(currentPage, range));
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("User", null);
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
                    retval = new UserMastersRepository().Delete(Convert.ToInt32(id));
                    return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "User"), null);
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
        public ActionResult RemoveProfile(string fileName)
        {
            try
            {
                new UserMastersRepository().RemoveProfilePic(fileName);
                return new ReplyFormat().Success(Messages.DELETE_MESSAGE_FILE);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpPost]
        public ActionResult AddOrUpdateUser(UserMasterVM UserVM)
        {
            try
            {
                var User = new UserMastersRepository().AddOrUpdate(UserVM);
                return new ReplyFormat().Success(Messages.SUCCESS, User);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            UserMasterVM UserVM = new UserMasterVM();
            ViewBag.PageType = "Create";
            try
            {
                ViewBag.lstDesignation = new DesignationMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.Designation, Value = d.Id.ToString() }).ToList();
                ViewBag.lstEntity = new EntityMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EntityName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLOS = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLocation = new LocationMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LocationName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstRegion = new RegionMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.Region, Value = d.Id.ToString() }).ToList();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View("ManageUserMaster", UserVM);

        }

        public ActionResult Edit(String id, bool isView)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);

                    ViewBag.lstDesignation = new DesignationMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.Designation, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstEntity = new EntityMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EntityName, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstSBU = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstLOS = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstLocation = new LocationMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LocationName, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstRegion = new RegionMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.Region, Value = d.Id.ToString() }).ToList();
                    ViewBag.ViewState = isView;

                    UserMasterVM UserVM = new UserMastersRepository().Get(Convert.ToInt32(id));
                    if (!string.IsNullOrEmpty(UserVM.ImagePath))
                    {
                        if (!new Common().GetFilePathExist(UserVM.ImagePath))
                        {
                            UserVM.ImagePath = string.Empty;
                        }
                    }

                    ViewBag.PageType = !isView ? "Edit" : "View";
                    return View("ManageUserMaster", UserVM);
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();

        }
        [HttpPost]
        public ActionResult IsExist(UserMasterVM UserVM)
        {
            try
            {
                var data = new UserMastersRepository().IsExist(UserVM);
                return new ReplyFormat().Success(data);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpPost]
        public ActionResult ImportUsers(string file)
        {
            try
            {
                string retval = new UserMastersRepository().UploadImportUsers(file);
                if (!string.IsNullOrEmpty(retval))
                {
                    int count = new UserMastersRepository().ImportUsers(retval);
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

        [HttpPost]
        public ActionResult GetLOSSettings(UserMasterVM UserVM)
        {
            try
            {
                var data = new LOSMasterRepository().Get(UserVM.LOSId);
                return new ReplyFormat().Success(Messages.SUCCESS, data);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpPost]
        public ActionResult AutoCompeleteManager(string managervalue)
       {
            try
            {
                var data = new UserMastersRepository().ManagerAutoCompelete(managervalue);
                return new ReplyFormat().Success(Messages.SUCCESS, data);
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


                var ws = package.Workbook.Worksheets.Add(Messages.UserMaster);
                //Headers
                ws.Cells["A1"].Value = Messages.EmployeeName;
                ws.Cells["B1"].Value = Messages.CreatedDate;
                ws.Cells["C1"].Value = Messages.Status;
                ws.Cells["D1"].Value = Messages.TimeType;
                ws.Cells["E1"].Value = Messages.BusinessTitle;
                ws.Cells["F1"].Value = Messages.Company;
                ws.Cells["G1"].Value = Messages.LOS;
                ws.Cells["H1"].Value = Messages.SBU;
                ws.Cells["I1"].Value = Messages.SubSBU;
                ws.Cells["J1"].Value = Messages.Competency;
                ws.Cells["K1"].Value = Messages.Region;
                ws.Cells["L1"].Value = Messages.Manager;
                ws.Cells["M1"].Value = Messages.CreatedBy;
                ws.Cells["N1"].Value = Messages.ModifiedDate;
                ws.Cells["O1"].Value = Messages.ModifiedBy;

                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.EmployeeName;

                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.CreatedDate;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.Status;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.TimeType;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.BusinessTitle;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.Company;

                ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                ws.Cells[rowNumber, 7].Value = Messages.LOS;

                ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                ws.Cells[rowNumber, 8].Value = Messages.SBU;

                ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                ws.Cells[rowNumber, 9].Value = Messages.SubSBU;

                ws.Cells[rowNumber, 10].Style.Font.Bold = true;
                ws.Cells[rowNumber, 10].Value = Messages.Competency;

                ws.Cells[rowNumber, 11].Style.Font.Bold = true;
                ws.Cells[rowNumber, 11].Value = Messages.Region;

                ws.Cells[rowNumber, 12].Style.Font.Bold = true;
                ws.Cells[rowNumber, 12].Value = Messages.Manager;


                ws.Cells[rowNumber, 13].Style.Font.Bold = true;
                ws.Cells[rowNumber, 13].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 14].Style.Font.Bold = true;
                ws.Cells[rowNumber, 14].Value = Messages.ModifiedDate;

                ws.Cells[rowNumber, 15].Style.Font.Bold = true;
                ws.Cells[rowNumber, 15].Value = Messages.ModifiedBy;

                foreach (var log in GetAll(0))
                {
                    rowNumber++;

                    ws.Cells[rowNumber, 1].Value = log.EmployeeName;
                    ws.Cells[rowNumber, 2].Value = log.CreatedDate != null ? log.CreatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 3].Value = log.Status ? Messages.Active : Messages.Inactive;
                    ws.Cells[rowNumber, 4].Value = log.TimeType;
                    ws.Cells[rowNumber, 5].Value = log.BusinessTitle;
                    ws.Cells[rowNumber, 6].Value = log.Company;
                    ws.Cells[rowNumber,7].Value = log.LOS;
                    ws.Cells[rowNumber, 8].Value = log.SBU;
                    ws.Cells[rowNumber, 9].Value = log.SubSBU;
                    ws.Cells[rowNumber, 10].Value = log.Competency;
                    ws.Cells[rowNumber, 11].Value = log.Region;
                    ws.Cells[rowNumber, 12].Value = log.Manager;
                    ws.Cells[rowNumber, 13].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 14].Value = log.UpdatedDate != null ? log.UpdatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 15].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;

                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.UserMaster + Messages.XLSX;
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


                var ws = package.Workbook.Worksheets.Add(Messages.UserMasterHistory);
                //Headers
                ws.Cells["A1"].Value = Messages.EmployeeName;
                ws.Cells["B1"].Value = Messages.CreatedDate;
                ws.Cells["C1"].Value = Messages.Status;
                ws.Cells["D1"].Value = Messages.TimeType;
                ws.Cells["E1"].Value = Messages.BusinessTitle;
                ws.Cells["F1"].Value = Messages.Company;
                ws.Cells["G1"].Value = Messages.LOS;
                ws.Cells["H1"].Value = Messages.SBU;
                ws.Cells["I1"].Value = Messages.SubSBU;
                ws.Cells["J1"].Value = Messages.Competency;
                ws.Cells["K1"].Value = Messages.Region;
                ws.Cells["L1"].Value = Messages.Manager;
                ws.Cells["M1"].Value = Messages.CreatedBy;
                ws.Cells["N1"].Value = Messages.ModifiedDate;
                ws.Cells["O1"].Value = Messages.ModifiedBy;
                ws.Cells["P1"].Value = Messages.EntityState;


                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.EmployeeName;

                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.CreatedDate;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.Status;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.TimeType;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.BusinessTitle;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.Company;

                ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                ws.Cells[rowNumber, 7].Value = Messages.LOS;

                ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                ws.Cells[rowNumber, 8].Value = Messages.SBU;

                ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                ws.Cells[rowNumber, 9].Value = Messages.SubSBU;

                ws.Cells[rowNumber, 10].Style.Font.Bold = true;
                ws.Cells[rowNumber, 10].Value = Messages.Competency;

                ws.Cells[rowNumber, 11].Style.Font.Bold = true;
                ws.Cells[rowNumber, 11].Value = Messages.Region;

                ws.Cells[rowNumber, 12].Style.Font.Bold = true;
                ws.Cells[rowNumber, 12].Value = Messages.Manager;


                ws.Cells[rowNumber, 13].Style.Font.Bold = true;
                ws.Cells[rowNumber, 13].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 14].Style.Font.Bold = true;
                ws.Cells[rowNumber, 14].Value = Messages.ModifiedDate;

                ws.Cells[rowNumber, 15].Style.Font.Bold = true;
                ws.Cells[rowNumber, 15].Value = Messages.ModifiedBy;


                ws.Cells[rowNumber, 16].Style.Font.Bold = true;
                ws.Cells[rowNumber, 16].Value = Messages.EntityState;
                var lst = GetAllHistory(0, id);
                foreach (var log in lst)
                {
                    rowNumber++;


                    ws.Cells[rowNumber, 1].Value = log.EmployeeName;
                    ws.Cells[rowNumber, 2].Value = log.CreatedDate != null ? log.CreatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 3].Value = log.Status ? Messages.Active : Messages.Inactive;
                    ws.Cells[rowNumber, 4].Value = log.TimeType;
                    ws.Cells[rowNumber, 5].Value = log.BusinessTitle;
                    ws.Cells[rowNumber, 6].Value = log.Company;
                    ws.Cells[rowNumber, 7].Value = log.LOS;
                    ws.Cells[rowNumber, 8].Value = log.SBU;
                    ws.Cells[rowNumber, 9].Value = log.SubSBU;
                    ws.Cells[rowNumber, 10].Value = log.Competency;
                    ws.Cells[rowNumber, 11].Value = log.Region;
                    ws.Cells[rowNumber, 12].Value = log.Manager;
                    ws.Cells[rowNumber, 13].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 14].Value = log.UpdatedDate != null ? log.UpdatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 15].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;
                    ws.Cells[rowNumber, 16].Value = log.EntityState;
                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.UserMasterHistory + Messages.XLSX;
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