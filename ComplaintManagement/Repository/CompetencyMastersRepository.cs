using AutoMapper;
using ComplaintManagement.Controllers;
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
    public class CompetencyMastersRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public CompetencyMasterVM AddOrUpdate(CompetencyMasterVM CompetencyVM)
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
                            var Competency = db.CompetencyMasters.FirstOrDefault(p => p.Id == CompetencyVM.Id);
                            if (Competency == null)
                            {
                                CompetencyVM.IsActive = true;
                                CompetencyVM.CreatedDate = DateTime.UtcNow;
                                CompetencyVM.UserId = Convert.ToInt32(sid);
                                CompetencyVM.CreatedBy = Convert.ToInt32(sid);
                                Competency = Mapper.Map<CompetencyMasterVM, CompetencyMaster>(CompetencyVM);
                                if (IsExist(Competency.CompetencyName))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.CompetencyMasters.Add(Competency);

                                CompetencyMasters_History CompetencyMasters_History = Mapper.Map<CompetencyMasterVM, CompetencyMasters_History>(CompetencyVM);
                                if (CompetencyMasters_History != null) { CompetencyMasters_History.EntityState = Messages.Added; CompetencyMasters_History.CompetencyId = Competency.Id; };
                                db.CompetencyMasters_History.Add(CompetencyMasters_History);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                return Mapper.Map<CompetencyMaster, CompetencyMasterVM>(Competency);
                            }
                            else
                            {
                                CompetencyVM.IsActive = true;
                                CompetencyVM.CreatedDate = Competency.CreatedDate;
                                CompetencyVM.CreatedBy = Competency.CreatedBy;
                                CompetencyVM.UpdatedDate = DateTime.UtcNow;
                                CompetencyVM.ModifiedBy = Convert.ToInt32(sid);
                                db.Entry(Competency).CurrentValues.SetValues(CompetencyVM);
                                if (IsExist(Competency.CompetencyName, Competency.Id))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SaveChanges();

                                CompetencyMasters_History CompetencyMasters_History = Mapper.Map<CompetencyMasterVM, CompetencyMasters_History>(CompetencyVM);
                                if (CompetencyMasters_History != null) { CompetencyMasters_History.EntityState = Messages.Updated; CompetencyMasters_History.CompetencyId = Competency.Id; };
                                db.CompetencyMasters_History.Add(CompetencyMasters_History);
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                return Mapper.Map<CompetencyMaster, CompetencyMasterVM>(Competency);

                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, ex.Message.ToString(), "Compentency Master");
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new CompetencyMasterVM();
            }
            catch (DbEntityValidationException dve)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)), "Compentency Master");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(dve);
                throw new Exception(string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)));
            }
            catch (Exception ex)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, ex.Message.ToString(), "Compentency Master");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
        }

        public List<CompetencyMasterVM> GetAll()
        {
            List<CompetencyMaster> Competency = new List<CompetencyMaster>();
            List<CompetencyMasterVM> CompetencyList = new List<CompetencyMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                Competency = db.CompetencyMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (Competency != null && Competency.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (CompetencyMaster item in Competency)
                    {
                        CompetencyMasterVM catObj = Mapper.Map<CompetencyMaster, CompetencyMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            CompetencyList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return CompetencyList;
        }

        public List<CompetencyMasterHistoryVM> GetAllHistory()
        {
            List<CompetencyMasters_History> competency = new List<CompetencyMasters_History>();
            List<CompetencyMasterHistoryVM> competencyList = new List<CompetencyMasterHistoryVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                competency = db.CompetencyMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (competency != null && competency.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (CompetencyMasters_History item in competency)
                    {
                        CompetencyMasterHistoryVM CompetencyMasters_History = Mapper.Map<CompetencyMasters_History, CompetencyMasterHistoryVM>(item);
                        if (CompetencyMasters_History != null)
                        {
                            CompetencyMasters_History.CreatedByName = usersList.FirstOrDefault(x => x.Id == CompetencyMasters_History.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == CompetencyMasters_History.CreatedBy).EmployeeName : string.Empty;
                            CompetencyMasters_History.UpdatedByName = usersList.FirstOrDefault(x => x.Id == CompetencyMasters_History.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == CompetencyMasters_History.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            competencyList.Add(CompetencyMasters_History);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return competencyList;

        }
        public CompetencyMasterVM Get(int id)
        {
            CompetencyMaster Competency = new CompetencyMaster();
            try
            {
                Competency = db.CompetencyMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<CompetencyMaster, CompetencyMasterVM>(Competency);
        }

        public bool Delete(int id)
        {
            var data = db.CompetencyMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }

        public bool IsExist(string CompetencyName)
        {
            return db.CompetencyMasters.Count(x => x.IsActive && x.CompetencyName.ToUpper() == CompetencyName.ToUpper()) > 0;
        }

        public bool IsExist(string CompetencyName, int id)
        {
            return db.CompetencyMasters.Count(x => x.IsActive && x.CompetencyName.ToUpper() == CompetencyName.ToUpper() && x.Id != id) > 0;
        }

        public string UploadImportCompetency(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportCompetency(string file)
        {
            List<CompetencyMaster> importCompetency = new List<CompetencyMaster>();
            CompetencyMaster CompetencyMasterDto = null;
            int count = 0;
            #region Indexes 
            int CompetencyNameIndex = 1; int StatusIndex = 2;
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
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelPackage package = new ExcelPackage(new FileInfo(file));
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    for (int i = 1; i <= workSheet.Dimension.Rows; i++)
                    {
                        if (i == 1) //skip header row if its there
                        {
                            continue;
                        }
                        CompetencyMasterDto = new CompetencyMaster();

                        #region Category Name
                        //Competency Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, CompetencyNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, CompetencyNameIndex }));
                        }
                        else
                        {
                            string CompetencyName = workSheet.Cells[i, CompetencyNameIndex].Value?.ToString();
                            CompetencyMasterVM competencyMasterDto = new CompetencyMasterVM { CompetencyName = CompetencyName };
                            if (IsExist(CompetencyName))
                            {
                                throw new Exception(string.Format(Messages.DataCompetencyAlreadyExists, new object[] { "Name", i, CompetencyNameIndex }));
                            }
                            else
                            {
                                CompetencyMasterDto.CompetencyName = workSheet.Cells[i, CompetencyNameIndex].Value?.ToString();
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
                                    CompetencyMasterDto.Status = true;
                                }
                                else
                                {
                                    CompetencyMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion
                        CompetencyMasterDto.CreatedBy = Convert.ToInt32(sid);
                        CompetencyMasterDto.CreatedDate = DateTime.UtcNow;
                        CompetencyMasterDto.IsActive = true;
                        importCompetency.Add(CompetencyMasterDto);
                    }

                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (importCompetency != null && importCompetency.Count > 0)
                            {
                                db.CompetencyMasters.AddRange(importCompetency);
                                db.SaveChanges();

                                List<CompetencyMasterVM> masterListVM = Mapper.Map<List<CompetencyMaster>, List<CompetencyMasterVM>>(importCompetency);

                                List<CompetencyMasters_History> comptencyMasters_History = Mapper.Map<List<CompetencyMasterVM>, List<CompetencyMasters_History>>(masterListVM);
                                if (comptencyMasters_History != null && comptencyMasters_History.Count > 0)
                                {
                                    comptencyMasters_History.Select(c => { c.EntityState = Messages.Added; c.CompetencyId = c.Id; return c; }).ToList();
                                }

                                db.CompetencyMasters_History.AddRange(comptencyMasters_History);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                count = importCompetency.Count;
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