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
        public ActionResult HistoryIndex(String id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);

                var lst = GetAllHistory(1, Convert.ToInt32(id));
                ViewBag.name = id.ToString();
                ViewBag.lstRoleHistory = JsonConvert.SerializeObject(lst);
                var DataTableDetail = new HomeController().getDataTableDetail("Role", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
                return View();
            }
            else
            {
                return View("Index");
            }

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
                        else
                        {
                            row.SubSBU = Messages.NotAvailable;
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
                                if (lstSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SBUId)).FirstOrDefault() != null)
                                {
                                    row.SBU = lstSBUMaster.Where(x => x.Id == Convert.ToInt32(Rol.SBUId)).FirstOrDefault().SBU;
                                }
                                else
                                {
                                    row.SBU = Messages.NotAvailable;
                                }
                            }
                        }
                        else
                        {
                            row.SBU = Messages.NotAvailable;
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
                        else
                        {
                            row.CompetencyName = Messages.NotAvailable;
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
                        else
                        {
                            row.LOS = Messages.NotAvailable;
                        }
                        if (Rol.UserId > 0)
                        {
                            row.UserName = lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId) != null ? lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId).EmployeeName : "";
                        }
                        else
                        {
                            row.UserName = Messages.NotAvailable;
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
                        else
                        {
                            row.SubSBU = Messages.NotAvailable;
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
                                else
                                {
                                    row.SBU = Messages.NotAvailable;
                                }
                            }
                        }
                        else
                        {
                            row.SBU = Messages.NotAvailable;
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
                                else
                                {
                                    row.CompetencyName = Messages.NotAvailable;
                                }
                            }
                        }
                        else
                        {
                            row.CompetencyName = Messages.NotAvailable;
                        }
                        if (!string.IsNullOrEmpty(Rol.LOSId))
                        {

                            if (Rol.LOSId.Contains(","))
                            {
                                string[] array = Rol.LOSId.Split(',');
                                List<string> LOSIdLst = new List<string>();
                                foreach (string LOSItem in array)
                                {
                                    var LOSDto = lstLOS.Where(x => x.Id == Convert.ToInt32(LOSItem)).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(LOSItem) && LOSDto != null)
                                    {
                                        LOSIdLst.Add(LOSDto.LOSName);
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
                        else
                        {
                            row.LOS = Messages.NotAvailable;
                        }
                        if (Rol.UserId > 0)
                        {
                            row.UserName = lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId) != null ? lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId).EmployeeName : "";
                        }
                        else
                        {
                            row.UserName = Messages.NotAvailable;
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
                        else
                        {
                            row.SubSBU = Messages.NotAvailable;
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
                        else
                        {
                            row.SBU = Messages.NotAvailable;
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
                        else
                        {
                            row.CompetencyName = Messages.NotAvailable;
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
                        else
                        {
                            row.LOS = Messages.NotAvailable;
                        }
                        if (Rol.UserId > 0)
                        {
                            row.UserName = lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId) != null ? lstUserMaster.FirstOrDefault(x => x.Id == Rol.UserId).EmployeeName : "";
                        }
                        else
                        {
                            row.UserName = Messages.NotAvailable;
                        }
                        row.Id = Rol.Id;

                        row.Status = Rol.Status;
                        row.UpdatedByName = Rol.UpdatedByName;
                        row.CreatedByName = Rol.CreatedByName;
                        row.ModifiedBy = Rol.ModifiedBy;
                        row.CreatedBy = Rol.CreatedBy;
                        row.EntityState = Rol.EntityState;
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
                        row.EntityState = Rol.EntityState;
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
        public ActionResult Delete(String id)
        {
            bool retval = true;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);
                    retval = new RoleMasterRepoitory().Delete(Convert.ToInt32(id));
                    return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Role"), null);
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

        public ActionResult Edit(String id, bool isView)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);
                    RoleMasterVM RoleVM = new RoleMasterRepoitory().Get(Convert.ToInt32(id));
                    ViewBag.ViewState = isView;
                    ViewBag.PageType = !isView ? "Edit" : "View";

                    ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstSBU = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstLOS = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();

                    return View("ManageRoleMaster", RoleVM);
                }
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

        public ActionResult ExportData()
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage();


                var ws = package.Workbook.Worksheets.Add(Messages.Role);
                //Headers
                ws.Cells["A1"].Value = Messages.User;
                ws.Cells["B1"].Value = Messages.SBU;
                ws.Cells["C1"].Value = Messages.SubSBU;
                ws.Cells["D1"].Value = Messages.Competency;
                ws.Cells["E1"].Value = Messages.CreatedDate;
                ws.Cells["F1"].Value = Messages.CreatedBy;
                ws.Cells["G1"].Value = Messages.ModifiedDate;
                ws.Cells["H1"].Value = Messages.ModifiedBy;
                ws.Cells["I1"].Value = Messages.Status;


                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.User;

                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.SBU;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.SubSBU;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.Competency;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.CreatedDate;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                ws.Cells[rowNumber, 7].Value = Messages.ModifiedDate;

                ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                ws.Cells[rowNumber, 8].Value = Messages.ModifiedBy;

                ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                ws.Cells[rowNumber, 9].Value = Messages.Status;
                foreach (var log in GetAll(0))
                {
                    rowNumber++;

                    ws.Cells[rowNumber, 1].Value = log.UserName;
                    ws.Cells[rowNumber, 2].Value = log.SBU;
                    ws.Cells[rowNumber, 3].Value = log.SubSBU;
                    ws.Cells[rowNumber, 4].Value = log.CompetencyName;
                    ws.Cells[rowNumber, 5].Value = log.CreatedDate != null ? log.CreatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 6].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 7].Value = log.UpdatedDate != null ? log.UpdatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 8].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;
                    ws.Cells[rowNumber, 9].Value = log.Status ? Messages.Active : Messages.Inactive;

                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.Role + Messages.XLSX;
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


                var ws = package.Workbook.Worksheets.Add(Messages.RoleHistory);
                //Headers
                ws.Cells["A1"].Value = Messages.User;
                ws.Cells["B1"].Value = Messages.SBU;
                ws.Cells["C1"].Value = Messages.SubSBU;
                ws.Cells["D1"].Value = Messages.Competency;
                ws.Cells["E1"].Value = Messages.CreatedDate;
                ws.Cells["F1"].Value = Messages.CreatedBy;
                ws.Cells["G1"].Value = Messages.ModifiedDate;
                ws.Cells["H1"].Value = Messages.ModifiedBy;
                ws.Cells["I1"].Value = Messages.Status;
                ws.Cells["J1"].Value = Messages.EntityState;


                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.User;

                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.SBU;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.SubSBU;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.Competency;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.CreatedDate;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                ws.Cells[rowNumber, 7].Value = Messages.ModifiedDate;

                ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                ws.Cells[rowNumber, 8].Value = Messages.ModifiedBy;

                ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                ws.Cells[rowNumber, 9].Value = Messages.Status;

                ws.Cells[rowNumber, 10].Style.Font.Bold = true;
                ws.Cells[rowNumber, 10].Value = Messages.EntityState;
                var lst = GetAllHistory(0, id);
                foreach (var log in lst)
                {
                    rowNumber++;

                    ws.Cells[rowNumber, 1].Value = log.UserName;
                    ws.Cells[rowNumber, 2].Value = log.SBU;
                    ws.Cells[rowNumber, 3].Value = log.SubSBU;
                    ws.Cells[rowNumber, 4].Value = log.CompetencyName;
                    ws.Cells[rowNumber, 5].Value = log.CreatedDate != null ? log.CreatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 6].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 7].Value = log.UpdatedDate != null ? log.UpdatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 8].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;
                    ws.Cells[rowNumber, 9].Value = log.Status ? Messages.Active : Messages.Inactive;
                    ws.Cells[rowNumber, 10].Value = log.EntityState;
                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.RoleHistory + Messages.XLSX;
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