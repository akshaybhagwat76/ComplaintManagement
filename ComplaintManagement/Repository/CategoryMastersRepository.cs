using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace ComplaintManagement.Repository
{
    public class CategoryMastersRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public CategoryMastersRepository()
        {

        }

        public CategoryMasterVM AddOrUpdate(CategoryMasterVM categoryVM)
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
                            var category = db.CategoryMasters.FirstOrDefault(p => p.Id == categoryVM.Id);
                            if (category == null)
                            {
                                categoryVM.IsActive = true;
                                categoryVM.CreatedDate = DateTime.UtcNow;
                                categoryVM.CreatedBy = Convert.ToInt32(sid);
                                category = Mapper.Map<CategoryMasterVM, CategoryMaster>(categoryVM);
                                if (IsExist(category.CategoryName))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.CategoryMasters.Add(category);
                                db.SaveChanges();

                                CategoryMasters_History categoryMasters_History = Mapper.Map<CategoryMasterVM, CategoryMasters_History>(categoryVM);
                                if (categoryMasters_History != null) { categoryMasters_History.EntityState = Messages.Added;categoryMasters_History.CategoryId = category.Id; };
                                db.CategoryMasters_History.Add(categoryMasters_History);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                return Mapper.Map<CategoryMaster, CategoryMasterVM>(category);
                            }
                            else
                            {
                                categoryVM.IsActive = true;
                                categoryVM.CreatedDate = category.CreatedDate;
                                categoryVM.CreatedBy = category.CreatedBy;
                                categoryVM.UpdatedDate = DateTime.UtcNow;
                                categoryVM.ModifiedBy = Convert.ToInt32(sid);
                                db.Entry(category).CurrentValues.SetValues(categoryVM);
                                if (IsExist(category.CategoryName, category.Id))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SaveChanges();

                                CategoryMasters_History categoryMasters_History = Mapper.Map<CategoryMasterVM, CategoryMasters_History>(categoryVM);
                                if (categoryMasters_History != null) { categoryMasters_History.EntityState = Messages.Updated; categoryMasters_History.CategoryId = category.Id; };
                                db.CategoryMasters_History.Add(categoryMasters_History);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                return Mapper.Map<CategoryMaster, CategoryMasterVM>(category);
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new CategoryMasterVM();
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

        public List<CategoryMasterVM> GetAll()
        {
            List<CategoryMaster> category = new List<CategoryMaster>();
            List<CategoryMasterVM> categoryList = new List<CategoryMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                category = db.CategoryMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (category != null && category.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (CategoryMaster item in category)
                    {
                        CategoryMasterVM catObj = Mapper.Map<CategoryMaster, CategoryMasterVM>(item);
                        if (catObj != null)
                        {
                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            categoryList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return categoryList;
        }

        public List<CategoryMasterHistoryVM> GetAllHistory()
        {
            List<CategoryMasters_History> category = new List<CategoryMasters_History>();
            List<CategoryMasterHistoryVM> categoryList = new List<CategoryMasterHistoryVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                category = db.CategoryMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (category != null && category.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (CategoryMasters_History item in category)
                    {
                        CategoryMasterHistoryVM catObj = Mapper.Map<CategoryMasters_History, CategoryMasterHistoryVM>(item);
                        if (catObj != null)
                        {
                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            categoryList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return categoryList;
        }

        public CategoryMasterVM Get(int id)
        {
            CategoryMaster category = new CategoryMaster();
            try
            {
                category = db.CategoryMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (category == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<CategoryMaster, CategoryMasterVM>(category);
        }

        public bool Delete(int id)
        {
            var data = db.CategoryMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }

        public bool IsExist(string categoryName)
        {
            return db.CategoryMasters.Count(x => x.IsActive && x.CategoryName.ToUpper() == categoryName.ToUpper()) > 0;
        }

        public bool IsExist(string categoryName, int id)
        {
            return db.CategoryMasters.Count(x => x.IsActive && x.CategoryName.ToUpper() == categoryName.ToUpper() && x.Id != id) > 0;
        }

        public string UploadImportCategories(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportCategories(string file)
        {
            List<CategoryMaster> importCategories = new List<CategoryMaster>();
            CategoryMaster CategoryMasterDto = null;
            int count = 0;
            #region Indexes 
            int CategoryNameIndex = 1; int StatusIndex = 2;
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
                        CategoryMasterDto = new CategoryMaster();

                        #region Category Name
                        //Category Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, CategoryNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, CategoryNameIndex }));
                        }
                        else
                        {
                            string CategoryName = workSheet.Cells[i, CategoryNameIndex].Value?.ToString();
                            CategoryMasterVM categoryMasterDto = new CategoryMasterVM { CategoryName = CategoryName };
                            if (IsExist(CategoryName))
                            {
                                throw new Exception(string.Format(Messages.DataCategoryAlreadyExists, new object[] { "Name", i, CategoryNameIndex }));
                            }
                            else
                            {
                                CategoryMasterDto.CategoryName = workSheet.Cells[i, CategoryNameIndex].Value?.ToString();
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
                                    CategoryMasterDto.Status = true;
                                }
                                else
                                {
                                    CategoryMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion
                        CategoryMasterDto.CreatedBy = Convert.ToInt32(sid);
                        CategoryMasterDto.CreatedDate = DateTime.UtcNow;
                        CategoryMasterDto.IsActive = true; 
                        importCategories.Add(CategoryMasterDto);
                    }
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (importCategories != null && importCategories.Count > 0)
                            {
                                db.CategoryMasters.AddRange(importCategories);
                                db.SaveChanges();

                                List<CategoryMasterVM> categoryDtoListVM = Mapper.Map<List<CategoryMaster>, List<CategoryMasterVM>>(importCategories);

                                List<CategoryMasters_History> categoryMasters_History = Mapper.Map<List<CategoryMasterVM>, List<CategoryMasters_History>>(categoryDtoListVM);
                                if (categoryMasters_History!=null && categoryMasters_History.Count > 0)
                                {
                                    categoryMasters_History.Select(c => { c.EntityState = Messages.Added; c.CategoryId = c.Id; return c; }).ToList();
                                }

                                db.CategoryMasters_History.AddRange(categoryMasters_History);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                count = importCategories.Count;
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
        public List<EmployeeCompliantMasterVM> GetAllReport(string range, int losid)
        {
            List<EmployeeCompliantMasterVM> WorkFlowList = new List<EmployeeCompliantMasterVM>();
            List<EmployeeComplaintMaster> WorkFlows = new List<EmployeeComplaintMaster>();
            List<EmployeeComplaintWorkFlowVM> WorkFlowList1 = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> WorkFlows1 = new List<EmployeeComplaintWorkFlow>();
            if (!string.IsNullOrEmpty(range))
            {
                string[] dates = range.Split(',');
                DateTime fromDate = Convert.ToDateTime(dates[0]);
                DateTime toDate = Convert.ToDateTime(dates[1]);
                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    try
                    {

                        WorkFlows = db.EmployeeComplaintMasters.Where(i => i.IsActive && i.CategoryId == losid && i.CreatedDate >= fromDate && i.CreatedDate <= toDate).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                        if (WorkFlows != null && WorkFlows.Count > 0) /*&& usersList != null && usersList.Count > 0)*/
                        {
                            foreach (EmployeeComplaintMaster item in WorkFlows)
                            {
                                EmployeeCompliantMasterVM catObj = Mapper.Map<EmployeeComplaintMaster, EmployeeCompliantMasterVM>(item);

                                WorkFlows1 = db.EmployeeComplaintWorkFlows.Where(x => x.ComplaintId == item.Id).ToList();
                                if (WorkFlows1 != null && WorkFlows1.Count > 0)
                                {
                                    foreach (EmployeeComplaintWorkFlow items in WorkFlows1)
                                    {

                                        int compid = items.Id;
                                        int complaintid= items.ComplaintId;
                                        if (items.ComplaintNo != null)
                                        {
                                            catObj.ComplaintNo = items.ComplaintNo;
                                        }
                                        else
                                        {
                                            catObj.ComplaintNo = Messages.NotAvailable;
                                        }

                                        int Losid = db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == compid).LOSId;
                                        int SBUId = db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == compid).SBUId;
                                        int SubSBUId = db.EmployeeComplaintWorkFlows.FirstOrDefault(x => x.Id == compid).SubSBUId;
                                        
                                        catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                        catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == Losid) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == Losid).LOSName : Messages.NotAvailable;
                                        catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == SBUId).SBU : Messages.NotAvailable;
                                        catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == SubSBUId) != null ? db.SubSBUMasters.FirstOrDefault(x => x.Id == SubSBUId).SubSBU : Messages.NotAvailable;

                                        int catid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == complaintid).CategoryId;
                                        catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == catid) !=null ?db.CategoryMasters.FirstOrDefault(x => x.Id == catid).CategoryName:Messages.NotAvailable;
                                        int subcatid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == complaintid).SubCategoryId;
                                        catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcatid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcatid).SubCategoryName : Messages.NotAvailable;
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

                                        int reginoid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).RegionId;
                                        int companyid = db.UserMasters.FirstOrDefault(x => x.Id == catObj.CreatedBy).Company;

                                        catObj.CompanyName = db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName != null ? db.EntityMasters.FirstOrDefault(x => x.Id == companyid).EntityName : Messages.NotAvailable;
                                        catObj.RegionName = db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region != null ? db.RegionMasters.FirstOrDefault(x => x.Id == reginoid).Region : Messages.NotAvailable;
                                        catObj.CaseType = db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == complaintid) != null ? db.HR_Role.FirstOrDefault(x => x.UserId == catObj.CreatedBy && x.ComplentId == complaintid).CaseType : Messages.NotAvailable;

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