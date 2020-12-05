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
    public class EmployeeComplaintHistoryRepository
    {
        public void AddComplaintHistory(string remarks, int complaintId,string actionType, DB_A6A061_complaintuserEntities db)
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

        public List<EmployeeComplaintHistoryVM> GetAll()
        {
            List<EmployeeComplaintHistory> EmployeeComplaintHistory = new List<EmployeeComplaintHistory>();
            List<EmployeeComplaintHistoryVM> EmployeeComplaintHistoryList = new List<EmployeeComplaintHistoryVM>();

            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                using (DB_A6A061_complaintuserEntities db  = new DB_A6A061_complaintuserEntities())
                {
                    var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(sid))
                    {
                        List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                        EmployeeComplaintHistory = db.EmployeeComplaintHistories.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                        if (EmployeeComplaintHistory != null && EmployeeComplaintHistory.Count > 0 && usersList != null && usersList.Count > 0)
                        {
                            foreach (EmployeeComplaintHistory item in EmployeeComplaintHistory)
                            {
                                EmployeeComplaintHistoryVM catObj = Mapper.Map<EmployeeComplaintHistory, EmployeeComplaintHistoryVM>(item);
                                if (catObj != null)
                                {
                                    catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                    EmployeeComplaintHistoryList.Add(catObj);
                                }
                            }
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return EmployeeComplaintHistoryList;
        }
    }
}