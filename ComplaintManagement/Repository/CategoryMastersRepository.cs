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
                var category = db.CategoryMasters.FirstOrDefault(p => p.Id == categoryVM.Id);
                if (category == null)
                {
                    categoryVM.IsActive = true;
                    categoryVM.CreatedDate = DateTime.UtcNow;
                    categoryVM.UserId = 1;
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
                    categoryVM.UpdatedDate= DateTime.UtcNow;
                    db.Entry(category).CurrentValues.SetValues(categoryVM);
                    if (IsExist(category.CategoryName,category.Id))
                    {
                        throw new Exception(Messages.ALREADY_EXISTS);
                    }
                    db.SaveChanges();
                    return Mapper.Map<CategoryMaster, CategoryMasterVM>(category);

                }
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
            try
            {
                category = db.CategoryMasters.Where(i => i.IsActive).ToList().OrderByDescending(x=>x.CreatedDate).OrderByDescending(x=>x.Id).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<CategoryMaster>, List<CategoryMasterVM>>(category);
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

        public bool IsExist(string categoryName,int id)
        {
            return db.CategoryMasters.Count(x => x.IsActive && x.CategoryName.ToUpper() == categoryName.ToUpper() && x.Id!=id) > 0;
        }
    }
}