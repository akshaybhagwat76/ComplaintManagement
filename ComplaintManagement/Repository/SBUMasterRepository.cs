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
using System.IO;
using OfficeOpenXml;

namespace ComplaintManagement.Repository
{
    public class SBUMasterRepository
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public SBUMasterRepository()
        {

        }

        public SBUMasterVM AddOrUpdate(SBUMasterVM SBUVM)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var SBU = db.SBUMasters.FirstOrDefault(p => p.Id == SBUVM.Id);
                    if (SBU == null)
                    {
                        SBUVM.IsActive = true;
                        SBUVM.CreatedDate = DateTime.UtcNow;
                        SBUVM.UserId = 1;
                        SBUVM.CreatedBy = Convert.ToInt32(sid);
                        SBU = Mapper.Map<SBUMasterVM, SBUMaster>(SBUVM);
                        if (IsExist(SBU.SBU))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SBUMasters.Add(SBU);
                        db.SaveChanges();
                        return Mapper.Map<SBUMaster, SBUMasterVM>(SBU);
                    }
                    else
                    {
                        SBUVM.IsActive = true;
                        SBUVM.CreatedDate = SBU.CreatedDate;
                        SBUVM.CreatedBy = SBU.CreatedBy;
                        SBUVM.UpdatedDate = DateTime.UtcNow;
                        SBUVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(SBU).CurrentValues.SetValues(SBUVM);
                        if (IsExist(SBU.SBU,SBU.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<SBUMaster, SBUMasterVM>(SBU);

                    }
                }
                return new SBUMasterVM();
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

        public List<SBUMasterVM> GetAll()
        {

            List<SBUMaster> SBU = new List<SBUMaster>();
            List<SBUMasterVM> SBUList = new List<SBUMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                SBU = db.SBUMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (SBU != null && SBU.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (SBUMaster item in SBU)
                    {
                        SBUMasterVM catObj = Mapper.Map<SBUMaster, SBUMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            SBUList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return SBUList;
        }

        public SBUMasterVM Get(int id)
        {
            SBUMaster SBU= new SBUMaster();
            try
            {
                SBU = db.SBUMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (SBU == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<SBUMaster, SBUMasterVM>(SBU);
        }


        public bool Delete(int id)
        {
            var data = db.SBUMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        

        public bool IsExist(string SBU)
        {
            return db.SBUMasters.Count(x => x.IsActive && x.SBU.ToUpper() == SBU.ToUpper()) > 0;
        }
      
        public bool IsExist(string SBU,int id)
        {
            return db.SBUMasters.Count(x => x.IsActive && x.SBU.ToUpper() == SBU.ToUpper() && x.Id!=id) > 0;
        }

        public string UploadImportSBU(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportSBU(string file)
        {
            List<SBUMaster> importSBU = new List<SBUMaster>();
            SBUMaster SBUMasterDto = null;
            int count = 0;
            #region Indexes 
            int SBUNameIndex = 1; int StatusIndex = 2;
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
                        SBUMasterDto = new SBUMaster();

                        #region SBU Name
                        //SBU Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, SBUNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, SBUNameIndex }));
                        }
                        else
                        {
                            string SBUName = workSheet.Cells[i, SBUNameIndex].Value?.ToString();
                            SBUMasterVM sBUMasterDto = new SBUMasterVM { SBU = SBUName };
                            if (IsExist(SBUName))
                            {
                                throw new Exception(string.Format(Messages.DataSBUAlreadyExists, new object[] { "Name", i, SBUNameIndex }));
                            }
                            else
                            {
                                SBUMasterDto.SBU = workSheet.Cells[i, SBUNameIndex].Value?.ToString();
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
                                    SBUMasterDto.Status = true;
                                }
                                else
                                {
                                    SBUMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion
                        SBUMasterDto.CreatedBy = Convert.ToInt32(sid);
                        SBUMasterDto.CreatedDate = DateTime.UtcNow;
                        SBUMasterDto.IsActive = true;
                        importSBU.Add(SBUMasterDto);
                    }
                    if (importSBU != null && importSBU.Count > 0)
                    {
                        db.SBUMasters.AddRange(importSBU);
                        db.SaveChanges();
                        count = importSBU.Count;
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