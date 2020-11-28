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
    public class SubCategoryMastersRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public SubCategoryMastersRepository()
        {

        }

        public SubCategoryMasterVM AddOrUpdate(SubCategoryMasterVM SubcategoryVM)
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
                            var Subcategory = db.SubCategoryMasters.FirstOrDefault(p => p.Id == SubcategoryVM.Id);
                            if (Subcategory == null)
                            {
                                SubcategoryVM.IsActive = true;
                                SubcategoryVM.CreatedDate = DateTime.UtcNow;
                                SubcategoryVM.UserId = 1;
                                SubcategoryVM.CreatedBy = Convert.ToInt32(sid);
                                Subcategory = Mapper.Map<SubCategoryMasterVM, SubCategoryMaster>(SubcategoryVM);
                                if (IsExist(Subcategory.SubCategoryName))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SubCategoryMasters.Add(Subcategory);
                                db.SaveChanges();

                                SubCategoryMasters_History subcategoryMasters_History = Mapper.Map<SubCategoryMasterVM, SubCategoryMasters_History>(SubcategoryVM);
                                if (subcategoryMasters_History != null) { subcategoryMasters_History.EntityState = Messages.Updated; };
                                db.SubCategoryMasters_History.Add(subcategoryMasters_History);
                                db.SaveChanges();

                                return Mapper.Map<SubCategoryMaster, SubCategoryMasterVM>(Subcategory);
                            }
                            else
                            {
                                SubcategoryVM.IsActive = true;
                                SubcategoryVM.CreatedDate = Subcategory.CreatedDate;
                                SubcategoryVM.CreatedBy = Subcategory.CreatedBy;
                                SubcategoryVM.UpdatedDate = DateTime.UtcNow;
                                SubcategoryVM.ModifiedBy = Convert.ToInt32(sid);
                                db.Entry(Subcategory).CurrentValues.SetValues(SubcategoryVM);
                                if (IsExist(Subcategory.SubCategoryName, Subcategory.Id))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SaveChanges();

                                SubCategoryMasters_History subcategoryMasters_History = Mapper.Map<SubCategoryMasterVM, SubCategoryMasters_History>(SubcategoryVM);
                                if (subcategoryMasters_History != null) { subcategoryMasters_History.EntityState = Messages.Updated; };
                                db.SubCategoryMasters_History.Add(subcategoryMasters_History);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                return Mapper.Map<SubCategoryMaster, SubCategoryMasterVM>(Subcategory);
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new SubCategoryMasterVM();
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

        public List<SubCategoryMasterVM> GetAll()
        {

            List<SubCategoryMaster> Subcategory = new List<SubCategoryMaster>();
            List<SubCategoryMasterVM> SubcategoryList = new List<SubCategoryMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                Subcategory = db.SubCategoryMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (Subcategory != null && Subcategory.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (SubCategoryMaster item in Subcategory)
                    {
                        SubCategoryMasterVM catObj = Mapper.Map<SubCategoryMaster, SubCategoryMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            SubcategoryList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return SubcategoryList;
        }

        public SubCategoryMasterVM Get(int id)
        {
            SubCategoryMaster Subcategory = new SubCategoryMaster();
            try
            {
                Subcategory = db.SubCategoryMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Subcategory == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<SubCategoryMaster, SubCategoryMasterVM>(Subcategory);
        }


        public bool Delete(int id)
        {
            var data = db.SubCategoryMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string SubCategoryName)
        {
            return db.SubCategoryMasters.Count(x => x.IsActive && x.SubCategoryName.ToUpper() == SubCategoryName.ToUpper()) > 0;
        }

        public bool IsExist(string SubCategoryName, int id)
        {
            return db.SubCategoryMasters.Count(x => x.IsActive && x.SubCategoryName.ToUpper() == SubCategoryName.ToUpper() && x.Id != id) > 0;
        }

        public string UploadImportSubCategories(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportSubCategories(string file)
        {
            List<SubCategoryMaster> importSubCategories = new List<SubCategoryMaster>();
            SubCategoryMaster SubCategoryMasterDto = null;
            int count = 0;
            #region Indexes 
            int SubCategoryNameIndex = 1; int StatusIndex = 2;
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
                        SubCategoryMasterDto = new SubCategoryMaster();

                        #region SubCategory Name
                        //SubCategory Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, SubCategoryNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, SubCategoryNameIndex }));
                        }
                        else
                        {
                            string SubCategoryName = workSheet.Cells[i, SubCategoryNameIndex].Value?.ToString();
                            SubCategoryMasterVM categoryMasterDto = new SubCategoryMasterVM {SubCategoryName  = SubCategoryName };
                            if (IsExist(SubCategoryName))
                            {
                                throw new Exception(string.Format(Messages.DataSubCategoryAlreadyExists, new object[] { "Name", i, SubCategoryNameIndex }));
                            }
                            else
                            {
                                SubCategoryMasterDto.SubCategoryName = workSheet.Cells[i, SubCategoryNameIndex].Value?.ToString();
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
                                    SubCategoryMasterDto.Status = true;
                                }
                                else
                                {
                                    SubCategoryMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion
                        SubCategoryMasterDto.CreatedBy = Convert.ToInt32(sid);
                        SubCategoryMasterDto.CreatedDate = DateTime.UtcNow;
                        SubCategoryMasterDto.IsActive = true;
                        importSubCategories.Add(SubCategoryMasterDto);
                    }
                    if (importSubCategories != null && importSubCategories.Count > 0)
                    {
                        db.SubCategoryMasters.AddRange(importSubCategories);
                        db.SaveChanges();
                        count = importSubCategories.Count;
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