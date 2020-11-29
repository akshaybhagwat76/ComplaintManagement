using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    [Authorize]
    public class CompetencyController : Controller
    {
        // GET: Competency
        public ActionResult Index()
        {
            ViewBag.lstCompetency = GetAll(1);
            var DataTableDetail = new HomeController().getDataTableDetail("Competency", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        [HttpGet]
        public ActionResult searchCompetency(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (search.ToLower() == Messages.Inactive.ToLower())
                {
                    ViewBag.lstCompetency = GetAll(0).ToList().Where(x => !x.Status).ToList();
                }
                if (search.ToLower() == Messages.Active.ToLower())
                {
                    ViewBag.lstCompetency = GetAll(0).ToList().Where(x => x.Status).ToList();
                }
                if (search.ToLower() != Messages.Active.ToLower() && search.ToLower() != Messages.Inactive.ToLower())
                {
                    ViewBag.lstCompetency = GetAll(0).ToList().Where(x => x.CompetencyName.Contains(search)).ToList();
                }

                var DataTableDetail = new HomeController().getDataTableDetail("Competency", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
            }
            return View("Index");
        }

        [HttpGet]
        public ActionResult LoadCompetency(int currentPageIndex, string range = "")
        {
            ViewBag.lstCompetency = GetAll(currentPageIndex, range);
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }

        [HttpPost]
        public JsonResult CheckIfExist(CompetencyMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.CompetencyName))
                {
                    var Competency = false;
                    if (data.Id == 0)
                    {
                        Competency = new CompetencyMastersRepository().IsExist(data.CompetencyName);
                    }
                    else
                    {
                        Competency = new CompetencyMastersRepository().IsExist(data.CompetencyName, data.Id);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, Competency);
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
        public List<CompetencyMasterVM> GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }
            
            var lst = new CompetencyMastersRepository().GetAll();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
             
                lst = (from Competency in lst
                       where Competency.CreatedDate.Date >= fromDate.Date && Competency.CreatedDate.Date <= toDate.Date
                       select Competency).ToList();
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
                lst = (from Competency in lst
                       select Competency)
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
        public ActionResult GetCompetency(string range, int currentPage)
        {
            ViewBag.lstCompetency = GetAll(currentPage,range);
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("Competency", null);
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
                retval = new CompetencyMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Competency"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpPost]
        public ActionResult AddOrUpdateCompetency(CompetencyMasterVM CompetencyVM)
        {
            try
            {
                var category = new CompetencyMastersRepository().AddOrUpdate(CompetencyVM);
                return new ReplyFormat().Success(Messages.SUCCESS, category);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        public ActionResult Create()
        {

            CompetencyMasterVM CompetencyMasterVM = new CompetencyMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageCompetencyMaster", CompetencyMasterVM);
            
        }

        public ActionResult Edit(int id,bool isView)
        {
            try
            {
                CompetencyMasterVM CompetencyVM = new CompetencyMastersRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                return View("ManageCompetencyMaster", CompetencyVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
           
        }
        [HttpPost]
        public ActionResult ImportCompetency(string file)
        {
            try
            {
                string retval = new CompetencyMastersRepository().UploadImportCompetency(file);
                if (!string.IsNullOrEmpty(retval))
                {
                    int count = new CompetencyMastersRepository().ImportCompetency(retval);
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