using ComplaintManagement.Models;
using ComplaintManagement.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Configuration;

namespace ComplaintManagement.Helpers
{
    public sealed class MailSend
    {
        
        public static void SendEmail(List<string> to, string subject, string body, string from = "", string password = "", List<string> attachmentUrls = null, bool isBodyHtml = true, List<string> cc = null)
        {
            try
            {
                //if (string.IsNullOrEmpty(from))
                //    from = "crmnotification@variablesoft.com";
                //if (string.IsNullOrEmpty(password))
                //    password = "a1b2z9@@!";

                var msg = new MailMessage
                {
                    //From = new MailAdd0ress(from),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHtml
                };
                foreach (var item in to)
                {
                    msg.To.Add(new MailAddress(item));
                }

                if (cc != null) cc.ForEach(x => { msg.CC.Add(x); });

                if (attachmentUrls != null)
                {
                    foreach (var attachmentUrl in attachmentUrls)
                    {
                        if (!string.IsNullOrEmpty(attachmentUrl))
                        {
                            var attachment = new Attachment(attachmentUrl);
                            msg.Attachments.Add(attachment);
                        }
                    }
                }
                // your remote SMTP server IP.
                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = true;
                NEVER_EAT_POISON_Disable_CertificateValidation();
                smtp.Send(msg);
                //using (var smtp = new SmtpClient
                //{
                //    Host = "mail.variablesoft.com",
                //    Port = 25,
                //    Credentials = new System.Net.NetworkCredential(from, password),
                //    EnableSsl = false
                //})
                //{


            }
            catch (Exception ex)
            {
                throw;
                // Throw exception or Log exception and error emails.
            }
        }

        public static void SendEmailWithByteExcel(List<string> to, string subject, string body, byte[] attachmentUrls = null, string from = "", string password = "", bool isBodyHtml = true, List<string> cc = null)
        {
            try
            {
                //if (string.IsNullOrEmpty(from))
                //    from = "crmnotification@variablesoft.com";
                //if (string.IsNullOrEmpty(password))
                //    password = "a1b2z9@@!";

                var msg = new MailMessage
                {
                    //From = new MailAdd0ress(from),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHtml
                };
                foreach (var item in to)
                {
                    msg.To.Add(new MailAddress(item));
                }

                if (cc != null) cc.ForEach(x => { msg.CC.Add(x); });

                System.IO.MemoryStream ms = new System.IO.MemoryStream(attachmentUrls);
                msg.Attachments.Add(new System.Net.Mail.Attachment(ms, "Reports.xls", "application/vnd.ms-excel"));

                //if (attachmentUrls != null)
                //{
                //    foreach (var attachmentUrl in attachmentUrls)
                //    {
                //        if (!string.IsNullOrEmpty(attachmentUrl))
                //        {
                //            var attachment = new Attachment(attachmentUrl);
                //            msg.Attachments.Add(attachment);
                //        }
                //    }
                //}
                // your remote SMTP server IP.
                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = true;
                NEVER_EAT_POISON_Disable_CertificateValidation();

                smtp.Send(msg);
                //using (var smtp = new SmtpClient
                //{
                //    Host = "mail.variablesoft.com",
                //    Port = 25,
                //    Credentials = new System.Net.NetworkCredential(from, password),
                //    EnableSsl = false
                //})
                //{


            }
            catch (Exception ex)
            {
                throw;
                // Throw exception or Log exception and error emails.
            }
        }
        public static void NEVER_EAT_POISON_Disable_CertificateValidation()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors
                )
                {
                    return true;
                };
        }



        public static void SendEmailWithDifferentBody(List<string> to, string subject, List<string> body, int? complanitId = null, string from = "", string password = "", List<string> attachmentUrls = null, bool isBodyHtml = true, List<string> cc = null)
        {

            Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)configurationFile.GetSectionGroup("system.net/mailSettings");
            string emailFromWebConfig = mailSettings.Smtp.From.ToString();
            try
            {
                //if (string.IsNullOrEmpty(from))
                //    from = "crmnotification@variablesoft.com";
                //if (string.IsNullOrEmpty(password))
                //    password = "a1b2z9@@!";

                ////var msg = new MailMessage
                ////{
                ////    //From = new MailAdd0ress(from),
                ////    Subject = subject,
                ////    Body = body,
                ////    IsBodyHtml = isBodyHtml
                ////};
                ///
               

                var msg = new MailMessage();
                if (attachmentUrls != null)
                {
                    foreach (var attachmentUrl in attachmentUrls)
                    {
                        if (!string.IsNullOrEmpty(attachmentUrl))
                        {
                            var attachment = new Attachment(attachmentUrl);
                            msg.Attachments.Add(attachment);
                        }
                    }
                }
                string[] bodyItem = body.ToArray();
                int i = 0;
                foreach (var Item in to)
                {
                    
                    msg.Subject = subject;
                    msg.Body = bodyItem[i];
                    msg.IsBodyHtml = isBodyHtml;
                    msg.To.Add(new MailAddress(Item));
                    //msg.To = Item;
                    SmtpClient smtp = new SmtpClient();
                    smtp.EnableSsl = true;
                    NEVER_EAT_POISON_Disable_CertificateValidation();
                    smtp.Send(msg);
                    i++;
                    new EmployeeComplaintHistoryRepository().AddEmailHistory(emailFromWebConfig, Item, complanitId, null, "Success", subject);
                }
               

                //if (cc != null) cc.ForEach(x => { msg.CC.Add(x); });
            }
            catch (Exception ex)
            {
                new EmployeeComplaintHistoryRepository().AddEmailHistory(emailFromWebConfig, null, complanitId, ex.Message, "Failure", subject);
            }
        }



        //2/1/2021
        public static void SendEmailJobDue(string to, string subject, string body, byte[] attachmentUrls = null, List<string> cc = null, string from = "", string password = "", bool isBodyHtml = true)
        {

            Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)configurationFile.GetSectionGroup("system.net/mailSettings");
            string emailFromWebConfig = mailSettings.Smtp.From.ToString();

            try
            {
               
                //if (string.IsNullOrEmpty(from))
                //    from = "crmnotification@variablesoft.com";
                //if (string.IsNullOrEmpty(password))
                //    password = "a1b2z9@@!";

                var msg = new MailMessage
                {
                    //From = new MailAddress(from),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHtml
                };

                msg.To.Add(new MailAddress(to));


                if (cc != null) cc.ForEach(x => { msg.CC.Add(x); });

                System.IO.MemoryStream ms = new System.IO.MemoryStream(attachmentUrls);
                msg.Attachments.Add(new System.Net.Mail.Attachment(ms, "ComplaintReport.xls", "application/vnd.ms-excel"));

                // your remote SMTP server IP.
                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = true;
                NEVER_EAT_POISON_Disable_CertificateValidation();
                smtp.Send(msg);
                //new EmployeeComplaintHistoryRepository().AddEmailHistory(emailFromWebConfig, Item, complanitId, null, "Success", subject);

            }
            catch (Exception ex)
            {
                throw;
                // Throw exception or Log exception and error emails.
            }

        }


    }
}