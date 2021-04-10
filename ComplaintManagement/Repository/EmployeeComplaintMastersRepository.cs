using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace ComplaintManagement.Repository
{
    public class EmployeeComplaintMastersRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public EmployeeCompliantMasterVM AddOrUpdate(EmployeeCompliantMasterVM EmployeeComplaintVM, string flag = null)
        {
            int ids = 0;
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                
                if (!string.IsNullOrEmpty(sid))
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        this.db.Database.CommandTimeout = 180;
                        try
                        {
                            var EmployeeComplaint = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == EmployeeComplaintVM.Id);
                            if (EmployeeComplaint == null)
                            {
                                
                                EmployeeComplaintVM.IsActive = true;
                                EmployeeComplaintVM.CreatedDate = DateTime.UtcNow;
                                EmployeeComplaintVM.CreatedBy = Convert.ToInt32(sid);
                                EmployeeComplaint = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMaster>(EmployeeComplaintVM);
                                EmployeeComplaint.UserId = EmployeeComplaintVM.UserId = EmployeeComplaint.UserId == 0 ? Convert.ToInt32(sid) : EmployeeComplaint.UserId;
                                EmployeeComplaint.Status = true;
                                db.EmployeeComplaintMasters.Add(EmployeeComplaint);
                                db.SaveChanges();

                                EmployeeComplaintVM.EmployeeComplaintMasterId = EmployeeComplaint.Id;
                                ids= EmployeeComplaint.Id;
                                if (flag != "B")
                                {
                                    EmployeeComplaintMastersHistory EmployeeComplaintMasters_History = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMastersHistory>(EmployeeComplaintVM);
                                    if (EmployeeComplaintMasters_History != null) { EmployeeComplaintMasters_History.EntityState = Messages.Added; EmployeeComplaintMasters_History.EmployeeComplaintMasterId = EmployeeComplaintMasters_History.Id; };
                                    db.EmployeeComplaintMastersHistories.Add(EmployeeComplaintMasters_History);
                                    new EmployeeComplaintHistoryRepository().AddComplaintHistory(EmployeeComplaint.Remark, EmployeeComplaint.Id, EmployeeComplaint.ComplaintStatus, db);
                                    db.SaveChanges();
                                }
                                dbContextTransaction.Commit();
                            }
                            else
                            {
                                EmployeeComplaintVM.IsActive = true;
                                EmployeeComplaintVM.CreatedDate = EmployeeComplaint.CreatedDate;
                                EmployeeComplaintVM.CreatedBy = EmployeeComplaint.CreatedBy;
                                EmployeeComplaintVM.UpdatedDate = DateTime.UtcNow;
                                EmployeeComplaintVM.UpdatedBy = Convert.ToInt32(sid);
                                if (!string.IsNullOrEmpty(EmployeeComplaint.Attachments))
                                {
                                    if (!string.IsNullOrEmpty(EmployeeComplaintVM.Attachments))
                                    {
                                        List<string> newAttachments = EmployeeComplaintVM.Attachments.Split(',').ToList();
                                        List<string> oldAttachments = EmployeeComplaint.Attachments.Split(',').ToList();
                                        List<string> updatedAttachements = new List<string>();
                                        foreach (string fileName in oldAttachments.Concat(newAttachments).ToList())
                                        {
                                            if (!string.IsNullOrEmpty(fileName) && fileName.Length > 5)
                                            {
                                                updatedAttachements.Add(fileName);
                                            }
                                        }
                                        EmployeeComplaintVM.Attachments = string.Join(",", updatedAttachements);
                                    }
                                    else
                                    {
                                        EmployeeComplaintVM.Attachments = EmployeeComplaint.Attachments;
                                    }
                                }
                                db.Entry(EmployeeComplaint).CurrentValues.SetValues(EmployeeComplaintVM);
                                ids = EmployeeComplaint.Id;
                                if (flag != "B")
                                {
                                    EmployeeComplaintMastersHistory EmployeeComplaintMasters_History = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMastersHistory>(EmployeeComplaintVM);
                                    if (EmployeeComplaintMasters_History != null) { EmployeeComplaintMasters_History.EntityState = Messages.Updated; EmployeeComplaintMasters_History.EmployeeComplaintMasterId = EmployeeComplaintMasters_History.Id; };
                                    db.EmployeeComplaintMastersHistories.Add(EmployeeComplaintMasters_History);
                                    new EmployeeComplaintHistoryRepository().AddComplaintHistory(EmployeeComplaint.Remark, EmployeeComplaint.Id, EmployeeComplaint.ComplaintStatus, db);
                                    db.SaveChanges();
                                }
                                dbContextTransaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            if (!string.IsNullOrEmpty(EmployeeComplaintVM.Attachments))
                            {
                                foreach (string file in EmployeeComplaintVM.Attachments.Split(','))
                                {
                                    if (!string.IsNullOrEmpty(file))
                                    {
                                        string filePath = "~/Documents/" + file;

                                        if (File.Exists(HttpContext.Current.Server.MapPath(filePath)))
                                        {
                                            File.Delete(HttpContext.Current.Server.MapPath(filePath));
                                        }

                                    }
                                }
                            }
                            new EmployeeComplaintHistoryRepository().ErrorLogHistory(ids, ex.Message.ToString(), Messages.Draft);
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                if (flag == "B")
                {
                    return EmployeeComplaintVM;
                }
                else
                {
                    return new EmployeeCompliantMasterVM();
                }

            }
            catch (DbEntityValidationException dve)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(ids, string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)), Messages.Draft);
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(dve);
                throw new Exception(string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)));
            }
            catch (Exception ex)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(ids, ex.Message.ToString(), Messages.Draft);
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
        }

        public UserMasterVM AddOrUpdateSaveCommittee(UserMasterVM EmployeeComplaintVM, string UserInvolved)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();

                if (!string.IsNullOrEmpty(sid))
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        this.db.Database.CommandTimeout = 180;
                        try
                        {
                            //CommitteeRole commitRole = new CommitteeRole();
                            int loginUserId = Convert.ToInt32(sid);
                            var commitRole = db.CommitteeRoles.Where(p => p.ComplaintId == EmployeeComplaintVM.ComplaintId && p.CommitteeUserId== loginUserId).FirstOrDefault();

                            if (commitRole == null)
                            {
                                commitRole = new CommitteeRole();
                                commitRole.IsActive = true;
                                commitRole.CreatedDate = DateTime.UtcNow;
                                commitRole.Userid = Convert.ToInt32(EmployeeComplaintVM.UserId);
                                commitRole.Status = "CommitteeSubmitted"; //committee save
                                commitRole.ComplaintId = EmployeeComplaintVM.ComplaintId;
                                commitRole.InvolvedUsersId = UserInvolved;
                                commitRole.CashTypeId = EmployeeComplaintVM.CashTypeId;
                                commitRole.CommitteeUserId = Convert.ToInt32(sid);
                                commitRole.Remark = EmployeeComplaintVM.RemarkCommittee;
                                commitRole.Attachment = EmployeeComplaintVM.AttachmentsCommittee;
                                db.CommitteeRoles.Add(commitRole);
                                db.SaveChanges();

                                //var ComplaintMaster = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == EmployeeComplaintVM.ComplaintId);
                                //if (ComplaintMaster != null)
                                //{
                                    
                                //    ComplaintMaster.LastPerformedBy = sid;
                                //    db.Entry(ComplaintMaster).State = EntityState.Modified;
                                //    db.SaveChanges();
                                //}

                                //var WorkFlow = db.EmployeeComplaintWorkFlows.FirstOrDefault(p => p.ComplaintId == EmployeeComplaintVM.ComplaintId);
                                //if (WorkFlow != null)
                                //{
                                //    WorkFlow.ActionType = "CommitteeSubmitted";
                                //    WorkFlow.UpdatedDate = DateTime.UtcNow;
                                //    db.Entry(WorkFlow).State = EntityState.Modified;

                                //    db.SaveChanges();
                                //}

                                
                               
                            }
                            else
                            {
                                commitRole.IsActive = true;
                                commitRole.UpdatedDate = DateTime.UtcNow;
                                commitRole.Status = "CommitteeSubmitted";
                                commitRole.InvolvedUsersId = UserInvolved;
                                commitRole.CashTypeId = EmployeeComplaintVM.CashTypeId;
                                commitRole.Remark = EmployeeComplaintVM.RemarkCommittee;

                                //if (!string.IsNullOrEmpty(commitRole.Attachment))
                                //{
                                    if (!string.IsNullOrEmpty(EmployeeComplaintVM.AttachmentsCommittee))
                                    {
                                        List<string> newAttachments = EmployeeComplaintVM.AttachmentsCommittee.Split(',').ToList();
                                        List<string> oldAttachments = commitRole.Attachment.Split(',').ToList();
                                        List<string> updatedAttachements = new List<string>();
                                        foreach (string fileName in oldAttachments.Concat(newAttachments).ToList())
                                        {
                                            if (!string.IsNullOrEmpty(fileName) && fileName.Length > 5)
                                            {
                                                updatedAttachements.Add(fileName);
                                            }
                                        }
                                        commitRole.Attachment = string.Join(",", updatedAttachements);
                                    }
                                    else
                                    {
                                        commitRole.Attachment = commitRole.Attachment;
                                    }
                                //}

                                //commitRole.Attachment = EmployeeComplaintVM.AttachmentsCommittee;

                                db.Entry(commitRole).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                           

                            var ComplaintMaster = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == EmployeeComplaintVM.ComplaintId);
                            if (ComplaintMaster != null)
                            {

                                ComplaintMaster.LastPerformedBy = sid;
                                db.Entry(ComplaintMaster).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            new EmployeeComplaintHistoryRepository().AddComplaintHistory(EmployeeComplaintVM.RemarkCommittee, EmployeeComplaintVM.ComplaintId, "Save", db);
                            dbContextTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            if (!string.IsNullOrEmpty(EmployeeComplaintVM.AttachmentsCommittee))
                            {
                                foreach (string file in EmployeeComplaintVM.AttachmentsCommittee.Split(','))
                                {
                                    if (!string.IsNullOrEmpty(file))
                                    {
                                        string filePath = "~/Documents/" + file;

                                        if (File.Exists(HttpContext.Current.Server.MapPath(filePath)))
                                        {
                                            File.Delete(HttpContext.Current.Server.MapPath(filePath));
                                        }

                                    }
                                }
                            }
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new UserMasterVM();
            }
            catch (DbEntityValidationException dve)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(EmployeeComplaintVM.ComplaintId, string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)), Messages.Draft);
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(dve);
                throw new Exception(string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)));
            }
            catch (Exception ex)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(EmployeeComplaintVM.ComplaintId, ex.Message.ToString(), Messages.Draft);
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
        }

        public UserMasterVM AddOrUpdateBackToBUHCCommittee(UserMasterVM EmployeeComplaintVM, string UserInvolved,string HrRoleId)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();

                if (!string.IsNullOrEmpty(sid))
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        this.db.Database.CommandTimeout = 180;
                        try
                        {
                            //CommitteeRole commitRole = new CommitteeRole();
                            int loginUserId = Convert.ToInt32(sid);
                            var commitRole = db.CommitteeRoles.FirstOrDefault(p => p.ComplaintId == EmployeeComplaintVM.ComplaintId && p.CommitteeUserId== loginUserId);

                            if (commitRole == null)
                            {
                                commitRole = new CommitteeRole();
                                commitRole.IsActive = true;
                                commitRole.CreatedDate = DateTime.UtcNow;
                                commitRole.Userid = Convert.ToInt32(EmployeeComplaintVM.UserId);
                                commitRole.Status = Messages.SUBMITTED; //committee save
                                commitRole.ComplaintId = EmployeeComplaintVM.ComplaintId;
                                commitRole.InvolvedUsersId = UserInvolved;
                                commitRole.CashTypeId = EmployeeComplaintVM.CashTypeId;
                                commitRole.CommitteeUserId = Convert.ToInt32(sid);
                                commitRole.Remark = EmployeeComplaintVM.RemarkCommittee;
                                commitRole.Attachment = EmployeeComplaintVM.AttachmentsCommittee;
                                db.CommitteeRoles.Add(commitRole);
                                db.SaveChanges();
                            }
                            else
                            {
                                commitRole.IsActive = true;
                                commitRole.UpdatedDate = DateTime.UtcNow;
                                commitRole.Status = Messages.SUBMITTED;
                                commitRole.InvolvedUsersId = UserInvolved;
                                commitRole.CashTypeId = EmployeeComplaintVM.CashTypeId;
                                commitRole.Remark = EmployeeComplaintVM.RemarkCommittee;
                                commitRole.Attachment = EmployeeComplaintVM.AttachmentsCommittee;
                                db.Entry(commitRole).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            var ComplaintMaster = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == EmployeeComplaintVM.ComplaintId);
                            if (ComplaintMaster != null)
                            {
                                ComplaintMaster.ComplaintStatus = Messages.SUBMITTED;
                                ComplaintMaster.UpdatedDate = DateTime.UtcNow;
                                ComplaintMaster.UpdatedBy = Convert.ToInt32(sid);
                                ComplaintMaster.LastPerformedBy = sid;
                                db.Entry(ComplaintMaster).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            var WorkFlow = db.EmployeeComplaintWorkFlows.FirstOrDefault(p => p.ComplaintId == EmployeeComplaintVM.ComplaintId);
                            if (WorkFlow != null)
                            {
                                WorkFlow.ActionType = Messages.SUBMITTED;
                                WorkFlow.UpdatedDate = DateTime.UtcNow;
                                db.Entry(WorkFlow).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            int hrRoleId = Convert.ToInt32(HrRoleId);
                            var HrRole = db.HR_Role.FirstOrDefault(p => p.Id == hrRoleId);
                            if (HrRole != null)
                            {
                                HrRole.CommitteeUSerId = commitRole.Id;
                                db.Entry(HrRole).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            new EmployeeComplaintHistoryRepository().AddComplaintHistory(EmployeeComplaintVM.RemarkCommittee, EmployeeComplaintVM.ComplaintId, Messages.BackToBUHC, db);

                            //Notification Saving Work
                            var LOSName = string.Empty; var CategoryName = string.Empty; var SubCategoryName = string.Empty;
                            string NotificationContent = string.Empty;
                            List<string> mailTo = new List<string>();
                            List<string> mailBody = new List<string>();
                            CategoryName = new CategoryMastersRepository().Get(Convert.ToInt32(ComplaintMaster.CategoryId)).CategoryName;
                            SubCategoryName = new SubCategoryMastersRepository().Get(Convert.ToInt32(ComplaintMaster.SubCategoryId)).SubCategoryName;
                            var userData = new UserMastersRepository().Get(Convert.ToInt32(sid));
                            LOSName = new LOSMasterRepository().Get(WorkFlow.LOSId).LOSName;
                            NotificationContent = WorkFlow.ComplaintNo+" (" + LOSName + "-" + CategoryName + "-" + SubCategoryName + ") has been send back  by " + userData.EmployeeName + " on " + DateTime.UtcNow.ToString("dd/MM/yyyy") + " for your approval.";

                            var CommitteeMemberData = (from u in db.CommitteeMasters
                                                       where u.IsActive
                                                       select u).FirstOrDefault();
                            List<string> assignToUserId = WorkFlow.AssignedUserRoles.Split(',').ToList();
                            foreach (var item in assignToUserId)
                            {
                                new NotificationAlertRepository().AddNotificatioAlert(NotificationContent, Convert.ToInt32(item));
                                mailTo.Add(new UserMastersRepository().Get(Convert.ToInt32(item)).WorkEmail);
                                mailBody.Add(@"<html><body><p>Subject:Compliant Send back by committee" + ",</p></br><p>Receipt-HR User" + ",</p></br><p>Dear " + new UserMastersRepository().Get(Convert.ToInt32(item)).EmployeeName + ",</p></br><p>" + NotificationContent + "</p><p>Thank You.</br></br>Employee Assistance Portal</p></body></html>");
                            }

                            dbContextTransaction.Commit();
                            MailSend.SendEmailWithDifferentBody( mailTo, "Compliant Send back by committee", mailBody, EmployeeComplaintVM.ComplaintId);
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            if (!string.IsNullOrEmpty(EmployeeComplaintVM.AttachmentsCommittee))
                            {
                                foreach (string file in EmployeeComplaintVM.AttachmentsCommittee.Split(','))
                                {
                                    if (!string.IsNullOrEmpty(file))
                                    {
                                        string filePath = "~/Documents/" + file;

                                        if (File.Exists(HttpContext.Current.Server.MapPath(filePath)))
                                        {
                                            File.Delete(HttpContext.Current.Server.MapPath(filePath));
                                        }

                                    }
                                }
                            }
                            new EmployeeComplaintHistoryRepository().ErrorLogHistory(EmployeeComplaintVM.ComplaintId, ex.Message.ToString(), "Compliant Send back by committee");
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new UserMasterVM();
            }
            catch (DbEntityValidationException dve)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(EmployeeComplaintVM.ComplaintId, string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)), "Compliant Send back by committee");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(dve);
                throw new Exception(string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)));
            }
            catch (Exception ex)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(EmployeeComplaintVM.ComplaintId, ex.Message.ToString(), "Compliant Send back by committee");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
        }

        public List<EmployeeCompliantMasterVM> GetAll()
        {
            List<EmployeeComplaintMaster> EmployeeComplaint = new List<EmployeeComplaintMaster>();
            List<EmployeeCompliantMasterVM> EmployeeCompliant_oneList = new List<EmployeeCompliantMasterVM>();
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                    EmployeeComplaint = db.EmployeeComplaintMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                    EmployeeComplaint = EmployeeComplaint.Where(x => x.CreatedBy == Convert.ToInt32(sid)).ToList();
                    if (EmployeeComplaint != null && EmployeeComplaint.Count > 0 && usersList != null && usersList.Count > 0)
                    {
                        foreach (EmployeeComplaintMaster item in EmployeeComplaint)
                        {
                            EmployeeCompliantMasterVM catObj = Mapper.Map<EmployeeComplaintMaster, EmployeeCompliantMasterVM>(item);
                            if (catObj != null)
                            {
                                catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.UpdatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.UpdatedBy).EmployeeName : Messages.NotAvailable;
                                catObj.CategoryName = new CategoryMastersRepository().Get(item.CategoryId) != null ? new CategoryMastersRepository().Get(item.CategoryId).CategoryName : Messages.NotAvailable;
                                catObj.SubCategoryName = new SubCategoryMastersRepository().Get(item.SubCategoryId) != null ? new SubCategoryMastersRepository().Get(item.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                                catObj.EmployeeName = usersList.FirstOrDefault(x => x.Id == catObj.UserId) != null ? usersList.FirstOrDefault(x => x.Id == catObj.UserId).EmployeeName : Messages.NotAvailable;

                                var CommitteeMemberData = (from u in db.CommitteeMasters
                                                           where u.IsActive
                                                           select u).FirstOrDefault();
                                string PendingWith = string.Empty;
                                if (item.LastPerformedBy !=null && item.LastPerformedBy != "")
                                {
                                    if (item.ComplaintStatus == Messages.COMMITTEE)
                                    {
                                        PendingWith = Messages.COMMITTEE;
                                    }
                                    else
                                    {
                                        PendingWith = Messages.HRUser;
                                    }

                                    //string[] lastPerformBy = item.LastPerformedBy.Split(',');
                                    //foreach (var Ids in lastPerformBy)
                                    //{
                                    //    string UserName = string.Empty;
                                    //    string UserRole = string.Empty;
                                    //    int Id = Convert.ToInt32(Ids);
                                    //    var isCommitteeUserAssigned = CommitteeMemberData.UserId.Split(',').Where(i => i.ToString() == Id.ToString()).Count() > 0;
                                    //    if (isCommitteeUserAssigned)
                                    //    {
                                    //        UserRole = Messages.COMMITTEE;
                                    //    }
                                    //    else
                                    //    {
                                    //        UserRole = new UserMastersRepository().Get(Convert.ToInt32(Ids)).Type;
                                    //    }
                                    //    //UserName = new UserMastersRepository().Get(Convert.ToInt32(Ids)).EmployeeName;// + ", ";
                                    //    PendingWith = UserRole;//+= UserName + "(" + UserRole + ")" + ", ";
                                    //}
                                }
                                else
                                {
                                    PendingWith = Messages.NotAvailable;
                                }

                                catObj.LastPerformedBy = PendingWith;
                                var ComplaintNo= new EmployeeWorkFlowRepository().Get(item.Id);
                                if (ComplaintNo != null)
                                    catObj.ComplaintNo = ComplaintNo.ComplaintNo;
                                else
                                    catObj.ComplaintNo = "";
                                EmployeeCompliant_oneList.Add(catObj);
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
            return EmployeeCompliant_oneList;
        }
        public List<EmployeeCompliantMasterVM> GetAllList()
        {
            List<EmployeeComplaintMaster> EmployeeComplaint = new List<EmployeeComplaintMaster>();
            List<EmployeeCompliantMasterVM> EmployeeCompliant_oneList = new List<EmployeeCompliantMasterVM>();
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                    EmployeeComplaint = db.EmployeeComplaintMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                    if (EmployeeComplaint != null && EmployeeComplaint.Count > 0 && usersList != null && usersList.Count > 0)
                    {
                        foreach (EmployeeComplaintMaster item in EmployeeComplaint)
                        {
                            EmployeeCompliantMasterVM catObj = Mapper.Map<EmployeeComplaintMaster, EmployeeCompliantMasterVM>(item);
                            if (catObj != null)
                            {
                                catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.UpdatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.UpdatedBy).EmployeeName : Messages.NotAvailable;
                                catObj.CategoryName = new CategoryMastersRepository().Get(item.CategoryId) != null ? new CategoryMastersRepository().Get(item.CategoryId).CategoryName : Messages.NotAvailable;
                                catObj.SubCategoryName = new SubCategoryMastersRepository().Get(item.SubCategoryId) != null ? new SubCategoryMastersRepository().Get(item.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                                catObj.EmployeeName = usersList.FirstOrDefault(x => x.Id == catObj.UserId) != null ? usersList.FirstOrDefault(x => x.Id == catObj.UserId).EmployeeName : Messages.NotAvailable;

                                EmployeeCompliant_oneList.Add(catObj);
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
            return EmployeeCompliant_oneList;
        }
        public List<CategoryMasterHistoryVM> GetAllHistory()
        {
            List<CategoryMasters_History> category = new List<CategoryMasters_History>();
            List<CategoryMasterHistoryVM> categoryList = new List<CategoryMasterHistoryVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                category = db.CategoryMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (category != null && category.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (CategoryMasters_History item in category)
                    {
                        CategoryMasterHistoryVM catObj = Mapper.Map<CategoryMasters_History, CategoryMasterHistoryVM>(item);
                        if (catObj != null)
                        {
                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            categoryList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return categoryList;
        }

        public EmployeeCompliantMasterVM Get(int id)
        {
            EmployeeComplaintMaster EmployeeComplaint = new EmployeeComplaintMaster();
            try
            {
                EmployeeComplaint = db.EmployeeComplaintMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (EmployeeComplaint == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<EmployeeComplaintMaster, EmployeeCompliantMasterVM>(EmployeeComplaint);
        }

        //public HrRoleUserMasterVM GetHrRole(int id)
        //{
        //    HrRoleUserMasterVM hrRoleUserMasterVM = new HrRoleUserMasterVM();
        //    try
        //    {
        //        hrRoleUserMasterVM = db.HR_Role.FirstOrDefault(i.IsActive && i => i.HRUserId == id );
        //        if (hrRoleUserMasterVM == null)
        //        {
        //            throw new Exception(Messages.BAD_DATA);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
        //        throw new Exception(ex.Message.ToString());
        //    }
        //    return Mapper.Map<HR_Role, HrRoleUserMasterVM>(hrRoleUserMasterVM);
        //}



        public bool Delete(int id)
        {
            var data = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }

        public bool SubmitComplaint(int id)
        {
            var Complaintdata = Get(id);
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    EmployeeComplaintWorkFlowVM employeeComplaintWorkFlowDto = new EmployeeComplaintWorkFlowVM();
                    if (Complaintdata != null)
                    {
                        
                        if (Complaintdata.CreatedBy > 0)
                        {
                            List<RoleMasterVM> roleMasterLineItem = new List<RoleMasterVM>();
                            var userData = new UserMastersRepository().Get(Complaintdata.CreatedBy); string rolesId = string.Empty;
                            string assignedUsersroleId = string.Empty;
                            var LOSName = string.Empty; var CategoryName = string.Empty; var SubCategoryName = string.Empty;
                            if (userData != null && userData.LOSId > 0 && userData.CompentencyId > 0 && userData.SBUId > 0 && userData.SubSBUId > 0)
                            {
                                int losId = userData.LOSId; int competencyId = userData.CompentencyId; int SBUId = userData.SBUId; int subSBUId = userData.SubSBUId;
                                var rolesMaster = new RoleMasterRepoitory().GetAll();
                                if (rolesMaster != null && rolesMaster.Count > 0)
                                {
                                    foreach (RoleMasterVM role in rolesMaster)
                                    {
                                        bool isLOSValid = false; bool isSBUValid = false;
                                        bool isSUBSBUValid = false; bool isComptencyValid = false;
                                        if (role != null)
                                        {
                                            if (!string.IsNullOrEmpty(role.LOSId))
                                            {
                                                int[] LosList = new Common().StringToIntArray(role.LOSId);
                                                foreach (int los in LosList)
                                                {
                                                    if (los > 0 && los == losId)
                                                    {
                                                        isLOSValid = true;
                                                    }
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(role.CompetencyId))
                                            {
                                                int[] CompetencyList = new Common().StringToIntArray(role.CompetencyId);
                                                foreach (int Competency in CompetencyList)
                                                {
                                                    if (Competency > 0 && Competency == competencyId)
                                                    {
                                                        isComptencyValid = true;
                                                    }
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(role.SBUId))
                                            {
                                                int[] SBUList = new Common().StringToIntArray(role.SBUId);
                                                foreach (int SBU in SBUList)
                                                {
                                                    if (SBU > 0 && SBU == SBUId)
                                                    {
                                                        isSBUValid = true;
                                                    }
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(role.SubSBUId))
                                            {
                                                int[] SubSBUList = new Common().StringToIntArray(role.SubSBUId);
                                                foreach (int SUBSbu in SubSBUList)
                                                {
                                                    if (SUBSbu > 0 && SUBSbu == subSBUId)
                                                    {
                                                        isSUBSBUValid = true;
                                                    }
                                                }
                                            }

                                            //Final Criteria Check
                                            if (isLOSValid && isComptencyValid && isSBUValid && isSUBSBUValid)
                                            {
                                                roleMasterLineItem.Add(role);
                                            }
                                        }
                                    }
                                }
                                if (roleMasterLineItem.Count == 0)
                                {
                                    throw new Exception(Messages.RoleMasterComplaintCriteriaNotFound);
                                }
                                else
                                {
                                    var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                                    //roleMasterLineItem.ForEach(x => rolesId = string.Join(",", x.Id));
                                    // roleMasterLineItem.ForEach(x => assignedUsersroleId = string.Join(",", x.UserId));

                                    string count = CreateComplaintNo();
                                    assignedUsersroleId = string.Join(",", roleMasterLineItem.Select(x => x.UserId).ToList());
                                    rolesId = string.Join(",", roleMasterLineItem.Select(x => x.Id).ToList());
                                    LOSName = losId > 0 ? new LOSMasterRepository().Get(losId) != null ? new LOSMasterRepository().Get(losId).LOSName : Messages.NotAvailable : Messages.NotAvailable;

                                    employeeComplaintWorkFlowDto.ActionType = Messages.SUBMITTED;
                                    employeeComplaintWorkFlowDto.ComplaintId = Complaintdata.Id;
                                    employeeComplaintWorkFlowDto.Remarks = Complaintdata.Remark;
                                    employeeComplaintWorkFlowDto.UserType = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();
                                    employeeComplaintWorkFlowDto.RoleId = rolesId;
                                    employeeComplaintWorkFlowDto.AssignedUserRoles = assignedUsersroleId; employeeComplaintWorkFlowDto.DueDate = Complaintdata.DueDate;
                                    employeeComplaintWorkFlowDto.LOSId = losId; employeeComplaintWorkFlowDto.SBUId = SBUId; employeeComplaintWorkFlowDto.SubSBUId = subSBUId; employeeComplaintWorkFlowDto.CompentencyId = competencyId;
                                    employeeComplaintWorkFlowDto.ComplaintNo = DateTime.UtcNow.ToString("dd")+""+ DateTime.UtcNow.ToString("MM")+""+DateTime.UtcNow.ToString("yyyy")+"_"+ count;
                                }
                                
                            }

                            var employeeComplaint = db.EmployeeComplaintMasters.Find(Complaintdata.Id);

                            employeeComplaint.IsSubmitted = true;
                            employeeComplaint.ComplaintStatus = Messages.SUBMITTED;
                            employeeComplaint.LastPerformedBy = assignedUsersroleId;

                            new EmployeeComplaintHistoryRepository().AddComplaintHistory(Complaintdata.Remark, Complaintdata.Id, Messages.SUBMITTED, db);
                            db.SaveChanges();
                            if (employeeComplaintWorkFlowDto != null && employeeComplaintWorkFlowDto.ComplaintId > 0)
                            {
                                new EmployeeWorkFlowRepository().AddOrUpdate(employeeComplaintWorkFlowDto, db);
                            }

                            //Notification Send 
                            string NotificationContent = string.Empty;
                            List<string> mailTo = new List<string>();
                            List<string> mailBody = new List<string>();
                            CategoryName = new CategoryMastersRepository().Get(Convert.ToInt32(employeeComplaint.CategoryId)).CategoryName;
                            SubCategoryName = new SubCategoryMastersRepository().Get(Convert.ToInt32(employeeComplaint.SubCategoryId)).SubCategoryName;

                            NotificationContent = employeeComplaintWorkFlowDto.ComplaintNo+" (" + LOSName + "-" + CategoryName + "-" + SubCategoryName + ") has been assigned to you by " + userData.EmployeeName + " on " + DateTime.UtcNow.ToString("dd/MM/yyyy") + " for your approval.";
                            foreach (var item in roleMasterLineItem)
                            {
                                new NotificationAlertRepository().AddNotificatioAlert(NotificationContent, item.UserId);
                                mailTo.Add(new UserMastersRepository().Get(Convert.ToInt32(item.UserId)).WorkEmail);
                                mailBody.Add(@"<html><body><p>Subject:Complent Submission" + ",</p></br><p>Receipt-HR User" + ",</p></br><p>Dear " + new UserMastersRepository().Get(Convert.ToInt32(item.UserId)).EmployeeName + ",</p></br><p>" + NotificationContent + "</p></br><p>MobileNo.:" + userData.MobileNo + "</p><p>Thank You.</br></br>Employee Assistance Portal</p></body></html>");
                            }
                            
                            MailSend.SendEmailWithDifferentBody(mailTo, "Compliant Submission", mailBody, Complaintdata.Id);
                            dbContextTransaction.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    new EmployeeComplaintHistoryRepository().ErrorLogHistory(Complaintdata.Id, ex.Message.ToString(), "Complaint Submit");
                    throw new Exception(ex.Message.ToString());
                }
            }
            return false;
        }

        public bool WithdrawComplaint(int id, string remarks)
        {
            int ID = 0;
            try
            {
                var data = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == id);
                ID = data.Id;
                if (data != null && !string.IsNullOrEmpty(remarks))
                {
                    data.IsSubmitted = false;
                    data.Remark = remarks;
                    data.ComplaintStatus = Messages.Withdrawn;
                    data.LastPerformedBy = null;

                }
                var dataWorkFlow = db.EmployeeComplaintWorkFlows.FirstOrDefault(p => p.ComplaintId == id);
                if (dataWorkFlow != null && !string.IsNullOrEmpty(remarks))
                {
                    dataWorkFlow.UpdatedDate = DateTime.UtcNow;
                    dataWorkFlow.Remarks = remarks;
                    dataWorkFlow.ActionType = Messages.Withdrawn;
                }
                new EmployeeComplaintHistoryRepository().AddComplaintHistory(data.Remark, data.Id, Messages.Withdrawn, db);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(ID, ex.Message.ToString(), "Complaint Withdraw");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
        }

        public string UploadImportEmployeeCompliant(string file)
        {
            try
            {
                return new Common().SaveExcelFromBase64(file);
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
        }

        public int ImportEmployeeCompliant(string file)
        {
            List<EmployeeComplaintMaster> importEmployeeComplaint = new List<EmployeeComplaintMaster>();
            EmployeeComplaintMaster EmployeeComplaintMasterDto = null;
            int count = 0;
            #region Indexes 
            int CategoryNameIndex = 1; int SubCategoryNameIndex = 2; int RemarkNameIndex = 3;
            #endregion

            string[] statuses = { "active", "inactive" };
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (Path.GetExtension(file) == ".xlsx" && !string.IsNullOrEmpty(sid))
                {
                    var lstEmployee = new UserMastersRepository().GetAll();
                    var lstCategory = new CategoryMastersRepository().GetAll();
                    var lstSubCategory = new SubCategoryMastersRepository().GetAll();

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelPackage package = new ExcelPackage(new FileInfo(file));
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    for (int i = 1; i <= workSheet.Dimension.Rows; i++)
                    {
                        if (i == 1) //skip header row if its there
                        {
                            continue;
                        }
                        EmployeeComplaintMasterDto = new EmployeeComplaintMaster();

                        EmployeeComplaintMasterDto.UserId = Convert.ToInt32(sid);

                        // Category Name
                        if (string.IsNullOrEmpty(workSheet.Cells[i, CategoryNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Employee Name", i, CategoryNameIndex }));
                        }
                        else
                        {
                            string Category = workSheet.Cells[i, CategoryNameIndex].Value?.ToString();
                            var CategoryDto = lstCategory.FirstOrDefault(x => x.CategoryName.ToLower() == Category.ToLower());
                            if (CategoryDto != null)
                            {
                                EmployeeComplaintMasterDto.CategoryId = CategoryDto.Id;
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "Category", i, CategoryNameIndex }));
                            }
                        }
                        // SubCategory Name
                        if (string.IsNullOrEmpty(workSheet.Cells[i, SubCategoryNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Category", i, SubCategoryNameIndex }));
                        }
                        else
                        {
                            string SubCategory = workSheet.Cells[i, SubCategoryNameIndex].Value?.ToString();
                            var SubCategoryDto = lstSubCategory.FirstOrDefault(x => x.SubCategoryName.ToLower() == SubCategory.ToLower());
                            if (SubCategoryDto != null)
                            {
                                EmployeeComplaintMasterDto.SubCategoryId = SubCategoryDto.Id;
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "Sub CategoryName", i, SubCategoryNameIndex }));
                            }
                        }
                        // Remark check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, RemarkNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Remarks", i, RemarkNameIndex }));
                        }
                        else
                        {
                            if (!new Common().HasSpecialCharacter(workSheet.Cells[i, RemarkNameIndex].Value?.ToString()))
                            {
                                EmployeeComplaintMasterDto.Remark = workSheet.Cells[i, RemarkNameIndex].Value?.ToString();
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.FieldIsInvalid, new object[] { "Remarks", i, RemarkNameIndex }));
                            }
                        }


                        EmployeeComplaintMasterDto.ComplaintStatus = Messages.Opened;

                        EmployeeComplaintMasterDto.CreatedBy = Convert.ToInt32(sid);
                        EmployeeComplaintMasterDto.CreatedDate = DateTime.UtcNow;
                        EmployeeComplaintMasterDto.IsActive = EmployeeComplaintMasterDto.Status = true;
                        EmployeeComplaintMasterDto.UserId = EmployeeComplaintMasterDto.UserId = EmployeeComplaintMasterDto.UserId == 0 ? Convert.ToInt32(sid) : EmployeeComplaintMasterDto.UserId;
                        EmployeeComplaintMasterDto.DueDate = DateTime.UtcNow.AddDays(5);
                        importEmployeeComplaint.Add(EmployeeComplaintMasterDto);
                    }
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (importEmployeeComplaint != null && importEmployeeComplaint.Count > 0)
                            {
                                db.EmployeeComplaintMasters.AddRange(importEmployeeComplaint);
                                db.SaveChanges();

                                foreach (EmployeeComplaintMaster item in importEmployeeComplaint)
                                {
                                    new EmployeeComplaintHistoryRepository().AddComplaintHistory(item.Remark, item.Id, item.ComplaintStatus, db);
                                }
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                count = importEmployeeComplaint.Count;
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                count = 0;
                throw ex;
            }
            return count;
        }


        //Aman work
        public EmployeeCompliantMasterVM SaveHRComplaint(EmployeeCompliantMasterVM EmployeeComplaintVM, String Id, int Hrid, string UserInvolved, int Status)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                int ids = Convert.ToInt32(Id);
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();

                var LoinUserId = Convert.ToInt32(sid);
                var LOSName = string.Empty; var CategoryName = string.Empty; var SubCategoryName = string.Empty;
                string remark = EmployeeComplaintVM.Remark;
                if (!string.IsNullOrEmpty(sid))
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        this.db.Database.CommandTimeout = 180;
                        try
                        {
                            if(Status==1)
                            {
                                var HRRoleData = db.HR_Role.FirstOrDefault(p => p.ComplentId == ids && p.HRUserId == LoinUserId);
                                if (HRRoleData == null)
                                {
                                    var HRRole = new HR_Role();
                                    HRRole.IsActive = true;
                                    HRRole.CreatedDate = DateTime.UtcNow;
                                    HRRole.ComplentId = ids;
                                    HRRole.UserId = EmployeeComplaintVM.UserId;
                                    HRRole.Remark = remark;
                                    HRRole.CaseType = EmployeeComplaintVM.CaseType;
                                    HRRole.HRUserId = Hrid;
                                    HRRole.InvolvedUsersId = UserInvolved;
                                    HRRole.Attachement = EmployeeComplaintVM.Attachments1;
                                    HRRole.Status = "Save";
                                    db.HR_Role.Add(HRRole);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    HRRoleData.IsActive = true;
                                    HRRoleData.Remark = remark;
                                    HRRoleData.CaseType = EmployeeComplaintVM.CaseType;
                                    HRRoleData.InvolvedUsersId = UserInvolved;
                                    //HRRoleData.Attachement = EmployeeComplaintVM.Attachments1;

                                    if (!string.IsNullOrEmpty(EmployeeComplaintVM.Attachments1))
                                    {
                                        List<string> newAttachments = EmployeeComplaintVM.Attachments1.Split(',').ToList();
                                        List<string> oldAttachments = HRRoleData.Attachement.Split(',').ToList();
                                        List<string> updatedAttachements = new List<string>();
                                        foreach (string fileName in oldAttachments.Concat(newAttachments).ToList())
                                        {
                                            if (!string.IsNullOrEmpty(fileName) && fileName.Length > 5)
                                            {
                                                updatedAttachements.Add(fileName);
                                            }
                                        }
                                        HRRoleData.Attachement = string.Join(",", updatedAttachements);
                                    }
                                    else
                                    {
                                        HRRoleData.Attachement = HRRoleData.Attachement;
                                    }

                                    db.Entry(HRRoleData).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                var WorkFlow = db.EmployeeComplaintWorkFlows.FirstOrDefault(p => p.ComplaintId == ids);
                                if (WorkFlow != null)
                                {
                                    WorkFlow.ActionType = Messages.SUBMITTED;
                                    WorkFlow.UpdatedDate = DateTime.UtcNow;
                                    WorkFlow.Remarks = remark;
                                    db.Entry(WorkFlow).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                var ComplaintMasters = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == ids);
                                if (ComplaintMasters != null)
                                {
                                    ComplaintMasters.LastPerformedBy = sid;
                                    db.Entry(WorkFlow).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                new EmployeeComplaintHistoryRepository().AddComplaintHistory(remark, ids, "Save", db);

                            }
                            else if(Status==2)
                            {
                                var HRRoleData = db.HR_Role.FirstOrDefault(p => p.ComplentId == ids && p.HRUserId == LoinUserId);
                                if (HRRoleData == null)
                                {
                                    var HRRole = new HR_Role();
                                    HRRole.IsActive = true;
                                    HRRole.CreatedDate = DateTime.UtcNow;
                                    HRRole.ComplentId = ids;
                                    HRRole.UserId = EmployeeComplaintVM.UserId;
                                    HRRole.Remark = remark;
                                    HRRole.CaseType = EmployeeComplaintVM.CaseType;
                                    HRRole.HRUserId = Hrid;
                                    HRRole.InvolvedUsersId = UserInvolved;
                                    HRRole.Attachement = EmployeeComplaintVM.Attachments1;
                                    HRRole.Status = Messages.COMMITTEE;
                                    db.HR_Role.Add(HRRole);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    HRRoleData.IsActive = true;
                                    HRRoleData.Remark = remark;
                                    HRRoleData.CaseType = EmployeeComplaintVM.CaseType;
                                    HRRoleData.InvolvedUsersId = UserInvolved;
                                    //HRRoleData.Attachement = EmployeeComplaintVM.Attachments1;
                                    HRRoleData.Status = Messages.COMMITTEE;

                                    if (!string.IsNullOrEmpty(EmployeeComplaintVM.Attachments1))
                                    {
                                        List<string> newAttachments = EmployeeComplaintVM.Attachments1.Split(',').ToList();
                                        List<string> oldAttachments = HRRoleData.Attachement.Split(',').ToList();
                                        List<string> updatedAttachements = new List<string>();
                                        foreach (string fileName in oldAttachments.Concat(newAttachments).ToList())
                                        {
                                            if (!string.IsNullOrEmpty(fileName) && fileName.Length > 5)
                                            {
                                                updatedAttachements.Add(fileName);
                                            }
                                        }
                                        HRRoleData.Attachement = string.Join(",", updatedAttachements);
                                    }
                                    else
                                    {
                                        HRRoleData.Attachement = HRRoleData.Attachement;
                                    }

                                    db.Entry(HRRoleData).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                var WorkFlow = db.EmployeeComplaintWorkFlows.FirstOrDefault(p => p.ComplaintId == ids);
                                if (WorkFlow != null)
                                {
                                    WorkFlow.ActionType = Messages.COMMITTEE;
                                    WorkFlow.UpdatedDate = DateTime.UtcNow;
                                    WorkFlow.Remarks = remark;
                                    db.Entry(WorkFlow).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                var ComplaintMaster = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == ids);
                                if (ComplaintMaster != null)
                                {
                                    ComplaintMaster.ComplaintStatus = Messages.COMMITTEE;
                                    ComplaintMaster.UpdatedDate = DateTime.UtcNow;
                                    ComplaintMaster.UpdatedBy = Convert.ToInt32(sid);
                                    ComplaintMaster.LastPerformedBy = sid;
                                    db.Entry(ComplaintMaster).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                new EmployeeComplaintHistoryRepository().AddComplaintHistory(remark, ids, Messages.PushToCommittee, db);

                                //Notification Saving Work
                                string NotificationContent = string.Empty;
                                List<string> mailTo = new List<string>();
                                List<string> mailBody = new List<string>();
                                CategoryName = new CategoryMastersRepository().Get(Convert.ToInt32(ComplaintMaster.CategoryId)).CategoryName;
                                SubCategoryName = new SubCategoryMastersRepository().Get(Convert.ToInt32(ComplaintMaster.SubCategoryId)).SubCategoryName;
                                var userData= new UserMastersRepository().Get(Convert.ToInt32(sid));
                                LOSName = new LOSMasterRepository().Get(WorkFlow.LOSId).LOSName;
                                NotificationContent = WorkFlow.ComplaintNo+" (" + LOSName + "-" + CategoryName + "-" + SubCategoryName + ") has been assigned to you by " + userData.EmployeeName + " on " + DateTime.UtcNow.ToString("dd/MM/yyyy") + " for your approval.";

                                var CommitteeMemberData = (from u in db.CommitteeMasters
                                                           where u.IsActive
                                                           select u).FirstOrDefault();
                                List<string> assignToUserId = CommitteeMemberData.UserId.Split(',').ToList();
                                foreach (var item in assignToUserId)
                                {
                                    new NotificationAlertRepository().AddNotificatioAlert(NotificationContent, Convert.ToInt32(item));
                                    mailTo.Add(new UserMastersRepository().Get(Convert.ToInt32(item)).WorkEmail);
                                    mailBody.Add(@"<html><body><p>Subject:Compliant Submission" + ",</p></br><p>Receipt-Committee User" + ",</p></br> <p>Dear " + new UserMastersRepository().Get(Convert.ToInt32(item)).EmployeeName + ",</p></br><p>" + NotificationContent + "</p><p>Thank You.</br></br>Employee Assistance Portal</p></body></html>");
                                }

                                //string htmlBody= @"<html><body><p>Dear Ms. Susan,</p></br><p>"+ NotificationContent + "</p><p>Thank You.</br></br>CMS</p></body></html>";
                                MailSend.SendEmailWithDifferentBody(mailTo, "Compliant Submission", mailBody, ids);

                            }
                            dbContextTransaction.Commit();
                        }

                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            if (!string.IsNullOrEmpty(EmployeeComplaintVM.Attachments))
                            {
                                foreach (string file in EmployeeComplaintVM.Attachments.Split(','))
                                {
                                    if (!string.IsNullOrEmpty(file))
                                    {
                                        string filePath = "~/Documents/" + file;

                                        if (File.Exists(HttpContext.Current.Server.MapPath(filePath)))
                                        {
                                            File.Delete(HttpContext.Current.Server.MapPath(filePath));
                                        }

                                    }
                                }
                            }
                            if (Status == 1)
                            {
                                new EmployeeComplaintHistoryRepository().ErrorLogHistory(ids, ex.Message.ToString(), "Save");
                            }
                            else if(Status == 2)
                            {
                                new EmployeeComplaintHistoryRepository().ErrorLogHistory(ids, ex.Message.ToString(), Messages.PushToCommittee);
                            }
                           
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new EmployeeCompliantMasterVM();
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

        public string CreateComplaintNo()
        {
            try
            {
                var numberSystem = db.NumberSystems.FirstOrDefault(); 
                if(numberSystem == null)
                {
                    numberSystem = new NumberSystem();
                    numberSystem.NoLength = 6;
                    numberSystem.AutoIncrement = 0;
                    db.NumberSystems.Add(numberSystem);
                    db.SaveChanges();

                    numberSystem = db.NumberSystems.FirstOrDefault();
                }
                else
                {
                    numberSystem.AutoIncrement = numberSystem.AutoIncrement + 1;
                    db.Entry(numberSystem).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var length = numberSystem.NoLength;
                var AutoIncrement = numberSystem.AutoIncrement;

                string newString = new string('0', (length)-(AutoIncrement.ToString().Length));
                string ComplaintNo = newString + AutoIncrement;
                return ComplaintNo;
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
        }

    }
}
