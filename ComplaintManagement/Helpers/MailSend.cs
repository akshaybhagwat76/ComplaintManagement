using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

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


        public static void SendEmailWithDifferentBody(List<string> to, string subject, List<string> body, string from = "", string password = "", List<string> attachmentUrls = null, bool isBodyHtml = true, List<string> cc = null)
        {
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
                    smtp.Send(msg);
                    i++;
                }


                //if (cc != null) cc.ForEach(x => { msg.CC.Add(x); });




            }
            catch (Exception ex)
            {
                //throw;
                // Throw exception or Log exception and error emails.
            }
        }



        //2/1/2021
        public static void SendEmailJobDue(string to, string subject, string body, byte[] attachmentUrls = null, List<string> cc = null, string from = "", string password = "", bool isBodyHtml = true)

        {
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
                smtp.Send(msg);

            }
            catch (Exception ex)
            {
                throw;
                // Throw exception or Log exception and error emails.
            }

        }


    }
}