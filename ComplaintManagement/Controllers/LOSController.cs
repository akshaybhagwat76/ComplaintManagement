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
using System.Web;
using System.Web.Mvc;
namespace ComplaintManagement.Controllers
{
    [Authorize]
    public class LOSController : Controller
    {
        // GET: LOS
        public ActionResult Index()
        {
            ViewBag.lstLOS = JsonConvert.SerializeObject(GetAll(1));
            var DataTableDetail = new HomeController().getDataTableDetail("LOS", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult HistoryIndex(String id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);

                var lst = GetAllHistory(1, Convert.ToInt32(id));
                ViewBag.name = id.ToString();
                ViewBag.lstHistoryLOS = JsonConvert.SerializeObject(lst);
                var DataTableDetail = new HomeController().getDataTableDetail("LOS", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
                return View();
            }
            else
            {
                return View("Index");
            }
        }
        public ActionResult SearchLOS(string search)
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
            ViewBag.lstLOS = JsonConvert.SerializeObject(data.Distinct().ToList());
            return View("Index");
        }

        [HttpGet]
        public ActionResult LoadLOS(int currentPageIndex, string range = "")
        {
            ViewBag.lstLOS = JsonConvert.SerializeObject(GetAll(currentPageIndex, range));
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult LoadHistoryLOS(int currentPageIndex, int range = 0)
        {
            ViewBag.lstHistoryLOS = JsonConvert.SerializeObject(GetAllHistory(currentPageIndex, range));
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
           
            var lst = new LOSMasterRepository().GetAll();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from LOS in lst
                       where LOS.CreatedDate.Date >= fromDate.Date && LOS.CreatedDate.Date <= toDate.Date
                       select LOS).ToList();
                lstCount = lst.Count;
                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                dynamic output = new List<dynamic>();
                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll().Where(x=>x.Status).ToList();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll().Where(x => x.Status).ToList();
                List<UserMasterVM> lstuser = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();

                #region Other joining logics
                if (lst != null && lst.Count > 0)
                {

                    foreach (LOSMasterVM los in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (!string.IsNullOrEmpty(los.SubSBUId))
                        {
                            if (los.SubSBUId.Contains(","))
                            {
                                string[] array = los.SubSBUId.Split(',');
                                List<string> subsbus = new List<string>();
                                foreach (string SubSBUIdItem in array)
                                {
                                    subsbus.Add(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault().SubSBU);
                                }
                                row.SubSBU = string.Join(",", subsbus);
                            }
                            else
                            {
                                row.SubSBU = lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SubSBUId)).FirstOrDefault().SubSBU;
                            }
                        }

                        if (!string.IsNullOrEmpty(los.SBUId))
                        {
                            if (los.SBUId.Contains(","))
                            {
                                string[] array = los.SBUId.Split(',');
                                List<string> sbus = new List<string>();
                                foreach (string SBUIdItem in array)
                                {
                                    sbus.Add(lstSBUMaster.Where(x => x.Id == Convert.ToInt32(SBUIdItem)).FirstOrDefault().SBU);
                                }
                                row.SBU = string.Join(",", sbus);
                            }
                            else
                            {
                                row.SBU = lstSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SBUId)).FirstOrDefault().SBU;
                            }
                        }

                        if (!string.IsNullOrEmpty(los.CompetencyId))
                        {
                            if (los.CompetencyId.Contains(","))
                            {
                                string[] array = los.CompetencyId.Split(',');
                                List<string> CompetencyLst = new List<string>();
                                foreach (string CompetencyItem in array)
                                {
                                    CompetencyLst.Add(lstCompetency.Where(x => x.Id == Convert.ToInt32(CompetencyItem)).FirstOrDefault().CompetencyName);
                                }
                                row.CompetencyName = string.Join(",", CompetencyLst);
                            }
                            else
                            {
                                row.CompetencyName = lstCompetency.Where(x => x.Id == Convert.ToInt32(los.CompetencyId)).FirstOrDefault().CompetencyName;
                            }
                        }
                        if (!string.IsNullOrEmpty(los.InvolvedUsersId))
                        {
                            if (los.InvolvedUsersId.Contains(","))
                            {
                                string[] array = los.InvolvedUsersId.Split(',');
                                List<string> UserLst = new List<string>();
                                foreach (string UserLstItem in array)
                                {
                 
                                    {
                                        UserLst.Add(lstuser.Where(x => x.Id == Convert.ToInt32(UserLstItem)).FirstOrDefault().EmployeeName);
                                    }
                                }
                                row.InvolvedUser = string.Join(",", UserLst);
                            }
                            else
                            {
                                row.InvolvedUser = lstuser.Where(x => x.Id == Convert.ToInt32(los.InvolvedUsersId)).FirstOrDefault() != null ? lstuser.Where(x => x.Id == Convert.ToInt32(los.InvolvedUsersId)).FirstOrDefault().EmployeeName : string.Empty;
                            }
                        }
                        else
                        {
                            row.InvolvedUser = "No data Added";
                        }

