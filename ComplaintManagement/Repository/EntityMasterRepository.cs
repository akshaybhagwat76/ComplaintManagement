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
                var Entity = db.EntityMasters.FirstOrDefault(p => p.Id == EntityVM.Id);
                if (Entity == null)
                {
                    EntityVM.IsActive = true;
                    EntityVM.CreatedDate = DateTime.UtcNow;
                    EntityVM.UserId = 1;
                    Entity = Mapper.Map<EntityMasterVM, EntityMaster>(EntityVM);
                    db.EntityMasters.Add(Entity);
                    db.SaveChanges();
                    return Mapper.Map<EntityMaster, EntityMasterVM>(Entity);
                }
                else
                {
                    EntityVM.IsActive = true;
                    EntityVM.UserId = 1; EntityVM.CreatedDate = Entity.CreatedDate;
                    EntityVM.UpdatedDate = DateTime.UtcNow;
                    db.Entry(Entity).CurrentValues.SetValues(EntityVM);
                    db.SaveChanges();
                    return Mapper.Map<EntityMaster ,EntityMasterVM>(Entity);

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

        public List<EntityMasterVM> GetAll()
        {
            List<EntityMaster> Entity = new List<EntityMaster>();
            try
            {
                Entity = db.EntityMasters.Where(i => i.IsActive).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<EntityMaster>, List<EntityMasterVM>>(Entity);
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
    }
}