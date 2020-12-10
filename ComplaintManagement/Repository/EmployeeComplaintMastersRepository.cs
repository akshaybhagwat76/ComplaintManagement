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

        public EmployeeCompliantMasterVM AddOrUpdate(EmployeeCompliantMasterVM EmployeeComplaintVM)
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

                                EmployeeComplaintMastersHistory EmployeeComplaintMasters_History = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMastersHistory>(EmployeeComplaintVM);
                                if (EmployeeComplaintMasters_History != null) { EmployeeComplaintMasters_History.EntityState = Messages.Added; EmployeeComplaintMasters_History.EmployeeComplaintMasterId = EmployeeComplaintMasters_History.Id; };
                                db.EmployeeComplaintMastersHistories.Add(EmployeeComplaintMasters_History);
                                new EmployeeComplaintHistoryRepository().AddComplaintHistory(EmployeeComplaint.Remark, EmployeeComplaint.Id, EmployeeComplaint.ComplaintStatus, db);
                                db.SaveChanges();
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

                                EmployeeComplaintMastersHistory EmployeeComplaintMasters_History = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMastersHistory>(EmployeeComplaintVM);
                                if (EmployeeComplaintMasters_History != null) { EmployeeComplaintMasters_History.EntityState = Messages.Updated; EmployeeComplaintMasters_History.EmployeeComplaintMasterId = EmployeeComplaintMasters_History.Id; };
                                db.EmployeeComplaintMastersHistories.Add(EmployeeComplaintMasters_History);
                                new EmployeeComplaintHistoryRepository().AddComplaintHistory(EmployeeComplaint.Remark, EmployeeComplaint.Id, EmployeeComplaint.ComplaintStatus, db);
                                db.SaveChanges();

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

        public UserMasterVM AddOrUpdateSaveCommittee(UserMasterVM EmployeeComplaintVM,int CommitteUserid)
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
                            CommitteeRole commitRole = new CommitteeRole();
                            var EmployeeComplaint = db.CommitteeRoles.FirstOrDefault(p => p.Id == EmployeeComplaintVM.Id);
                            int? Id = EmployeeComplaintVM.Id;

                            commitRole.IsActive = true;
                            commitRole.CreatedDate = DateTime.UtcNow;
                            //EmployeeComplaint = Mapper.Map<CommitteeRole, CommitteeRole>(EmployeeComplaintVM);
                            commitRole.Userid = Id;
                            commitRole.Status = 1; //committee save
                            commitRole.ComplaintId = 1;
                            commitRole.InvolvedUsersId = EmployeeComplaintVM.InvolvedUsersId;
                            commitRole.CashTypeId = EmployeeComplaintVM.CashTypeId;
                            commitRole.CommitteeUserId = CommitteUserid;
                            commitRole.Remark = EmployeeComplaintVM.RemarkCommittee;
                            db.CommitteeRoles.Add(commitRole);
                            db.SaveChanges();
                            dbContextTransaction.Commit();

                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            if (!string.IsNullOrEmpty(EmployeeComplaintVM.AttachmentsCommittee))
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
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new UserMasterVM();
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

        public UserMasterVM AddOrUpdateBackToBUHCCommittee(UserMasterVM EmployeeComplaintVM, int CommitteUserid)
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
                            CommitteeRole commitRole = new CommitteeRole();
                            var EmployeeComplaint = db.CommitteeRoles.FirstOrDefault(p => p.Id == EmployeeComplaintVM.Id);
                            int? Id = EmployeeComplaintVM.Id;

                            commitRole.IsActive = true;
                            commitRole.CreatedDate = DateTime.UtcNow;
                            commitRole.Userid = Id;
                            commitRole.Status = 2; //committee Back to buhc
                            commitRole.ComplaintId = 1;
                            commitRole.InvolvedUsersId = EmployeeComplaintVM.InvolvedUsersId;
                            commitRole.CashTypeId = EmployeeComplaintVM.CashTypeId;
                            commitRole.CommitteeUserId = CommitteUserid;
                            commitRole.Remark = EmployeeComplaintVM.RemarkCommittee;
                            db.CommitteeRoles.Add(commitRole);
                            db.SaveChanges();
                            dbContextTransaction.Commit();

                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            if (!string.IsNullOrEmpty(EmployeeComplaintVM.AttachmentsCommittee))
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
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new UserMasterVM();
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
                                    roleMasterLineItem.ForEach(x => rolesId = string.Join(",", x.Id));
                                    roleMasterLineItem.ForEach(x => assignedUsersroleId = string.Join(",", x.UserId));
                                    employeeComplaintWorkFlowDto.ActionType = Messages.SUBMITTED;
                                    employeeComplaintWorkFlowDto.ComplaintId = Complaintdata.Id;
                                    employeeComplaintWorkFlowDto.Remarks = Complaintdata.Remark;
                                    employeeComplaintWorkFlowDto.UserType = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();
                                    employeeComplaintWorkFlowDto.RoleId = rolesId;
                                    employeeComplaintWorkFlowDto.AssignedUserRoles = assignedUsersroleId; employeeComplaintWorkFlowDto.DueDate = Complaintdata.DueDate;
                                    employeeComplaintWorkFlowDto.LOSId = losId; employeeComplaintWorkFlowDto.SBUId = SBUId; employeeComplaintWorkFlowDto.SubSBUId = subSBUId; employeeComplaintWorkFlowDto.CompentencyId = competencyId;
                                }
                            }

                            var employeeComplaint = db.EmployeeComplaintMasters.Find(Complaintdata.Id);

                            employeeComplaint.IsSubmitted = true;
                            employeeComplaint.ComplaintStatus = Messages.SUBMITTED;

                            new EmployeeComplaintHistoryRepository().AddComplaintHistory(Complaintdata.Remark, Complaintdata.Id, Complaintdata.ComplaintStatus, db);
                            db.SaveChanges();
                            if (employeeComplaintWorkFlowDto != null && employeeComplaintWorkFlowDto.ComplaintId > 0)
                            {
                                new EmployeeWorkFlowRepository().AddOrUpdate(employeeComplaintWorkFlowDto, db);
                            }
                            dbContextTransaction.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message.ToString());
                }
            }
            return false;
        }

        public bool WithdrawComplaint(int id, string remarks)
        {
            try
            {
                var data = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == id);
                if (data != null && !string.IsNullOrEmpty(remarks))
                {
                    data.IsSubmitted = false;
                    data.Remark = remarks;
                    data.ComplaintStatus = Messages.Withdrawn;
                    new EmployeeComplaintHistoryRepository().AddComplaintHistory(data.Remark, data.Id, data.ComplaintStatus, db);
                }
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
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
                if (!string.IsNullOrEmpty(sid))
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        this.db.Database.CommandTimeout = 180;
                        try
                        {
                            var EmployeeComplaint = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == ids);
                            if (Id != null)
                            {
                                if (Status == 1)
                                {
                                    HR_Role roles = new HR_Role();
                                    roles.IsActive = true;
                                    roles.CreatedDate = DateTime.UtcNow;
                                    roles.ComplentId = ids;
                                    roles.UserId = EmployeeComplaintVM.UserId;
                                    roles.HRUserId = Hrid;
                                    roles.InvolvedUsersId = UserInvolved;
                                    roles.Attachement = EmployeeComplaintVM.Attachments1;
                                    roles.Status = "SaveHRComplaint";

                                    db.HR_Role.Add(roles);
                                    db.SaveChanges();
                                }
                                else if (Status == 2)
                                {
                                    HR_Role roles = new HR_Role();
                                    roles.IsActive = true;
                                    roles.CreatedDate = DateTime.UtcNow;
                                    roles.ComplentId = ids;
                                    roles.UserId = EmployeeComplaintVM.UserId;
                                    roles.HRUserId = Hrid;
                                    roles.InvolvedUsersId = UserInvolved;
                                    roles.Attachement = EmployeeComplaintVM.Attachments1;
                                    roles.Status = "PushHRComplaint";
                                    db.HR_Role.Add(roles);
                                    db.SaveChanges();

                                    EmployeeComplaintWorkFlowVM VM = new EmployeeComplaintWorkFlowVM();
                                    VM.ActionType = "Committe";
                                    db.SaveChanges();

                                    EmployeeComplaintHistory hrs = new EmployeeComplaintHistory();
                                    hrs.CreatedBy = EmployeeComplaint.UserId;
                                    hrs.CreatedDate = DateTime.UtcNow;
                                    hrs.IsActive = true;
                                    hrs.Remarks =
                                    hrs.ActionType = "Committed";
                                    hrs.UserType = "Normal";
                                    hrs.ComplaintId = ids;
                                    db.SaveChanges();

                                }
                                //EmployeeComplaintMastersHistory EmployeeComplaintMasters_History = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMastersHistory>(EmployeeComplaintVM);
                                //if (EmployeeComplaintMasters_History != null) { EmployeeComplaintMasters_History.EntityState = Messages.Added; EmployeeComplaintMasters_History.EmployeeComplaintMasterId = EmployeeComplaintMasters_History.Id; };
                                //db.EmployeeComplaintMastersHistories.Add(EmployeeComplaintMasters_History);
                                //new EmployeeComplaintHistoryRepository().AddComplaintHistory(EmployeeComplaint.Remark, EmployeeComplaint.Id, EmployeeComplaint.ComplaintStatus, db);
                                //db.SaveChanges();
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


    }
}