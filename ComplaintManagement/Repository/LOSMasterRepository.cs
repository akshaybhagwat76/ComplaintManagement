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
    public class LOSMasterRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public LOSMasterRepository()
        {
        }

        public LOSMasterVM AddOrUpdate(LOSMasterVM LOSVM)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var LOS = db.LOSMasters.FirstOrDefault(p => p.Id == LOSVM.Id);
                    if (LOS == null)
                    {
                        LOSVM.IsActive = true;
                        LOSVM.CreatedDate = DateTime.UtcNow;
                        LOSVM.UserId = 1;
                        LOSVM.CreatedBy = Convert.ToInt32(sid);
                        LOS = Mapper.Map<LOSMasterVM, LOSMaster>(LOSVM);
                        if (IsExist(LOS.LOSName))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.LOSMasters.Add(LOS);
                        db.SaveChanges();
                        return Mapper.Map<LOSMaster, LOSMasterVM>(LOS);
                    }
                    else
                    {
                        LOSVM.IsActive = true;
                         LOSVM.CreatedDate = LOS.CreatedDate;
                        LOSVM.CreatedBy = LOS.CreatedBy;
                        LOSVM.UpdatedDate = DateTime.UtcNow;
                        LOSVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(LOS).CurrentValues.SetValues(LOSVM);
                        if (IsExist(LOS.LOSName,LOS.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<LOSMaster, LOSMasterVM>(LOS);
                    }
                }
                return new LOSMasterVM();
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

        public List<LOSMasterVM> GetAll()
        {
            List<LOSMaster> LOS = new List<LOSMaster>();
            List<LOSMasterVM> LOSList = new List<LOSMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                LOS = db.LOSMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (LOS != null && LOS.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (LOSMaster item in LOS)
                    {
                        LOSMasterVM catObj = Mapper.Map<LOSMaster, LOSMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            LOSList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return LOSList;
        }

        public LOSMasterVM Get(int id)
        {
            LOSMaster los = new LOSMaster();
            try
            {
                los = db.LOSMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (los == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<LOSMaster, LOSMasterVM>(los);
        }


        public bool Delete(int id)
        {
            var data = db.LOSMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string LOSName)
        {
            return db.LOSMasters.Count(x => x.IsActive && x.LOSName.ToUpper() == LOSName.ToUpper()) > 0;
        }

        public bool IsExist(string LOSName, int id)
        {
            return db.LOSMasters.Count(x => x.IsActive && x.LOSName.ToUpper() == LOSName.ToUpper() && x.Id != id) > 0;
        }

        public string UploadImportLOS(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportImportLOS(string file)
        {
            List<LOSMaster> importLOS = new List<LOSMaster>();
            LOSMaster LOSMasterDto = null;
            int count = 0;
            #region Indexes 
            int LOSNameIndex = 1; int SBUIndex = 2;int  SubSBUIndex = 3; int CompetencyIndex = 4; int StatusIndex = 5;
            #endregion

            string[] statuses = { "active", "inactive" };
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

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
                        LOSMasterDto = new LOSMaster();

                        #region LOS Name
                        //LOS Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, LOSNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, LOSNameIndex }));
                        }
                        else
                        {
                            string LOS = workSheet.Cells[i, LOSNameIndex].Value?.ToString();
                            if (IsExist(LOS))
                            {
                                throw new Exception(string.Format(Messages.DataLOSAlreadyExists, new object[] { "Name", i, LOSNameIndex }));
                            }
                            else
                            {
                                LOSMasterDto.LOSName = workSheet.Cells[i, LOSNameIndex].Value?.ToString();
                            }
                        }
                        #endregion

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
                                LOSMasterDto.SBUId = String.Join(",", SBUDtoValues);
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
                                LOSMasterDto.SubSBUId = String.Join(",", SubSBUDtoValues);
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
                                LOSMasterDto.CompetencyId = String.Join(",", CompetencyDtoValues);
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
                                    LOSMasterDto.Status = true;
                                }
                                else
                                {
                                    LOSMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion

                        LOSMasterDto.CreatedBy = Convert.ToInt32(sid);
                        LOSMasterDto.CreatedDate = DateTime.UtcNow;
                        LOSMasterDto.IsActive = true;
                        importLOS.Add(LOSMasterDto);
                    }
                    if (importLOS != null && importLOS.Count > 0)
                    {
                        db.LOSMasters.AddRange(importLOS);
                        db.SaveChanges();
                        count = importLOS.Count;
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