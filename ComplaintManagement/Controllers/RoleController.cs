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
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    [Authorize]
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
        [HttpGet]
        public ActionResult HistoryIndex(int id)
        {
            var lst = GetAllHistory(1, id);
            ViewBag.name = id.ToString();
            ViewBag.lstRole = JsonConvert.SerializeObject(lst);
            var DataTableDetail = new HomeController().getDataTableDetail("Role", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();

        }
        public ActionResult SearchRole(string search)
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
                            if (kvp.Value != null)
                            {
                                output.Add(item);
                            }

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
            ViewBag.lstRole = JsonConvert.SerializeObject(data.Distinct().ToList());
            return View("Index");
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
        public ActionResult LoadHistoryRole(int currentPageIndex, int range = 0)
        {
            ViewBag.lstRoleHistory = JsonConvert.SerializeObject(GetAllHistory(currentPageIndex, range));
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

            var lst = new RoleMasterRepoitory().GetAll();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Role in lst
                       where Role.CreatedDate.Date >= fromDate.Date && Role.CreatedDate.Date <= toDate.Date
                       select Role).ToList();
                lstCount = lst.Count;

                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();


                dynamic output = new List<dynamic>();


                var lstUsers = new UserMastersRepository().GetAll();
                List<LOSMasterVM> lstLOS = new LOSMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll().Where(x => x.Status).ToList();
                List<UserMasterVM> lstUserMaster = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();


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
                        row.UpdatedByName = Rol.UpdatedByName;
                        row.CreatedByName = Rol.CreatedByName;
                        row.ModifiedBy = Rol.ModifiedBy;
                        row.CreatedBy = Rol.CreatedBy;
                        row.UpdatedDate = Rol.UpdatedDate;
                        row.CreatedDate = Rol.CreatedDate;

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
           .OrderByDescending(Role => Role.Id)
           .Skip((currentPage - 1) * maxRows)
           .Take(maxRows).ToList();

                var lstUsers = new UserMastersRepository().GetAll();

                List<LOSMasterVM> lstLOS = new LOSMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll().Where(x => x.Status).ToList();
                List<UserMasterVM> lstUserMaster = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();


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
                                    if(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault()!= null)
                                    {
                                    subsbus.Add(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault().SubSBU);

                                    }
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
                                    if (lstSBUMaster.Where(x => x.Id == Convert.ToInt32(SBUIdItem)).FirstOrDefault() != null)
                                    {
                                        sbus.Add(lstSBUMaster.Where(x => x.Id == Convert.ToInt32(SBUIdItem)).FirstOrDefault().SBU);
                                    }
                                }
                                row.SBU = string.Join(",", sbus);
                            }
                            else
                            {
                                if (lstSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SBUId)).FirstOrDefault() != null)
                                {
                                    row.SBU = lstSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SBUId)).FirstOrDefault().SBU;

                                }

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
                                    if (lstCompetency.Where(x => x.Id == Convert.ToInt32(CompetencyItem)).FirstOrDefault() != null)
                                    {
                                        CompetencyLst.Add(lstCompetency.Where(x => x.Id == Convert.ToInt32(CompetencyItem)).FirstOrDefault().CompetencyName);
                                    }
                                }
                                row.CompetencyName = string.Join(",", CompetencyLst);
                            }
                            else
                            {
                                if(lstCompetency.Where(x => x.Id == Convert.ToInt32(Rol.CompetencyId)).FirstOrDefault()!= null)
                                {
                                  row.CompetencyName = lstCompetency.Where(x => x.Id == Convert.ToInt32(Rol.CompetencyId)).FirstOrDefault().CompetencyName;

                                }
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
                                    if (!string.IsNullOrEmpty(LOSItem))
                                    {
                                        LOSIdLst.Add(lstLOS.Where(x => x.Id == Convert.ToInt32(LOSItem)).FirstOrDefault().LOSName);
                                    }
                                }
                                if (LOSIdLst.Count > 0)
                                {
                                    row.LOS = string.Join(",", LOSIdLst);
                                }
                            }
                            else
                            {
                                row.LOS = lstLOS.Where(x => x.Id == Convert.ToInt32(Rol.LOSId)).FirstOrDefault()!=null? lstLOS.Where(x => x.Id == Convert.ToInt32(Rol.LOSId)).FirstOrDefault().LOSName:String.Empty;
                            }
                        }

                        if (Rol.UserId > 0)
                        {
                            row.UserName = lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId) != null ? lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId).EmployeeName : "";
                        }
                        row.Id = Rol.Id;
                        row.UpdatedByName = Rol.UpdatedByName;
                        row.CreatedByName = Rol.CreatedByName;
                        row.ModifiedBy = Rol.ModifiedBy;
                        row.CreatedBy = Rol.CreatedBy;
                        row.UpdatedDate = Rol.UpdatedDate;
                        row.CreatedDate = Rol.CreatedDate;
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
        public dynamic GetAllHistory(int currentPage, int range = 0)
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var lst = new RoleMasterRepoitory().GetAllHistory();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range.ToString()))
            {
                //string[] dates = range.Split(',');
                //DateTime fromDate = Convert.ToDateTime(dates[0]);
                //DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Role in lst
                       where Role.RoleId == range
                       select Role).ToList();
                lstCount = lst.Count;

                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();


                dynamic output = new List<dynamic>();


                var lstUsers = new UserMastersRepository().GetAll();
                List<LOSMasterVM> lstLOS = new LOSMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll().Where(x => x.Status).ToList();
                List<UserMasterVM> lstUserMaster = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();


                if (lst != null && lst.Count > 0)
                {

                    foreach (RoleMasterHistoryVM Rol in lst)
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
                                    if(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault() != null)
                                    {
                                    subsbus.Add(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault().SubSBU);

                                    }
                                }
                                row.SubSBU = string.Join(",", subsbus);
                            }
                            else
                            {
                                if(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SubSBUId)).FirstOrDefault()!= null)
                                {
                                    row.SubSBU = lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SubSBUId)).FirstOrDefault().SubSBU;

                                }
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
                                    if(lstSBUMaster.Where(x => x.Id == Convert.ToInt32(SBUIdItem)).FirstOrDefault()!=null)
                                    {
                                        sbus.Add(lstSBUMaster.Where(x => x.Id == Convert.ToInt32(SBUIdItem)).FirstOrDefault().SBU);

                                    }
                                }
                                row.SBU = string.Join(",", sbus);
                            }
                            else
                            {
                                if(lstSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SBUId)).FirstOrDefault()!= null)
                                {
                                row.SBU = lstSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SBUId)).FirstOrDefault().SBU;

                                }
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
                        row.UpdatedByName = Rol.UpdatedByName;
                        row.CreatedByName = Rol.CreatedByName;
                        row.ModifiedBy = Rol.ModifiedBy;
                        row.CreatedBy = Rol.CreatedBy;
                        row.UpdatedDate = Rol.UpdatedDate;
                        row.CreatedDate = Rol.CreatedDate;

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
           .OrderByDescending(Role => Role.Id)
           .Skip((currentPage - 1) * maxRows)
           .Take(maxRows).ToList();

                var lstUsers = new UserMastersRepository().GetAll();

                List<LOSMasterVM> lstLOS = new LOSMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll().Where(x => x.Status).ToList();
                List<UserMasterVM> lstUserMaster = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();


                if (lst != null && lst.Count > 0)
                {

                    foreach (RoleMasterHistoryVM Rol in lst)
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
                                    if (lstSBUMaster.Where(x => x.Id == Convert.ToInt32(SBUIdItem)).FirstOrDefault() != null)
                                    {
                                        sbus.Add(lstSBUMaster.Where(x => x.Id == Convert.ToInt32(SBUIdItem)).FirstOrDefault().SBU);
                                    }
                                }
                                row.SBU = string.Join(",", sbus);
                            }
                            else
                            {
                                if (lstSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SBUId)).FirstOrDefault() != null)
                                {
                                    row.SBU = lstSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SBUId)).FirstOrDefault().SBU;

                                }

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
                                    if (lstCompetency.Where(x => x.Id == Convert.ToInt32(CompetencyItem)).FirstOrDefault() != null)
                                    {
                                        CompetencyLst.Add(lstCompetency.Where(x => x.Id == Convert.ToInt32(CompetencyItem)).FirstOrDefault().CompetencyName);
                                    }
                                }
                                row.CompetencyName = string.Join(",", CompetencyLst);
                            }
                            else
                            {
                                if (lstCompetency.Where(x => x.Id == Convert.ToInt32(Rol.CompetencyId)).FirstOrDefault() != null)
                                {
                                    row.CompetencyName = lstCompetency.Where(x => x.Id == Convert.ToInt32(Rol.CompetencyId)).FirstOrDefault().CompetencyName;

                                }
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
                                    if (!string.IsNullOrEmpty(LOSItem))
                                    {
                                        LOSIdLst.Add(lstLOS.Where(x => x.Id == Convert.ToInt32(LOSItem)).FirstOrDefault().LOSName);
                                    }
                                }
                                if (LOSIdLst.Count > 0)
                                {
                                    row.LOS = string.Join(",", LOSIdLst);
                                }
                            }
                            else
                            {
                                row.LOS = lstLOS.Where(x => x.Id == Convert.ToInt32(Rol.LOSId)).FirstOrDefault() != null ? lstLOS.Where(x => x.Id == Convert.ToInt32(Rol.LOSId)).FirstOrDefault().LOSName : String.Empty;
                            }
                        }

                        if (Rol.UserId > 0)
                        {
                            row.UserName = lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId) != null ? lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId).EmployeeName : "";
                        }
                        row.Id = Rol.Id;
                        row.UpdatedByName = Rol.UpdatedByName;
                        row.CreatedByName = Rol.CreatedByName;
                        row.ModifiedBy = Rol.ModifiedBy;
                        row.CreatedBy = Rol.CreatedBy;
                        row.UpdatedDate = Rol.UpdatedDate;
                        row.CreatedDate = Rol.CreatedDate;
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
            ViewBag.lstRole = JsonConvert.SerializeObject(GetAll(currentPage, range));
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
            ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSBU = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
            ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
            ViewBag.lstLOS = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();

            return View("ManageRoleMaster", RoleMasterVM);

        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                RoleMasterVM RoleVM = new RoleMasterRepoitory().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";

                ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstLOS = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();

                return View("ManageRoleMaster", RoleVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();

        }

        [HttpPost]
        public ActionResult ImportRole(string file)
        {
            try
            {
                string retval = new RoleMasterRepoitory().UploadImportRole(file);
                if (!string.IsNullOrEmpty(retval))
                {
                    int count = new RoleMasterRepoitory().ImportImportRole(retval);
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