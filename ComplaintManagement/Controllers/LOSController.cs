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
        public ActionResult SearchLOS(string search)
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
            ViewBag.lstLOS = JsonConvert.SerializeObject(output);
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
        public dynamic GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            var lst = new LOSMasterRepository().GetAll();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from LOS in lst
                       where LOS.CreatedDate >= fromDate && LOS.CreatedDate <= toDate
                       select LOS).ToList();
                lstCount = lst.Count;
                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                dynamic output = new List<dynamic>();
                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll();


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
                        row.Id = los.Id;
                        row.LOSName = los.LOSName;
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

                List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll();
                List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll();



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
                                    subsbus.Add(lstSubSBUMaster.Where(x => x.Id == Convert.ToInt32(SubSBUIdItem)).FirstOrDefault().SubSBU);
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
                                    CompetencyLst.Add(lstCompetency.Where(x => x.Id == Convert.ToInt32(CompetencyItem)).FirstOrDefault().CompetencyName);
                                }
                                row.CompetencyName = string.Join(",", CompetencyLst);
                            }
                            else
                            {
                                row.CompetencyName = lstCompetency.Where(x => x.Id == Convert.ToInt32(los.CompetencyId)).FirstOrDefault()!=null? lstCompetency.Where(x => x.Id == Convert.ToInt32(los.CompetencyId)).FirstOrDefault().CompetencyName:string.Empty;
                            }
                        }
                        row.Id = los.Id;
                        row.LOSName = los.LOSName;
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
        public ActionResult Delete(int id)
        {
            bool retval = true;
            try
            {
                retval = new LOSMasterRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "LOS"), null);
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

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View("ManageLOSMaster", LOSVM);

        }



        public ActionResult Edit(int id, bool isView)
        {

            try
            {
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();

                LOSMasterVM LOSVM = new LOSMasterRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                return View("ManageLOSMaster", LOSVM);
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
    }
}