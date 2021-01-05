using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace ComplaintManagement.Helpers
{
    public static class ViewExtensions
    {
        public static string RenderToString(this PartialViewResult partialView)
        {
            var httpContext = HttpContext.Current;

            if (httpContext == null)
            {
                throw new NotSupportedException("An HTTP context is required to render the partial view to a string");
            }

            var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();

            var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory().CreateController(httpContext.Request.RequestContext, controllerName);

            var controllerContext = new ControllerContext(httpContext.Request.RequestContext, controller);

            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    view.Render(new ViewContext(controllerContext, view, partialView.ViewData, partialView.TempData, tw), tw);
                }
            }

            return sb.ToString();
        }

        public static DateTime? ToDateTime(this string str, bool isWithTime = false)
        {
            if (string.IsNullOrWhiteSpace(str))
                return (DateTime?)null;

            string[] formats = { "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "dd/MM/yyyy h:mm:ss tt", "d/MM/yyyy h:mm:ss tt", "dd/M/yyyy h:mm:ss tt", "yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d", "yyyy-MM-dd h:mm:ss tt", "yyyy-M-dd h:mm:ss tt", "yyyy-MM-d h:mm:ss tt", "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "dd-MM-yyyy h:mm:ss tt", "d-MM-yyyy h:mm:ss tt", "dd-M-yyyy h:mm:ss tt", "yyyy/MM/dd", "yyyy/M/dd", "yyyy/MM/d", "yyyy/MM/dd  h:mm:ss tt", "yyyy/M/dd  h:mm:ss tt", "yyyy/MM/d  h:mm:ss tt", "d/M/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "MM/d/yyyy h:mm:ss tt", "M/dd/yyyy", "MM/dd/yyyy", "MM/d/yyyy", "yyyyMMdd", "d/M/yy" };
            //CultureInfo enGB = new CultureInfo("en-GB");

            if (isWithTime)
            {
                return DateTime.ParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            return DateTime.ParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
    }
}