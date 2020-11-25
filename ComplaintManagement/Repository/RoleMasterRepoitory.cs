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
    public class RoleMasterRepoitory
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public RoleMasterRepoitory()
        {

        }

        public RoleMasterVM AddOrUpdate(RoleMasterVM RoleVM)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var Role = db.RoleMasters.FirstOrDefault(p => p.Id == RoleVM.Id);
                    if (Role == null)
                    {
                        RoleVM.IsActive = true;
                        RoleVM.CreatedDate = DateTime.UtcNow;
                        RoleVM.CreatedBy = Convert.ToInt32(sid);
                        Role = Mapper.Map<RoleMasterVM, RoleMaster>(RoleVM);
                      
                        db.RoleMasters.Add(Role);
                        db.SaveChanges();
                        return Mapper.Map<RoleMaster, RoleMasterVM>(Role);
                    }
                    else
                    {
                        RoleVM.IsActive = true;
                        RoleVM.CreatedDate = Role.CreatedDate;
                        RoleVM.UpdatedDate = DateTime.UtcNow;
                        RoleVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(Role).CurrentValues.SetValues(RoleVM);
                        db.SaveChanges();
                        return Mapper.Map<RoleMaster, RoleMasterVM>(Role);

                    }
                }
                return new RoleMasterVM();
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

        public List<RoleMasterVM> GetAll()
        {
            List<RoleMaster> Role= new List<RoleMaster>();
            try
            {
                Role = db.RoleMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<RoleMaster>, List<RoleMasterVM>>(Role);
        }

        public RoleMasterVM Get(int id)
        {
            RoleMaster Role = new RoleMaster();
            try
            {
                Role = db.RoleMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Role == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<RoleMaster, RoleMasterVM>(Role);
        }


        public bool Delete(int id)
        {
            var data = db.RoleMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
    }
}