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
    public class DesignationMasterRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();
        public DesignationMasterRepository()
        {

        }
        public DesignationMasterVM AddOrUpdate(DesignationMasterVM DesignationVM)
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
                            var Designation = db.DesignationMasters.FirstOrDefault(p => p.Id == DesignationVM.Id);
                            if (Designation == null)
                            {
                                DesignationVM.IsActive = true;
                                DesignationVM.CreatedDate = DateTime.UtcNow;
                                DesignationVM.CreatedBy = Convert.ToInt32(sid);

                                Designation = Mapper.Map<DesignationMasterVM, DesignationMaster>(DesignationVM);
                                if (IsExist(Designation.Designation))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.DesignationMasters.Add(Designation);
                                db.SaveChanges();

                                DesignationMasters_History historyObj = Mapper.Map<DesignationMasterVM, DesignationMasters_History>(DesignationVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Added; historyObj.DesignationId = Designation.Id; };
                                db.DesignationMasters_History.Add(historyObj);
                                db.SaveChanges();

                                dbContextTransaction.Commit();

                                return Mapper.Map<DesignationMaster, DesignationMasterVM>(Designation);
                            }
                            else
                            {
                                DesignationVM.IsActive = true;
                                DesignationVM.CreatedDate = Designation.CreatedDate;
                                DesignationVM.UpdatedDate = DateTime.UtcNow;
                                DesignationVM.CreatedBy = Designation.CreatedBy;
                                DesignationVM.ModifiedBy = Convert.ToInt32(sid);
                                db.Entry(Designation).CurrentValues.SetValues(DesignationVM);
                                if (IsExist(Designation.Designation, Designation.Id))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SaveChanges();

                                DesignationMasters_History historyObj = Mapper.Map<DesignationMasterVM, DesignationMasters_History>(DesignationVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Updated; historyObj.DesignationId = Designation.Id; };
                                db.DesignationMasters_History.Add(historyObj);
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                return Mapper.Map<DesignationMaster, DesignationMasterVM>(Designation);
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new DesignationMasterVM();
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
        public List<DesignationMasterVM> GetAll()
        {
            List<DesignationMaster> Designation = new List<DesignationMaster>();
            List<DesignationMasterVM> DesignationList = new List<DesignationMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                Designation = db.DesignationMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (Designation != null && Designation.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (DesignationMaster item in Designation)
                    {
                        DesignationMasterVM catObj = Mapper.Map<DesignationMaster, DesignationMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            DesignationList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return DesignationList;
        }
        public DesignationMasterVM Get(int id)
        {
            DesignationMaster Designation = new DesignationMaster();
            try
            {
                Designation = db.DesignationMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<DesignationMaster, DesignationMasterVM>(Designation);
        }
        public List<DesignationMasterHistoryVM> GetAllHistory()
        {
            List<DesignationMasters_History> listdto = new List<DesignationMasters_History>();
            List<DesignationMasterHistoryVM> lst = new List<DesignationMasterHistoryVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                listdto = db.DesignationMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (listdto != null && listdto.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (DesignationMasters_History item in listdto)
                    {
                        DesignationMasterHistoryVM ViewModelDto = Mapper.Map<DesignationMasters_History, DesignationMasterHistoryVM>(item);
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
            var data = db.DesignationMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string Designation)
        {
            return db.DesignationMasters.Count(x => x.IsActive && x.Designation.ToUpper() == Designation.ToUpper()) > 0;
        }
        public bool IsExist(string Designation, int id)
        {
            return db.DesignationMasters.Count(x => x.IsActive && x.Designation.ToUpper() == Designation.ToUpper() && x.Id != id) > 0;
        }
        public string UploadImportDesignation(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }
        public int ImportDesignation(string file)
        {
            List<DesignationMaster> importDesignation = new List<DesignationMaster>();
            DesignationMaster DesignationMasterDto = null;
            int count = 0;
            #region Indexes 
            int DesignationNameIndex = 1; int StatusIndex = 2;
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
                        DesignationMasterDto = new DesignationMaster();

                        #region designation Name
                        //designation Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, DesignationNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, DesignationNameIndex }));
                        }
                        else
                        {
                            string DesignationName = workSheet.Cells[i, DesignationNameIndex].Value?.ToString();
                            if (IsExist(DesignationName))
                            {
                                throw new Exception(string.Format(Messages.DataDesignationAlreadyExists, new object[] { "Name", i, DesignationNameIndex }));
                            }
                            else
                            {
                                DesignationMasterDto.Designation = workSheet.Cells[i, DesignationNameIndex].Value?.ToString();
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
                                    DesignationMasterDto.Status = true;
                                }
                                else
                                {
                                    DesignationMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion
                        DesignationMasterDto.CreatedBy = Convert.ToInt32(sid);
                        DesignationMasterDto.CreatedDate = DateTime.UtcNow;
                        DesignationMasterDto.IsActive = true;
                        importDesignation.Add(DesignationMasterDto);
                    }
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (importDesignation != null && importDesignation.Count > 0)
                            {
                                db.DesignationMasters.AddRange(importDesignation);
                                db.SaveChanges();


                                List<DesignationMasterHistoryVM> listVMDto = Mapper.Map<List<DesignationMaster>, List<DesignationMasterHistoryVM>>(importDesignation);

                                List<DesignationMasters_History> HistoryDto = Mapper.Map<List<DesignationMasterHistoryVM>, List<DesignationMasters_History>>(listVMDto);
                                if (HistoryDto != null && HistoryDto.Count > 0)
                                {
                                    HistoryDto.Select(c => { c.EntityState = Messages.Added; c.DesignationId = c.Id; return c; }).ToList();
                                }

                                db.DesignationMasters_History.AddRange(HistoryDto);
                                db.SaveChanges();
                                                                                                                                       
                                dbContextTransaction.Commit();
                                count = importDesignation.Count;
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