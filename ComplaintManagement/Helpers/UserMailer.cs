using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;
using RazorEngine;
using System.Net;

namespace ComplaintManagement.Helpers
{
    public sealed class UserMailer : MailerBase
    {

        public UserMailer()
        {
            MasterName = null;
        }

        
        public string UserMailed(string code, string comment, string email, string browser, string ip)
        {
            try
            {
                ViewBag.OS = System.Environment.OSVersion.VersionString;
                ViewBag.Data = code;
                ViewBag.comment = comment;
                ViewBag.BrowserName = browser;
                ViewBag.IP = ip;
                ViewBag.Email = email;
             
                var body = comment;
                var msg = new System.Net.Mail.MailMessage
                {

                    Subject = "Complaint Management",
                    Body = body,
                    IsBodyHtml = true,
                };
                msg.To.Add(new System.Net.Mail.MailAddress(email));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (var smtp = new System.Net.Mail.SmtpClient())
                {
                    smtp.Send(msg);
                }

            }
            catch (Exception ex)
            {
                throw ex;
             
            }
            return "success";
        }

    }
}