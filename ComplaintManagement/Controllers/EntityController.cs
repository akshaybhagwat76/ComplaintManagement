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
    public class EntityController : Controller
    {
        // GET: Entity
        public ActionResult Index()
        {
            ViewBag.lstEntity = GetAll(1);
            var DataTableDetail = new HomeController().getDataTableDetail("Entity", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult SearchEntity(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (search.ToLower() == Messages.Inactive.ToLower())
                {
                    ViewBag.lstEntity = GetAll(1).ToList().Where(x => !x.Status).ToList();
                }
                if (search.ToLower() == Messages.Active.ToLower())
                {
                    ViewBag.lstEntity = GetAll(1).ToList().Where(x => x.Status).ToList();
                }
                if (search.ToLower() != Messages.Active.ToLower() && search.ToLower() != Messages.Inactive.ToLower())
                {
                    ViewBag.lstEntity = GetAll(1).ToList().Where(x => x.EntityName.Contains(search)).ToList();
                }

                var DataTableDetail = new HomeController().getDataTableDetail("Entity", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult LoadEntity(int currentPageIndex, string range = "")
        {
            ViewBag.lstEntity = GetAll(currentPageIndex, range);
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        [HttpPost]
        public JsonResult CheckIfExist(EntityMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.EntityName))
                {
                    var Entity = false;
                    if (data.Id == 0)
                    {
                        Entity = new EntityMasterRepository().IsExist(data.EntityName);
                    }
                    else
                    {
                        Entity = new EntityMasterRepository().IsExist(data.EntityName, data.Id);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, Entity);
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
        public List<EntityMasterVM> GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            var lst = new EntityMasterRepository().GetAll();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Entity in lst
                       where Entity.CreatedDate >= fromDate && Entity.CreatedDate <= toDate
                       select Entity).ToList();
                lstCount = lst.Count;
                lst = (lst)
                        .OrderBy(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return lst;
            }
            else
            {
                lst = (from Entity in lst
                       select Entity)
             .OrderBy(customer => customer.Id)
             .Skip((currentPage - 1) * maxRows)
             .Take(maxRows).ToList();
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return lst;
            }
        }

        [HttpGet]
        public ActionResult GetEntity(string range, int currentPage)
        {
            ViewBag.lstEntity = GetAll(currentPage,range);
            ViewBag.startDate = range.Split(',')[0];
            ViewBag.toDate = range.Split(',')[1];

            var DataTableDetail = new HomeController().getDataTableDetail("Entity", null);
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
                retval = new EntityMasterRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Entity"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateEntity(EntityMasterVM EntityVM)
        {
            try
            {
                var Entity = new EntityMasterRepository().AddOrUpdate(EntityVM);
                return new ReplyFormat().Success(Messages.SUCCESS, Entity);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            EntityMasterVM EntityMasterVM = new EntityMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageEntityMaster", EntityMasterVM);
        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                EntityMasterVM EntityVM = new EntityMasterRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                return View("ManageEntityMaster", EntityVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
        }
    }
}