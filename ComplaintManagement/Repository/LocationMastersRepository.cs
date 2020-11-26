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
    public class LocationMastersRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public LocationMastersRepository()
        {

        }

        public LocationMasterVM AddOrUpdate(LocationMasterVM LocationVM)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var Location = db.LocationMasters.FirstOrDefault(p => p.Id == LocationVM.Id);
                    if (Location == null)
                    {
                        LocationVM.IsActive = true;
                        LocationVM.CreatedDate = DateTime.UtcNow;
                        LocationVM.UserId = 1;
                        LocationVM.CreatedBy = Convert.ToInt32(sid);
                        Location = Mapper.Map<LocationMasterVM, LocationMaster>(LocationVM);
                        if (IsExist(Location.LocationName))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.LocationMasters.Add(Location);
                        db.SaveChanges();
                        return Mapper.Map<LocationMaster, LocationMasterVM>(Location);
                    }
                    else
                    {
                        LocationVM.IsActive = true;
                      LocationVM.CreatedDate = Location.CreatedDate;
                        LocationVM.CreatedBy = Location.CreatedBy;
                        LocationVM.UpdatedDate = DateTime.UtcNow;
                        LocationVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(Location).CurrentValues.SetValues(LocationVM);
                        if (IsExist(Location.LocationName,Location.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<LocationMaster, LocationMasterVM>(Location);

                    }
                }
                return new LocationMasterVM();
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

        public List<LocationMasterVM> GetAll()
        {

            List<LocationMaster> Location = new List<LocationMaster>();
            List<LocationMasterVM> LocationList = new List<LocationMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                Location = db.LocationMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (Location != null && Location.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (LocationMaster item in Location)
                    {
                        LocationMasterVM catObj = Mapper.Map<LocationMaster, LocationMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            LocationList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return LocationList;
        }

        public LocationMasterVM Get(int id)
        {
            LocationMaster Location = new LocationMaster();
            try
            {
                Location = db.LocationMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Location == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<LocationMaster, LocationMasterVM>(Location);
        }


        public bool Delete(int id)
        {
            var data = db.LocationMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string LocationName)
        {
            return db.LocationMasters.Count(x => x.IsActive && x.LocationName.ToUpper() == LocationName.ToUpper()) > 0;
        }

        public bool IsExist(string LocationName, int id)
        {
            return db.LocationMasters.Count(x => x.IsActive && x.LocationName.ToUpper() == LocationName.ToUpper() && x.Id != id) > 0;
        }
    }
}