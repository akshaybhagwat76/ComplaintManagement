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
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var Region = db.RegionMasters.FirstOrDefault(p => p.Id == RegionVM.Id);
                    if (Region == null)
                    {
                        RegionVM.IsActive = true;
                        RegionVM.CreatedDate = DateTime.UtcNow;
                        RegionVM.UserId = 1;
                        RegionVM.CreatedBy = Convert.ToInt32(sid);
                        Region = Mapper.Map<RegionMasterVM, RegionMaster>(RegionVM);
                        if (IsExist(Region.Region))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.RegionMasters.Add(Region);
                        db.SaveChanges();
                        return Mapper.Map<RegionMaster, RegionMasterVM>(Region);
                    }
                    else
                    {
                        RegionVM.IsActive = true;
                        RegionVM.UserId = 1; RegionVM.CreatedDate = Region.CreatedDate;
                        RegionVM.UpdatedDate = DateTime.UtcNow;
                        RegionVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(Region).CurrentValues.SetValues(RegionVM);
                        if (IsExist(Region.Region,Region.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<RegionMaster, RegionMasterVM>(Region);

                    }
                }
                return new RegionMasterVM();
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
                Region = db.RegionMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
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
        public bool IsExist(string Region)
        {
            return db.RegionMasters.Count(x => x.IsActive && x.Region.ToUpper() == Region.ToUpper()) > 0;
        }

        public bool IsExist(string Region, int id)
        {
            return db.RegionMasters.Count(x => x.IsActive && x.Region.ToUpper() == Region.ToUpper() && x.Id != id) > 0;
        }
    }
}