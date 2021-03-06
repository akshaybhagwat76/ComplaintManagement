﻿using AutoMapper;
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
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var SubSBU = db.SubSBUMasters.FirstOrDefault(p => p.Id == SubSBUVM.Id);
                            if (SubSBU == null)
                            {
                                SubSBUVM.IsActive = true;
                                SubSBUVM.CreatedDate = DateTime.UtcNow;
                                SubSBUVM.UserId = Convert.ToInt32(sid);
                                SubSBUVM.CreatedBy = Convert.ToInt32(sid);
                                SubSBU = Mapper.Map<SubSBUMasterVM, SubSBUMaster>(SubSBUVM);
                                if (IsExist(SubSBU.SubSBU))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SubSBUMasters.Add(SubSBU);
                                db.SaveChanges();

                                SubSBUMasters_History historyObj = Mapper.Map<SubSBUMasterVM, SubSBUMasters_History>(SubSBUVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Added; historyObj.SubSBUId = SubSBU.Id; };
                                db.SubSBUMasters_History.Add(historyObj);
                                db.SaveChanges();
                                dbContextTransaction.Commit();

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
                                if (IsExist(SubSBU.SubSBU, SubSBU.Id))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SaveChanges();


                                SubSBUMasters_History historyObj = Mapper.Map<SubSBUMasterVM, SubSBUMasters_History>(SubSBUVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Updated; historyObj.SubSBUId = SubSBU.Id; };
                                db.SubSBUMasters_History.Add(historyObj);
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                return Mapper.Map<SubSBUMaster, SubSBUMasterVM>(SubSBU);
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, ex.Message.ToString(), "Sub-SBU Create");
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new SubSBUMasterVM();
            }
            catch (DbEntityValidationException dve)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)), "Sub-SBU Create");
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(dve);
                throw new Exception(string.Join("\n", dve.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(y => y.ErrorMessage)));
            }
            catch (Exception ex)
            {
                new EmployeeComplaintHistoryRepository().ErrorLogHistory(null, ex.Message.ToString(), "Sub-SBU Create");
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
                List<UserMasterVM> lstuser = new UserMastersRepository().GetAll().Where(x => x.Status).ToList();
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
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<SubSBUMaster, SubSBUMasterVM>(SubSBU);
        }

        public List<SubSBUMasterHistoryVM> GetAllHistory()
        {
            List<SubSBUMasters_History> listdto = new List<SubSBUMasters_History>();
            List<SubSBUMasterHistoryVM> lst = new List<SubSBUMasterHistoryVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                listdto = db.SubSBUMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (listdto != null && listdto.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (SubSBUMasters_History item in listdto)
                    {
                        SubSBUMasterHistoryVM ViewModelDto = Mapper.Map<SubSBUMasters_History, SubSBUMasterHistoryVM>(item);
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
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (importSubSBU != null && importSubSBU.Count > 0)
                            {
                                db.SubSBUMasters.AddRange(importSubSBU);
                                db.SaveChanges();

                                List<SubSBUMasterHistoryVM> listVMDto = Mapper.Map<List<SubSBUMaster>, List<SubSBUMasterHistoryVM>>(importSubSBU);

                                List<SubSBUMasters_History> HistoryDto = Mapper.Map<List<SubSBUMasterHistoryVM>, List<SubSBUMasters_History>>(listVMDto);
                                if (HistoryDto != null && HistoryDto.Count > 0)
                                {
                                    HistoryDto.Select(c => { c.EntityState = Messages.Added; c.SubSBUId = c.Id; return c; }).ToList();
                                }

                                db.SubSBUMasters_History.AddRange(HistoryDto);
                                db.SaveChanges();

                                dbContextTransaction.Commit();

                                count = importSubSBU.Count;
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

                        WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SubSBUId == losid && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
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
        public List<SubSBUMasterVM> GetAllSubSBU()
        {
            List<SubSBUMaster> SubSBU = new List<SubSBUMaster>();
            List<SubSBUMasterVM> SubSBUList = new List<SubSBUMasterVM>();
            List<SubSBUMasterVM> SubSBULists = new List<SubSBUMasterVM>();
            try
            {

                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();


                if (!string.IsNullOrEmpty(sid))
                {
                    SubSBULists = GetAll();

                    foreach (SubSBUMasterVM item in SubSBULists)
                    {
                        SubSBUMasterVM catObj = item;

                        if (item.InvolvedUsersId != null)
                        {
                            var involveduser = item.InvolvedUsersId.Split(',');
                            foreach (var items in involveduser)
                            {
                                if (sid == items)
                                {
                                    catObj.SubSBU = item.SubSBU;
                                    catObj.Id = item.Id;
                                    SubSBUList.Add(catObj);
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
            return SubSBUList;
        }


        public List<EmployeeComplaintWorkFlowVM> GetAllSubSBUReport(string range, string losid)
        {
            List<EmployeeComplaintWorkFlowVM> WorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> WorkFlows = new List<EmployeeComplaintWorkFlow>();
            List<SubSBUMasterVM> SubSBULists = new List<SubSBUMasterVM>();
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    try
                    {
                        SubSBULists = GetAllSubSBU();

                        foreach (var item in SubSBULists)
                        {

                            WorkFlows = db.EmployeeComplaintWorkFlows.Where(i => i.IsActive && i.SubSBUId == item.Id && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();

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