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
    public class EmployeeComplaintMastersRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public EmployeeCompliantMasterVM AddOrUpdate(EmployeeCompliantMasterVM EmployeeComplaintVM)
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
                            var EmployeeComplaint = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == EmployeeComplaintVM.Id);
                            if (EmployeeComplaint == null)
                            {
                                EmployeeComplaintVM.IsActive = true;
                                EmployeeComplaintVM.CreatedDate = DateTime.UtcNow;
                                EmployeeComplaintVM.CreatedBy = Convert.ToInt32(sid);
                                EmployeeComplaint = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMaster>(EmployeeComplaintVM);
                                EmployeeComplaint.UserId = EmployeeComplaint.UserId == 0 ? Convert.ToInt32(sid) : EmployeeComplaint.UserId;
                                EmployeeComplaint.ComplaintStatus = Messages.Opened;
                                    EmployeeComplaint.Status = true;
                                db.EmployeeComplaintMasters.Add(EmployeeComplaint);
                                db.SaveChanges();

                                EmployeeComplaintMastersHistory EmployeeComplaintMasters_History = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMastersHistory>(EmployeeComplaintVM);
                                if (EmployeeComplaintMasters_History != null) { EmployeeComplaintMasters_History.EntityState = Messages.Added; EmployeeComplaintMasters_History.EmployeeComplaintMasterId = EmployeeComplaintMasters_History.Id; };
                                db.EmployeeComplaintMastersHistories.Add(EmployeeComplaintMasters_History);
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                            }
                            else
                            {
                                EmployeeComplaintVM.IsActive = true;
                                EmployeeComplaintVM.CreatedDate = EmployeeComplaint.CreatedDate;
                                EmployeeComplaintVM.CreatedBy = EmployeeComplaint.CreatedBy;
                                EmployeeComplaintVM.UpdatedDate = DateTime.UtcNow;
                                EmployeeComplaintVM.UpdatedBy = Convert.ToInt32(sid);
                                db.Entry(EmployeeComplaint).CurrentValues.SetValues(EmployeeComplaintVM);
                                db.SaveChanges();
                                dbContextTransaction.Commit();

                                EmployeeComplaintMastersHistory EmployeeComplaintMasters_History = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMastersHistory>(EmployeeComplaintVM);
                                if (EmployeeComplaintMasters_History != null) { EmployeeComplaintMasters_History.EntityState = Messages.Updated; EmployeeComplaintMasters_History.EmployeeComplaintMasterId = EmployeeComplaintMasters_History.Id; };
                                db.EmployeeComplaintMastersHistories.Add(EmployeeComplaintMasters_History);
                                db.SaveChanges();
                                dbContextTransaction.Commit();

                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new EmployeeCompliantMasterVM();
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

        public List<EmployeeCompliantMasterVM> GetAll()
        {
            List<EmployeeComplaintMaster> EmployeeComplaint = new List<EmployeeComplaintMaster>();
            List<EmployeeCompliantMasterVM> EmployeeCompliant_oneList = new List<EmployeeCompliantMasterVM>();
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                    EmployeeComplaint = db.EmployeeComplaintMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                    EmployeeComplaint = EmployeeComplaint.Where(x => x.CreatedBy == Convert.ToInt32(sid)).ToList();
                    if (EmployeeComplaint != null && EmployeeComplaint.Count > 0 && usersList != null && usersList.Count > 0)
                    {
                        foreach (EmployeeComplaintMaster item in EmployeeComplaint)
                        {
                            EmployeeCompliantMasterVM catObj = Mapper.Map<EmployeeComplaintMaster, EmployeeCompliantMasterVM>(item);
                            if (catObj != null)
                            {
                                catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.UpdatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.UpdatedBy).EmployeeName : Messages.NotAvailable;
                                catObj.CategoryName = new CategoryMastersRepository().Get(item.CategoryId) != null ? new CategoryMastersRepository().Get(item.CategoryId).CategoryName : Messages.NotAvailable;
                                catObj.SubCategoryName = new SubCategoryMastersRepository().Get(item.SubCategoryId) != null ? new SubCategoryMastersRepository().Get(item.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                                catObj.EmployeeName = usersList.FirstOrDefault(x => x.Id == catObj.UserId) != null ? usersList.FirstOrDefault(x => x.Id == catObj.UserId).EmployeeName : Messages.NotAvailable;

                                EmployeeCompliant_oneList.Add(catObj);
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
            return EmployeeCompliant_oneList;
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

        public EmployeeCompliantMasterVM Get(int id)
        {
            EmployeeComplaintMaster EmployeeComplaint = new EmployeeComplaintMaster();
            try
            {
                EmployeeComplaint = db.EmployeeComplaintMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (EmployeeComplaint == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<EmployeeComplaintMaster, EmployeeCompliantMasterVM>(EmployeeComplaint);
        }
        public bool Delete(int id)
        {
            var data = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public string UploadImportEmployeeCompliant(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportEmployeeCompliant(string file)
        {
            List<EmployeeComplaintMaster> importEmployeeComplaint = new List<EmployeeComplaintMaster>();
            EmployeeComplaintMaster EmployeeComplaintMasterDto = null;
            int count = 0;
            #region Indexes 
            int EmployeeIdIndex = 1; int CategoryNameIndex = 2; int SubCategoryNameIndex = 3; int RemarkNameIndex = 4; int StatusIndex = 5;
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
                    var lstEmployee = new UserMastersRepository().GetAll();
                    var lstCategory= new CategoryMastersRepository().GetAll();
                    var lstSubCategory = new SubCategoryMastersRepository().GetAll();

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelPackage package = new ExcelPackage(new FileInfo(file));
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    for (int i = 1; i <= workSheet.Dimension.Rows; i++)
                    {
                        if (i == 1) //skip header row if its there
                        {
                            continue;
                        }
                        EmployeeComplaintMasterDto = new EmployeeComplaintMaster();
                        //Employee check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, EmployeeIdIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Employee Name", i, EmployeeIdIndex }));
                        }
                        else
                        {
                            string LOS = workSheet.Cells[i, EmployeeIdIndex].Value?.ToString();
                            var LOSDto = lstEmployee.FirstOrDefault(x => x.EmployeeName.ToLower() == LOS.ToLower());
                            if (LOSDto != null)
                            {
                                EmployeeComplaintMasterDto.UserId = EmployeeComplaintMasterDto.Id;
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "Employee Name", i, EmployeeIdIndex }));
                            }
                        }
                        // Category Name
                        if (string.IsNullOrEmpty(workSheet.Cells[i, CategoryNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Employee Name", i, CategoryNameIndex }));
                        }
                        else
                        {
                            string Category = workSheet.Cells[i, CategoryNameIndex].Value?.ToString();
                            var CategoryDto = lstCategory.FirstOrDefault(x => x.CategoryName.ToLower() == Category.ToLower());
                            if (CategoryDto != null)
                            {
                                EmployeeComplaintMasterDto.CategoryId = CategoryDto.Id;
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "Category", i, CategoryNameIndex }));
                            }
                        }
                        // SubCategory Name
                        if (string.IsNullOrEmpty(workSheet.Cells[i, SubCategoryNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Category", i, SubCategoryNameIndex }));
                        }
                        else
                        {
                            string SubCategory = workSheet.Cells[i, SubCategoryNameIndex].Value?.ToString();
                            var SubCategoryDto = lstSubCategory.FirstOrDefault(x => x.SubCategoryName.ToLower() == SubCategory.ToLower());
                            if (SubCategoryDto != null)
                            {
                                EmployeeComplaintMasterDto.SubCategoryId = SubCategoryDto.Id;
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "Sub CategoryName", i, SubCategoryNameIndex }));
                            }
                        }
                        // Remark check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, RemarkNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Remarks", i, RemarkNameIndex }));
                        }
                        else
                        {
                            string Remark = workSheet.Cells[i, CategoryNameIndex].Value?.ToString();
                            EmployeeCompliantMasterVM RemarkDto = new EmployeeCompliantMasterVM { Remark = Remark };
                           
                                throw new Exception(string.Format(Messages.DataCategoryAlreadyExists, new object[] { "Remarks", i, RemarkNameIndex }));
                           
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
                                    EmployeeComplaintMasterDto.Status = true;
                                }
                                else
                                {
                                    EmployeeComplaintMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion
                        EmployeeComplaintMasterDto.CreatedBy = Convert.ToInt32(sid);
                        EmployeeComplaintMasterDto.CreatedDate = DateTime.UtcNow;
                        EmployeeComplaintMasterDto.IsActive = true;
                        importEmployeeComplaint.Add(EmployeeComplaintMasterDto);
                    }
                    //using (var dbContextTransaction = db.Database.BeginTransaction())
                    //{
                    //    try
                    //    {
                    //        if (importEmployeeComplaint != null && importEmployeeComplaint.Count > 0)
                    //        {
                    //            db.CategoryMasters.AddRange(importEmployeeComplaint);
                    //            db.SaveChanges();

                    //            List<CategoryMasterVM> categoryDtoListVM = Mapper.Map<List<CategoryMaster>, List<CategoryMasterVM>>(importCategories);

                    //            List<CategoryMasters_History> categoryMasters_History = Mapper.Map<List<CategoryMasterVM>, List<CategoryMasters_History>>(categoryDtoListVM);
                    //            if (categoryMasters_History != null && categoryMasters_History.Count > 0)
                    //            {
                    //                categoryMasters_History.Select(c => { c.EntityState = Messages.Added; c.CategoryId = c.Id; return c; }).ToList();
                    //            }

                    //            db.CategoryMasters_History.AddRange(categoryMasters_History);
                    //            db.SaveChanges();

                    //            dbContextTransaction.Commit();
                    //            count = importCategories.Count;
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        dbContextTransaction.Rollback();
                    //        throw new Exception(ex.Message.ToString());
                    //    }
                    //}
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