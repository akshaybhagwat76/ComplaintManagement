using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
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
                var Subcategory = db.SubCategoryMasters.FirstOrDefault(p => p.Id == SubcategoryVM.Id);
                if (Subcategory == null)
                {
                    SubcategoryVM.IsActive = true;
                    SubcategoryVM.CreatedDate = DateTime.UtcNow;
                    SubcategoryVM.UserId = 1;
                    Subcategory = Mapper.Map<SubCategoryMasterVM, SubCategoryMaster>(SubcategoryVM);
                    db.SubCategoryMasters.Add(Subcategory);
                    db.SaveChanges();
                    return Mapper.Map<SubCategoryMaster, SubCategoryMasterVM>(Subcategory);
                }
                else
                {
                    SubcategoryVM.IsActive = true;
                    SubcategoryVM.UserId = 1; SubcategoryVM.CreatedDate = Subcategory.CreatedDate;
                    SubcategoryVM.UpdatedDate = DateTime.UtcNow;
                    db.Entry(Subcategory).CurrentValues.SetValues(SubcategoryVM);
                    db.SaveChanges();
                    return Mapper.Map<SubCategoryMaster, SubCategoryMasterVM>(Subcategory);

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

        public List<SubCategoryMasterVM> GetAll()
        {
            List<SubCategoryMaster> Subcategory = new List<SubCategoryMaster>();
            try
            {
                Subcategory = db.SubCategoryMasters.Where(i => i.IsActive).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<SubCategoryMaster>, List<SubCategoryMasterVM>>(Subcategory);
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
    }
}