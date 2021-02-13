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
    public class RoleMasterRepoitory
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public RoleMasterRepoitory()
        {

        }

        public RoleMasterVM AddOrUpdate(RoleMasterVM RoleVM)
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
                        try
                        {
                            var Role = db.RoleMasters.FirstOrDefault(p => p.Id == RoleVM.Id);
                            if (Role == null)
                            {
                                RoleVM.IsActive = true;
                                RoleVM.CreatedDate = DateTime.UtcNow;
                                RoleVM.CreatedBy = Convert.ToInt32(sid);
                                Role = Mapper.Map<RoleMasterVM, RoleMaster>(RoleVM);

                                db.RoleMasters.Add(Role);
                                db.SaveChanges();


                                RoleMasters_History historyObj = Mapper.Map<RoleMasterVM, RoleMasters_History>(RoleVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Added; historyObj.RoleId = Role.Id; };
                                db.RoleMasters_History.Add(historyObj);
                                db.SaveChanges();
                                dbContextTransaction.Commit();

                                return Mapper.Map<RoleMaster, RoleMasterVM>(Role);
                            }
                            else
                            {
                                RoleVM.IsActive = true;
                                RoleVM.CreatedDate = Role.CreatedDate;
                                RoleVM.CreatedBy = Role.CreatedBy;
                                RoleVM.UpdatedDate = DateTime.UtcNow;
                                RoleVM.ModifiedBy = Convert.ToInt32(sid);
                                db.Entry(Role).CurrentValues.SetValues(RoleVM);
                                db.SaveChanges();

                                RoleMasters_History historyObj = Mapper.Map<RoleMasterVM, RoleMasters_History>(RoleVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Updated; historyObj.RoleId = Role.Id; };
                                db.RoleMasters_History.Add(historyObj);
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                return Mapper.Map<RoleMaster, RoleMasterVM>(Role);

                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, ex.Message.ToString(), "Role Masters Create");
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new RoleMasterVM();
            }
            catch (DbEntityValidationException dve)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)), "Role Masters Create");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(dve);
                throw new Exception(string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)));
            }
            catch (Exception ex)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, ex.Message.ToString(), "Role Masters Create");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
        }

        public List<RoleMasterVM> GetAll()
        {
            List<RoleMaster> Role = new List<RoleMaster>();
            List<RoleMasterVM> RoleList = new List<RoleMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                Role = db.RoleMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (Role != null && Role.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (RoleMaster item in Role)
                    {
                        RoleMasterVM catObj = Mapper.Map<RoleMaster, RoleMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            RoleList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return RoleList;
        }

        public RoleMasterVM Get(int id)
        {
            RoleMaster Role = new RoleMaster();
            try
            {
                Role = db.RoleMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Role == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<RoleMaster, RoleMasterVM>(Role);
        }

        public List<RoleMasterHistoryVM> GetAllHistory()
        {
            List<RoleMasters_History> listdto = new List<RoleMasters_History>();
            List<RoleMasterHistoryVM> lst = new List<RoleMasterHistoryVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                listdto = db.RoleMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (listdto != null && listdto.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (RoleMasters_History item in listdto)
                    {
                        RoleMasterHistoryVM ViewModelDto = Mapper.Map<RoleMasters_History, RoleMasterHistoryVM>(item);
                        if (ViewModelDto != null)
                        {
                            ViewModelDto.CreatedByName = usersList.FirstOrDefault(x => x.Id == ViewModelDto.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == ViewModelDto.CreatedBy).EmployeeName : string.Empty;
                            ViewModelDto.UpdatedByName = usersList.FirstOrDefault(x => x.Id == ViewModelDto.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == ViewModelDto.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            lst.Add(ViewModelDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return lst;
        }

        public bool Delete(int id)
        {
            var data = db.RoleMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }

        public string UploadImportRole(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportImportRole(string file)
        {
            List<RoleMaster> importRole = new List<RoleMaster>();
            RoleMaster RoleMasterDto = null;
            int count = 0;
            #region Indexes 
            int UserIndex = 1; int LOSNameIndex = 2; int SBUIndex = 3; int SubSBUIndex = 4; int CompetencyIndex = 5; int StatusIndex = 6;
            #endregion

            string[] statuses = { "active", "inactive" };
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                List<UserMasterVM> lstUsers = new UserMastersRepository().GetAll();
                List<LOSMasterVM> lstLOS = new LOSMasterRepository().GetAll();
                List<SBUMasterVM> lstSBU = new SBUMasterRepository().GetAll();
                List<SubSBUMasterVM> lstSubSBU = new SubSBUMasterRepository().GetAll();
                List<CompetencyMasterVM> lstCompetency = new CompetencyMastersRepository().GetAll();

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

                        RoleMasterDto = new RoleMaster();
                        if (string.IsNullOrEmpty(workSheet.Cells[i, UserIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, UserIndex }));
                        }
                        else
                        {
                            string UserName = workSheet.Cells[i, UserIndex].Value?.ToString();
                            UserMasterVM UserMasterVMDto = new UserMasterVM { EmployeeName = UserName };
                            if (lstUsers != null && lstUsers.Count > 0)
                            {
                                UserMasterVM isExist = lstUsers.FirstOrDefault(x => x.EmployeeName.ToLower() == UserName.ToLower());
                                if (isExist != null)
                                {
                                    RoleMasterDto.UserId = isExist.Id;
                                }
                                else
                                {

                                    throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "Name", i, UserIndex }));
                                }
                            }
                        }
                        //los
                        if (string.IsNullOrEmpty(workSheet.Cells[i, LOSNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "LOS", i, LOSNameIndex }));
                        }
                        else
                        {
                            string LOS = workSheet.Cells[i, LOSNameIndex].Value?.ToString();

                            if (!string.IsNullOrEmpty(LOS))
                            {
                                string[] LOSvalues = LOS.Split(',').Select(sValue => sValue.Trim()).ToArray();
                                List<string> LOSDtoValues = new List<string>();

                                foreach (string LOSvalue in LOSvalues)
                                {
                                    if (!string.IsNullOrEmpty(LOSvalue))
                                    {
                                        var LOSDto = lstLOS.FirstOrDefault(x => x.LOSName.ToLower() == LOSvalue.ToLower());
                                        if (LOSDto == null)
                                        {
                                            throw new Exception(string.Format(Messages.DataNOTExists, new object[] { "LOS", i, LOSNameIndex }));
                                        }
                                        else
                                        {
                                            LOSDtoValues.Add(LOSDto.Id.ToString());
                                        }
                                    }
                                }
                                RoleMasterDto.LOSId = String.Join(",", LOSDtoValues);
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "LOS", i, LOSNameIndex }));
                            }
                        }

                        #region SBU Name
                        //SBU Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, SBUIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "SBU", i, SBUIndex }));
                        }
                        else
                        {
                            string SBU = workSheet.Cells[i, SBUIndex].Value?.ToString();

                            if (!string.IsNullOrEmpty(SBU))
                            {
                                string[] SBUvalues = SBU.Split(',').Select(sValue => sValue.Trim()).ToArray();
                                List<string> SBUDtoValues = new List<string>();

                                foreach (string SBUvalue in SBUvalues)
                                {
                                    if (!string.IsNullOrEmpty(SBUvalue))
                                    {
                                        var SBUDto = lstSBU.FirstOrDefault(x => x.SBU.ToLower() == SBUvalue.ToLower());
                                        if (SBUDto == null)
                                        {
                                            throw new Exception(string.Format(Messages.DataNOTExists, new object[] { "SBU", i, SBUIndex }));
                                        }
                                        else
                                        {
                                            SBUDtoValues.Add(SBUDto.Id.ToString());
                                        }
                                    }
                                }
                                RoleMasterDto.SBUId = String.Join(",", SBUDtoValues);
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "User", i, SBUIndex }));
                            }
                        }
                        #endregion
                        //SubSBU Check

                        if (string.IsNullOrEmpty(workSheet.Cells[i, SubSBUIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "SUbSBU", i, SubSBUIndex }));
                        }
                        else
                        {
                            string SubSBU = workSheet.Cells[i, SubSBUIndex].Value?.ToString();

                            if (!string.IsNullOrEmpty(SubSBU))
                            {
                                string[] SubSBUvalues = SubSBU.Split(',').Select(sValue => sValue.Trim()).ToArray();
                                List<string> SubSBUDtoValues = new List<string>();

                                foreach (string SubSBUvalue in SubSBUvalues)
                                {
                                    if (!string.IsNullOrEmpty(SubSBUvalue))
                                    {
                                        var SubSBUDto = lstSubSBU.FirstOrDefault(x => x.SubSBU.ToLower() == SubSBUvalue.ToLower());
                                        if (SubSBUDto == null)
                                        {
                                            throw new Exception(string.Format(Messages.DataNOTExists, new object[] { "SubSBU", i, SubSBUIndex }));
                                        }
                                        else
                                        {
                                            SubSBUDtoValues.Add(SubSBUDto.Id.ToString());
                                        }
                                    }
                                }
                                RoleMasterDto.SubSBUId = String.Join(",", SubSBUDtoValues);
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "SubSBU", i, SubSBUIndex }));
                            }
                        }
                        // Competency Name Check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, CompetencyIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Competency", i, CompetencyIndex }));
                        }
                        else
                        {
                            string Competency = workSheet.Cells[i, CompetencyIndex].Value?.ToString();

                            if (!string.IsNullOrEmpty(Competency))
                            {
                                string[] Competencyvalues = Competency.Split(',').Select(sValue => sValue.Trim()).ToArray();
                                List<string> CompetencyDtoValues = new List<string>();

                                foreach (string Competencyvalue in Competencyvalues)
                                {
                                    if (!string.IsNullOrEmpty(Competencyvalue))
                                    {
                                        var CompetencyDto = lstCompetency.FirstOrDefault(x => x.CompetencyName.ToLower() == Competencyvalue.ToLower());
                                        if (CompetencyDto == null)
                                        {
                                            throw new Exception(string.Format(Messages.DataNOTExists, new object[] { "Competency", i, CompetencyIndex }));
                                        }
                                        else
                                        {
                                            CompetencyDtoValues.Add(CompetencyDto.Id.ToString());
                                        }
                                    }
                                }
                                RoleMasterDto.CompetencyId = String.Join(",", CompetencyDtoValues);
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Competency", i, CompetencyIndex }));
                            }
                        }
                        #region Status
                        //Status check
                        if (!string.IsNullOrEmpty(workSheet.Cells[i, StatusIndex].Value?.ToString()))
                        {
                            string Status = workSheet.Cells[i, StatusIndex].Value?.ToString();
                            if (statuses.Any(Status.ToLower().Contains))
                            {
                                if (workSheet.Cells[i, StatusIndex].Value?.ToString().ToLower() == Messages.Active.ToLower())
                                {
                                    RoleMasterDto.Status = true;
                                }
                                else
                                {
                                    RoleMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion

                        RoleMasterDto.CreatedBy = Convert.ToInt32(sid);
                        RoleMasterDto.CreatedDate = DateTime.UtcNow;
                        RoleMasterDto.IsActive = true;
                        importRole.Add(RoleMasterDto);
                    }
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (importRole != null && importRole.Count > 0)
                            {
                                db.RoleMasters.AddRange(importRole);
                                db.SaveChanges();

                                List<RoleMasterHistoryVM> listVMDto = Mapper.Map<List<RoleMaster>, List<RoleMasterHistoryVM>>(importRole);

                                List<RoleMasters_History> HistoryDto = Mapper.Map<List<RoleMasterHistoryVM>, List<RoleMasters_History>>(listVMDto);
                                if (HistoryDto != null && HistoryDto.Count > 0)
                                {
                                    HistoryDto.Select(c => { c.EntityState = Messages.Added; c.RoleId = c.Id; return c; }).ToList();
                                }

                                db.RoleMasters_History.AddRange(HistoryDto);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                count = importRole.Count;
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
    }
}