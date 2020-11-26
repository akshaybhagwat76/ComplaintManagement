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
    [Authorize]
    public class CommitteeController : Controller
    {
        // GET: Committee
        public ActionResult Index()
        {
            var lst = GetAll(1);

            var DataTableDetail = new HomeController().getDataTableDetail("Committee", null);
            ViewBag.lstCommittee = JsonConvert.SerializeObject(lst);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View();
        }
        public ActionResult SearchCommittee(string search)
        {
            var originalList = GetAll(0);
            dynamic output = new List<dynamic>();

            foreach (var item in originalList)
            {
                var itemIndex = (IDictionary<string, object>)item;
                foreach (var kvp in itemIndex)
                {
                    if (kvp.Value != null)
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
            }
            ViewBag.lstCommittee = JsonConvert.SerializeObject(output);
            return View("Index");
        }
        [HttpGet]
        public ActionResult LoadCommittee(int currentPageIndex, string range = "")
        {
            ViewBag.lstCommittee = JsonConvert.SerializeObject(GetAll(currentPageIndex, range));
            if (!string.IsNullOrEmpty(range))
            {
                ViewBag.startDate = range.Split(',')[0];
                ViewBag.toDate = range.Split(',')[1];
            }
            return View("Index");
        }
        [HttpPost]
        public JsonResult CheckIfExist(CommitteeMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.CommitteeName))
                {
                    var Committee = false;
                    if (data.Id == 0)
                    {
                        Committee = new CommitteeMastersRepository().IsExist(data.CommitteeName);
                    }
                    else
                    {
                        Committee = new CommitteeMastersRepository().IsExist(data.CommitteeName, data.Id);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, Committee);
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
        public dynamic GetAll(int currentPage, string range = "")
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var lst = new CommitteeMastersRepository().GetAll();
            lstCount = lst.Count;
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                lst = (from Committee in lst
                       where Committee.CreatedDate.Date >= fromDate.Date && Committee.CreatedDate.Date <= toDate.Date
                       select Committee).ToList();

                lstCount = lst.Count;
                lst = (lst)
                        .OrderByDescending(customer => customer.Id)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

                dynamic output = new List<dynamic>();
                var lstUsers = new UserMastersRepository().GetAll();
                if (lst != null && lst.Count > 0)
                {
                    foreach (CommitteeMasterVM com in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (com.UserId > 0)
                        {
                            if (lstUsers.FirstOrDefault(x => x.Id == com.UserId) != null)
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
                        row.UpdatedByName = com.UpdatedByName;
                        row.CreatedByName = com.CreatedByName;
                        row.ModifiedBy = com.ModifiedBy;
                        row.CreatedBy = com.CreatedBy;
                        row.UpdatedDate = com.UpdatedDate;
                        row.CreatedDate = com.CreatedDate;

                        row.Status = com.Status;

                        output.Add(row);
                    }
                }

                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;
                return output;
            }
            else
            {
                dynamic output = new List<dynamic>();

                lst = (from Committee in lst
                       select Committee)
           .OrderByDescending(user => user.Id)
           .Skip((currentPage - 1) * maxRows)
           .Take(maxRows).ToList();
                var lstUsers = new UserMastersRepository().GetAll();


                if (lst != null && lst.Count > 0)
                {
                    foreach (CommitteeMasterVM com in lst)
                    {
                        dynamic row = new ExpandoObject();
                        if (com.UserId > 0)
                        {
                            if (lstUsers.FirstOrDefault(x => x.Id == com.UserId) != null)
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
                        row.UpdatedByName = com.UpdatedByName;
                        row.CreatedByName = com.CreatedByName;
                        row.ModifiedBy = com.ModifiedBy;
                        row.CreatedBy = com.CreatedBy;
                        row.UpdatedDate = com.UpdatedDate;
                        row.CreatedDate = com.CreatedDate;
                        output.Add(row);
                    }
                }
                double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
                ViewBag.PageCount = (int)Math.Ceiling(pageCount);

                ViewBag.CurrentPageIndex = currentPage;

                return output;

            }
        }

        [HttpGet]
        public ActionResult GetCommittee(string range, int currentPage)
        {
            ViewBag.lstCommittee = JsonConvert.SerializeObject(GetAll(currentPage, range));
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
            ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();

            return View("ManageCommitteeMaster", CommitteeMasterVM);

        }

        public ActionResult Edit(int id, bool isView)
        {
            try
            {
                CommitteeMasterVM CommitteeVM = new CommitteeMastersRepository().Get(id);
                ViewBag.ViewState = isView;
                ViewBag.PageType = !isView ? "Edit" : "View";
                ViewBag.lstUser = new UserMastersRepository().GetAll().Where(c => c.Status).ToList().Select(d => new SelectListItem { Text = d.EmployeeName, Value = d.Id.ToString() }).ToList();


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