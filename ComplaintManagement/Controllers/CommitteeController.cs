using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
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
        public ActionResult Create()
        {
            return View("ManageCommitteeMaster");
        }

        public ActionResult Edit()
        {
            return View("ManageCommitteeMaster");
        }
    }
}