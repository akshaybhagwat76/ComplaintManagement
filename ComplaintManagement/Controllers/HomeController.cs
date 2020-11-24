using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

    }
}