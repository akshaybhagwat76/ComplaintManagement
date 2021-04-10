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
using System.Web.Mvc;

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
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var LOS = db.LOSMasters.FirstOrDefault(p => p.Id == LOSVM.Id);
                            if (LOS == null)
                            {
                                LOSVM.IsActive = true;
                                LOSVM.CreatedDate = DateTime.UtcNow;
                                LOSVM.UserId = Convert.ToInt32(sid);
                                LOSVM.CreatedBy = Convert.ToInt32(sid);
                                LOS = Mapper.Map<LOSMasterVM, LOSMaster>(LOSVM);
                                if (IsExist(LOS.LOSName))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.LOSMasters.Add(LOS);
                                db.SaveChanges();
                                LOSMasters_History historyObj = Mapper.Map<LOSMasterVM, LOSMasters_History>(LOSVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Added; historyObj.LOSId = LOS.Id; };
                                db.LOSMasters_History.Add(historyObj);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
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
                                if (IsExist(LOS.LOSName, LOS.Id))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SaveChanges();
                                LOSMasters_History historyObj = Mapper.Map<LOSMasterVM, LOSMasters_History>(LOSVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Updated; historyObj.LOSId = LOS.Id; };
                                db.LOSMasters_History.Add(historyObj);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                return Mapper.Map<LOSMaster, LOSMasterVM>(LOS);
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, ex.Message.ToString(), "LOS Master");
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new LOSMasterVM();
            }
            catch (DbEntityValidationException dve)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)), "LOS Master");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(dve);
                throw new Exception(string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)));
            }
            catch (Exception ex)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, ex.Message.ToString(), "LOS Master");
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
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<LOSMaster, LOSMasterVM>(los);
        }
        public List<LOSMasterHistoryVM> GetAllHistory()
        {
            List<LOSMasters_History> listdto = new List<LOSMasters_History>();
            List<LOSMasterHistoryVM> lst = new List<LOSMasterHistoryVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                listdto = db.LOSMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (listdto != null && listdto.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (LOSMasters_History item in listdto)
                    {
                        LOSMasterHistoryVM ViewModelDto = Mapper.Map<LOSMasters_History, LOSMasterHistoryVM>(item);
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
            int LOSNameIndex = 1; int SBUIndex = 2; int SubSBUIndex = 3; int CompetencyIndex = 4; int StatusIndex = 5;
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

                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (importLOS != null && importLOS.Count > 0)
                            {
                                db.LOSMasters.AddRange(importLOS);
                                db.SaveChanges();

                                List<LOSMasterHistoryVM> listVMDto = Mapper.Map<List<LOSMaster>, List<LOSMasterHistoryVM>>(importLOS);

                                List<LOSMasters_History> HistoryDto = Mapper.Map<List<LOSMasterHistoryVM>, List<LOSMasters_History>>(listVMDto);
                                if (HistoryDto != null && HistoryDto.Count > 0)
                                {
                                    HistoryDto.Select(c => { c.EntityState = Messages.Added; c.LOSId = c.Id; return c; }).ToList();
                                }

                                db.LOSMasters_History.AddRange(HistoryDto);
                                db.SaveChanges();

                                dbContextTransaction.Commit();

                                count = importLOS.Count;
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


        //12/17/2020
        //public List<EmployeeCompliantMasterVM> GetAllReportLos(int losid)
        //{
        //    List<EmployeeCompliantMasterVM> LOSListDto = new List<EmployeeCompliantMasterVM>();
        //    List<EmployeeComplaintMaster> LOSList = new List<EmployeeComplaintMaster>();

        //    List<UserMaster> User = new List<UserMaster>();
        //    List<UserMasterVM> UsersListDto = new List<UserMasterVM>();
        //    try
        //    {
        //         User = db.UserMasters.Where(i => i.IsActive && i.LOSId == losid).OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
        //         if (User != null && User.Count > 0)
        //         {
        //            int userid = 0;
        //            foreach (UserMaster item in User)
        //            {
        //                userid = Convert.ToInt32(item.Id);
        //                LOSList = db.EmployeeComplaintMasters.Where(i => i.IsActive && i.UserId == userid).ToList();
        //                LOSListDto.Add(LOSList);
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
        //        throw new Exception(ex.Message.ToString());
        //    }
        //    return LOSListDto;
        //}
        public List<EmployeeComplaintWorkFlowVM> GetAllReport(string range, int losid)
        {


            List<EmployeeComplaintWorkFlowVM> WorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> WorkFlows = new List<EmployeeComplaintWorkFlow>();
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    try
                    {

                        WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.LOSId == losid && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                        if (WorkFlows != null && WorkFlows.Count > 0) /*&& usersList != null && usersList.Count > 0)*/
                        {
                            foreach (EmployeeComplaintWorkFlow item in WorkFlows)
                            {
                                EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(item);
                                if (catObj != null)
                                {
                                    catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                    catObj.UpdatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                                    catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId).LOSName : Messages.NotAvailable;
                                    catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId).SBU : Messages.NotAvailable;
                                    catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId) != null ? db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId).SubSBU : Messages.NotAvailable;
                                    var ActionType = db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id) != null ? db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id).ActionType : Messages.NotAvailable;
                                    if (ActionType == "Submitted" || ActionType == "Committee")
                                    {
                                        catObj.ActionType = "In-Progress";
                                    }
                                    else if (ActionType == "Completed")
                                    {
                                        catObj.ActionType = "Closed";
                                    }
                                    else if (ActionType == "Withdrawn")
                                    {
                                        catObj.ActionType = "Withdrawn";
                                    }
                                    else if (ActionType == "Opened")
                                    {
                                        catObj.ActionType = "Opened";
                                    }

                                    if (item.ComplaintNo != null)
                                    {
                                        catObj.ComplaintNo = item.ComplaintNo;
                                    }
                                    else
                                    {
                                        catObj.ComplaintNo = "Not Available";
                                    }

                                    if (item.ComplaintId != 0)
                                    {
                                        int reginoid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).RegionId;

                                        int companyid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).Company;
                                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).CategoryId;
                                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).SubCategoryId;
                                        catObj.CaseType = db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId) != null ? db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId).CaseType : Messages.NotAvailable;
                                        if (categoryid != 0)
                                        {
                                            catObj.CompanyName = db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName != null ? db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName : Messages.NotAvailable;
                                            catObj.RegionName = db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region != null ? db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region : Messages.NotAvailable;
                                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : Messages.NotAvailable;
                                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : Messages.NotAvailable;

                                        }
                                    }


                                    WorkFlowList.Add(catObj);


                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                        throw new Exception(ex.Message.ToString());
                    }
                }
            }
            return WorkFlowList;
        }


        //4/1/2021
        public List<EmployeeComplaintWorkFlowVM> GetAllLosReport(string range)
        {


            List<EmployeeComplaintWorkFlowVM> WorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> WorkFlows = new List<EmployeeComplaintWorkFlow>();
            List<ViewModel.LOSMasterVM> Losmaster = new List<ViewModel.LOSMasterVM>();
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    try
                    {
                        Losmaster = GetAll();
                        if(Losmaster.Count>0)
                        {
                            foreach(var item in Losmaster)
                            {
                                WorkFlows.AddRange(db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.LOSId == item.Id && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList());
                            }
                        }

                      
                        if (WorkFlows != null && WorkFlows.Count > 0) /*&& usersList != null && usersList.Count > 0)*/
                        {
                            foreach (EmployeeComplaintWorkFlow item in WorkFlows)
                            {
                                EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(item);
                                if (catObj != null)
                                {
                                    catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                    catObj.UpdatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                                    catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId).LOSName : Messages.NotAvailable;
                                    catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId).SBU : Messages.NotAvailable;
                                    catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId) != null ? db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId).SubSBU : Messages.NotAvailable;
                                    var ActionType = db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id) != null ? db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id).ActionType : Messages.NotAvailable;
                                    if (ActionType == "Submitted" || ActionType == "Committee")
                                    {
                                        catObj.ActionType = "In-Progress";
                                    }
                                    else if (ActionType == "Completed")
                                    {
                                        catObj.ActionType = "Closed";
                                    }
                                    else if (ActionType == "Withdrawn")
                                    {
                                        catObj.ActionType = "Withdrawn";
                                    }
                                    else if (ActionType == "Opened")
                                    {
                                        catObj.ActionType = "Opened";
                                    }

                                    if (item.ComplaintNo != null)
                                    {
                                        catObj.ComplaintNo = item.ComplaintNo;
                                    }
                                    else
                                    {
                                        catObj.ComplaintNo = "Not Available";
                                    }

                                    if (item.ComplaintId != 0)
                                    {
                                        int reginoid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).RegionId;

                                        int companyid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).Company;
                                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).CategoryId;
                                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).SubCategoryId;
                                        catObj.CaseType = db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId) != null ? db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId).CaseType : Messages.NotAvailable;
                                        if (categoryid != 0)
                                        {
                                            catObj.CompanyName = db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName != null ? db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName : Messages.NotAvailable;
                                            catObj.RegionName = db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region != null ? db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region : Messages.NotAvailable;
                                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : Messages.NotAvailable;
                                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : Messages.NotAvailable;

                                        }
                                    }


                                    WorkFlowList.Add(catObj);


                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                        throw new Exception(ex.Message.ToString());
                    }
                }
            }
            return WorkFlowList;
        }


        public List<EmployeeComplaintWorkFlowVM> GetAllCaseStageReport(string range, string casestype)
        {
            List<EmployeeComplaintWorkFlowVM> WorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> WorkFlows = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> WorkFlows1 = new List<EmployeeComplaintWorkFlow>();
            List<LOSMasterVM> LOSLists = new List<LOSMasterVM>();
            List<SBUMasterVM> SBULists = new List<SBUMasterVM>();
            List<SubSBUMasterVM> SubSBULists = new List<SubSBUMasterVM>();
            var WorkFlowdata = new List<EmployeeComplaintWorkFlow>();
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
               .Select(c => c.Value).SingleOrDefault();


            if (!string.IsNullOrEmpty(sid))
            {
                LOSLists = GetAllLOSdata();
                SBUMasterRepository sbr = new SBUMasterRepository();
                SBULists = sbr.GetAllSBU();
                SubSBUMasterRepository subsbr = new SubSBUMasterRepository();
                SubSBULists = subsbr.GetAllSubSBU();
            }


            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    try
                    {
                        if (casestype == "InProgess")
                        {

                            if (LOSLists != null || SBULists != null || SubSBULists != null)
                            {
                                if (LOSLists != null)
                                {
                                    foreach (LOSMasterVM item in LOSLists)
                                    {
                                        var Losid = db.EmployeeComplaintWorkFlows.Where(s => s.LOSId == item.LOSId).ToList();
                                        if (Losid.Count > 0)
                                        {
                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.LOSId == item.Id && (i.ActionType == "Submitted" || i.ActionType == "Committee") && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                                            WorkFlows1.AddRange(WorkFlows);
                                        }

                                    }
                                }
                                if (SBULists != null)
                                {
                                    foreach (SBUMasterVM item in SBULists)
                                    {


                                        var SBUId = db.EmployeeComplaintWorkFlows.Where(s => s.SBUId == item.Id).ToList();
                                        if (SBUId.Count > 0)
                                        {
                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SBUId == item.Id && (i.ActionType == "Submitted" || i.ActionType == "Committee") && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).Distinct().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();

                                            WorkFlows1.AddRange(WorkFlows);

                                        }
                                    }
                                }
                                if (SubSBULists != null)
                                {
                                    foreach (SubSBUMasterVM item in SubSBULists)
                                    {

                                        WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SubSBUId == item.Id && (i.ActionType == "Submitted" || i.ActionType == "Committee") && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).Distinct().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                                        WorkFlows1.AddRange(WorkFlows);
                                    }
                                }
                            }
                        }


                        else if (casestype == "Closed")
                        {

                            if (LOSLists != null || SBULists != null || SubSBULists != null)
                            {
                                if (LOSLists != null)
                                {
                                    foreach (LOSMasterVM item in LOSLists)
                                    {
                                        var Losid = db.EmployeeComplaintWorkFlows.Where(s => s.LOSId == item.LOSId).ToList();
                                        if (Losid.Count > 0)
                                        {
                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.LOSId == item.Id && (i.ActionType == "Completed") && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                                            WorkFlows1.AddRange(WorkFlows);
                                        }

                                    }
                                }
                                if (SBULists != null)
                                {
                                    foreach (SBUMasterVM item in SBULists)
                                    {


                                        var SBUId = db.EmployeeComplaintWorkFlows.Where(s => s.SBUId == item.Id).ToList();
                                        if (SBUId.Count > 0)
                                        {
                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SBUId == item.Id && (i.ActionType == "Completed") && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).Distinct().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();

                                            WorkFlows1.AddRange(WorkFlows);

                                        }
                                    }
                                }
                                if (SubSBULists != null)
                                {
                                    foreach (SubSBUMasterVM item in SubSBULists)
                                    {

                                        WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SubSBUId == item.Id && (i.ActionType == "S") && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).Distinct().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                                        WorkFlows1.AddRange(WorkFlows);
                                    }
                                }
                            }

                            //WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.ActionType == "Completed" && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();

                        }



                        if (WorkFlows1 != null && WorkFlows1.Count > 0) /*&& usersList != null && usersList.Count > 0)*/
                        {
                            foreach (EmployeeComplaintWorkFlow item in WorkFlows1.Distinct())
                            {
                                EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(item);
                                if (catObj != null)
                                {
                                    catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                    catObj.UpdatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                                    catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId).LOSName : Messages.NotAvailable;
                                    catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId).SBU : Messages.NotAvailable;
                                    catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId) != null ? db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId).SubSBU : Messages.NotAvailable;
                                    var ActionType = db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id) != null ? db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id).ActionType : Messages.NotAvailable;

                                    if (ActionType == "Submitted" || ActionType == "Committee")
                                    {
                                        catObj.ActionType = "In-Progress";
                                    }
                                    else if (ActionType == "Completed")
                                    {
                                        catObj.ActionType = "Closed";
                                    }
                                    else if (ActionType == "Withdrawn")
                                    {
                                        catObj.ActionType = "Withdrawn";
                                    }
                                    else if (ActionType == "Opened")
                                    {
                                        catObj.ActionType = "Opened";
                                    }
                                    if (item.ComplaintId != 0)
                                    {
                                        int reginoid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).RegionId;

                                        int companyid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).Company;
                                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).CategoryId;
                                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).SubCategoryId;
                                        catObj.CaseType = db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId) != null ? db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId).CaseType : Messages.NotAvailable;
                                        if (categoryid != 0)
                                        {
                                            catObj.CompanyName = db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName != null ? db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName : Messages.NotAvailable;
                                            catObj.RegionName = db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region != null ? db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region : Messages.NotAvailable;
                                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : Messages.NotAvailable;
                                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : Messages.NotAvailable;

                                        }
                                    }


                                    WorkFlowList.Add(catObj);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                        throw new Exception(ex.Message.ToString());
                    }
                }
            }
            return WorkFlowList;
        }

        //2:55
        public List<EmployeeComplaintWorkFlowVM> GetAllTypeStageReport(string range, string casestage)
        {
            List<EmployeeComplaintWorkFlowVM> WorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> WorkFlows = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> WorkFlows1 = new List<EmployeeComplaintWorkFlow>();
            List<HrRoleUserMasterVM> WorkFlowListed1 = new List<HrRoleUserMasterVM>();
            List<HR_Role> WorkFlow1 = new List<HR_Role>();
            List<LOSMasterVM> LOSLists = new List<LOSMasterVM>();
            List<SBUMasterVM> SBULists = new List<SBUMasterVM>();
            List<SubSBUMasterVM> SubSBULists = new List<SubSBUMasterVM>();
            var WorkFlowdata = new List<EmployeeComplaintWorkFlow>();
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
               .Select(c => c.Value).SingleOrDefault();


            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    LOSLists = GetAllLOSdata();
                    SBUMasterRepository sbr = new SBUMasterRepository();
                    SBULists = sbr.GetAllSBU();
                    SubSBUMasterRepository subsbr = new SubSBUMasterRepository();
                    SubSBULists = subsbr.GetAllSubSBU();

                    try
                    {
                        if (casestage == "Actionable")
                        {
                            WorkFlow1 = db.HR_Role.Where(i => i.IsActive.Value && i.CaseType == "Actionable" && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();

                            if (LOSLists != null || SBULists != null || SubSBULists != null)
                            {
                                if (LOSLists != null)
                                {
                                    foreach (LOSMasterVM item in LOSLists)
                                    {
                                        var Losid = db.EmployeeComplaintWorkFlows.Where(s => s.LOSId == item.LOSId).ToList();
                                        if (Losid.Count > 0)
                                        {

                                            foreach (HR_Role items in WorkFlow1)
                                            {
                                                //HrRoleUserMasterVM catObj = Mapper.Map<HR_Role, HrRoleUserMasterVM>(item);

                                                WorkFlows = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.ComplaintId == items.ComplentId && x.CreatedBy == items.UserId && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();

                                                WorkFlows1.AddRange(WorkFlows);
                                            }

                                        }
                                    }
                                }
                                if (SBULists != null)
                                {
                                    foreach (SBUMasterVM item in SBULists)
                                    {


                                        var SBUId = db.EmployeeComplaintWorkFlows.Where(s => s.SBUId == item.Id).ToList();
                                        if (SBUId.Count > 0)
                                        {
                                            foreach (HR_Role items in WorkFlow1)
                                            {
                                                //HrRoleUserMasterVM catObj = Mapper.Map<HR_Role, HrRoleUserMasterVM>(item);

                                                WorkFlows = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.ComplaintId == items.ComplentId && x.CreatedBy == items.UserId && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();

                                                WorkFlows1.AddRange(WorkFlows);
                                            }

                                        }
                                    }
                                }
                                if (SubSBULists != null)
                                {
                                    foreach (SubSBUMasterVM item in SubSBULists)
                                    {
                                        foreach (HR_Role items in WorkFlow1)
                                        {
                                            //HrRoleUserMasterVM catObj = Mapper.Map<HR_Role, HrRoleUserMasterVM>(item);

                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.ComplaintId == items.ComplentId && x.CreatedBy == items.UserId && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();

                                            WorkFlows1.AddRange(WorkFlows);
                                        }
                                    }
                                }
                            }
                        }
                        else if (casestage == "NonActionable")
                        {
                            //WorkFlow1 = db.HR_Role.Where(i => i.IsActive && i.CaseType == "NonActionable" && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                            WorkFlow1 = db.HR_Role.Where(i => i.IsActive.Value && i.CaseType == "NonActionable" && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();

                            if (LOSLists != null || SBULists != null || SubSBULists != null)
                            {
                                if (LOSLists != null)
                                {
                                    foreach (LOSMasterVM item in LOSLists)
                                    {
                                        var Losid = db.EmployeeComplaintWorkFlows.Where(s => s.LOSId == item.LOSId).ToList();
                                        if (Losid.Count > 0)
                                        {

                                            foreach (HR_Role items in WorkFlow1)
                                            {
                                                //HrRoleUserMasterVM catObj = Mapper.Map<HR_Role, HrRoleUserMasterVM>(item);

                                                WorkFlows = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.ComplaintId == items.ComplentId && x.CreatedBy == items.UserId && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();

                                                WorkFlows1.AddRange(WorkFlows);
                                            }

                                        }
                                    }
                                }
                                if (SBULists != null)
                                {
                                    foreach (SBUMasterVM item in SBULists)
                                    {


                                        var SBUId = db.EmployeeComplaintWorkFlows.Where(s => s.SBUId == item.Id).ToList();
                                        if (SBUId.Count > 0)
                                        {
                                            foreach (HR_Role items in WorkFlow1)
                                            {
                                                //HrRoleUserMasterVM catObj = Mapper.Map<HR_Role, HrRoleUserMasterVM>(item);

                                                WorkFlows = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.ComplaintId == items.ComplentId && x.CreatedBy == items.UserId && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();

                                                WorkFlows1.AddRange(WorkFlows);
                                            }

                                        }
                                    }
                                }
                                if (SubSBULists != null)
                                {
                                    foreach (SubSBUMasterVM item in SubSBULists)
                                    {
                                        foreach (HR_Role items in WorkFlow1)
                                        {
                                            //HrRoleUserMasterVM catObj = Mapper.Map<HR_Role, HrRoleUserMasterVM>(item);

                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.ComplaintId == items.ComplentId && x.CreatedBy == items.UserId && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();

                                            WorkFlows1.AddRange(WorkFlows);
                                        }
                                    }
                                }

                            }
                        }
                        if (WorkFlows1 != null && WorkFlows1.Count > 0) /*&& usersList != null && usersList.Count > 0)*/
                        {
                            //foreach (HR_Role item in WorkFlow1)
                            //{
                            //HrRoleUserMasterVM catObj = Mapper.Map<HR_Role, HrRoleUserMasterVM>(item);

                            //WorkFlows = db.EmployeeComplaintWorkFlows.Where(x => x.ComplaintId == item.ComplentId && x.CreatedBy==item.UserId ).ToList();

                            foreach (EmployeeComplaintWorkFlow items in WorkFlows1.Distinct())
                            {

                                EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(items);
                                if (WorkFlows1 != null)
                                {
                                    catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                    catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId).LOSName : Messages.NotAvailable;
                                    catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId).SBU : Messages.NotAvailable;
                                    catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId) != null ? db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId).SubSBU : Messages.NotAvailable;
                                    var ActionType = db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == items.Id) != null ? db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == items.Id).ActionType : Messages.NotAvailable;
                                    if (ActionType == "Submitted" || ActionType == "Committee")
                                    {
                                        catObj.ActionType = "In-Progress";
                                    }
                                    else if (ActionType == "Completed")
                                    {
                                        catObj.ActionType = "Closed";
                                    }
                                    else if (ActionType == "Withdrawn")
                                    {
                                        catObj.ActionType = "Withdrawn";
                                    }
                                    else if (ActionType == "Opened")
                                    {
                                        catObj.ActionType = "Opened";
                                    }

                                    if (items.ComplaintNo != null)
                                    {
                                        catObj.ComplaintNo = items.ComplaintNo;
                                    }
                                    else
                                    {
                                        catObj.ComplaintNo = "Not Available";
                                    }

                                    if (items.ComplaintId != 0)
                                    {
                                        int reginoid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).RegionId;

                                        int companyid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).Company;
                                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).CategoryId;
                                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).SubCategoryId;

                                        if (casestage == "Actionable")
                                        {
                                            catObj.CaseType = "Actionable";
                                        }
                                        else if (casestage == "NonActionable")
                                        {
                                            catObj.CaseType = "NonActionable";
                                        }
                                        if (categoryid != 0)
                                        {
                                            catObj.CompanyName = db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName != null ? db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName : Messages.NotAvailable;
                                            catObj.RegionName = db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region != null ? db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region : Messages.NotAvailable;
                                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : Messages.NotAvailable;
                                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : Messages.NotAvailable;

                                        }
                                    }

                                }
                                WorkFlowList.Add(catObj);

                            }


                        }
                    }
                    catch (Exception ex)
                    {
                        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                        throw new Exception(ex.Message.ToString());
                    }
                }
            }

            return WorkFlowList;
        }

        //added on 12/24/2020
        public List<LOSMasterVM> GetAllLOSdata()
        {
            List<LOSMaster> LOS = new List<LOSMaster>();
            List<LOSMasterVM> LOSList = new List<LOSMasterVM>();
            List<LOSMasterVM> LOSLists = new List<LOSMasterVM>();
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();


                if (!string.IsNullOrEmpty(sid))
                {
                    LOSLists = GetAll();

                    foreach (LOSMasterVM item in LOSLists)
                    {
                        LOSMasterVM catObj = item;

                        if (item.InvolvedUsersId != null)
                        {
                            var involveduser = item.InvolvedUsersId.Split(',');
                            foreach (var items in involveduser)
                            {
                                if (sid == items)
                                {
                                    catObj.LOSName = item.LOSName;
                                    catObj.LOSId = item.Id;
                                    LOSList.Add(catObj);
                                }



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
            return LOSList;
        }
        public List<CategoryMasterVM> GetAllCategoryReport()
        {
            List<CategoryMaster> category = new List<CategoryMaster>();
            List<CategoryMasterVM> categoryList = new List<CategoryMasterVM>();
            List<CategoryMasterVM> categoryLists = new List<CategoryMasterVM>();
            List<LOSMasterVM> LOSLists = new List<LOSMasterVM>();
            List<SBUMasterVM> SBULists = new List<SBUMasterVM>();
            List<SubSBUMasterVM> SubSBULists = new List<SubSBUMasterVM>();
            List<EmployeeComplaintWorkFlow> emp = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintMaster> emps = new List<EmployeeComplaintMaster>();
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();


                if (!string.IsNullOrEmpty(sid))
                {
                    LOSLists = GetAllLOSdata();
                    SBUMasterRepository sbr = new SBUMasterRepository();
                    SBULists = sbr.GetAllSBU();
                    SubSBUMasterRepository subsbr = new SubSBUMasterRepository();
                    SubSBULists = subsbr.GetAllSubSBU();

                    if (LOSLists != null || SBULists != null || SubSBULists != null)
                    {
                        if (LOSLists != null)
                        {
                            foreach (LOSMasterVM item in LOSLists)
                            {

                                emp = db.EmployeeComplaintWorkFlows.Where(x => x.LOSId == item.Id).ToList();
                                if (emp != null)
                                {
                                    foreach (EmployeeComplaintWorkFlow items in emp)
                                    {
                                        if (items != null)
                                        {

                                            int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == items.ComplaintId).CategoryId;

                                            CategoryMasterVM catObj = new CategoryMasterVM();
                                            if (categoryid != 0)
                                            {
                                                catObj.CategoryName = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName;
                                                catObj.Id = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).Id;
                                            }

                                            categoryList.Add(catObj);

                                        }


                                    }
                                }






                            }
                        }

                        if (SBULists != null)
                        {
                            foreach (SBUMasterVM item in SBULists)
                            {

                                emp = db.EmployeeComplaintWorkFlows.Where(x => x.SBUId == item.Id).ToList();
                                if (emp != null)
                                {
                                    foreach (EmployeeComplaintWorkFlow items in emp)
                                    {
                                        if (items != null)
                                        {

                                            int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == items.ComplaintId).CategoryId;

                                            CategoryMasterVM catObj = new CategoryMasterVM();
                                            if (categoryid != 0)
                                            {
                                                catObj.CategoryName = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName;
                                                catObj.Id = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).Id;
                                            }

                                            categoryList.Add(catObj);

                                        }


                                    }
                                }






                            }
                        }

                        if (SubSBULists != null)
                        {
                            foreach (SubSBUMasterVM item in SubSBULists)
                            {

                                emp = db.EmployeeComplaintWorkFlows.Where(x => x.SubSBUId == item.Id).ToList();
                                if (emp != null)
                                {
                                    foreach (EmployeeComplaintWorkFlow items in emp)
                                    {
                                        if (items != null)
                                        {

                                            int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == items.ComplaintId).CategoryId;

                                            CategoryMasterVM catObj = new CategoryMasterVM();
                                            if (categoryid != 0)
                                            {
                                                catObj.CategoryName = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName;
                                                catObj.Id = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).Id;
                                            }

                                            categoryList.Add(catObj);

                                        }


                                    }
                                }






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
            categoryList = categoryList.Distinct().ToList();

            return categoryList;
        }

           public List<EmployeeComplaintWorkFlowVM> GetAllReportLos(string range, string values)
        {


            List<EmployeeComplaintWorkFlowVM> WorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> WorkFlows = new List<EmployeeComplaintWorkFlow>();
            List<LOSMasterVM> LOSLists = new List<LOSMasterVM>();
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    try
                    {

                     LOSLists= GetAllLOSdata();

                        foreach (var item in LOSLists)
                        {
                                WorkFlows.AddRange(db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.LOSId == item.Id && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList());
                            
                        }
                       
                        if (WorkFlows != null && WorkFlows.Count > 0) /*&& usersList != null && usersList.Count > 0)*/
                        {
                            foreach (EmployeeComplaintWorkFlow item in WorkFlows)
                            {
                                EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(item);
                                if (catObj != null)
                                {
                                    catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                    catObj.UpdatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                                    catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId).LOSName : Messages.NotAvailable;
                                    catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId).SBU : Messages.NotAvailable;
                                    catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId) != null ? db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId).SubSBU : Messages.NotAvailable;
                                    var ActionType = db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id) != null ? db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id).ActionType : Messages.NotAvailable;
                                    if (ActionType == "Submitted" || ActionType == "Committee")
                                    {
                                        catObj.ActionType = "In-Progress";
                                    }
                                    else if (ActionType == "Completed")
                                    {
                                        catObj.ActionType = "Closed";
                                    }
                                    else if (ActionType == "Withdrawn")
                                    {
                                        catObj.ActionType = "Withdrawn";
                                    }
                                    else if (ActionType == "Opened")
                                    {
                                        catObj.ActionType = "Opened";
                                    }

                                    if (item.ComplaintNo != null)
                                    {
                                        catObj.ComplaintNo = item.ComplaintNo;
                                    }
                                    else
                                    {
                                        catObj.ComplaintNo = "Not Available";
                                    }

                                    if (item.ComplaintId != 0)
                                    {
                                        int reginoid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).RegionId;

                                        int companyid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).Company;
                                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).CategoryId;
                                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).SubCategoryId;
                                        catObj.CaseType = db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId) != null ? db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId).CaseType : Messages.NotAvailable;
                                        if (categoryid != 0)
                                        {
                                            catObj.CompanyName = db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName != null ? db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName : Messages.NotAvailable;
                                            catObj.RegionName = db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region != null ? db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region : Messages.NotAvailable;
                                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : Messages.NotAvailable;
                                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : Messages.NotAvailable;

                                        }
                                    }


                                    WorkFlowList.Add(catObj);


                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                        throw new Exception(ex.Message.ToString());
                    }
                }
            }
            return WorkFlowList;
        }
        public List<EmployeeComplaintWorkFlowVM> GetAllTypeReport(string range, string casestage)
        {
            List<EmployeeComplaintWorkFlowVM> WorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> WorkFlows = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> WorkFlows1 = new List<EmployeeComplaintWorkFlow>();
            List<HrRoleUserMasterVM> WorkFlowListed1 = new List<HrRoleUserMasterVM>();
            List<HR_Role> WorkFlow1 = new List<HR_Role>();
            List<LOSMasterVM> LOSLists = new List<LOSMasterVM>();
            List<SBUMasterVM> SBULists = new List<SBUMasterVM>();
            List<SubSBUMasterVM> SubSBULists = new List<SubSBUMasterVM>();
            var WorkFlowdata = new List<EmployeeComplaintWorkFlow>();
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
               .Select(c => c.Value).SingleOrDefault();


            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    LOSLists = GetAllLOSdata();
                    SBUMasterRepository sbr = new SBUMasterRepository();
                    SBULists = sbr.GetAllSBU();
                    SubSBUMasterRepository subsbr = new SubSBUMasterRepository();
                    SubSBULists = subsbr.GetAllSubSBU();

                    try
                    {
                        
                            WorkFlow1 = db.HR_Role.Where(i => i.IsActive.Value  && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();

                            if (LOSLists != null || SBULists != null || SubSBULists != null)
                            {
                                if (LOSLists != null)
                                {
                                    foreach (LOSMasterVM item in LOSLists)
                                    {
                                        var Losid = db.EmployeeComplaintWorkFlows.Where(s => s.LOSId == item.LOSId).ToList();
                                        if (Losid.Count > 0)
                                        {

                                            foreach (HR_Role items in WorkFlow1)
                                            {
                                                //HrRoleUserMasterVM catObj = Mapper.Map<HR_Role, HrRoleUserMasterVM>(item);

                                                WorkFlows = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.ComplaintId == items.ComplentId && x.CreatedBy == items.UserId && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();

                                                WorkFlows1.AddRange(WorkFlows);
                                            }

                                        }
                                    }
                                }
                                if (SBULists != null)
                                {
                                    foreach (SBUMasterVM item in SBULists)
                                    {


                                        var SBUId = db.EmployeeComplaintWorkFlows.Where(s => s.SBUId == item.Id).ToList();
                                        if (SBUId.Count > 0)
                                        {
                                            foreach (HR_Role items in WorkFlow1)
                                            {
                                                //HrRoleUserMasterVM catObj = Mapper.Map<HR_Role, HrRoleUserMasterVM>(item);

                                                WorkFlows = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.ComplaintId == items.ComplentId && x.CreatedBy == items.UserId && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();

                                                WorkFlows1.AddRange(WorkFlows);
                                            }

                                        }
                                    }
                                }
                                if (SubSBULists != null)
                                {
                                    foreach (SubSBUMasterVM item in SubSBULists)
                                    {
                                        foreach (HR_Role items in WorkFlow1)
                                        {
                                            //HrRoleUserMasterVM catObj = Mapper.Map<HR_Role, HrRoleUserMasterVM>(item);

                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(x => x.IsActive && x.LOSId == item.Id && x.ComplaintId == items.ComplentId && x.CreatedBy == items.UserId && x.CreatedDate >= fromDate && x.CreatedDate <= toDate).ToList();

                                            WorkFlows1.AddRange(WorkFlows);
                                        }
                                    }
                                }
                            }
                        
                       
                        if (WorkFlows1 != null && WorkFlows1.Count > 0) /*&& usersList != null && usersList.Count > 0)*/
                        {
                            //foreach (HR_Role item in WorkFlow1)
                            //{
                            //HrRoleUserMasterVM catObj = Mapper.Map<HR_Role, HrRoleUserMasterVM>(item);

                            //WorkFlows = db.EmployeeComplaintWorkFlows.Where(x => x.ComplaintId == item.ComplentId && x.CreatedBy==item.UserId ).ToList();

                            foreach (EmployeeComplaintWorkFlow items in WorkFlows1.Distinct())
                            {

                                EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(items);
                                if (WorkFlows1 != null)
                                {
                                    catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                    catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId).LOSName : Messages.NotAvailable;
                                    catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId).SBU : Messages.NotAvailable;
                                    catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId) != null ? db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId).SubSBU : Messages.NotAvailable;
                                    var ActionType = db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == items.Id) != null ? db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == items.Id).ActionType : Messages.NotAvailable;
                                    if (ActionType == "Submitted" || ActionType == "Committee")
                                    {
                                        catObj.ActionType = "In-Progress";
                                    }
                                    else if (ActionType == "Completed")
                                    {
                                        catObj.ActionType = "Closed";
                                    }
                                    else if (ActionType == "Withdrawn")
                                    {
                                        catObj.ActionType = "Withdrawn";
                                    }
                                    else if (ActionType == "Opened")
                                    {
                                        catObj.ActionType = "Opened";
                                    }

                                    if (items.ComplaintNo != null)
                                    {
                                        catObj.ComplaintNo = items.ComplaintNo;
                                    }
                                    else
                                    {
                                        catObj.ComplaintNo = "Not Available";
                                    }

                                    if (items.ComplaintId != 0)
                                    {
                                        int reginoid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).RegionId;

                                        int companyid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).Company;
                                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).CategoryId;
                                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).SubCategoryId;

                                        if (casestage == "Actionable")
                                        {
                                            catObj.CaseType = "Actionable";
                                        }
                                        else if (casestage == "NonActionable")
                                        {
                                            catObj.CaseType = "NonActionable";
                                        }
                                        if (categoryid != 0)
                                        {
                                            catObj.CompanyName = db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName != null ? db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName : Messages.NotAvailable;
                                            catObj.RegionName = db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region != null ? db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region : Messages.NotAvailable;
                                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : Messages.NotAvailable;
                                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : Messages.NotAvailable;
                                            catObj.CaseType = db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId) != null ? db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId).CaseType : Messages.NotAvailable;

                                        }
                                    }

                                }
                                WorkFlowList.Add(catObj);

                            }


                        }
                    }
                    catch (Exception ex)
                    {
                        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                        throw new Exception(ex.Message.ToString());
                    }
                }
            }

            return WorkFlowList;
        }
        public List<EmployeeComplaintWorkFlowVM> GetAllStageReport(string range, string casestype)
        {
            List<EmployeeComplaintWorkFlowVM> WorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> WorkFlows = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> WorkFlows1 = new List<EmployeeComplaintWorkFlow>();
            List<LOSMasterVM> LOSLists = new List<LOSMasterVM>();
            List<SBUMasterVM> SBULists = new List<SBUMasterVM>();
            List<SubSBUMasterVM> SubSBULists = new List<SubSBUMasterVM>();
            var WorkFlowdata = new List<EmployeeComplaintWorkFlow>();
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
               .Select(c => c.Value).SingleOrDefault();


            if (!string.IsNullOrEmpty(sid))
            {
                LOSLists = GetAllLOSdata();
                SBUMasterRepository sbr = new SBUMasterRepository();
                SBULists = sbr.GetAllSBU();
                SubSBUMasterRepository subsbr = new SubSBUMasterRepository();
                SubSBULists = subsbr.GetAllSubSBU();
            }


            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    try
                    {
                       
                            if (LOSLists != null || SBULists != null || SubSBULists != null)
                            {
                                if (LOSLists != null)
                                {
                                    foreach (LOSMasterVM item in LOSLists)
                                    {
                                        var Losid = db.EmployeeComplaintWorkFlows.Where(s => s.LOSId == item.LOSId).ToList();
                                        if (Losid.Count > 0)
                                        {
                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.LOSId == item.Id && (i.ActionType == "Submitted" || i.ActionType == "Committee" || i.ActionType == "Completed") && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                                            WorkFlows1.AddRange(WorkFlows);
                                        }

                                    }
                                }
                                if (SBULists != null)
                                {
                                    foreach (SBUMasterVM item in SBULists)
                                    {


                                        var SBUId = db.EmployeeComplaintWorkFlows.Where(s => s.SBUId == item.Id).ToList();
                                        if (SBUId.Count > 0)
                                        {
                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SBUId == item.Id && (i.ActionType == "Submitted" || i.ActionType == "Committee" || i.ActionType == "Completed") && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).Distinct().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();

                                            WorkFlows1.AddRange(WorkFlows);

                                        }
                                    }
                                }
                                if (SubSBULists != null)
                                {
                                    foreach (SubSBUMasterVM item in SubSBULists)
                                    {

                                        WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SubSBUId == item.Id && (i.ActionType == "Submitted" || i.ActionType == "Committee" || i.ActionType == "Completed") && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).Distinct().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                                        WorkFlows1.AddRange(WorkFlows);
                                    }
                                }
                            }
                        

                     

                        if (WorkFlows1 != null && WorkFlows1.Count > 0) /*&& usersList != null && usersList.Count > 0)*/
                        {
                            foreach (EmployeeComplaintWorkFlow item in WorkFlows1.Distinct())
                            {
                                EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(item);
                                if (catObj != null)
                                {
                                    catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                    catObj.UpdatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                                    catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId).LOSName : Messages.NotAvailable;
                                    catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId).SBU : Messages.NotAvailable;
                                    catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId) != null ? db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId).SubSBU : Messages.NotAvailable;
                                    var ActionType = db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id) != null ? db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id).ActionType : Messages.NotAvailable;

                                    if (ActionType == "Submitted" || ActionType == "Committee")
                                    {
                                        catObj.ActionType = "In-Progress";
                                    }
                                    else if (ActionType == "Completed")
                                    {
                                        catObj.ActionType = "Closed";
                                    }
                                    else if (ActionType == "Withdrawn")
                                    {
                                        catObj.ActionType = "Withdrawn";
                                    }
                                    else if (ActionType == "Opened")
                                    {
                                        catObj.ActionType = "Opened";
                                    }
                                    if (item.ComplaintId != 0)
                                    {
                                        int reginoid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).RegionId;

                                        int companyid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).Company;
                                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).CategoryId;
                                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).SubCategoryId;
                                        catObj.CaseType = db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId) != null ? db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId).CaseType : Messages.NotAvailable;
                                        if (categoryid != 0)
                                        {
                                            catObj.CompanyName = db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName != null ? db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName : Messages.NotAvailable;
                                            catObj.RegionName = db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region != null ? db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region : Messages.NotAvailable;
                                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : Messages.NotAvailable;
                                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : Messages.NotAvailable;

                                        }
                                    }


                                    WorkFlowList.Add(catObj);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                        throw new Exception(ex.Message.ToString());
                    }
                }
            }
            return WorkFlowList;
        }
        public List<EmployeeComplaintWorkFlowVM> GetAllCaseReport(string casestype)
        {
            List<EmployeeComplaintWorkFlowVM> WorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> WorkFlows = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> WorkFlows1 = new List<EmployeeComplaintWorkFlow>();
            List<LOSMasterVM> LOSLists = new List<LOSMasterVM>();
            List<SBUMasterVM> SBULists = new List<SBUMasterVM>();
            List<SubSBUMasterVM> SubSBULists = new List<SubSBUMasterVM>();
            var WorkFlowdata = new List<EmployeeComplaintWorkFlow>();
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
               .Select(c => c.Value).SingleOrDefault();


            if (!string.IsNullOrEmpty(sid))
            {
                LOSLists = GetAllLOSdata();
                SBUMasterRepository sbr = new SBUMasterRepository();
                SBULists = sbr.GetAllSBU();
                SubSBUMasterRepository subsbr = new SubSBUMasterRepository();
                SubSBULists = subsbr.GetAllSubSBU();
            }


            
              
                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    try
                    {
                        if (casestype == "InProgess")
                        {

                            if (LOSLists != null || SBULists != null || SubSBULists != null)
                            {
                                if (LOSLists != null)
                                {
                                    foreach (LOSMasterVM item in LOSLists)
                                    {
                                        var Losid = db.EmployeeComplaintWorkFlows.Where(s => s.LOSId == item.LOSId).ToList();
                                        if (Losid.Count > 0)
                                        {
                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.LOSId == item.Id && (i.ActionType == "Submitted" || i.ActionType == "Committee")).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                                            WorkFlows1.AddRange(WorkFlows);
                                        }

                                    }
                                }
                                if (SBULists != null)
                                {
                                    foreach (SBUMasterVM item in SBULists)
                                    {


                                        var SBUId = db.EmployeeComplaintWorkFlows.Where(s => s.SBUId == item.Id).ToList();
                                        if (SBUId.Count > 0)
                                        {
                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SBUId == item.Id && (i.ActionType == "Submitted" || i.ActionType == "Committee")).Distinct().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();

                                            WorkFlows1.AddRange(WorkFlows);

                                        }
                                    }
                                }
                                if (SubSBULists != null)
                                {
                                    foreach (SubSBUMasterVM item in SubSBULists)
                                    {

                                        WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SubSBUId == item.Id && (i.ActionType == "Submitted" || i.ActionType == "Committee")).Distinct().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                                        WorkFlows1.AddRange(WorkFlows);
                                    }
                                }
                            }
                        }


                        else if (casestype == "Closed")
                        {

                            if (LOSLists != null || SBULists != null || SubSBULists != null)
                            {
                                if (LOSLists != null)
                                {
                                    foreach (LOSMasterVM item in LOSLists)
                                    {
                                        var Losid = db.EmployeeComplaintWorkFlows.Where(s => s.LOSId == item.LOSId).ToList();
                                        if (Losid.Count > 0)
                                        {
                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.LOSId == item.Id && (i.ActionType == "Completed")).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                                            WorkFlows1.AddRange(WorkFlows);
                                        }

                                    }
                                }
                                if (SBULists != null)
                                {
                                    foreach (SBUMasterVM item in SBULists)
                                    {


                                        var SBUId = db.EmployeeComplaintWorkFlows.Where(s => s.SBUId == item.Id).ToList();
                                        if (SBUId.Count > 0)
                                        {
                                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SBUId == item.Id && (i.ActionType == "Completed")).Distinct().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();

                                            WorkFlows1.AddRange(WorkFlows);

                                        }
                                    }
                                }
                                if (SubSBULists != null)
                                {
                                    foreach (SubSBUMasterVM item in SubSBULists)
                                    {

                                        WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SubSBUId == item.Id && (i.ActionType == "S")).Distinct().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                                        WorkFlows1.AddRange(WorkFlows);
                                    }
                                }
                            }

                            //WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.ActionType == "Completed" && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();

                        }



                        if (WorkFlows1 != null && WorkFlows1.Count > 0) /*&& usersList != null && usersList.Count > 0)*/
                        {
                            foreach (EmployeeComplaintWorkFlow item in WorkFlows1.Distinct())
                            {
                                EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(item);
                                if (catObj != null)
                                {
                                    catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                    catObj.UpdatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                                    catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == catObj.LOSId).LOSName : Messages.NotAvailable;
                                    catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == catObj.SBUId).SBU : Messages.NotAvailable;
                                    catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId) != null ? db.SubSBUMasters.FirstOrDefault(x => x.Id == catObj.SubSBUId).SubSBU : Messages.NotAvailable;
                                    var ActionType = db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id) != null ? db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == item.Id).ActionType : Messages.NotAvailable;

                                    if (ActionType == "Submitted" || ActionType == "Committee")
                                    {
                                        catObj.ActionType = "In-Progress";
                                    }
                                    else if (ActionType == "Completed")
                                    {
                                        catObj.ActionType = "Closed";
                                    }
                                    else if (ActionType == "Withdrawn")
                                    {
                                        catObj.ActionType = "Withdrawn";
                                    }
                                    else if (ActionType == "Opened")
                                    {
                                        catObj.ActionType = "Opened";
                                    }
                                    if (item.ComplaintId != 0)
                                    {
                                        int reginoid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).RegionId;

                                        int companyid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).Company;
                                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).CategoryId;
                                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == catObj.ComplaintId).SubCategoryId;
                                        catObj.CaseType = db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId) != null ? db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == catObj.ComplaintId).CaseType : Messages.NotAvailable;
                                        if (categoryid != 0)
                                        {
                                            catObj.CompanyName = db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName != null ? db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName : Messages.NotAvailable;
                                            catObj.RegionName = db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region != null ? db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region : Messages.NotAvailable;
                                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : Messages.NotAvailable;
                                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : Messages.NotAvailable;

                                        }
                                    }


                                    WorkFlowList.Add(catObj);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                        throw new Exception(ex.Message.ToString());
                    }
                }
            
            return WorkFlowList;
        }


    }
}