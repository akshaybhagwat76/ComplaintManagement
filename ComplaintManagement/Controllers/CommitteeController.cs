using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using System;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class CommitteeController : Controller
    {
        // GET: Committee
        public ActionResult Index()
        {
            ViewBag.lstCommittee = new CommitteeMastersRepository().GetAll();
            var DataTableDetail = new HomeController().getDataTableDetail("Committee", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            bool retval = true;
            try
            {
                retval = new CommitteeMastersRepository().Delete(id);
                return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "Committee"), null);
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        [HttpPost]
        public ActionResult AddOrUpdateCommittee(CommitteeMasterVM CommitteeVM)
        {
            try
            {
                var Committee = new CommitteeMastersRepository().AddOrUpdate(CommitteeVM);
                return new ReplyFormat().Success(Messages.SUCCESS, Committee);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        public ActionResult Create()
        {
            CommitteeMasterVM CommitteeMasterVM = new CommitteeMasterVM();
            ViewBag.PageType = "Create";
            return View("ManageCommitteeMaster", CommitteeMasterVM);
           
        }

        public ActionResult Edit(int id)
        {
            try
            {
                CommitteeMasterVM CommitteeVM = new CommitteeMastersRepository().Get(id);
                ViewBag.PageType = "Edit";
                return View("ManageCommitteeMaster", CommitteeVM);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
           
        }
    }
}