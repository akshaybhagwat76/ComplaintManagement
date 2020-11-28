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
    public class SubSBUMasterRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public SubSBUMasterRepository()
        {

        }

        public SubSBUMasterVM AddOrUpdate(SubSBUMasterVM SubSBUVM)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var SubSBU = db.SubSBUMasters.FirstOrDefault(p => p.Id == SubSBUVM.Id);
                    if (SubSBU == null)
                    {
                        SubSBUVM.IsActive = true;
                        SubSBUVM.CreatedDate = DateTime.UtcNow;
                        SubSBUVM.UserId = 1;
                        SubSBUVM.CreatedBy = Convert.ToInt32(sid);
                        SubSBU = Mapper.Map<SubSBUMasterVM, SubSBUMaster>(SubSBUVM);
                        if (IsExist(SubSBU.SubSBU))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SubSBUMasters.Add(SubSBU);
                        db.SaveChanges();
                        return Mapper.Map<SubSBUMaster, SubSBUMasterVM>(SubSBU);
                    }
                    else
                    {
                        SubSBUVM.IsActive = true;
                         SubSBUVM.CreatedDate = SubSBU.CreatedDate;
                        SubSBUVM.CreatedBy = SubSBU.CreatedBy;
                        SubSBUVM.UpdatedDate = DateTime.UtcNow;
                        SubSBUVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(SubSBU).CurrentValues.SetValues(SubSBUVM);
                        if (IsExist(SubSBU.SubSBU,SubSBU.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<SubSBUMaster, SubSBUMasterVM>(SubSBU);

                    }
                }
                return new SubSBUMasterVM();
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

        public List<SubSBUMasterVM> GetAll()
        {
            List<SubSBUMaster> SubSBU = new List<SubSBUMaster>();
            List<SubSBUMasterVM> SubSBUList = new List<SubSBUMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                SubSBU = db.SubSBUMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (SubSBU != null && SubSBU.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (SubSBUMaster item in SubSBU)
                    {
                        SubSBUMasterVM catObj = Mapper.Map<SubSBUMaster, SubSBUMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            SubSBUList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return SubSBUList;
        }

        public SubSBUMasterVM Get(int id)
        {
            SubSBUMaster SubSBU = new SubSBUMaster();
            try
            {
                SubSBU = db.SubSBUMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (SubSBU == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<SubSBUMaster, SubSBUMasterVM>(SubSBU);
        }


        public bool Delete(int id)
        {
            var data = db.SubSBUMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string SubSBU)
        {
            return db.SubSBUMasters.Count(x => x.IsActive && x.SubSBU.ToUpper() == SubSBU.ToUpper()) > 0;
        }

        public bool IsExist(string SubSBU, int id)
        {
            return db.SubSBUMasters.Count(x => x.IsActive && x.SubSBU.ToUpper() == SubSBU.ToUpper() && x.Id != id) > 0;
        }

        public string UploadImportSubSBU(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportSubSBU(string file)
        {
            List<SubSBUMaster> importSubSBU = new List<SubSBUMaster>();
            SubSBUMaster SubSBUMasterDto = null;
            int count = 0;
            #region Indexes 
            int SubSBUNameIndex = 1; int StatusIndex = 2;
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
                        SubSBUMasterDto = new SubSBUMaster();

                        #region SBU Name
                        //SBU Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, SubSBUNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, SubSBUNameIndex }));
                        }
                        else
                        {
                            string SubSBUName = workSheet.Cells[i, SubSBUNameIndex].Value?.ToString();
                            SubSBUMasterVM SubsBUMasterDto = new SubSBUMasterVM { SubSBU = SubSBUName };
                            if (IsExist(SubSBUName))
                            {
                                throw new Exception(string.Format(Messages.DataSubSBUAlreadyExists, new object[] { "Name", i, SubSBUNameIndex }));
                            }
                            else
                            {
                                SubSBUMasterDto.SubSBU = workSheet.Cells[i, SubSBUNameIndex].Value?.ToString();
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
                                    SubSBUMasterDto.Status = true;
                                }
                                else
                                {
                                    SubSBUMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion
                        SubSBUMasterDto.CreatedBy = Convert.ToInt32(sid);
                        SubSBUMasterDto.CreatedDate = DateTime.UtcNow;
                        SubSBUMasterDto.IsActive = true;
                        importSubSBU.Add(SubSBUMasterDto);
                    }
                    if (importSubSBU != null && importSubSBU.Count > 0)
                    {
                        db.SubSBUMasters.AddRange(importSubSBU);
                        db.SaveChanges();
                        count = importSubSBU.Count;
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