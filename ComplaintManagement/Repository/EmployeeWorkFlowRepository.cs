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
    public class EmployeeWorkFlowRepository
    {
        public EmployeeComplaintWorkFlowVM AddOrUpdate(EmployeeComplaintWorkFlowVM WorkFlowVM,DB_A6A061_complaintuserEntities db)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {

                    try
                    {
                        var WorkFlow = db.EmployeeComplaintWorkFlows.FirstOrDefault(p => p.Id == WorkFlowVM.Id);
                        if (WorkFlow == null)
                        {
                            WorkFlowVM.IsActive = true;
                            WorkFlowVM.CreatedDate = DateTime.UtcNow;
                            WorkFlowVM.CreatedBy = Convert.ToInt32(sid);
                            WorkFlow = Mapper.Map<EmployeeComplaintWorkFlowVM, EmployeeComplaintWorkFlow>(WorkFlowVM);

                            db.EmployeeComplaintWorkFlows.Add(WorkFlow);
                            db.SaveChanges();

                            return Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(WorkFlow);
                        }
                        else
                        {
                            WorkFlowVM.IsActive = true;
                            WorkFlowVM.CreatedDate = WorkFlow.CreatedDate;
                            WorkFlowVM.CreatedBy = WorkFlow.CreatedBy;
                            WorkFlowVM.UpdatedDate = DateTime.UtcNow;
                            WorkFlowVM.ModifiedBy = Convert.ToInt32(sid);
                            db.Entry(WorkFlow).CurrentValues.SetValues(WorkFlowVM);

                            db.SaveChanges();


                            return Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(WorkFlow);

                        }
                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message.ToString());
                    }
                }
                return new EmployeeComplaintWorkFlowVM();
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