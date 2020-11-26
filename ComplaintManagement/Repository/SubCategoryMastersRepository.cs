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
                        if (IsExist(Subcategory.SubCategoryName,Subcategory.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<SubCategoryMaster, SubCategoryMasterVM>(Subcategory);

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
    }
}