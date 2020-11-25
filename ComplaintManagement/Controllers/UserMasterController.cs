using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
        public ActionResult SearchUser(string search)
        {
            var originalList = GetAll(1);
            dynamic output = new List<dynamic>();

            foreach (var item in originalList)
            {
                var itemIndex = (IDictionary<string, object>)item;
                foreach (var kvp in itemIndex)
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
            ViewBag.lstUser = JsonConvert.SerializeObject(output);
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

        public dynamic GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            var lst = new UserMastersRepository().GetAll().ToList();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from user in lst
                       where user.CreatedDate >= fromDate && user.CreatedDate <= toDate
                       select user).ToList();
                lstCount = lst.Count;
                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();


                dynamic output = new List<dynamic>();

                #region Other logics
                lst = lst.Where(x => x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();
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
        public ActionResult Delete(int id)
        {
            bool retval = true;
            try
            {
                retval = new UserMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "User"), null);
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
                ViewBag.lstDesignation = new DesignationMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.Designation, Value = d.Id.ToString() }).ToList();
                ViewBag.lstEntity = new EntityMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.EntityName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLOS = new LOSMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLocation = new LocationMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LocationName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstRegion = new RegionMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.Region, Value = d.Id.ToString() }).ToList();


            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View("ManageUserMaster", UserVM);

        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {

                ViewBag.lstDesignation = new DesignationMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.Designation, Value = d.Id.ToString() }).ToList();
                ViewBag.lstEntity = new EntityMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.EntityName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLOS = new LOSMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLocation = new LocationMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LocationName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstRegion = new RegionMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.Region, Value = d.Id.ToString() }).ToList();
                ViewBag.ViewState = isView;

                UserMasterVM UserVM = new UserMastersRepository().Get(id);
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
                }
                return new ReplyFormat().Success(retval);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
    }
}