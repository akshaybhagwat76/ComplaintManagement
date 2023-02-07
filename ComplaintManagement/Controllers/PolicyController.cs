using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
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
    public class PolicyController : Controller
    {
        // GET: Policy
        public ActionResult Index()
        {
       
            ViewBag.lstpolicy = GetAll(1);

            var DataTableDetail = new HomeController().getDataTableDetail("Policy", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;

            return View();



        }
        [HttpGet]
        public ActionResult LoadPolicies(int currentPageIndex)
        {
            ViewBag.lstPolicy = GetAll(currentPageIndex);
            
            return View("Index");
        }
        [HttpPost]
        public ActionResult AddOrUpdatepolicy(PolicyMasterVM policyVM)
        {
            try
            {
                var policy = new PolicyMasterRepository().AddOrUpdate(policyVM);
                return new ReplyFormat().Success(Messages.SUCCESS, policy);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            bool retval = true;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    id = CryptoEngineUtils.Decrypt(id.Replace(" ", "+"), true);

                    retval = new PolicyMasterRepository().Delete(Convert.ToInt32(id));
                    return new ReplyFormat().Success(string.Format(Messages.DELETE_MESSAGE, "policy"), null);
                }
                else
                {
                    return new ReplyFormat().Error(Messages.FAIL);
                }
            }
            catch (Exception ex)
            {

                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        public ActionResult Create()
        {
            PolicyMasterVM policyMasterVM = new PolicyMasterVM();
            ViewBag.PageType = "Create";
            ViewBag.companylist  = new CompanyMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompanyName, Value = d.Id.ToString() }).ToList();
            ViewBag.operationList = new OperationMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.OperationName, Value = d.Id.ToString() }).ToList();
            //ViewBag.operationList = new PolicyMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d., Value = d.Id.ToString() }).ToList();

            return View("ManagePolicyMaster", policyMasterVM);
        }

        [HttpPost]
        public JsonResult CheckIfExist(PolicyMasterVM data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.PolicyName))
                {
                    var policy = false;
                    if (data.PolicyId == 0)
                    {
                        policy = new PolicyMasterRepository().IsExist(data.PolicyName);
                    }
                    else
                    {
                        policy = new PolicyMasterRepository().IsExist(data.PolicyName, data.PolicyId);
                    }
                    return new ReplyFormat().Success(Messages.SUCCESS, policy);
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
        [HttpGet]
        public ActionResult GetPolicy( int currentPage)
        {
            ViewBag.lstPolicy = GetAll(currentPage);
       
            var DataTableDetail = new HomeController().getDataTableDetail("Categories", null);
            ViewBag.Page = DataTableDetail.Item1;
            ViewBag.PageIndex = DataTableDetail.Item2;
            return View("Index");
        }

        public ActionResult SearchPolicy(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (search.ToLower() == Messages.Inactive.ToLower())
                {
                    ViewBag.lstpolicy = GetAll(0).ToList().Where(x => !x.Status).ToList();
                }
                if (search.ToLower() == Messages.Active.ToLower())
                {
                    ViewBag.lstpolicy = GetAll(0).ToList().Where(x => x.Status).ToList();
                }
                if (search.ToLower() != Messages.Active.ToLower() && search.ToLower() != Messages.Inactive.ToLower())
                {
                    ViewBag.lstpolicy = GetAll(0).ToList().Where(x => x.PolicyName.Contains(search)).ToList();
                }

                var DataTableDetail = new HomeController().getDataTableDetail("Policy", null);
                ViewBag.Page = DataTableDetail.Item1;
                ViewBag.PageIndex = DataTableDetail.Item2;
            }
            return View("Index");
        }

        public List<PolicyMasterVM> GetAll(int currentPage)
        {
            int maxRows = 10; int lstCount = 0;
            if (currentPage == 0)
            {
                maxRows = 2147483647;
            }

            var lst = new PolicyMasterRepository().GetAll();
            lstCount = lst.Count;



            lst = (from policy in lst
                   select policy)
         .OrderByDescending(customer => customer.PolicyId)
         .Skip((currentPage - 1) * maxRows)
         .Take(maxRows).ToList();
            double pageCount = (double)((decimal)lstCount / Convert.ToDecimal(maxRows));
            ViewBag.PageCount = (int)Math.Ceiling(pageCount);

            ViewBag.CurrentPageIndex = currentPage;
            return lst;

        }

        public ActionResult Edit(string PolicyId, bool isView)
        {
            try
            {
                if (!string.IsNullOrEmpty(PolicyId))
                {
                    PolicyId = CryptoEngineUtils.Decrypt(PolicyId.Replace(" ", "+"), true);

                    PolicyMasterVM policyVM = new PolicyMasterRepository().Get(Convert.ToInt32(PolicyId));
                    ViewBag.ViewState = isView;
                    ViewBag.PageType = !isView ? "Edit Policy" : "View";
                    ViewBag.companylist = new CompanyMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.CompanyName, Value = d.Id.ToString() }).ToList();
                    ViewBag.operationList = new OperationMasterRepository().GetAll().ToList().Select(d => new SelectListItem { Text = d.OperationName, Value = d.Id.ToString() }).ToList();
                    policyVM.Validuntil = DateTime.Now;
                  

                    return View("ManagePolicyMaster", policyVM);
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
        }

    }
}