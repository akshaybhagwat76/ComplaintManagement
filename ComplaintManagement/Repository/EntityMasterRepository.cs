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
    public class EntityMasterRepository
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public EntityMasterRepository()
        {

        }

        public EntityMasterVM AddOrUpdate(EntityMasterVM EntityVM)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var Entity = db.EntityMasters.FirstOrDefault(p => p.Id == EntityVM.Id);
                    if (Entity == null)
                    {
                        EntityVM.IsActive = true;
                        EntityVM.CreatedDate = DateTime.UtcNow;
                        EntityVM.UserId = 1;
                        EntityVM.CreatedBy = Convert.ToInt32(sid);
                        Entity = Mapper.Map<EntityMasterVM, EntityMaster>(EntityVM);
                        if (IsExist(Entity.EntityName))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.EntityMasters.Add(Entity);
                        db.SaveChanges();
                        return Mapper.Map<EntityMaster, EntityMasterVM>(Entity);
                    }
                    else
                    {
                        EntityVM.IsActive = true;
                         EntityVM.CreatedDate = Entity.CreatedDate;
                        EntityVM.CreatedBy = Entity.CreatedBy;
                        EntityVM.UpdatedDate = DateTime.UtcNow;
                        EntityVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(Entity).CurrentValues.SetValues(EntityVM);
                        if (IsExist(Entity.EntityName,Entity.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<EntityMaster, EntityMasterVM>(Entity);

                    }
                }
                return new EntityMasterVM();
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

        public List<EntityMasterVM> GetAll()
        {

            List<EntityMaster> Entity = new List<EntityMaster>();
            List<EntityMasterVM> EntityList = new List<EntityMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                Entity = db.EntityMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (Entity != null && Entity.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (EntityMaster item in Entity)
                    {
                        EntityMasterVM catObj = Mapper.Map<EntityMaster, EntityMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            EntityList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return EntityList;
        }

        public EntityMasterVM Get(int id)
        {
            EntityMaster Entity = new EntityMaster();
            try
            {
                Entity = db.EntityMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Entity == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<EntityMaster, EntityMasterVM>(Entity);
        }


        public bool Delete(int id)
        {
            var data = db.EntityMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string EntityName)
        {
            return db.EntityMasters.Count(x => x.IsActive && x.EntityName.ToUpper() == EntityName.ToUpper()) > 0;
        }

        public bool IsExist(string EntityName, int id)
        {
            return db.EntityMasters.Count(x => x.IsActive && x.EntityName.ToUpper() == EntityName.ToUpper() && x.Id != id) > 0;
        }
    }
}