using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace ComplaintManagement.Repository
{
    public class NotificationAlertRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();
        public void AddNotificatioAlert(string NotificationContent, int AssignToUserID)
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                string usertypeValue = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var NotificationAlertData = new NotificationAlert();
                    NotificationAlertData.Status = true;
                    NotificationAlertData.NotificationContent = NotificationContent;
                    NotificationAlertData.AssignToUserId = AssignToUserID;
                    NotificationAlertData.CreatedDate= DateTime.UtcNow;
                    NotificationAlertData.UserId = Convert.ToInt32(sid);

                    db.NotificationAlerts.Add(NotificationAlertData);
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException dve)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)), "Notification Alert");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(dve);
                throw new Exception(string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)));
            }
            catch (Exception ex)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, ex.Message.ToString(), "Notification Alert");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
        }

        public List<NotificationAlert> Get(int id)
        {
            List<NotificationAlert> NotificationAlert = new List<NotificationAlert>();
            try
            {
                NotificationAlert = db.NotificationAlerts.Where(i => i.AssignToUserId == id).ToList();
                if (NotificationAlert == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return NotificationAlert;
        }
        public void UpdateNotificatioAlert()
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                string usertypeValue = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                int loginuserId = Convert.ToInt32(sid);
                if (!string.IsNullOrEmpty(sid))
                {
                    var data = db.NotificationAlerts.Where(p => p.AssignToUserId == loginuserId);

                    var ls = new int[] { 2, 3, 4 };
                    
                        var some = db.NotificationAlerts.Where(x => loginuserId.ToString().Contains(x.AssignToUserId.ToString())).ToList();
                        some.ForEach(a => a.Status = false);
                        db.SaveChanges();

                    
                }
            }
            catch (DbEntityValidationException dve)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(dve);
                throw new Exception(string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)));
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}