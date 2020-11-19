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
    public class RoleController : Controller
    {
        // GET: Role
        public ActionResult Index()
        {
            ViewBag.lstRole = JsonConvert.SerializeObject(GetAll(1));
            var DataTableDetail = new HomeController().getDataTableDetail("Role", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
           
        }
        public ActionResult LoadRole(int currentPageIndex, string range = "")
        {
            ViewBag.lstRole = JsonConvert.SerializeObject(GetAll(currentPageIndex, range));
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        public dynamic GetAll(int currentPage, string range = "")
        {
            int maxRows = 2; int lstCount = 0;
            var lst = new RoleMasterRepoitory().GetAll();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Role in lst
                       where Role.CreatedDate >= fromDate && Role.CreatedDate <= toDate
                       select Role).ToList();
                lstCount = lst.Count;

                lst = (lst)
                        .OrderBy(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                
                dynamic output = new List<dynamic>();

               
                var lstUsers = new UserMastersRepository().GetAll();
                List<LOSMasterVM> lstLOS = new LOSMasterRepository().GetAll();
                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll();
                List<UserMasterVM> lstUserMaster = new UserMastersRepository().GetAll();

               
                if (lst != null && lst.Count > 0)
                {

                    foreach (RoleMasterVM Rol in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (!string.IsNullOrEmpty(Rol.SubSBUId))
                        {
                            if (Rol.SubSBUId.Contains(","))
                            {
                                string[] array = Rol.SubSBUId.Split(',');
                                List<string> subsbus = new List<string>();
                                foreach (string SubSBUIdItem in array)
                                {
                                    subsbus.Add(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault().SubSBU);
                                }
                                row.SubSBU = string.Join(",", subsbus);
                            }
                            else
                            {
                                row.SubSBU = lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SubSBUId)).FirstOrDefault().SubSBU;
                            }
                        }

                        if (!string.IsNullOrEmpty(Rol.SBUId))
                        {
                            if (Rol.SBUId.Contains(","))
                            {
                                string[] array = Rol.SBUId.Split(',');
                                List<string> sbus = new List<string>();
                                foreach (string SBUIdItem in array)
                                {
                                    sbus.Add(lstSBUMaster.Where(x => x.Id == Convert.ToInt32(SBUIdItem)).FirstOrDefault().SBU);
                                }
                                row.SBU = string.Join(",", sbus);
                            }
                            else
                            {
                                row.SBU = lstSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SBUId)).FirstOrDefault().SBU;
                            }
                        }

                        if (!string.IsNullOrEmpty(Rol.CompetencyId))
                        {
                            if (Rol.CompetencyId.Contains(","))
                            {
                                string[] array = Rol.CompetencyId.Split(',');
                                List<string> CompetencyLst = new List<string>();
                                foreach (string CompetencyItem in array)
                                {
                                    CompetencyLst.Add(lstCompetency.Where(x => x.Id == Convert.ToInt32(CompetencyItem)).FirstOrDefault().CompetencyName);
                                }
                                row.CompetencyName = string.Join(",", CompetencyLst);
                            }
                            else
                            {
                                row.CompetencyName = lstCompetency.Where(x => x.Id == Convert.ToInt32(Rol.CompetencyId)).FirstOrDefault().CompetencyName;
                            }
                        }

                        if (!string.IsNullOrEmpty(Rol.LOSId))
                        {

                            if (Rol.LOSId.Contains(","))
                            {
                                string[] array = Rol.LOSId.Split(',');
                                List<string> LOSIdLst = new List<string>();
                                foreach (string LOSItem in array)
                                {
                                    LOSIdLst.Add(lstLOS.Where(x => x.Id == Convert.ToInt32(LOSItem)).FirstOrDefault().LOSName);
                                }
                                row.LOS = string.Join(",", LOSIdLst);
                            }
                            else
                            {
                                row.LOS = lstLOS.Where(x => x.Id == Convert.ToInt32(Rol.LOSId)).FirstOrDefault().LOSName;
                            }
                        }

                        if (Rol.UserId > 0)
                        {
                            row.UserName = lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId) != null ? lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId).EmployeeName : "";
                        }
                        row.Id = Rol.Id;

                        row.Status = Rol.Status;

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

                lst = (from Role in lst
                       select Role)
           .OrderBy(Role => Role.Id)
           .Skip((currentPage - 1) * maxRows)
           .Take(maxRows).ToList();
                
                var lstUsers = new UserMastersRepository().GetAll();
                
                List<LOSMasterVM> lstLOS = new LOSMasterRepository().GetAll();
                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll();
                List<UserMasterVM> lstUserMaster = new UserMastersRepository().GetAll();


                if (lst != null && lst.Count > 0)
                {

                    foreach (RoleMasterVM Rol in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (!string.IsNullOrEmpty(Rol.SubSBUId))
                        {
                            if (Rol.SubSBUId.Contains(","))
                            {
                                string[] array = Rol.SubSBUId.Split(',');
                                List<string> subsbus = new List<string>();
                                foreach (string SubSBUIdItem in array)
                                {
                                    subsbus.Add(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault().SubSBU);
                                }
                                row.SubSBU = string.Join(",", subsbus);
                            }
                            else
                            {
                                row.SubSBU = lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SubSBUId)).FirstOrDefault().SubSBU;
                            }
                        }

                        if (!string.IsNullOrEmpty(Rol.SBUId))
                        {
                            if (Rol.SBUId.Contains(","))
                            {
                                string[] array = Rol.SBUId.Split(',');
                                List<string> sbus = new List<string>();
                                foreach (string SBUIdItem in array)
                                {
                                    sbus.Add(lstSBUMaster.Where(x => x.Id == Convert.ToInt32(SBUIdItem)).FirstOrDefault().SBU);
                                }
                                row.SBU = string.Join(",", sbus);
                            }
                            else
                            {
                                row.SBU = lstSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SBUId)).FirstOrDefault().SBU;
                            }
                        }

                        if (!string.IsNullOrEmpty(Rol.CompetencyId))
                        {
                            if (Rol.CompetencyId.Contains(","))
                            {
                                string[] array = Rol.CompetencyId.Split(',');
                                List<string> CompetencyLst = new List<string>();
                                foreach (string CompetencyItem in array)
                                {
                                    CompetencyLst.Add(lstCompetency.Where(x => x.Id == Convert.ToInt32(CompetencyItem)).FirstOrDefault().CompetencyName);
                                }
                                row.CompetencyName = string.Join(",", CompetencyLst);
                            }
                            else
                            {
                                row.CompetencyName = lstCompetency.Where(x => x.Id == Convert.ToInt32(Rol.CompetencyId)).FirstOrDefault().CompetencyName;
                            }
                        }

                        if (!string.IsNullOrEmpty(Rol.LOSId))
                        {

                            if (Rol.LOSId.Contains(","))
                            {
                                string[] array = Rol.LOSId.Split(',');
                                List<string> LOSIdLst = new List<string>();
                                foreach (string LOSItem in array)
                                {
                                    LOSIdLst.Add(lstLOS.Where(x => x.Id == Convert.ToInt32(LOSItem)).FirstOrDefault().LOSName);
                                }
                                row.LOS = string.Join(",", LOSIdLst);
                            }
                            else
                            {
                                row.LOS = lstLOS.Where(x => x.Id == Convert.ToInt32(Rol.LOSId)).FirstOrDefault().LOSName;
                            }
                        }

                        if (Rol.UserId > 0)
                        {
                            row.UserName = lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId) != null ? lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId).EmployeeName : "";
                        }
                        row.Id = Rol.Id;

                        row.Status = Rol.Status;

                        output.Add(row);
                    }
                }
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;

                return output;

            }
        }

        [HttpGet]
        public ActionResult GetRole(string range, int currentPage)
        {
            ViewBag.lstRole = JsonConvert.SerializeObject(GetAll(currentPage,range));
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("Role", null);
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
                retval = new RoleMasterRepoitory().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Role"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateRole1(RoleMasterVM data)
        {
            try
            {
                var Role = new RoleMasterRepoitory().AddOrUpdate(data);
                return new ReplyFormat().Success(Messages.SUCCESS, Role);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateRole(string roleParams)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<RoleMasterVM>(roleParams);
                var Role = new RoleMasterRepoitory().AddOrUpdate(data);
                return new ReplyFormat().Success(Messages.SUCCESS, Role);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            RoleMasterVM RoleMasterVM = new RoleMasterVM();
            ViewBag.PageType = "Create";
            ViewBag.lstUser = new UserMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSBU = new SBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
            ViewBag.lstLOS = new LOSMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();

            return View("ManageRoleMaster", RoleMasterVM);
           
        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                RoleMasterVM RoleVM = new RoleMasterRepoitory().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";

                ViewBag.lstUser = new UserMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLOS = new LOSMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();

                return View("ManageRoleMaster", RoleVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
            
        }
    }
}