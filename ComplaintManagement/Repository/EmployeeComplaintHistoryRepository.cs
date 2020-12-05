using ComplaintManagement.Models;
using Elmah;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace ComplaintManagement.Repository
{
    public class EmployeeComplaintHistoryRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public void AddComplaintHistory(string remarks, int complaintId,string actionType)
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    EmployeeComplaintHistory employeeComplaintHistory = new EmployeeComplaintHistory();
                    employeeComplaintHistory.IsActive = true;
                    employeeComplaintHistory.CreatedDate = DateTime.UtcNow;
                    employeeComplaintHistory.CreatedBy = Convert.ToInt32(sid);
                    employeeComplaintHistory.Remarks = remarks;
                    employeeComplaintHistory.ComplaintId = complaintId;
                    employeeComplaintHistory.ActionType = actionType;
                    employeeComplaintHistory.UserType = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();
                    db.EmployeeComplaintHistories.Add(employeeComplaintHistory);
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