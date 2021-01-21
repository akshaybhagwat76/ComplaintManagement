using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using Microsoft.Office.Interop.PowerPoint;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;


namespace ComplaintManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();
        public ActionResult Index()
        {
            ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();
            return View(new CompliantMastersRepository().GetDashboardCounts());
        }

        public ActionResult Index1()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Index2()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public Tuple<int, int> getDataTableDetail(string SessionName, int? default_number)
        {
            int PageLength = System.Web.HttpContext.Current.Session[SessionName + "PageLength"] != null && System.Web.HttpContext.Current.Session[SessionName + "PageLength"].ToString() != "" ? Convert.ToInt16(System.Web.HttpContext.Current.Session[SessionName + "PageLength"]) : 0;
            int Page = System.Web.HttpContext.Current.Session[SessionName + "Page"] != null && System.Web.HttpContext.Current.Session[SessionName + "Page"].ToString() != "" ? Convert.ToInt16(System.Web.HttpContext.Current.Session[SessionName + "Page"]) : 0;
            int PageIndex = PageLength == 0 ? (default_number.HasValue ? default_number.Value : 25) : PageLength;

            System.Web.HttpContext.Current.Session[SessionName + "PageLength"] = null;
            System.Web.HttpContext.Current.Session[SessionName + "Page"] = null;

            return Tuple.Create<int, int>(Page, PageIndex);
        }
        [HttpGet]
        public ActionResult DashboardPiChart(string dateFrom, string dateTo)
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
               .Select(c => c.Value).SingleOrDefault();
            var Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
               .Select(c => c.Value).SingleOrDefault();
            if (dateFrom == "" && dateTo == "")
            {
                int year = DateTime.UtcNow.Year;
                dateFrom = new DateTime(year, 1, 1).ToString("yyyy-MM-dd");
                dateTo = DateTime.UtcNow.ToString("yyyy-MM-dd");
            }

            DateTime? dateF = dateFrom.ToDateTime();
            DateTime? dateT = dateTo.ToDateTime();

            List<LOSMasterVM> losMasterList = new List<LOSMasterVM>();
            List<SBUMasterVM> sbuMasterList = new List<SBUMasterVM>();
            List<SubSBUMasterVM> subSBUMasterList = new List<SubSBUMasterVM>();

            DashboardPiChartVM dashboardPiChart = new DashboardPiChartVM();
            List<EmployeeComplaintWorkFlow> complaintList = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> complaintTList = new List<EmployeeComplaintWorkFlow>();
            if (Role == Messages.LeadUser)
            {
                var LOSData = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList();
                var SBUData = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList();
                var SubSBUData = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList();

                foreach (var item in LOSData)
                {
                    bool isLOSValid = false;
                    if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                    {
                        int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                        foreach (int los in LosList)
                        {
                            if (los > 0 && los == Convert.ToInt32(sid))
                            {
                                isLOSValid = true;
                            }
                        }
                    }
                    if (isLOSValid)
                    {
                        losMasterList.Add(item);
                    }
                }
                foreach (var item in SBUData)
                {
                    bool isSBUValid = false;
                    if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                    {
                        int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                        foreach (int los in LosList)
                        {
                            if (los > 0 && los == Convert.ToInt32(sid))
                            {
                                isSBUValid = true;
                            }
                        }
                    }
                    if (isSBUValid)
                    {
                        sbuMasterList.Add(item);
                    }
                }
                foreach (var item in SubSBUData)
                {
                    bool isSubSBUValid = false;
                    if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                    {
                        int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                        foreach (int los in LosList)
                        {
                            if (los > 0 && los == Convert.ToInt32(sid))
                            {
                                isSubSBUValid = true;
                            }
                        }
                    }
                    if (isSubSBUValid)
                    {
                        subSBUMasterList.Add(item);
                    }
                }

                if (losMasterList != null)
                {
                    foreach (var item in losMasterList)
                    {
                        complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                    }
                    complaintTList.AddRange(complaintList);
                }
                if (sbuMasterList != null)
                {
                    foreach (var item in sbuMasterList)
                    {
                        complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.SBUId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                    }
                    complaintTList.AddRange(complaintList);
                }
                if (subSBUMasterList != null)
                {
                    foreach (var item in subSBUMasterList)
                    {
                        complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.SubSBUId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                    }
                    complaintTList.AddRange(complaintList);
                }
            }
            else
            {
                complaintTList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
            }
            //dashboard count start
            //foreach (var item in complaintTList.Distinct())
            //{
            //    dashboardPiChart.casePiChart.Actionable = db.HR_Role.Where(x => x.ComplentId == item.ComplaintId && x.CaseType == "Actionable" && x.IsActive).Count();
            //    dashboardPiChart.casePiChart.NonActionable = db.HR_Role.Where(x => x.ComplentId == item.ComplaintId && x.CaseType == "NonActionable" && x.IsActive).Count();
            //}

            dashboardPiChart.caseTypeofComplaint = (from a in complaintTList.Distinct()
                                                    join b in db.HR_Role on a.ComplaintId equals b.ComplentId
                                                    group a by new { b.CaseType } into caseTypeGroup
                                                    select new ListPiChartVM
                                                    {
                                                        Label = caseTypeGroup.Key.CaseType,
                                                        Value = caseTypeGroup.Count(),
                                                    }).ToList();

            dashboardPiChart.categoryPiCharts = (from a in complaintTList.Distinct()
                                                 join b in db.CategoryMasters on a.EmployeeComplaintMaster.CategoryId equals b.Id
                                                 group a by a.EmployeeComplaintMaster.CategoryId into categoryGroup
                                                 select new ListPiChartVM
                                                 {
                                                     Label = new CategoryMastersRepository().Get(categoryGroup.Key).CategoryName,
                                                     Value = categoryGroup.Count()
                                                 }).ToList();

            dashboardPiChart.subCategoryPiCharts = (from a in complaintTList.Distinct()
                                                    join b in db.SubCategoryMasters on a.EmployeeComplaintMaster.SubCategoryId equals b.Id
                                                    group a by a.EmployeeComplaintMaster.SubCategoryId into subCategoryGroup
                                                    select new ListPiChartVM
                                                    {
                                                        Label = new SubCategoryMastersRepository().Get(subCategoryGroup.Key).SubCategoryName,
                                                        Value = subCategoryGroup.Count()
                                                    }).ToList();

            dashboardPiChart.regionPiCharts = (from a in complaintTList.Distinct()
                                               join b in db.UserMasters on a.CreatedBy equals b.Id
                                               join c in db.RegionMasters on b.RegionId equals c.Id
                                               group b by b.RegionId into reagionGroup
                                               select new ListPiChartVM
                                               {
                                                   Label = new RegionMasterRepository().Get(reagionGroup.Key).Region,
                                                   Value = reagionGroup.Count()
                                               }).ToList();

            dashboardPiChart.officePiCharts = (from a in complaintTList.Distinct()
                                               join b in db.UserMasters on a.CreatedBy equals b.Id
                                               join c in db.EntityMasters on b.Company equals c.Id
                                               group b by b.Company into officeGroup
                                               select new ListPiChartVM
                                               {
                                                   Label = new EntityMasterRepository().Get(officeGroup.Key).EntityName,
                                                   Value = officeGroup.Count()
                                               }).ToList();

            dashboardPiChart.losPiCharts = (from a in complaintTList.Distinct()
                                            join b in db.LOSMasters on a.LOSId equals b.Id
                                            group a by a.LOSId into losGroup
                                            select new ListPiChartVM
                                            {
                                                Label = new LOSMasterRepository().Get(losGroup.Key).LOSName,
                                                Value = losGroup.Count()
                                            }).ToList();

            dashboardPiChart.sbuPiCharts = (from a in complaintTList.Distinct()
                                            join b in db.SBUMasters on a.SBUId equals b.Id
                                            group a by a.SBUId into sbuGroup
                                            select new ListPiChartVM
                                            {
                                                Label = new SBUMasterRepository().Get(sbuGroup.Key).SBU,
                                                Value = sbuGroup.Count()
                                            }).ToList();

            dashboardPiChart.subSBUPiCharts = (from a in complaintTList.Distinct()
                                               join b in db.SubSBUMasters on a.SubSBUId equals b.Id
                                               group a by a.SubSBUId into subSUBGroup
                                               select new ListPiChartVM
                                               {
                                                   Label = new SubSBUMasterRepository().Get(subSUBGroup.Key).SubSBU,
                                                   Value = subSUBGroup.Count()
                                               }).ToList();

            dashboardPiChart.subSBUPiCharts = (from a in complaintTList.Distinct()
                                               join b in db.SubSBUMasters on a.SubSBUId equals b.Id
                                               group a by a.SubSBUId into subSUBGroup
                                               select new ListPiChartVM
                                               {
                                                   Label = new SubSBUMasterRepository().Get(subSUBGroup.Key).SubSBU,
                                                   Value = subSUBGroup.Count()
                                               }).ToList();

            dashboardPiChart.genderofComplainant = (from a in complaintTList.Distinct()
                                                    join b in db.UserMasters on a.CreatedBy equals b.Id
                                                    group b by b.Gender into genderGroup
                                                    select new ListPiChartVM
                                                    {
                                                        Label = genderGroup.FirstOrDefault().Gender,
                                                        Value = genderGroup.Count()
                                                    }).ToList();

            dashboardPiChart.genderofRespondent = (from a in complaintTList.Distinct()
                                                   join b in db.HR_Role on a.ComplaintId equals b.ComplentId
                                                   join c in db.UserMasters on b.HRUserId equals c.Id
                                                   group c by c.Gender into genderGroup
                                                   select new ListPiChartVM
                                                   {
                                                       Label = genderGroup.FirstOrDefault().Gender,
                                                       Value = genderGroup.Count()
                                                   }).ToList();



            dashboardPiChart.designationofComplainant = (from a in complaintTList.Distinct()
                                                         join b in db.UserMasters on a.CreatedBy equals b.Id
                                                         group b by b.Manager into genderGroup
                                                         select new ListPiChartVM
                                                         {
                                                             Label = genderGroup.FirstOrDefault().Manager,
                                                             Value = genderGroup.Count()
                                                         }).ToList();

            dashboardPiChart.designationofRespondent = (from a in complaintTList.Distinct()
                                                        join b in db.HR_Role on a.ComplaintId equals b.ComplentId
                                                        join c in db.UserMasters on b.HRUserId equals c.Id
                                                        group c by c.Manager into genderGroup
                                                        select new ListPiChartVM
                                                        {
                                                            Label = genderGroup.FirstOrDefault().Manager,
                                                            Value = genderGroup.Count()
                                                        }).ToList();

            dashboardPiChart.modeofComplaint = (from a in complaintTList.Distinct()
                                                join b in db.UserMasters on a.CreatedBy equals b.Id
                                                group b by b.TimeType into modeOfComplaintGroup
                                                select new ListPiChartVM
                                                {
                                                    Label = modeOfComplaintGroup.FirstOrDefault().TimeType,
                                                    Value = modeOfComplaintGroup.Count()
                                                }).ToList();

            int i = 0, j = 15;

            foreach (var item in complaintTList.Distinct())
            {
                //DateTime dateStart = Convert.ToDateTime(dateFrom).AddDays(i);
                //DateTime dateEnd = Convert.ToDateTime(dateFrom).AddDays(j);
                DateTime? dateStart = dateFrom.ToDateTime().Value.AddDays(i);
                DateTime? dateEnd = dateFrom.ToDateTime().Value.AddDays(j - 1);
                if (i <= j && i < 45)
                {
                    var ageingPiBarCharts = from a in complaintTList.Distinct().Where(a => a.CreatedDate.Date >= dateStart && a.CreatedDate.Date <= dateEnd)
                                            select a;
                    dashboardPiChart.ageingPiBarCharts.Add(new ListPiAndBarChartVM
                    {
                        Label = i + "-" + j,
                        Value1 = ageingPiBarCharts.Count(),
                    });
                }
                else
                {
                    var ageingPiBarCharts = from a in complaintTList.Distinct().Where(a => a.CreatedDate.Date >= dateStart && a.CreatedDate.Date <= dateT)
                                            select a;
                    dashboardPiChart.ageingPiBarCharts.Add(new ListPiAndBarChartVM
                    {
                        Label = i + " and Above",
                        Value1 = ageingPiBarCharts.Count(),
                    });
                    break;
                }
                i = j;
                j += 15;

            }
            return Json(dashboardPiChart, JsonRequestBehavior.AllowGet);



        }

        [HttpGet]
        public ActionResult DashboardBarChart(string dateFrom, string dateTo)
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
               .Select(c => c.Value).SingleOrDefault();
            var Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
               .Select(c => c.Value).SingleOrDefault();

            int yearBack = DateTime.UtcNow.AddYears(-1).Year;
            int yearCurrent = DateTime.UtcNow.Year;
            if (dateFrom == "" && dateTo == "")
            {

                dateFrom = new DateTime(yearBack, 1, 1).ToString("yyyy-MM-dd");
                dateTo = DateTime.UtcNow.ToString("yyyy-MM-dd");
            }

            DateTime? dateF = dateFrom.ToDateTime();
            DateTime? dateT = dateTo.ToDateTime();

            List<LOSMasterVM> losMasterList = new List<LOSMasterVM>();
            List<SBUMasterVM> sbuMasterList = new List<SBUMasterVM>();
            List<SubSBUMasterVM> subSBUMasterList = new List<SubSBUMasterVM>();

            DashboardPiChartVM dashboardPiChart = new DashboardPiChartVM();
            List<EmployeeComplaintWorkFlow> complaintList = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> complaintTList = new List<EmployeeComplaintWorkFlow>();
            if (Role == Messages.LeadUser)
            {
                var LOSData = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList();
                var SBUData = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList();
                var SubSBUData = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList();

                foreach (var item in LOSData)
                {
                    bool isLOSValid = false;
                    if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                    {
                        int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                        foreach (int los in LosList)
                        {
                            if (los > 0 && los == Convert.ToInt32(sid))
                            {
                                isLOSValid = true;
                            }
                        }
                    }
                    if (isLOSValid)
                    {
                        losMasterList.Add(item);
                    }
                }
                foreach (var item in SBUData)
                {
                    bool isSBUValid = false;
                    if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                    {
                        int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                        foreach (int los in LosList)
                        {
                            if (los > 0 && los == Convert.ToInt32(sid))
                            {
                                isSBUValid = true;
                            }
                        }
                    }
                    if (isSBUValid)
                    {
                        sbuMasterList.Add(item);
                    }
                }
                foreach (var item in SubSBUData)
                {
                    bool isSubSBUValid = false;
                    if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                    {
                        int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                        foreach (int los in LosList)
                        {
                            if (los > 0 && los == Convert.ToInt32(sid))
                            {
                                isSubSBUValid = true;
                            }
                        }
                    }
                    if (isSubSBUValid)
                    {
                        subSBUMasterList.Add(item);
                    }
                }

                if (losMasterList != null)
                {
                    foreach (var item in losMasterList)
                    {
                        complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                    }
                    complaintTList.AddRange(complaintList);
                }
                if (sbuMasterList != null)
                {
                    foreach (var item in sbuMasterList)
                    {
                        complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.SBUId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                    }
                    complaintTList.AddRange(complaintList);
                }
                if (subSBUMasterList != null)
                {
                    foreach (var item in subSBUMasterList)
                    {
                        complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.SubSBUId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                    }
                    complaintTList.AddRange(complaintList);
                }
            }
            else
            {
                complaintTList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
            }
            //dashboard count start

            var data = (from a in complaintTList.Distinct()
                        join b in db.HR_Role on a.ComplaintId equals b.ComplentId
                        group a by new { b.CreatedDate?.Year, b.CaseType } into caseTypeGroup
                        select new ListPiAndBarChartVM
                        {
                            Label = caseTypeGroup.Key.CaseType,
                            Year = caseTypeGroup.Key.Year.Value,
                            Value1 = caseTypeGroup.Count(),

                            //Label = caseTypeGroup.Key.CaseType,
                            //Value1 = caseTypeGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                            //Value2 = caseTypeGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                            //Year = caseTypeGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                        }).ToList();

            dashboardPiChart.caseTypeofComplaint1 = data.GroupBy(c => c.Year).Select(g => new ListPiAndBarChartVM
            {
                Year = g.Key,
                Value1 = g.Where(x => x.Label == "Actionable").Sum(x => x.Value1),
                Value2 = g.Where(x => x.Label == "NonActionable").Sum(x => x.Value1),
                Label = g.Select(c => c.Label).FirstOrDefault()
            }).ToList();

            dashboardPiChart.categoryPiBarCharts = (from a in complaintTList.Distinct()
                                                    join b in db.CategoryMasters on a.EmployeeComplaintMaster.CategoryId equals b.Id
                                                    group a by new { a.EmployeeComplaintMaster.CategoryId } into categoryGroup
                                                    select new ListPiAndBarChartVM
                                                    {
                                                        Label = new CategoryMastersRepository().Get(categoryGroup.Key.CategoryId).CategoryName,
                                                        Value1 = categoryGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                        Value2 = categoryGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                        Year = categoryGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                                    }).ToList();

            dashboardPiChart.subCategoryPiBarCharts = (from a in complaintTList.Distinct()
                                                       join b in db.SubCategoryMasters on a.EmployeeComplaintMaster.SubCategoryId equals b.Id
                                                       group a by new { a.EmployeeComplaintMaster.SubCategoryId } into subCategoryGroup
                                                       select new ListPiAndBarChartVM
                                                       {
                                                           Label = new SubCategoryMastersRepository().Get(subCategoryGroup.Key.SubCategoryId).SubCategoryName,
                                                           Value1 = subCategoryGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                           Value2 = subCategoryGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                           Year = subCategoryGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                                       }).ToList();

            dashboardPiChart.regionPiBarCharts = (from a in complaintTList.Distinct()
                                                  join b in db.UserMasters on a.CreatedBy equals b.Id
                                                  join c in db.RegionMasters on b.RegionId equals c.Id
                                                  group a by new { b.RegionId } into reagionGroup
                                                  select new ListPiAndBarChartVM
                                                  {
                                                      Label = new RegionMasterRepository().Get(reagionGroup.Key.RegionId).Region,
                                                      Value1 = reagionGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                      Value2 = reagionGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                      Year = reagionGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                                  }).ToList();

            dashboardPiChart.officePiBarCharts = (from a in complaintTList.Distinct()
                                                  join b in db.UserMasters on a.CreatedBy equals b.Id
                                                  join c in db.EntityMasters on b.Company equals c.Id
                                                  group a by new { b.Company } into officeGroup
                                                  select new ListPiAndBarChartVM
                                                  {
                                                      Label = new EntityMasterRepository().Get(officeGroup.Key.Company).EntityName,
                                                      Value1 = officeGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                      Value2 = officeGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                      Year = officeGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                                  }).ToList();

            dashboardPiChart.losPiBarCharts = (from a in complaintTList.Distinct()
                                               join b in db.LOSMasters on a.LOSId equals b.Id
                                               group a by new { a.LOSId } into losGroup
                                               select new ListPiAndBarChartVM
                                               {
                                                   Label = new LOSMasterRepository().Get(losGroup.Key.LOSId).LOSName,
                                                   Value1 = losGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                   Value2 = losGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                   Year = losGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                               }).ToList();

            dashboardPiChart.sbuPiBarCharts = (from a in complaintTList.Distinct()
                                               join b in db.SBUMasters on a.SBUId equals b.Id
                                               group a by new { a.SBUId } into sbuGroup
                                               select new ListPiAndBarChartVM
                                               {
                                                   Label = new SBUMasterRepository().Get(sbuGroup.Key.SBUId).SBU,
                                                   Value1 = sbuGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                   Value2 = sbuGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                   Year = sbuGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                               }).ToList();

            dashboardPiChart.subSBUPiBarCharts = (from a in complaintTList.Distinct()
                                                  join b in db.SubSBUMasters on a.SubSBUId equals b.Id
                                                  group a by new { a.SubSBUId } into subSUBGroup
                                                  select new ListPiAndBarChartVM
                                                  {
                                                      Label = new SubSBUMasterRepository().Get(subSUBGroup.Key.SubSBUId).SubSBU,
                                                      Value1 = subSUBGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                      Value2 = subSUBGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                      Year = subSUBGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                                  }).ToList();

            dashboardPiChart.genderofComplainantPiBarCharts = (from a in complaintTList.Distinct()
                                                               join b in db.UserMasters on a.CreatedBy equals b.Id
                                                               group a by new { b.Gender } into genderGroup
                                                               select new ListPiAndBarChartVM
                                                               {
                                                                   Label = genderGroup.Key.Gender,
                                                                   Value1 = genderGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                                   Value2 = genderGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                                   Year = genderGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                                               }).ToList();

            dashboardPiChart.genderofRespondentPiBarCharts = (from a in complaintTList.Distinct()
                                                              join b in db.HR_Role on a.ComplaintId equals b.ComplentId
                                                              join c in db.UserMasters on b.HRUserId equals c.Id
                                                              group a by new { c.Gender } into genderGroup
                                                              select new ListPiAndBarChartVM
                                                              {
                                                                  Label = genderGroup.Key.Gender,
                                                                  Value1 = genderGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                                  Value2 = genderGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                                  Year = genderGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                                              }).ToList();



            dashboardPiChart.designationofComplainantPiBarCharts = (from a in complaintTList.Distinct()
                                                                    join b in db.UserMasters on a.CreatedBy equals b.Id
                                                                    group a by new { b.Manager } into genderGroup
                                                                    select new ListPiAndBarChartVM
                                                                    {
                                                                        Label = genderGroup.Key.Manager,
                                                                        Value1 = genderGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                                        Value2 = genderGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                                        Year = genderGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                                                    }).ToList();

            dashboardPiChart.designationofRespondentPiBarCharts = (from a in complaintTList.Distinct()
                                                                   join b in db.HR_Role on a.ComplaintId equals b.ComplentId
                                                                   join c in db.UserMasters on b.HRUserId equals c.Id
                                                                   group a by new { c.Manager } into genderGroup
                                                                   select new ListPiAndBarChartVM
                                                                   {
                                                                       Label = genderGroup.Key.Manager,
                                                                       Value1 = genderGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                                       Value2 = genderGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                                       Year = genderGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                                                   }).ToList();

            dashboardPiChart.modeofComplaintPiBarCharts = (from a in complaintTList.Distinct()
                                                           join b in db.UserMasters on a.CreatedBy equals b.Id
                                                           group a by new { b.TimeType } into modeOfComplaintGroup
                                                           select new ListPiAndBarChartVM
                                                           {
                                                               Label = modeOfComplaintGroup.Key.TimeType,
                                                               Value1 = modeOfComplaintGroup.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                                                               Value2 = modeOfComplaintGroup.Where(a => a.CreatedDate.Year == yearBack).Count(),
                                                               Year = modeOfComplaintGroup.Select(a => a.CreatedDate.Year).FirstOrDefault()
                                                           }).ToList();


            int i = 0, j = 15;

            foreach (var item in complaintTList.Distinct())
            {
                //DateTime dateStart = Convert.ToDateTime(dateFrom).AddDays(i);
                //DateTime dateEnd = Convert.ToDateTime(dateFrom).AddDays(j);
                DateTime? dateStart = dateFrom.ToDateTime().Value.AddDays(i);
                DateTime? dateEnd = dateFrom.ToDateTime().Value.AddDays(j - 1);
                if (i <= j && i < 45)
                {
                    var ageingPiBarCharts = from a in complaintTList.Distinct().Where(a => a.CreatedDate.Date >= dateStart && a.CreatedDate.Date <= dateEnd)
                                            select a;
                    dashboardPiChart.ageingPiBarCharts.Add(new ListPiAndBarChartVM
                    {
                        Label = i + "-" + j,
                        Value1 = ageingPiBarCharts.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                        Value2 = ageingPiBarCharts.Where(a => a.CreatedDate.Year == yearBack).Count(),
                    });
                }
                else
                {
                    var ageingPiBarCharts = from a in complaintTList.Distinct().Where(a => a.CreatedDate.Date >= dateStart && a.CreatedDate.Date <= dateT)
                                            select a;
                    dashboardPiChart.ageingPiBarCharts.Add(new ListPiAndBarChartVM
                    {
                        Label = i + " and Above",
                        Value1 = ageingPiBarCharts.Where(a => a.CreatedDate.Year == yearCurrent).Count(),
                        Value2 = ageingPiBarCharts.Where(a => a.CreatedDate.Year == yearBack).Count(),
                    });
                    break;
                }
                i = j;
                j += 15;

            }

            return Json(dashboardPiChart, JsonRequestBehavior.AllowGet);



        }

        [HttpGet]
        public ActionResult DashboardPiChartTableBind(string dateFrom, string dateTo, string chart, string label)
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                var Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();
                string modelTitle = string.Empty;
                if (dateFrom == "" && dateTo == "")
                {
                    int year = DateTime.UtcNow.Year;
                    dateFrom = new DateTime(year, 1, 1).ToString("yyyy-MM-dd");
                    dateTo = DateTime.UtcNow.ToString("yyyy-MM-dd");
                }

                DateTime? dateF = dateFrom.ToDateTime();
                DateTime? dateT = dateTo.ToDateTime();

                List<LOSMasterVM> losMasterList = new List<LOSMasterVM>();
                List<SBUMasterVM> sbuMasterList = new List<SBUMasterVM>();
                List<SubSBUMasterVM> subSBUMasterList = new List<SubSBUMasterVM>();

                List<TableListPiAndBarChartVM> dashboardPiChart = new List<TableListPiAndBarChartVM>();
                List<EmployeeComplaintWorkFlow> complaintList = new List<EmployeeComplaintWorkFlow>();
                List<EmployeeComplaintWorkFlow> complaintTList = new List<EmployeeComplaintWorkFlow>();
                if (Role == Messages.LeadUser)
                {
                    var LOSData = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList();
                    var SBUData = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList();
                    var SubSBUData = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList();

                    foreach (var item in LOSData)
                    {
                        bool isLOSValid = false;
                        if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                        {
                            int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                            foreach (int los in LosList)
                            {
                                if (los > 0 && los == Convert.ToInt32(sid))
                                {
                                    isLOSValid = true;
                                }
                            }
                        }
                        if (isLOSValid)
                        {
                            losMasterList.Add(item);
                        }
                    }
                    foreach (var item in SBUData)
                    {
                        bool isSBUValid = false;
                        if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                        {
                            int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                            foreach (int los in LosList)
                            {
                                if (los > 0 && los == Convert.ToInt32(sid))
                                {
                                    isSBUValid = true;
                                }
                            }
                        }
                        if (isSBUValid)
                        {
                            sbuMasterList.Add(item);
                        }
                    }
                    foreach (var item in SubSBUData)
                    {
                        bool isSubSBUValid = false;
                        if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                        {
                            int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                            foreach (int los in LosList)
                            {
                                if (los > 0 && los == Convert.ToInt32(sid))
                                {
                                    isSubSBUValid = true;
                                }
                            }
                        }
                        if (isSubSBUValid)
                        {
                            subSBUMasterList.Add(item);
                        }
                    }

                    if (losMasterList != null)
                    {
                        foreach (var item in losMasterList)
                        {
                            complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                        }
                        complaintTList.AddRange(complaintList);
                    }
                    if (sbuMasterList != null)
                    {
                        foreach (var item in sbuMasterList)
                        {
                            complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.SBUId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                        }
                        complaintTList.AddRange(complaintList);
                    }
                    if (subSBUMasterList != null)
                    {
                        foreach (var item in subSBUMasterList)
                        {
                            complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.SubSBUId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                        }
                        complaintTList.AddRange(complaintList);
                    }
                }
                else
                {
                    complaintTList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                }
                if (chart == "CaseType")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.HR_Role on a.ComplaintId equals c.ComplentId
                                        where c.CaseType.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Case Type: " + label;
                }
                else if (chart == "Category")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.CategoryMasters on a.EmployeeComplaintMaster.CategoryId equals c.Id
                                        where c.CategoryName.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Category wise: " + label;
                }
                else if (chart == "SubCategory")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.SubCategoryMasters on a.EmployeeComplaintMaster.SubCategoryId equals c.Id
                                        where c.SubCategoryName.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = c.SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Sub-Category wise: " + label;
                }
                else if (chart == "Region")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.RegionMasters on b.RegionId equals c.Id
                                        where c.Region.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = c.Region,
                                        }).ToList();

                    modelTitle = "Region wise: " + label;
                }
                else if (chart == "Office")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.EntityMasters on b.Company equals c.Id
                                        where c.EntityName.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Office wise: " + label;
                }
                else if (chart == "LOS")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.LOSMasters on a.LOSId equals c.Id
                                        where c.LOSName.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = c.LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "LOS wise: " + label;
                }
                else if (chart == "SBU")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.SBUMasters on a.SBUId equals c.Id
                                        where c.SBU.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = c.SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "SBU wise: " + label;
                }
                else if (chart == "SubSBU")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.SubSBUMasters on a.SubSBUId equals c.Id
                                        where c.SubSBU.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = c.SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Sub-SBU wise: " + label;
                }
                else if (chart == "GenderofComplainant")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        where b.Gender.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Gender of Complainant: " + label;
                }
                else if (chart == "GenderofRespondent")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.HR_Role on a.ComplaintId equals b.ComplentId
                                        join c in db.UserMasters on b.HRUserId equals c.Id
                                        where c.Gender.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(c.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Gender of Respondent: " + label;
                }
                else if (chart == "DesignationofComplainant")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        where b.Manager.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Designation of Complainant: " + label;
                }
                else if (chart == "DesignationofRespondent")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.HR_Role on a.ComplaintId equals b.ComplentId
                                        join c in db.UserMasters on b.HRUserId equals c.Id
                                        where c.Manager.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(c.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Designation of Respondent: " + label;
                }
                else if (chart == "ModeofComplaint")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        where b.TimeType.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "")
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Mode of Complaint: " + label;
                }
                else if (chart == "Ageing")
                {
                    int i = 0, j = 15;
                    DateTime? dateStart = dateFrom.ToDateTime().Value.AddDays(i);
                    DateTime? dateEnd = dateFrom.ToDateTime().Value.AddDays(j - 1);

                    if (label.ToLower().Trim().Replace(" ", "") == "15-30")
                    {
                        dateStart = dateFrom.ToDateTime().Value.AddDays(15);
                        dateEnd = dateFrom.ToDateTime().Value.AddDays(29);
                    }
                    else if (label.ToLower().Trim().Replace(" ", "") == "30-45")
                    {
                        dateStart = dateFrom.ToDateTime().Value.AddDays(29);
                        dateEnd = dateFrom.ToDateTime().Value.AddDays(44);
                    }
                    else if (label.ToLower().Trim().Replace(" ", "") == ("45 and Above").ToLower().Trim().Replace(" ", ""))
                    {
                        dateStart = dateFrom.ToDateTime().Value.AddDays(i);
                        dateEnd = dateTo.ToDateTime().Value;

                    }
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        where a.CreatedDate.Date >= dateStart && a.CreatedDate.Date <= dateEnd
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    //from a in complaintTList.Distinct().Where(a => a.CreatedDate.Date >= dateStart && a.CreatedDate.Date <= dateEnd)
                    //select a;

                    //dashboardPiChart.AddRange(ageingPiBarCharts.Select(a => new TableListPiAndBarChartVM
                    //{
                    //    LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                    //    Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                    //    SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                    //    CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                    //    CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                    //    CaseNo = a.ComplaintNo,
                    //    ComplaintId = a.ComplaintId,
                    //    SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                    //    SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                    //    //CaseStage = b.CaseType,
                    //    Region = new RegionMasterRepository().Get(a.RegionId).Region,
                    //}));

                    modelTitle = "Ageing/Case Closure: " + label;
                }

                if (TempData["ModelTitle"] != null)
                {
                    TempData.Remove("ModelTitle");
                }
                TempData["ModelTitle"] = modelTitle.ToString();
                TempData.Keep("ModelTitle");
                ViewBag.dashboardChartWiseData = dashboardPiChart;
                return PartialView("_DashboardChartTableContent");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpGet]
        public ActionResult DashboardBarChartTableBind(string dateFrom, string dateTo, string chart, string label, string year)
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                var Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();
                string modelTitle = string.Empty;
                int yearBack = DateTime.UtcNow.AddYears(-1).Year;
                int yearCurrent = DateTime.UtcNow.Year;
                if (dateFrom == "" && dateTo == "")
                {
                    dateFrom = new DateTime(yearBack, 1, 1).ToString("yyyy-MM-dd");
                    dateTo = DateTime.UtcNow.ToString("yyyy-MM-dd");
                }

                DateTime? dateF = dateFrom.ToDateTime();
                DateTime? dateT = dateTo.ToDateTime();

                List<LOSMasterVM> losMasterList = new List<LOSMasterVM>();
                List<SBUMasterVM> sbuMasterList = new List<SBUMasterVM>();
                List<SubSBUMasterVM> subSBUMasterList = new List<SubSBUMasterVM>();

                List<TableListPiAndBarChartVM> dashboardPiChart = new List<TableListPiAndBarChartVM>();
                List<EmployeeComplaintWorkFlow> complaintList = new List<EmployeeComplaintWorkFlow>();
                List<EmployeeComplaintWorkFlow> complaintTList = new List<EmployeeComplaintWorkFlow>();
                if (Role == Messages.LeadUser)
                {
                    var LOSData = new LOSMasterRepository().GetAll().Where(c => c.Status).ToList();
                    var SBUData = new SBUMasterRepository().GetAll().Where(c => c.Status).ToList();
                    var SubSBUData = new SubSBUMasterRepository().GetAll().Where(c => c.Status).ToList();

                    foreach (var item in LOSData)
                    {
                        bool isLOSValid = false;
                        if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                        {
                            int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                            foreach (int los in LosList)
                            {
                                if (los > 0 && los == Convert.ToInt32(sid))
                                {
                                    isLOSValid = true;
                                }
                            }
                        }
                        if (isLOSValid)
                        {
                            losMasterList.Add(item);
                        }
                    }
                    foreach (var item in SBUData)
                    {
                        bool isSBUValid = false;
                        if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                        {
                            int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                            foreach (int los in LosList)
                            {
                                if (los > 0 && los == Convert.ToInt32(sid))
                                {
                                    isSBUValid = true;
                                }
                            }
                        }
                        if (isSBUValid)
                        {
                            sbuMasterList.Add(item);
                        }
                    }
                    foreach (var item in SubSBUData)
                    {
                        bool isSubSBUValid = false;
                        if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                        {
                            int[] LosList = new Common().StringToIntArray(item.InvolvedUsersId);
                            foreach (int los in LosList)
                            {
                                if (los > 0 && los == Convert.ToInt32(sid))
                                {
                                    isSubSBUValid = true;
                                }
                            }
                        }
                        if (isSubSBUValid)
                        {
                            subSBUMasterList.Add(item);
                        }
                    }

                    if (losMasterList != null)
                    {
                        foreach (var item in losMasterList)
                        {
                            complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                        }
                        complaintTList.AddRange(complaintList);
                    }
                    if (sbuMasterList != null)
                    {
                        foreach (var item in sbuMasterList)
                        {
                            complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.SBUId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                        }
                        complaintTList.AddRange(complaintList);
                    }
                    if (subSBUMasterList != null)
                    {
                        foreach (var item in subSBUMasterList)
                        {
                            complaintList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.SubSBUId == item.Id && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                        }
                        complaintTList.AddRange(complaintList);
                    }
                }
                else
                {
                    complaintTList = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.CreatedDate >= dateF && x.CreatedDate <= dateT).ToList();
                }
                if (chart == "CaseType")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.HR_Role on a.ComplaintId equals c.ComplentId
                                        where c.CaseType.ToLower().Trim() == label.ToLower().Trim() && c.CreatedDate?.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Case Type: " + label;
                }
                else if (chart == "Category")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.CategoryMasters on a.EmployeeComplaintMaster.CategoryId equals c.Id
                                        where c.CategoryName.ToLower().Trim() == label.ToLower().Trim() && a.CreatedDate.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Category wise: " + label;
                }
                else if (chart == "SubCategory")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.SubCategoryMasters on a.EmployeeComplaintMaster.SubCategoryId equals c.Id
                                        where c.SubCategoryName.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "") && a.CreatedDate.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = c.SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Sub-Category wise: " + label;
                }
                else if (chart == "Region")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.RegionMasters on b.RegionId equals c.Id
                                        where c.Region.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "") && a.CreatedDate.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = c.Region,
                                        }).ToList();

                    modelTitle = "Region wise: " + label;
                }
                else if (chart == "Office")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.EntityMasters on b.Company equals c.Id
                                        where c.EntityName.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "") && a.CreatedDate.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Office wise: " + label;
                }
                else if (chart == "LOS")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.LOSMasters on a.LOSId equals c.Id
                                        where c.LOSName.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "") && a.CreatedDate.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = c.LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "LOS wise: " + label;
                }
                else if (chart == "SBU")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.SBUMasters on a.SBUId equals c.Id
                                        where c.SBU.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "") && a.CreatedDate.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = c.SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "SBU wise: " + label;
                }
                else if (chart == "SubSBU")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        join c in db.SubSBUMasters on a.SubSBUId equals c.Id
                                        where c.SubSBU.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "") && a.CreatedDate.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = c.SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Sub-SBU wise: " + label;
                }
                else if (chart == "GenderofComplainant")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        where b.Gender.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "") && a.CreatedDate.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Gender of Complainant: " + label;
                }
                else if (chart == "GenderofRespondent")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.HR_Role on a.ComplaintId equals b.ComplentId
                                        join c in db.UserMasters on b.HRUserId equals c.Id
                                        where c.Gender.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "") && b.CreatedDate?.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(c.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Gender of Respondent: " + label;
                }
                else if (chart == "DesignationofComplainant")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        where b.Manager.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "") && a.CreatedDate.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Designation of Complainant: " + label;
                }
                else if (chart == "DesignationofRespondent")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.HR_Role on a.ComplaintId equals b.ComplentId
                                        join c in db.UserMasters on b.HRUserId equals c.Id
                                        where c.Manager.ToLower().Trim().Replace(" ", "") == label.ToLower().Trim().Replace(" ", "") && b.CreatedDate?.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(c.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Designation of Respondent: " + label;
                }
                else if (chart == "ModeofComplaint")
                {
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        where b.TimeType.ToLower().Trim().Contains(label.ToLower().Trim()) && a.CreatedDate.Year.ToString() == year//== label.ToLower().Trim()
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();

                    modelTitle = "Mode of Complaint: " + label;
                }
                else if (chart == "Ageing")
                {
                    int i = 0, j = 15;
                    DateTime? dateStart = dateFrom.ToDateTime().Value.AddDays(i);
                    DateTime? dateEnd = dateFrom.ToDateTime().Value.AddDays(j - 1);

                    if (label.ToLower().Trim() == "15-30")
                    {
                        dateStart = dateFrom.ToDateTime().Value.AddDays(15);
                        dateEnd = dateFrom.ToDateTime().Value.AddDays(29);
                    }
                    else if (label.ToLower().Trim() == "30-45")
                    {
                        dateStart = dateFrom.ToDateTime().Value.AddDays(29);
                        dateEnd = dateFrom.ToDateTime().Value.AddDays(44);
                    }
                    else if (label.ToLower().Trim().Replace(" ", "") == ("45 and Above").ToLower().Trim().Replace(" ", ""))
                    {
                        dateStart = dateFrom.ToDateTime().Value.AddDays(i);
                        dateEnd = dateTo.ToDateTime().Value;

                    }
                    dashboardPiChart = (from a in complaintTList.Distinct()
                                        join b in db.UserMasters on a.CreatedBy equals b.Id
                                        where (a.CreatedDate.Date >= dateStart && a.CreatedDate.Date <= dateEnd) && a.CreatedDate.Year.ToString() == year
                                        select new TableListPiAndBarChartVM
                                        {
                                            LOSName = new LOSMasterRepository().Get(a.LOSId).LOSName,
                                            Category = new CategoryMastersRepository().Get(a.EmployeeComplaintMaster.CategoryId).CategoryName,
                                            SubCategory = new SubCategoryMastersRepository().Get(a.EmployeeComplaintMaster.SubCategoryId).SubCategoryName,
                                            CreatedBy = new UserMastersRepository().Get(a.CreatedBy).EmployeeName,
                                            CreatedOn = a.CreatedDate.ToString("dd/MM/yyyy"),
                                            CaseNo = a.ComplaintNo,
                                            ComplaintId = a.ComplaintId,
                                            SBU = new SBUMasterRepository().Get(a.SBUId).SBU,
                                            SubSBU = new SubSBUMasterRepository().Get(a.SubSBUId).SubSBU,
                                            Status = a.ActionType,
                                            Region = new RegionMasterRepository().Get(b.RegionId).Region,
                                        }).ToList();
                    modelTitle = "Ageing/Case Closure: " + label;
                }

                ViewBag.ModelTitle = modelTitle;
                ViewBag.dashboardChartWiseData = dashboardPiChart;
                return PartialView("_DashboardChartTableContent");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpPost]
        public ActionResult PrintBase64ToPPT(string jsonInput = "")
        {
            try
            {
                List<ImageBase64> reqObj = JsonConvert.DeserializeObject<List<ImageBase64>>(jsonInput);
                String path = Server.MapPath("~/Documents/ChartPPT/");
                string fileName = Server.MapPath("~/Documents/ChartPPT/Charts.pptx");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    if ((System.IO.File.Exists(fileName)))
                    {
                        System.IO.File.Delete(fileName);
                    }
                }
                Application pptAppliCation = new Application();
                Presentation pptPresentation = pptAppliCation.Presentations.Add(Microsoft.Office.Core.MsoTriState.msoTrue);
                int i = 0;
                foreach (var imageItem in reqObj)
                {
                    i++;
                    Microsoft.Office.Interop.PowerPoint.Slides slides;
                    Microsoft.Office.Interop.PowerPoint.Slide slide;
                    Microsoft.Office.Interop.PowerPoint.TextRange textRange;
                    Microsoft.Office.Interop.PowerPoint.CustomLayout customLayout = pptPresentation.SlideMaster.CustomLayouts[Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutText];
                    slides = pptPresentation.Slides;
                    slide = slides.AddSlide(i, customLayout);
                    textRange = slide.Shapes[1].TextFrame.TextRange;
                    textRange.Text = "                   " + imageItem.Heading;
                    textRange.Font.Name = "Arial";
                    textRange.Font.Size = 40;

                    Microsoft.Office.Interop.PowerPoint.Shape shape = slide.Shapes[2];
                    string imgFileName = Server.MapPath("~/Documents/ChartPPT/" + i + ".png");
                    if ((System.IO.File.Exists(imgFileName)))
                    {
                        System.IO.File.Delete(imgFileName);
                    }
                    System.IO.File.WriteAllBytes(imgFileName, imageItem.UrlBase64);
                    slide.Shapes.AddPicture(imgFileName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, shape.Left, shape.Top, shape.Width, shape.Height);
                }
                var random = new Random();
                var objGuid = new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
                var ServerSavePath = Path.Combine(Server.MapPath("~/Documents/ChartPPT/Charts_") + objGuid + ".pptx");
                pptPresentation.SaveAs(ServerSavePath, Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsDefault, Microsoft.Office.Core.MsoTriState.msoTrue);
                return File(ServerSavePath, "application/mspowerpoint", "Charts.pptx");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpPost]
        public ActionResult SendMailBase64ToPPT(string jsonInput = "", bool IsMailSend = false, string JsonData = "", string[] InvolveUserId = null)
        {
            try
            {
                List<ImageBase64> reqObj = JsonConvert.DeserializeObject<List<ImageBase64>>(jsonInput);
                String path = Server.MapPath("~/Documents/ChartPPT/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Application pptAppliCation = new Application();
                Presentation pptPresentation = pptAppliCation.Presentations.Add(Microsoft.Office.Core.MsoTriState.msoFalse);
                int i = 0;
                foreach (var imageItem in reqObj)
                {
                    i++;
                    Microsoft.Office.Interop.PowerPoint.Slides slides;
                    Microsoft.Office.Interop.PowerPoint.Slide slide;
                    Microsoft.Office.Interop.PowerPoint.TextRange textRange;
                    Microsoft.Office.Interop.PowerPoint.CustomLayout customLayout = pptPresentation.SlideMaster.CustomLayouts[Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutText];
                    slides = pptPresentation.Slides;
                    slide = slides.AddSlide(i, customLayout);
                    textRange = slide.Shapes[1].TextFrame.TextRange;
                    textRange.Text = "<p style='text-align:center;'>" + imageItem.Heading;
                    textRange.Font.Name = "Arial";
                    textRange.Font.Size = 40;

                    Microsoft.Office.Interop.PowerPoint.Shape shape = slide.Shapes[2];
                    string imgFileName = Server.MapPath("~/Documents/ChartPPT/" + i + ".png");
                    if ((System.IO.File.Exists(imgFileName)))
                    {
                        System.IO.File.Delete(imgFileName);
                    }
                    System.IO.File.WriteAllBytes(imgFileName, imageItem.UrlBase64);
                    slide.Shapes.AddPicture(imgFileName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, shape.Left, shape.Top, shape.Width, shape.Height);
                }
                var random = new Random();
                
                var objGuid = new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
                var ServerSavePath = Path.Combine(Server.MapPath("~/Documents/ChartPPT/Charts_") + objGuid + ".pptx");
                pptPresentation.SaveAs(ServerSavePath, Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsDefault, Microsoft.Office.Core.MsoTriState.msoTrue);
                pptPresentation.Close();
                pptAppliCation.Quit();
                List<DashboardMailSend> mailObj = JsonConvert.DeserializeObject<List<DashboardMailSend>>(JsonData);
                List<string> mailTo = new List<string>();
                List<string> mailBody = new List<string>();
                List<string> mailAttachment = new List<string>();
                string[] assignToUserId = InvolveUserId;//.Split(',').ToList();
                var Subject = string.Empty;
                var Comment = string.Empty;

                foreach (var item in mailObj)
                {
                    Subject = item.ChartType.ToUpper() + " " + item.DateFrom + " " + item.DateTo;
                    Comment = item.Comment;
                }

                foreach (var item in assignToUserId)
                {
                    mailTo.Add(new UserMastersRepository().Get(Convert.ToInt32(item)).WorkEmail);
                    mailBody.Add(@"<html><body><p>Dear " + new UserMastersRepository().Get(Convert.ToInt32(item)).EmployeeName + ",</p></br><p> Please Find attachment.</p></br><p>" + Comment + "</p><p>Thank You.</br></br>CMS</p></body></html>");
                }
                mailAttachment.Add(ServerSavePath);
                MailSend.SendEmailWithDifferentBody(mailTo, Subject, mailBody, "", "", mailAttachment);
                return new ReplyFormat().Success(Messages.SUCCESS);

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }


    }

}