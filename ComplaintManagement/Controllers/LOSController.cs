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
    public class LOSController : Controller
    {
        // GET: LOS
        public ActionResult Index()
        {
            List<LOSMasterVM> lst = new LOSMasterRepository().GetAll();
            List<SBUMasterVM> lstSBUMaster = new SBUMasterRepository().GetAll();
            List<SubSBUMasterVM> lstSubSBUMaster = new SubSBUMasterRepository().GetAll();
            List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll();

                dynamic output = new List<dynamic>();
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

            ViewBag.lstLOS = JsonConvert.SerializeObject(output);
            var DataTableDetail = new HomeController().getDataTableDetail("LOS", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
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
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View("ManageLOSMaster", LOSVM);

        }



        public ActionResult Edit(int id)
        {

            try
            {
                ViewBag.lstSBU = new SBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstSubSBU = new SubSBUMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.SubSBU, Value = d.Id.ToString() }).ToList();
                ViewBag.lstCompetency = new CompetencyMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompetencyName, Value = d.Id.ToString() }).ToList();

                LOSMasterVM LOSVM = new LOSMasterRepository().Get(id);
                ViewBag.PageType = "Edit";
                return View("ManageLOSMaster", LOSVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();

        }
    }
}