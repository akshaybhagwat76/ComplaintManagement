using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Xml.Linq;

namespace ComplaintManagement.Repository
{
    public class PolicyMasterRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public PolicyMasterRepository()
        {
                    
        }

        public PolicyMasterVM AddOrUpdate(PolicyMasterVM policyVM)
            {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var userId = identity.Identity.GetUserId();
                var userName = identity.Identity.GetUserName();

                //policyVM.CreatedBy = Convert.ToInt32(userId);
                //policyVM.CreatedByName = userName;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var policy = db.PolicyMasters.FirstOrDefault(p => p.PolicyId == policyVM.PolicyId);
                            if (policy == null)
                                {
                              
                               
                                policy = Mapper.Map<PolicyMasterVM, PolicyMaster>(policyVM);
                                if (IsExist(policy.PolicyName))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.PolicyMasters.Add(policy);
                                db.SaveChanges();
                                if(policyVM.PolicyDetailsMaster!=null && policyVM.PolicyDetailsMaster.Count > 0)
                                {
                                    foreach (PolicyDetailsMasterVM item in policyVM.PolicyDetailsMaster)
                                    {
                                        PolicyDetail policy_Detail = Mapper.Map<PolicyDetailsMasterVM, PolicyDetail>(item);
                                        if (policy_Detail != null) { policy_Detail.PolicyId = policy.PolicyId; };
                                        db.PolicyDetails.Add(policy_Detail);
                                        db.SaveChanges();

                                    }
                                }

                            
                                dbContextTransaction.Commit();
                                return Mapper.Map<PolicyMaster, PolicyMasterVM>(policy);
                            }
                            else
                            {
                                //policyVM.IsActive = true;
                                //policyVM.CreatedDate = policy.CreatedDate;
                                //policyVM.CreatedBy = policy.CreatedBy;
                                //policyVM.UpdatedDate = DateTime.UtcNow;
                                //policyVM.ModifiedBy = Convert.ToInt32(sid);
                                db.Entry(policy).CurrentValues.SetValues(policyVM);
                                if (IsExist(policy.PolicyName, policy.PolicyId))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                if (policyVM.PolicyDetailsMaster != null && policyVM.PolicyDetailsMaster.Count > 0)
                                {
                                    foreach (PolicyDetailsMasterVM item in policyVM.PolicyDetailsMaster)
                                    {
                                        PolicyDetail policy_Detail = db.PolicyDetails.FirstOrDefault(x=>x.Id == item.Id);
                                        if (policy_Detail != null)
                                        {
                                            PolicyDetail policyUpdatedDetail = Mapper.Map<PolicyDetailsMasterVM, PolicyDetail>(item);
                                            policyUpdatedDetail.PolicyId = policy.PolicyId;
                                            db.Entry(policy).CurrentValues.SetValues(policyUpdatedDetail);

                                        }
                                        else
                                        {
                                            if (policy_Detail != null) { policy_Detail.PolicyId = policy.PolicyId; };
                                            policy_Detail.PolicyId = policy.PolicyId;
                                            db.PolicyDetails.Add(policy_Detail);
                                        }
                                    }
                                }
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                return Mapper.Map<PolicyMaster, PolicyMasterVM>(policy);
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new PolicyMasterVM();
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
        public bool IsExist(string policyname)
        {
            return db.PolicyMasters.Count(x => x.IsActive && x.PolicyName.ToUpper() == policyname.ToUpper()) > 0;
        }

        public List<PolicyMasterVM> GetAll()
            {
            List<PolicyMaster> policy = new List<PolicyMaster>();
            List<PolicyMasterVM> policyList = new List<PolicyMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                List<CompanyVM> companies = new CompanyMasterRepository().GetAll();
                List<OperationMasterVM> operation = new OperationMasterRepository().GetAll();





                policy = db.PolicyMasters.Where(i => i.IsActive==false).ToList().OrderByDescending(x => x.PolicyId).ToList();

                

                var q = (from pm in policy
                         join cm in companies on
                         pm.CompanyId equals cm.Id
                         select new PolicyMaster
                         {
                             CompanyName=cm.CompanyName,
                             Able=pm.Able,
                             PolicyNumber= pm.PolicyNumber,
                             PolicyName =   pm.PolicyName,
                             Validuntil = pm.Validuntil,
                             OperationId = pm.OperationId 
                         }).ToList();

                var newq= (from pm in q
                           join om in operation on
                           pm.OperationId equals om.Id
                           select new PolicyMaster
                           {
                               OperationName=om.OperationName,
                               Able = pm.Able,
                               PolicyNumber = pm.PolicyNumber,
                               PolicyName = pm.PolicyName,  
                               Validuntil = pm.Validuntil,
                               CompanyName=pm.CompanyName   
                               
                           }).ToList();
                

                if (policy != null && policy.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (PolicyMaster item in newq)
                    {
                        PolicyMasterVM polObj = Mapper.Map<PolicyMaster, PolicyMasterVM>(item);
                        if (polObj != null)
                        {
                            polObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == polObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == polObj.CreatedBy).EmployeeName : string.Empty;
                           polObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == polObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == polObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            policyList.Add(polObj);
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
               
                // throw new Exception(ex.Message.ToString());
            }
            return policyList;
        }

        public bool IsExist(string policyname, int id)
        {
            return db.PolicyMasters.Count(x => x.IsActive && x.PolicyName.ToUpper() == policyname.ToUpper() && x.PolicyId != id) > 0;
        }

        public bool Delete(int id)
        {
            var data = db.PolicyMasters.FirstOrDefault(p => p.PolicyId == id);
            if (data != null)
            {
                data.IsActive = true;
            }
            db.SaveChanges();
            return true;
        }

        public PolicyMasterVM Get(int id)
        {
            PolicyMaster policy = new PolicyMaster();
            try
            {
                policy = db.PolicyMasters.FirstOrDefault(i => i.PolicyId == id && i.IsActive==false);
                if (policy == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<PolicyMaster, PolicyMasterVM>(policy);
        }

    }
}