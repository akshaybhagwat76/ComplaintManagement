using ComplaintManagement.Helpers;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using Elmah;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    public class CommitteeController : Controller
    {
        // GET: Committee
        public ActionResult Index()
        {
            var lst = new CommitteeMastersRepository().GetAll();
            dynamic output = new List<dynamic>();
            var lstUsers = new UserMastersRepository().GetAll();
            if (lst != null && lst.Count > 0)
            {
                foreach (CommitteeMasterVM com in lst)
                {
                    dynamic row = new ExpandoObject();
                    if (com.UserId>0)
                    {
                        if(lstUsers.FirstOrDefault(x => x.Id == com.UserId)!= null)
                        {
                            row.User = lstUsers.FirstOrDefault(x => x.Id == com.UserId).EmployeeName;
                        }
                        else
                        {
                            row.User = "";
                        }
                    }
                    row.Id = com.Id;
                    row.CommitteeName = com.CommitteeName;
                    row.Status = com.Status;

                    output.Add(row);
                }
            }

            var DataTableDetail = new HomeController().getDataTableDetail("Committee", null);
            ViewBag.lstCommittee = JsonConvert.SerializeObject(output);
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
            ViewBag.lstUser = new UserMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();

            return View("ManageCommitteeMaster", CommitteeMasterVM);

        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                CommitteeMasterVM CommitteeVM = new CommitteeMastersRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                ViewBag.lstUser = new UserMastersRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();


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