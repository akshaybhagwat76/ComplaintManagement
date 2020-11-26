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
    public class LocationController : Controller
    {
        // GET: Location
        public ActionResult Index()
        {
            ViewBag.lstLocation = GetAll(1);
            var DataTableDetail = new HomeController().getDataTableDetail("Location", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult SearchLocation(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (search.ToLower() == Messages.Inactive.ToLower())
                {
                    ViewBag.lstLocation = GetAll(1).ToList().Where(x => !x.Status).ToList();
                }
                if (search.ToLower() == Messages.Active.ToLower())
                {
                    ViewBag.lstLocation = GetAll(1).ToList().Where(x => x.Status).ToList();
                }
                if (search.ToLower() != Messages.Active.ToLower() && search.ToLower() != Messages.Inactive.ToLower())
                {
                    ViewBag.lstLocation = GetAll(1).ToList().Where(x => x.LocationName.Contains(search)).ToList();
                }

                var DataTableDetail = new HomeController().getDataTableDetail("Location", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult LoadLocation(int currentPageIndex, string range = "")
        {
            ViewBag.lstLocation = GetAll(currentPageIndex, range);
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        public List<LocationMasterVM> GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            var lst = new LocationMastersRepository().GetAll();
            lstCount = lst.Count;


            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Location in lst
                       where Location.CreatedDate.Date >= fromDate.Date && Location.CreatedDate.Date <= toDate.Date
                       select Location).ToList();
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
                lst = (from Location in lst
                       select Location)
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
        public ActionResult GetLocations(string range, int currentPage)
        {
            ViewBag.lstLocation = GetAll(currentPage,range);
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("Categories", null);
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
                retval = new LocationMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Location"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateLocation(LocationMasterVM LocationVM)
        {
            try
            {
                var Location = new LocationMastersRepository().AddOrUpdate(LocationVM);
                return new ReplyFormat().Success(Messages.SUCCESS, Location);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            LocationMasterVM LocationMasterVM = new LocationMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageLocationMaster", LocationMasterVM);
            
        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                LocationMasterVM LocationVM = new LocationMastersRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                return View("ManageLocationMaster", LocationVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
           
        }
        [HttpPost]
        public JsonResult CheckIfExist(LocationMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.LocationName))
                {
                    var Location = false;
                    if (data.Id == 0)
                    {
                        Location = new LocationMastersRepository().IsExist(data.LocationName);
                    }
                    else
                    {
                        Location = new LocationMastersRepository().IsExist(data.LocationName, data.Id);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, Location);
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