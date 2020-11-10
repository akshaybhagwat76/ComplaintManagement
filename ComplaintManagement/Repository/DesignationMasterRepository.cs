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
    public class DesignationMasterRepository
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public DesignationMasterRepository()
        {

        }

        public DesignationMasterVM AddOrUpdate(DesignationMasterVM DesignationVM)
        {
            try
            {
                var Designation = db.DesignationMasters.FirstOrDefault(p => p.Id == DesignationVM.Id);
                if (Designation == null)
                {
                    DesignationVM.IsActive = true;
                    DesignationVM.CreatedDate = DateTime.UtcNow;
                    DesignationVM.UserId = 1;
                    Designation = Mapper.Map<DesignationMasterVM, DesignationMaster>(DesignationVM);
                    db.DesignationMasters.Add(Designation);
                    db.SaveChanges();
                    return Mapper.Map<DesignationMaster, DesignationMasterVM>(Designation);
                }
                else
                {
                    DesignationVM.IsActive = true;
                    DesignationVM.UserId = 1; DesignationVM.CreatedDate = Designation.CreatedDate;
                    DesignationVM.UpdatedDate = DateTime.UtcNow;
                    db.Entry(Designation).CurrentValues.SetValues(DesignationVM);
                    db.SaveChanges();
                    return Mapper.Map<DesignationMaster, DesignationMasterVM>(Designation);

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

        public List<DesignationMasterVM> GetAll()
        {
            List<DesignationMaster> Designation = new List<DesignationMaster>();
            try
            {
                Designation = db.DesignationMasters.Where(i => i.IsActive).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<DesignationMaster>, List<DesignationMasterVM>>(Designation);
        }

        public DesignationMasterVM Get(int id)
        {
            DesignationMaster Designation = new DesignationMaster();
            try
            {
                Designation = db.DesignationMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Designation == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<DesignationMaster, DesignationMasterVM>(Designation);
        }


        public bool Delete(int id)
        {
            var data = db.DesignationMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
    }
}