                        row.Id = los.Id;
                        row.LOSName = los.LOSName;
                        row.UpdatedByName = los.UpdatedByName;
                        row.CreatedByName = los.CreatedByName;
                        row.ModifiedBy = los.ModifiedBy;
                        row.CreatedBy = los.CreatedBy;
                        row.UpdatedDate = los.UpdatedDate;
                        row.CreatedDate = los.CreatedDate;
                        row.Status = los.Status;

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

                lst = (from LOS in lst
                       select LOS)
           .OrderByDescending(LOS => LOS.Id)
           .Skip((currentPage - 1) * maxRows)
           .Take(maxRows).ToList();

                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll().Where(x => x.Status).ToList();
                List<UserMasterVM> lstuser = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();



                #region Other logicals joining
                if (lst != null && lst.Count > 0)
                {

                    foreach (LOSMasterVM los in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (!string.IsNullOrEmpty(los.SubSBUId))
                        {
                            if (los.SubSBUId.Contains(","))
                            {
                                string[] array = los.SubSBUId.Split(',');
                                List<string> subsbus = new List<string>();
                                foreach (string SubSBUIdItem in array)
                                {
                                    if (lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault() != null)
                                    { subsbus.Add(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault().SubSBU); }
                                }
                                row.SubSBU = string.Join(",", subsbus);
                            }
                            else
                            {
                                row.SubSBU = lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SubSBUId)).FirstOrDefault()!=null? lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SubSBUId)).FirstOrDefault().SubSBU:string.Empty;
                            }
                        }

                        if (!string.IsNullOrEmpty(los.SBUId))
                        {
                            if (los.SBUId.Contains(","))
                            {
                                string[] array = los.SBUId.Split(',');
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
                                row.SBU = lstSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SBUId)).FirstOrDefault()!=null ? lstSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SBUId)).FirstOrDefault().SBU:string.Empty;
                            }
                        }

                        if (!string.IsNullOrEmpty(los.CompetencyId))
                        {
                            if (los.CompetencyId.Contains(","))
                            {
                                string[] array = los.CompetencyId.Split(',');
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
                                row.CompetencyName = lstCompetency.Where(x => x.Id == Convert.ToInt32(los.CompetencyId)).FirstOrDefault()!=null? lstCompetency.Where(x => x.Id == Convert.ToInt32(los.CompetencyId)).FirstOrDefault().CompetencyName:string.Empty;
                            }
                        }


                        if (!string.IsNullOrEmpty(los.InvolvedUsersId))
                        {
                            if (los.InvolvedUsersId.Contains(","))
                            {
                                string[] array = los.InvolvedUsersId.Split(',');
                                List<string> UserLst = new List<string>();
                                foreach (string UserLstItem in array)
                                {
                                    if (lstuser.Where(x => x.Id == Convert.ToInt32(UserLstItem)).FirstOrDefault() != null)
                                    {
                                        UserLst.Add(lstuser.Where(x => x.Id == Convert.ToInt32(UserLstItem)).FirstOrDefault().EmployeeName);
                                    }
                                }
                                row.InvolvedUser = string.Join(",", UserLst);
                            }
                            else
                            {
                                row.InvolvedUser = lstuser.Where(x => x.Id == Convert.ToInt32(los.InvolvedUsersId)).FirstOrDefault() != null ? lstuser.Where(x => x.Id == Convert.ToInt32(los.InvolvedUsersId)).FirstOrDefault().EmployeeName : string.Empty;
                            }
                        }
                        else
                        {
                            row.InvolvedUser = "No data Added";
                        }


                        row.Id = los.Id;
                        row.LOSName = los.LOSName;
                        row.UpdatedByName = los.UpdatedByName;
                        row.CreatedByName = los.CreatedByName;
                        row.ModifiedBy = los.ModifiedBy;
                        row.CreatedBy = los.CreatedBy;
                        row.UpdatedDate = los.UpdatedDate;
                        row.CreatedDate = los.CreatedDate;
                        row.Status = los.Status;
      

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

            var lst = new LOSMasterRepository().GetAllHistory();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range.ToString()))
            {
                //string[] dates = range.Split(',');
                //DateTime fromDate = Convert.ToDateTime(dates[0]);
                //DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from LOS in lst
                       where LOS.LOSId == range
                       select LOS).ToList();
                lstCount = lst.Count;
                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                dynamic output = new List<dynamic>();
                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll().Where(x => x.Status).ToList();


                #region Other joining logics
                if (lst != null && lst.Count > 0)
                {

                    foreach (LOSMasterHistoryVM los in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (!string.IsNullOrEmpty(los.SubSBUId))
                        {
                            if (los.SubSBUId.Contains(","))
                            {
                                string[] array = los.SubSBUId.Split(',');
                                List<string> subsbus = new List<string>();
                                foreach (string SubSBUIdItem in array)
                                {
                                    subsbus.Add(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault().SubSBU);
                                }
                                row.SubSBU = string.Join(",", subsbus);
                            }
                            else
                            {
                                row.SubSBU = lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SubSBUId)).FirstOrDefault().SubSBU;
                            }
                        }

                        if (!string.IsNullOrEmpty(los.SBUId))
                        {
                            if (los.SBUId.Contains(","))
                            {
                                string[] array = los.SBUId.Split(',');
                                List<string> sbus = new List<string>();
                                foreach (string SBUIdItem in array)
                                {
                                    sbus.Add(lstSBUMaster.Where(x => x.Id == Convert.ToInt32(SBUIdItem)).FirstOrDefault().SBU);
                                }
                                row.SBU = string.Join(",", sbus);
                            }
                            else
                            {
                                row.SBU = lstSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SBUId)).FirstOrDefault().SBU;
                            }
                        }

                        if (!string.IsNullOrEmpty(los.CompetencyId))
                        {
                            if (los.CompetencyId.Contains(","))
                            {
                                string[] array = los.CompetencyId.Split(',');
                                List<string> CompetencyLst = new List<string>();
                                foreach (string CompetencyItem in array)
                                {
                                    CompetencyLst.Add(lstCompetency.Where(x => x.Id == Convert.ToInt32(CompetencyItem)).FirstOrDefault().CompetencyName);
                                }
                                row.CompetencyName = string.Join(",", CompetencyLst);
                            }
                            else
                            {
                                row.CompetencyName = lstCompetency.Where(x => x.Id == Convert.ToInt32(los.CompetencyId)).FirstOrDefault().CompetencyName;
                            }
                        }
                        row.Id = los.Id;
                        row.LOSName = los.LOSName;
                        row.UpdatedByName = los.UpdatedByName;
                        row.CreatedByName = los.CreatedByName;
                        row.ModifiedBy = los.ModifiedBy;
                        row.CreatedBy = los.CreatedBy;
                        row.UpdatedDate = los.UpdatedDate;
                        row.CreatedDate = los.CreatedDate;
                        row.Status = los.Status;
                        row.EntityState = los.EntityState;

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

                lst = (from LOS in lst
                       select LOS)
           .OrderByDescending(LOS => LOS.Id)
           .Skip((currentPage - 1) * maxRows)
           .Take(maxRows).ToList();

                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll().Where(x => x.Status).ToList();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll().Where(x => x.Status).ToList();



                #region Other logicals joining
                if (lst != null && lst.Count > 0)
                {

                    foreach (LOSMasterHistoryVM los in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (!string.IsNullOrEmpty(los.SubSBUId))
                        {
                            if (los.SubSBUId.Contains(","))
                            {
                                string[] array = los.SubSBUId.Split(',');
                                List<string> subsbus = new List<string>();
                                foreach (string SubSBUIdItem in array)
                                {
                                    if (lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault() != null)
                                    { subsbus.Add(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault().SubSBU); }
                                }
                                row.SubSBU = string.Join(",", subsbus);
                            }
                            else
                            {
                                row.SubSBU = lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SubSBUId)).FirstOrDefault() != null ? lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SubSBUId)).FirstOrDefault().SubSBU : string.Empty;
                            }
                        }

                        if (!string.IsNullOrEmpty(los.SBUId))
                        {
                            if (los.SBUId.Contains(","))
                            {
                                string[] array = los.SBUId.Split(',');
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
                                row.SBU = lstSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SBUId)).FirstOrDefault() != null ? lstSBUMaster.Where(x => x.Id == Convert.ToInt32(los.SBUId)).FirstOrDefault().SBU : string.Empty;
                            }
                        }

                        if (!string.IsNullOrEmpty(los.CompetencyId))
                        {
                            if (los.CompetencyId.Contains(","))
                            {
                                string[] array = los.CompetencyId.Split(',');
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
                                row.CompetencyName = lstCompetency.Where(x => x.Id == Convert.ToInt32(los.CompetencyId)).FirstOrDefault() != null ? lstCompetency.Where(x => x.Id == Convert.ToInt32(los.CompetencyId)).FirstOrDefault().CompetencyName : string.Empty;
                            }
                        }
                        row.Id = los.Id;
                        row.LOSName = los.LOSName;
                        row.UpdatedByName = los.UpdatedByName;
                        row.CreatedByName = los.CreatedByName;
                        row.ModifiedBy = los.ModifiedBy;
                        row.CreatedBy = los.CreatedBy;
                        row.UpdatedDate = los.UpdatedDate;
                        row.CreatedDate = los.CreatedDate;
                        row.Status = los.Status;
                        row.EntityState = los.EntityState;

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
        public ActionResult GetLOS(string range, int currentPage)
        {
            ViewBag.lstLOS = JsonConvert.SerializeObject(GetAll(currentPage, range));
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("Categories", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View("Index");
        }
        public ActionResult Delete(String id)
        {
            bool retval = true;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);
                    retval = new LOSMasterRepository().Delete(Convert.ToInt32(id));
                    return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "LOS"), null);
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
        public ActionResult AddOrUpdateLOS(LOSMasterVM LOSVM)
        {
            try
            {
                var LOS = new LOSMasterRepository().AddOrUpdate(LOSVM);
                return new ReplyFormat().Success(Messages.SUCCESS, LOS);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            LOSMasterVM LOSVM = new LOSMasterVM();
            ViewBag.PageType = "Create";
            try
            {
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();
                ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View("ManageLOSMaster", LOSVM);

        }



        public ActionResult Edit(String id, bool isView)
        {

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);

                    ViewBag.lstSBU = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();
                    ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
                    LOSMasterVM LOSVM = new LOSMasterRepository().Get(Convert.ToInt32(id));
                    ViewBag.ViewState = isView;
                    ViewBag.PageType = !isView ? "Edit" : "View";
                    return View("ManageLOSMaster", LOSVM);
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();

        }
        [HttpPost]
        public JsonResult CheckIfExist(LOSMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.LOSName))
                {
                    var LOS = false;
                    if (data.Id == 0)
                    {
                        LOS = new LOSMasterRepository().IsExist(data.LOSName);
                    }
                    else
                    {
                        LOS = new LOSMasterRepository().IsExist(data.LOSName, data.Id);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, LOS);
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
        public ActionResult ImportLOS(string file)
        {
            try
            {
                string retval = new LOSMasterRepository().UploadImportLOS(file);
                if (!string.IsNullOrEmpty(retval))
                {
                    int count = new LOSMasterRepository().ImportImportLOS(retval);
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


                var ws = package.Workbook.Worksheets.Add(Messages.LOS);
                //Headers
                ws.Cells["A1"].Value = Messages.LOS;
                ws.Cells["B1"].Value = Messages.SBU;
                ws.Cells["C1"].Value = Messages.SubSBU;
                ws.Cells["D1"].Value = Messages.Competency;
                ws.Cells["E1"].Value = Messages.InvolvedUser;
                ws.Cells["F1"].Value = Messages.CreatedDate;
                ws.Cells["G1"].Value = Messages.CreatedBy;
                ws.Cells["H1"].Value = Messages.ModifiedDate;
                ws.Cells["I1"].Value = Messages.ModifiedBy;
                ws.Cells["J1"].Value = Messages.Status;


                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.LOS;

                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.SBU;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.SubSBU;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.Competency;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.InvolvedUser;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.CreatedDate;

                ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                ws.Cells[rowNumber, 7].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                ws.Cells[rowNumber, 8].Value = Messages.ModifiedDate;

                ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                ws.Cells[rowNumber, 9].Value = Messages.ModifiedBy;
                                    
                ws.Cells[rowNumber, 10].Style.Font.Bold = true;
                ws.Cells[rowNumber, 10].Value = Messages.Status;
                foreach (var log in  GetAll(0))
                {
                    rowNumber++;

                    ws.Cells[rowNumber, 1].Value = log.LOSName;
                    ws.Cells[rowNumber, 2].Value = log.SBU;
                    ws.Cells[rowNumber, 3].Value = log.SubSBU;
                    ws.Cells[rowNumber, 4].Value = log.CompetencyName;
                    ws.Cells[rowNumber, 5].Value = log.InvolvedUser;
                    ws.Cells[rowNumber, 6].Value = log.CreatedDate != null? log.CreatedDate.ToString("dd/MM/yyyy"):Messages.NotAvailable;
                    ws.Cells[rowNumber, 7].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 8].Value = log.UpdatedDate != null ? log.UpdatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 9].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;
                    ws.Cells[rowNumber, 10].Value = log.Status ? Messages.Active : Messages.Inactive;

                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.LOS + Messages.XLSX;
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                stream.Position = 0;
                if(fileName!=null)
                {
                    return File(stream, contentType, fileName);
                }

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
            return View("LosReport"); 
        }

        public ActionResult ExportDataHistory(int id)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage();


                var ws = package.Workbook.Worksheets.Add(Messages.LOSHistory);
                //Headers
                ws.Cells["A1"].Value = Messages.LOS;
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
                ws.Cells[rowNumber, 1].Value = Messages.LOS;

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
                var lst = GetAllHistory(0,id);
                foreach (var log in lst)
                {
                    rowNumber++;

                    ws.Cells[rowNumber, 1].Value = log.LOSName;
                    ws.Cells[rowNumber, 2].Value = log.SBU;
                    ws.Cells[rowNumber, 3].Value = log.SubSBU;
                    ws.Cells[rowNumber, 4].Value = log.CompetencyName;
                    ws.Cells[rowNumber, 5].Value = log.CreatedDate != null ? log.CreatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 6].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 7].Value = log.UpdatedDate != null ? log.UpdatedDate.ToString("dd/MM/yyyy") : Messages.NotAvailable;
                    ws.Cells[rowNumber, 8].Value = !string.IsNullOrEmpty(log.UpdatedByName) ? log.UpdatedByName : Messages.NotAvailable;
                    ws.Cells[rowNumber, 9].Value = log.Status ? Messages.Active : Messages.Inactive;
                    ws.Cells[rowNumber, 10].Value =log.EntityState;
                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.LOSHistory + Messages.XLSX;
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

        //17/12/2020(Los Report)
        public ActionResult LosReport()
        {
            ViewBag.los = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.WorkEmail.ToString() }).ToList();
            
            var DataTableDetail = new HomeController().getDataTableDetail("LOS", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        [HttpGet]
        public ActionResult GetLOSReport(string range,int losid, int currentPage)
        {
            ViewBag.los = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();

    
                var LosReport = new LOSMasterRepository().GetAllReport(range,losid);
                ViewBag.LossReporting = LosReport;
            ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.WorkEmail.ToString() }).ToList();

            //ViewBag.lstLOS = JsonConvert.SerializeObject(GetAllReportLos(currentPage,losid ,range));
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("Categories", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View("LosReport");
        }
        //12/18/2020
        public ActionResult ExportLDataLOsReport(string range, int losid, int currentPage)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage();


                var ws = package.Workbook.Worksheets.Add(Messages.LOS);
                //Headers
                ws.Cells["A1"].Value = Messages.LOS;
                ws.Cells["B1"].Value = Messages.SBU;
                ws.Cells["C1"].Value = Messages.SubSBU;
                ws.Cells["D1"].Value = Messages.CreatedBy;
                ws.Cells["E1"].Value = Messages.Category;
                ws.Cells["F1"].Value = Messages.SubCategory;
                ws.Cells["G1"].Value = Messages.Region;
                ws.Cells["H1"].Value = Messages.Company;
                ws.Cells["I1"].Value = Messages.CaseStage;
             


                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.LOS;

                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.SBU;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.SubSBU;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.Category;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.SubCategory;

                ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                ws.Cells[rowNumber, 7].Value = Messages.Region;

                ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                ws.Cells[rowNumber, 8].Value = Messages.Company;

                ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                ws.Cells[rowNumber, 9].Value = Messages.CaseStage;

              
                var LosReport = new LOSMasterRepository().GetAllReport(range, losid);
                foreach (var log in LosReport)
                {
                    rowNumber++;

                    ws.Cells[rowNumber, 1].Value = log.LOSName;
                    ws.Cells[rowNumber, 2].Value = log.SBU;
                    ws.Cells[rowNumber, 3].Value = log.SubSbU;
                    ws.Cells[rowNumber, 4].Value = log.CreatedByName;
                    ws.Cells[rowNumber, 5].Value = log.Category;
                    ws.Cells[rowNumber, 7].Value = log.SubCategory;
                    ws.Cells[rowNumber, 8].Value = log.RegionName;
                    ws.Cells[rowNumber, 9].Value = log.CompanyName;
                    ws.Cells[rowNumber, 10].Value = log.CaseType;

                }


                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.LOS + Messages.XLSX;
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

        [HttpPost]
        [AllowAnonymous]
        public JsonResult UserMail(LOSMasterVM los)
        {
            string response = "";
            try
            {

                if (!ReferenceEquals(los, null))
                {
                    string[] UserEmailList =los.UserInvolved.Split(',');


                    foreach (string useremail in UserEmailList)
                    {
                        var token = Guid.NewGuid().ToString("n");

                     response=  new UserMailer().UserMailed(token,los.Comment, useremail, Request.Browser.Browser, GetIp());
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS);
                }
                else
                {
                    return new ReplyFormat().Error(Messages.FAIL);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        public string GetIp()
        {
            var visitorsIpAddr = string.Empty;
            try
            {
                if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    visitorsIpAddr = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }
                else if (!string.IsNullOrEmpty(Request.UserHostAddress))
                {
                    visitorsIpAddr = Request.UserHostAddress;
                }
            }
            catch (Exception e)
            {
                
            }
            return visitorsIpAddr;
        }
        public ActionResult LosReport2()
        {
            ViewBag.los = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
            ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.WorkEmail.ToString() }).ToList();
            
            var DataTableDetail = new HomeController().getDataTableDetail("LOS", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            ViewModel.LOSMasterVM Ls = new LOSMasterVM();
            return View(Ls);
        }
        public ActionResult GetTypeValue(string Type)
        {
           
            if(Type== "CaseStage")
            {
                var types = new List<SelectListItem>
                {
             new SelectListItem{ Text="Actionable", Value = "Actionable" },
             new SelectListItem{ Text="Non-Actionable", Value = "NonActionable" },
              };
                return Json(types);
            }
            else if(Type== "CaseType")
            {
                var types = new List<SelectListItem>
                {
             new SelectListItem{ Text="In Progress", Value = "InProgess" },
             new SelectListItem{ Text="Closed", Value = "Closed" },
          };
                return Json(types);
            }
            else if(Type=="LOS")
            {
                var types = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                return Json(types);
            }
            else if(Type=="SBU")
            {
                var types = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                return Json(types);
            }
            else if(Type== "SubSBU")
            {
             var types= new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                return Json(types);  
            }
            else if(Type== "categoryOfComplaint")
            {
                var types = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();
                return Json(types);
            }
            else
            {
                return Json("");
            }
        
        }
        public ActionResult GetXOLReport(int currentPage, string range,string types,string typevalues)
        {
            
            ViewModel.LOSMasterVM Ls = new ViewModel.LOSMasterVM();
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }



            if (types == "LOS")
            {

                var LosReport = new LOSMasterRepository().GetAllReport(range,Convert.ToInt32(typevalues));
                ViewBag.LossReporting = LosReport.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList(); 

                lstCount = LosReport.Count;
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;

                //var types = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();
                Ls.CaseType = "LOS";
                int typeval = Convert.ToInt32(typevalues);
               var typevalued = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.LOSName, Value = d.Id.ToString() }).ToList();

                ViewBag.typevalues =typevalued;
            
            }
            else if (types == "SBU")
            {
                var SBUReport = new SBUMasterRepository().GetAllReport(range,Convert.ToInt32(typevalues));
                ViewBag.LossReporting = SBUReport.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();

                lstCount = SBUReport.Count;
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                Ls.CaseType ="SBU" ;
                int typeval = Convert.ToInt32(typevalues);
                var typevalued = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();

                ViewBag.typevalues = typevalued;

            }
            else if (types == "SubSBU")
            {
                var SubSBUReport = new SubSBUMasterRepository().GetAllReport(range,Convert.ToInt32(typevalues));
                ViewBag.LossReporting = SubSBUReport.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();

                lstCount = SubSBUReport.Count;
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;

                Ls.CaseType = "SubSBU";
                int typeval = Convert.ToInt32(typevalues);
                var typevalued = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();

                ViewBag.typevalues = typevalued;
            }
            else if (types == "categoryOfComplaint")
            {
                var CategoryReport = new CategoryMastersRepository().GetAllReport(range,Convert.ToInt32(typevalues));
                ViewBag.LossReporting = CategoryReport.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();

                lstCount = CategoryReport.Count;
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;



                Ls.CaseType = types;
                int typeval = Convert.ToInt32(typevalues);
                var typevalued = new CategoryMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CategoryName, Value = d.Id.ToString() }).ToList();

                ViewBag.typevalues = typevalued;
            }
            else if (types == "CaseType")
            {
                var Casetypes = new LOSMasterRepository().GetAllCaseStageReport(range, typevalues);
                ViewBag.LossReporting = Casetypes.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();

                lstCount = Casetypes.Count;
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;


                Ls.CaseType = "CaseType";
                var typed = new List<SelectListItem>
                {
             new SelectListItem{ Text="In Progress", Value = "InProgess" },
             new SelectListItem{ Text="Closed", Value = "Closed" },
              };
                ViewBag.typevalues = typed;

            }
            else if (types == "CaseStage")
            {
                var Casetypes = new LOSMasterRepository().GetAllTypeStageReport(range, typevalues);
                ViewBag.LossReporting = Casetypes.Skip((currentPage - 1) * maxRows).Take(maxRows).ToList();

                lstCount = Casetypes.Count;
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;


                Ls.CaseType = "CaseStage";
                var typed = new List<SelectListItem>
                {
                new SelectListItem{ Text="Actionable", Value = "Actionable" },
                new SelectListItem{ Text="Non-Actionable", Value = "NonActionable" },
              };
                ViewBag.typevalues = typed;

            }
            ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.WorkEmail.ToString() }).ToList();

                       
            //ViewBag.lstLOS = JsonConvert.SerializeObject(GetAllReportLos(currentPage,losid ,range));
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("Categories", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View("LosReport2",Ls);
        }
        public ActionResult ExportLDataLOSReport2(string range, string types, string typevalues)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage();


                var ws = package.Workbook.Worksheets.Add(Messages.LOS);
                //Headers
                ws.Cells["A1"].Value = Messages.ComplaintNo;
                ws.Cells["B1"].Value = Messages.LOS;
                ws.Cells["C1"].Value = Messages.SBU;
                ws.Cells["D1"].Value = Messages.SubSBU;
                ws.Cells["E1"].Value = Messages.CreatedBy;
                ws.Cells["F1"].Value = Messages.Category;
                ws.Cells["G1"].Value = Messages.SubCategory;
                ws.Cells["H1"].Value = Messages.Region;
                ws.Cells["I1"].Value = Messages.Company;
                ws.Cells["J1"].Value = Messages.CaseType;
                ws.Cells["K1"].Value = Messages.CaseStage;


                var rowNumber = 1;
                ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                ws.Cells[rowNumber, 1].Value = Messages.ComplaintNo;
                ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                ws.Cells[rowNumber, 2].Value = Messages.LOS;

                ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                ws.Cells[rowNumber, 3].Value = Messages.SBU;

                ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                ws.Cells[rowNumber, 4].Value = Messages.SubSBU;

                ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                ws.Cells[rowNumber, 5].Value = Messages.CreatedBy;

                ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                ws.Cells[rowNumber, 6].Value = Messages.Category;

                ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                ws.Cells[rowNumber, 7].Value = Messages.SubCategory;

                ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                ws.Cells[rowNumber, 8].Value = Messages.Region;

                ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                ws.Cells[rowNumber, 9].Value = Messages.Company;

                ws.Cells[rowNumber, 10].Style.Font.Bold = true;
                ws.Cells[rowNumber, 10].Value = Messages.CaseStage;

                ws.Cells[rowNumber, 11].Style.Font.Bold = true;
                ws.Cells[rowNumber, 11].Value = Messages.CaseType;


                if (types == "LOS")
                {
                    var LosReport = new LOSMasterRepository().GetAllReport(range, Convert.ToInt32(typevalues));
                    foreach (var log in LosReport)
                    {
                        rowNumber++;
                        ws.Cells[rowNumber, 1].Value = log.ComplaintNo;
                        ws.Cells[rowNumber, 1].Value = log.LOSName;
                        ws.Cells[rowNumber, 2].Value = log.SBU;
                        ws.Cells[rowNumber, 3].Value = log.SubSbU;
                        ws.Cells[rowNumber, 4].Value = log.CreatedByName;
                        ws.Cells[rowNumber, 5].Value = log.Category;
                        ws.Cells[rowNumber, 7].Value = log.SubCategory;
                        ws.Cells[rowNumber, 8].Value = log.RegionName;
                        ws.Cells[rowNumber, 9].Value = log.CompanyName;
                        ws.Cells[rowNumber, 10].Value = log.CaseType;
                        ws.Cells[rowNumber, 11].Value = log.ActionType;
                    }

                }
                else if (types == "SBU")
                {
                    var SBUReport = new SBUMasterRepository().GetAllReport(range, Convert.ToInt32(typevalues));
                    foreach (var log in SBUReport)
                    {
                        rowNumber++;
                        ws.Cells[rowNumber, 1].Value = log.ComplaintNo;
                        ws.Cells[rowNumber, 1].Value = log.LOSName;
                        ws.Cells[rowNumber, 2].Value = log.SBU;
                        ws.Cells[rowNumber, 3].Value = log.SubSbU;
                        ws.Cells[rowNumber, 4].Value = log.CreatedByName;
                        ws.Cells[rowNumber, 5].Value = log.Category;
                        ws.Cells[rowNumber, 7].Value = log.SubCategory;
                        ws.Cells[rowNumber, 8].Value = log.RegionName;
                        ws.Cells[rowNumber, 9].Value = log.CompanyName;
                        ws.Cells[rowNumber, 10].Value = log.CaseType;
                        ws.Cells[rowNumber, 11].Value = log.ActionType;
                    }


                }
                else if (types == "SubSBU")
                {
                    var SubSBUReport = new SubSBUMasterRepository().GetAllReport(range, Convert.ToInt32(typevalues));
                    foreach (var log in SubSBUReport)
                    {
                        rowNumber++;
                        ws.Cells[rowNumber, 1].Value = log.ComplaintNo;
                        ws.Cells[rowNumber, 1].Value = log.LOSName;
                        ws.Cells[rowNumber, 2].Value = log.SBU;
                        ws.Cells[rowNumber, 3].Value = log.SubSbU;
                        ws.Cells[rowNumber, 4].Value = log.CreatedByName;
                        ws.Cells[rowNumber, 5].Value = log.Category;
                        ws.Cells[rowNumber, 7].Value = log.SubCategory;
                        ws.Cells[rowNumber, 8].Value = log.RegionName;
                        ws.Cells[rowNumber, 9].Value = log.CompanyName;
                        ws.Cells[rowNumber, 10].Value = log.CaseType;
                        ws.Cells[rowNumber, 11].Value = log.ActionType;
                    }
                }
                else if (types == "categoryOfComplaint")
                {
                    var CategoryReport = new CategoryMastersRepository().GetAllReport(range, Convert.ToInt32(typevalues));
                    foreach (var log in CategoryReport)
                    {
                        rowNumber++;
                        ws.Cells[rowNumber, 1].Value = log.ComplaintNo;
                        ws.Cells[rowNumber, 1].Value = log.LOSName;
                        ws.Cells[rowNumber, 2].Value = log.SBU;
                        ws.Cells[rowNumber, 3].Value = log.SubSbU;
                        ws.Cells[rowNumber, 4].Value = log.CreatedByName;
                        ws.Cells[rowNumber, 5].Value = log.Category;
                        ws.Cells[rowNumber, 7].Value = log.SubCategory;
                        ws.Cells[rowNumber, 8].Value = log.RegionName;
                        ws.Cells[rowNumber, 9].Value = log.CompanyName;
                        ws.Cells[rowNumber, 10].Value = log.CaseType;
                        ws.Cells[rowNumber, 11].Value = log.ActionType;
                    }
                }
                else if (types == "CaseType")
                {
                    var Casetypes = new LOSMasterRepository().GetAllCaseStageReport(range, typevalues);

                    foreach (var log in Casetypes)
                    {
                        rowNumber++;
                        ws.Cells[rowNumber, 1].Value = log.ComplaintNo;
                        ws.Cells[rowNumber, 1].Value = log.LOSName;
                        ws.Cells[rowNumber, 2].Value = log.SBU;
                        ws.Cells[rowNumber, 3].Value = log.SubSbU;
                        ws.Cells[rowNumber, 4].Value = log.CreatedByName;
                        ws.Cells[rowNumber, 5].Value = log.Category;
                        ws.Cells[rowNumber, 7].Value = log.SubCategory;
                        ws.Cells[rowNumber, 8].Value = log.RegionName;
                        ws.Cells[rowNumber, 9].Value = log.CompanyName;
                        ws.Cells[rowNumber, 10].Value = log.CaseType;
                        ws.Cells[rowNumber, 11].Value = log.ActionType;
                    }

                }
                else if (types == "CaseStage")
                {
                    var Casetypes = new LOSMasterRepository().GetAllTypeStageReport(range, typevalues);
                    foreach (var log in Casetypes)
                    {
                        rowNumber++;
                        ws.Cells[rowNumber, 1].Value = log.ComplaintNo;
                        ws.Cells[rowNumber, 1].Value = log.LOSName;
                        ws.Cells[rowNumber, 2].Value = log.SBU;
                        ws.Cells[rowNumber, 3].Value = log.SubSbU;
                        ws.Cells[rowNumber, 4].Value = log.CreatedByName;
                        ws.Cells[rowNumber, 5].Value = log.Category;
                        ws.Cells[rowNumber, 7].Value = log.SubCategory;
                        ws.Cells[rowNumber, 8].Value = log.RegionName;
                        ws.Cells[rowNumber, 9].Value = log.CompanyName;
                        ws.Cells[rowNumber, 10].Value = log.CaseType;
                        ws.Cells[rowNumber, 11].Value = log.ActionType;
                    }

                }
                

                var stream = new MemoryStream();
                package.SaveAs(stream);

                string fileName = Messages.LOS + Messages.XLSX;
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