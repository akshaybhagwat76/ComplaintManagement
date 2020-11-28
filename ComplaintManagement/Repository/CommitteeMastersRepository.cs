using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace ComplaintManagement.Repository
{
    public class CommitteeMastersRepository
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public CommitteeMastersRepository()
        {

        }

        public CommitteeMasterVM AddOrUpdate(CommitteeMasterVM CommitteeVM)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var Committee = db.CommitteeMasters.FirstOrDefault(p => p.Id == CommitteeVM.Id);
                    if (Committee == null)
                    {
                        CommitteeVM.IsActive = true;
                        CommitteeVM.CreatedDate = DateTime.UtcNow;
                        CommitteeVM.CreatedBy = Convert.ToInt32(sid);

                        Committee = Mapper.Map<CommitteeMasterVM, CommitteeMaster>(CommitteeVM);
                        if (IsExist(Committee.CommitteeName))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.CommitteeMasters.Add(Committee);
                        db.SaveChanges();
                        return Mapper.Map<CommitteeMaster, CommitteeMasterVM>(Committee);
                    }
                    else
                    {
                        CommitteeVM.IsActive = true;

                        CommitteeVM.CreatedDate = Committee.CreatedDate;
                        CommitteeVM.UpdatedDate = DateTime.UtcNow;
                        CommitteeVM.CreatedBy = Committee.CreatedBy;
                        CommitteeVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(Committee).CurrentValues.SetValues(CommitteeVM);
                        if (IsExist(Committee.CommitteeName, Committee.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<CommitteeMaster, CommitteeMasterVM>(Committee);

                    }
                }
                return new CommitteeMasterVM();
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

        public List<CommitteeMasterVM> GetAll()
        {
            List<CommitteeMaster> Committee = new List<CommitteeMaster>();
            List<CommitteeMasterVM> CommitteeList = new List<CommitteeMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                Committee = db.CommitteeMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (Committee != null && Committee.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (CommitteeMaster item in Committee)
                    {
                        CommitteeMasterVM catObj = Mapper.Map<CommitteeMaster, CommitteeMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            CommitteeList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return CommitteeList;
        }

        public CommitteeMasterVM Get(int id)
        {
            CommitteeMaster Committee = new CommitteeMaster();
            try
            {
                Committee = db.CommitteeMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Committee == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<CommitteeMaster, CommitteeMasterVM>(Committee);
        }


        public bool Delete(int id)
        {
            var data = db.CommitteeMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string CommitteeName)
        {
            return db.CommitteeMasters.Count(x => x.IsActive && x.CommitteeName.ToLower() == CommitteeName.ToLower()) > 0;
        }

        public bool IsExist(string CommitteeName, int id)
        {
            return db.CommitteeMasters.Count(x => x.IsActive && x.CommitteeName.ToUpper() == CommitteeName.ToUpper() && x.Id != id) > 0;
        }

        public string UploadImportCommitties(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportImportCommitties(string file)
        {
            List<CommitteeMaster> importCommittee = new List<CommitteeMaster>();
            CommitteeMaster CommitteeMasterDto = null;
            int count = 0;
            #region Indexes 
            int CommitteeNameIndex = 1; int UserIndex = 2; int StatusIndex = 3;
            #endregion

            string[] statuses = { "active", "inactive" };
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                List<UserMasterVM> lstUsers = new UserMastersRepository().GetAll();

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (Path.GetExtension(file) == ".xlsx" && !string.IsNullOrEmpty(sid))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelPackage package = new ExcelPackage(new FileInfo(file));
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    for (int i = 1; i <= workSheet.Dimension.Rows; i++)
                    {
                        if (i == 1) //skip header row if its there
                        {
                            continue;
                        }
                        CommitteeMasterDto = new CommitteeMaster();

                        #region Committee Name
                        //Category Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, CommitteeNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Committee Name", i, CommitteeNameIndex }));
                        }
                        else
                        {
                            string Committee = workSheet.Cells[i, CommitteeNameIndex].Value?.ToString();
                            if (IsExist(Committee))
                            {
                                throw new Exception(string.Format(Messages.DataCommitteeAlreadyExists, new object[] { "Category Name", i, CommitteeNameIndex }));
                            }
                            else
                            {
                                CommitteeMasterDto.CommitteeName = workSheet.Cells[i, CommitteeNameIndex].Value?.ToString();
                            }
                        }
                        #endregion

                        #region User Name
                        //User Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, UserIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "User", i, UserIndex }));
                        }
                        else
                        {
                            string User = workSheet.Cells[i, UserIndex].Value?.ToString();

                            if (!string.IsNullOrEmpty(User))
                            {
                                string[] Uservalues = User.Split(',').Select(sValue => sValue.Trim()).ToArray();
                                List<string> UserDtoValues = new List<string>();

                                foreach (string userValue in Uservalues)
                                {
                                    if (!string.IsNullOrEmpty(userValue))
                                    {
                                        var UserDto = lstUsers.FirstOrDefault(x => x.EmployeeName.ToLower() == userValue.ToLower());
                                        if (UserDto == null)
                                        {
                                            throw new Exception(string.Format(Messages.DataNOTExists, new object[] { "User", i, UserIndex }));
                                        }
                                        else
                                        {
                                            UserDtoValues.Add(UserDto.Id.ToString());
                                        }
                                    }
                                }
                                CommitteeMasterDto.UserId = String.Join(",", UserDtoValues);
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "User", i, UserIndex }));
                            }
                        }
                        #endregion

                        #region Status
                        //Status check
                        if (!string.IsNullOrEmpty(workSheet.Cells[i, StatusIndex].Value?.ToString()))
                        {
                            string Status = workSheet.Cells[i, StatusIndex].Value?.ToString();
                            if (statuses.Any(Status.ToLower().Contains))
                            {
                                if (workSheet.Cells[i, StatusIndex].Value?.ToString().ToLower() == Messages.Active.ToLower())
                                {
                                    CommitteeMasterDto.Status = true;
                                }
                                else
                                {
                                    CommitteeMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion

                        CommitteeMasterDto.CreatedBy = Convert.ToInt32(sid);
                        CommitteeMasterDto.CreatedDate = DateTime.UtcNow;
                        CommitteeMasterDto.IsActive = true;
                        importCommittee.Add(CommitteeMasterDto);
                    }
                    if (importCommittee != null && importCommittee.Count > 0)
                    {
                        db.CommitteeMasters.AddRange(importCommittee);
                        db.SaveChanges();
                        count = importCommittee.Count;
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
    }
}