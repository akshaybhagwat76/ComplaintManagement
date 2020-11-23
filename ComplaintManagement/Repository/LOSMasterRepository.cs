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
    public class LOSMasterRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public LOSMasterRepository()
        {
        }

        public LOSMasterVM AddOrUpdate(LOSMasterVM LOSVM)
        {
            try
            {
                var LOS = db.LOSMasters.FirstOrDefault(p => p.Id == LOSVM.Id);
                if (LOS == null)
                {
                    LOSVM.IsActive = true;
                    LOSVM.CreatedDate = DateTime.UtcNow;
                    LOSVM.UserId = 1;
                    LOS = Mapper.Map<LOSMasterVM, LOSMaster>(LOSVM);
                    db.LOSMasters.Add(LOS);
                    db.SaveChanges();
                    return Mapper.Map<LOSMaster, LOSMasterVM>(LOS);
                }
                else
                {
                    LOSVM.IsActive = true;
                    LOSVM.UserId = 1; LOSVM.CreatedDate = LOS.CreatedDate;
                    LOSVM.UpdatedDate = DateTime.UtcNow;
                    db.Entry(LOS).CurrentValues.SetValues(LOSVM);
                    db.SaveChanges();
                    return Mapper.Map<LOSMaster, LOSMasterVM>(LOS);
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

        public List<LOSMasterVM> GetAll()
        {
            List<LOSMaster> los = new List<LOSMaster>();
            try
            {
                los = db.LOSMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<LOSMaster>, List<LOSMasterVM>>(los);
        }

        public LOSMasterVM Get(int id)
        {
            LOSMaster los = new LOSMaster();
            try
            {
                los = db.LOSMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (los == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<LOSMaster, LOSMasterVM>(los);
        }


        public bool Delete(int id)
        {
            var data = db.LOSMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string LOSName)
        {
            return db.LOSMasters.Count(x => x.IsActive && x.LOSName.ToUpper() == LOSName.ToUpper()) > 0;
        }

        public bool IsExist(string LOSName, int id)
        {
            return db.LOSMasters.Count(x => x.IsActive && x.LOSName.ToUpper() == LOSName.ToUpper() && x.Id != id) > 0;
        }
    }
}