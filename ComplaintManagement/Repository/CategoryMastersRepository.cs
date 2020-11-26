using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
                        return Mapper.Map<CategoryMaster, CategoryMasterVM>(category);
                    }
                    else
                    {
                        categoryVM.IsActive = true;
                        categoryVM.UserId = 1; categoryVM.CreatedDate = category.CreatedDate;
                        categoryVM.CreatedBy = category.CreatedBy;
                        categoryVM.UpdatedDate = DateTime.UtcNow;
                        categoryVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(category).CurrentValues.SetValues(categoryVM);
                        if (IsExist(category.CategoryName, category.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<CategoryMaster, CategoryMasterVM>(category);
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
    }
}