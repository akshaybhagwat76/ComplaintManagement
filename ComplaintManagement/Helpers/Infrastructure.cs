using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Web.Mvc;
namespace ComplaintManagement.Helpers
{
    public class JsonNetResult : JsonResult
    {
        private JsonNetResult()
        {
            this.ContentType = "application/json";
        }

        public JsonNetResult(object data, JsonRequestBehavior jsonRequestBehavior)
        {
            this.ContentEncoding = Encoding.UTF8;
            this.ContentType = "application/json";
            this.Data = data;
            this.JsonRequestBehavior = jsonRequestBehavior;
        }
    }
}