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
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var SBU = db.SBUMasters.FirstOrDefault(p => p.Id == SBUVM.Id);
                            if (SBU == null)
                            {
                                SBUVM.IsActive = true;
                                SBUVM.CreatedDate = DateTime.UtcNow;
                                SBUVM.UserId = Convert.ToInt32(sid);
                                SBUVM.CreatedBy = Convert.ToInt32(sid);
                                SBU = Mapper.Map<SBUMasterVM, SBUMaster>(SBUVM);
                                if (IsExist(SBU.SBU))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SBUMasters.Add(SBU);
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                SBUMasters_History historyObj = Mapper.Map<SBUMasterVM, SBUMasters_History>(SBUVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Added; historyObj.SBUId = SBU.Id; };
                                db.SBUMasters_History.Add(historyObj);
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
                                if (IsExist(SBU.SBU, SBU.Id))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SaveChanges();

                                SBUMasters_History historyObj = Mapper.Map<SBUMasterVM, SBUMasters_History>(SBUVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Updated; historyObj.SBUId = SBU.Id; };
                                db.SBUMasters_History.Add(historyObj);
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                return Mapper.Map<SBUMaster, SBUMasterVM>(SBU);
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, ex.Message.ToString(), "SBU Create");
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new SBUMasterVM();
            }
            catch (DbEntityValidationException dve)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)), "SBU Create");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(dve);
                throw new Exception(string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)));
            }
            catch (Exception ex)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, ex.Message.ToString(), "SBU Create");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
        }
        public List<SBUMasterVM> GetAll()
        {

            List<SBUMaster> SBU = new List<SBUMaster>();
            List<SBUMasterVM> SBUList = new List<SBUMasterVM>();
            List<UserMasterVM> lstuser = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();
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



                            if (!string.IsNullOrEmpty(item.InvolvedUsersId))
                            {
                                if (item.InvolvedUsersId.Contains(","))
                                {
                                    string[] array = item.InvolvedUsersId.Split(',');
                                    List<string> UserLst = new List<string>();
                                    foreach (string UserLstItem in array)
                                    {
                                        if (lstuser.Where(x => x.Id == Convert.ToInt32(UserLstItem)).FirstOrDefault() != null)
                                        {
                                            UserLst.Add(lstuser.Where(x => x.Id == Convert.ToInt32(UserLstItem)).FirstOrDefault().EmployeeName);
                                        }
                                    }
                                    catObj.InvolvedUser = string.Join(",", UserLst);
                                }
                                else
                                {
                                    catObj.InvolvedUser = lstuser.Where(x => x.Id == Convert.ToInt32(item.InvolvedUsersId)).FirstOrDefault() != null ? lstuser.Where(x => x.Id == Convert.ToInt32(item.InvolvedUsersId)).FirstOrDefault().EmployeeName : string.Empty;
                                }
                            }
                            else
                            {
                                catObj.InvolvedUser = "No data Added";
                            }



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
            SBUMaster SBU = new SBUMaster();
            try
            {
                SBU = db.SBUMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<SBUMaster, SBUMasterVM>(SBU);
        }
        public List<SBUMasterHistoryVM> GetAllHistory()
        {
            List<SBUMasters_History> listdto = new List<SBUMasters_History>();
            List<SBUMasterHistoryVM> lst = new List<SBUMasterHistoryVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                listdto = db.SBUMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (listdto != null && listdto.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (SBUMasters_History item in listdto)
                    {
                        SBUMasterHistoryVM ViewModelDto = Mapper.Map<SBUMasters_History, SBUMasterHistoryVM>(item);
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
        public bool IsExist(string SBU, int id)
        {
            return db.SBUMasters.Count(x => x.IsActive && x.SBU.ToUpper() == SBU.ToUpper() && x.Id != id) > 0;
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
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (importSBU != null && importSBU.Count > 0)
                            {
                                db.SBUMasters.AddRange(importSBU);
                                db.SaveChanges();

                                List<SBUMasterHistoryVM> listVMDto = Mapper.Map<List<SBUMaster>, List<SBUMasterHistoryVM>>(importSBU);

                                List<SBUMasters_History> HistoryDto = Mapper.Map<List<SBUMasterHistoryVM>, List<SBUMasters_History>>(listVMDto);
                                if (HistoryDto != null && HistoryDto.Count > 0)
                                {
                                    HistoryDto.Select(c => { c.EntityState = Messages.Added; c.SBUId = c.Id; return c; }).ToList();
                                }

                                db.SBUMasters_History.AddRange(HistoryDto);
                                db.SaveChanges();
                                dbContextTransaction.Commit();

                                count = importSBU.Count;
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

                        WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SBUId == losid && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
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







        //12/24/2020
        public List<SBUMasterVM> GetAllSBU()
        {

            List<SBUMaster> SBU = new List<SBUMaster>();
            List<SBUMasterVM> SBUList = new List<SBUMasterVM>();
            List<SBUMasterVM> SBULists = new List<SBUMasterVM>();
            List<UserMasterVM> lstuser = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();
            try
            {

                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();


                if (!string.IsNullOrEmpty(sid))
                {
                    SBULists = GetAll();

                    foreach (SBUMasterVM item in SBULists)
                    {
                        SBUMasterVM catObj = item;

                        if (item.InvolvedUsersId != null)
                        {
                            var involveduser = item.InvolvedUsersId.Split(',');
                            foreach (var items in involveduser)
                            {
                                if (sid == items)
                                {
                                    catObj.SBU = item.SBU;
                                    catObj.Id = item.Id;
                                    SBUList.Add(catObj);
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
            return SBUList;


        }
        public List<EmployeeComplaintWorkFlowVM> GetAllSBUReport(string range, string losid)
        {
            List<EmployeeComplaintWorkFlowVM> WorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> WorkFlows = new List<EmployeeComplaintWorkFlow>();
            List<SBUMasterVM> SBULists = new List<SBUMasterVM>();
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    try
                    {
                        SBULists = GetAllSBU();

                        foreach (var item in SBULists)
                        {

                            WorkFlows.AddRange(db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SBUId == item.Id && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList());
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

    }
}