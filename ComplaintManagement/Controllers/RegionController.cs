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
    public class RegionController : Controller
    {
        // GET: Region
        public ActionResult Index()
        {
            ViewBag.lstRegion = GetAll(1);
            var DataTableDetail = new HomeController().getDataTableDetail("Region", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult SearchRegion(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (search.ToLower() == Messages.Inactive.ToLower())
                {
                    ViewBag.lstRegion = GetAll(0).ToList().Where(x => !x.Status).ToList();
                }
                if (search.ToLower() == Messages.Active.ToLower())
                {
                    ViewBag.lstRegion = GetAll(0).ToList().Where(x => x.Status).ToList();
                }
                if (search.ToLower() != Messages.Active.ToLower() && search.ToLower() != Messages.Inactive.ToLower())
                {
                    ViewBag.lstRegion = GetAll(0).ToList().Where(x => x.Region.Contains(search)).ToList();
                }

                var DataTableDetail = new HomeController().getDataTableDetail("Region", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult LoadRegion(int currentPageIndex, string range = "")
        {
            ViewBag.lstRegion = GetAll(currentPageIndex, range);
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        [HttpPost]
        public JsonResult CheckIfExist(RegionMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.Region))
                {
                    var Region = false;
                    if (data.Id == 0)
                    {
                        Region = new RegionMasterRepository().IsExist(data.Region);
                    }
                    else
                    {
                        Region = new RegionMasterRepository().IsExist(data.Region, data.Id);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, Region);
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
        public List<RegionMasterVM> GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }
           
            var lst = new RegionMasterRepository().GetAll();
            lstCount = lst.Count;


            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Region in lst
                       where Region.CreatedDate.Date >= fromDate.Date && Region.CreatedDate.Date <= toDate.Date
                       select Region).ToList();
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
                lst = (from Region in lst
                       select Region)
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
        public ActionResult GetRegion( int currentPage,string range)
        {
            ViewBag.lstRegion = GetAll(currentPage,range);
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("Region", null);
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
                retval = new RegionMasterRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Region"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateRegion(RegionMasterVM RegionVM)
        {
            try
            {
                var Region = new RegionMasterRepository().AddOrUpdate(RegionVM);
                return new ReplyFormat().Success(Messages.SUCCESS, Region);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            RegionMasterVM RegionMasterVM = new RegionMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageRegionMaster", RegionMasterVM);
        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                RegionMasterVM RegionVM = new RegionMasterRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                return View("ManageRegionMaster", RegionVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
        }
        [HttpPost]
        public ActionResult ImportRegion(string file)
        {
            try
            {
                string retval = new RegionMasterRepository().UploadImportRegion(file);
                if (!string.IsNullOrEmpty(retval))
                {
                    int count = new RegionMasterRepository().ImportRegion(retval);
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