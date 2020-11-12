using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ComplaintManagement.Helpers
{
    public class ReplyFormat
    {
        public class Reply
        {
            public string status { get; set; }
            public string msg { get; set; }
            public string error { get; set; }
            public dynamic data { get; set; }
        }

        public JsonResult Success(string txt, dynamic data = null)
        {
            return PrepareReply(false, txt, data);
        }

        public JsonResult Error(string txt)
        {
            return PrepareReply(true, txt);
        }

        public JsonResult PrepareReply(bool isError, string txt, dynamic data = null)
        {
            var reply = new Reply
            {
                status = isError ? Messages.FAIL : Messages.SUCCESS,
                msg = isError ? "" : txt,
                error = isError ? txt : null,
                data = data,
            };
            return new JsonNetResult(reply, JsonRequestBehavior.AllowGet);
        }
    }
}