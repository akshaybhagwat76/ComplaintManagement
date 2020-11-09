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
    public class SBUMasterRepository
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public SBUMasterRepository()
        {

        }

        public SBUMasterVM AddOrUpdate(SBUMasterVM SBUVM)
        {
            try
            {
                var SBU = db.SBUMasters.FirstOrDefault(p => p.Id == SBUVM.Id);
                if (SBU == null)
                {
                    SBUVM.IsActive = true;

                    SBU = Mapper.Map<SBUMasterVM, SBUMaster>(SBUVM);
                    db.SBUMasters.Add(SBU);
                    db.SaveChanges();
                    return Mapper.Map<SBUMaster, SBUMasterVM>(SBU);
                }
                else
                {
                    SBUVM.IsActive = true;
                    db.Entry(SBU).CurrentValues.SetValues(SBUVM);
                    db.SaveChanges();
                    return Mapper.Map<SBUMaster, SBUMasterVM>(SBU);

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

        public List<SBUMasterVM> GetAll()
        {
            List<SBUMaster> SBU = new List<SBUMaster>();
            try
            {
              SBU = db.SBUMasters.Where(i => i.IsActive).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<SBUMaster>, List<SBUMasterVM>>(SBU);
        }

        public SBUMasterVM Get(int id)
        {
            SBUMaster SBU= new SBUMaster();
            try
            {
                SBU = db.SBUMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (SBU == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<SBUMaster, SBUMasterVM>(SBU);
        }


        public bool Delete(int id)
        {
            var data = db.SBUMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
    }
}