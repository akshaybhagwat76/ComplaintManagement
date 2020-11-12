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
    public class RegionMasterRepository
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public RegionMasterRepository()
        {

        }

        public RegionMasterVM AddOrUpdate(RegionMasterVM RegionVM)
        {
            try
            {
                var Region = db.RegionMasters.FirstOrDefault(p => p.Id == RegionVM.Id);
                if (Region == null)
                {
                    RegionVM.IsActive = true;
                    RegionVM.CreatedDate = DateTime.UtcNow;
                    RegionVM.UserId = 1;
                    Region = Mapper.Map<RegionMasterVM, RegionMaster>(RegionVM);
                    db.RegionMasters.Add(Region);
                    db.SaveChanges();
                    return Mapper.Map<RegionMaster, RegionMasterVM>(Region);
                }
                else
                {
                    RegionVM.IsActive = true;
                    RegionVM.UserId = 1; RegionVM.CreatedDate = Region.CreatedDate;
                    RegionVM.UpdatedDate = DateTime.UtcNow;
                    db.Entry(Region).CurrentValues.SetValues(RegionVM);
                    db.SaveChanges();
                    return Mapper.Map<RegionMaster, RegionMasterVM>(Region);

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

        public List<RegionMasterVM> GetAll()
        {
            List<RegionMaster> Region = new List<RegionMaster>();
            try
            {
                Region = db.RegionMasters.Where(i => i.IsActive).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<RegionMaster>, List<RegionMasterVM>>(Region);
        }

        public RegionMasterVM Get(int id)
        {
            RegionMaster Region = new RegionMaster();
            try
            {
                Region = db.RegionMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Region == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<RegionMaster, RegionMasterVM>(Region);
        }


        public bool Delete(int id)
        {
            var data = db.RegionMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
    }
}