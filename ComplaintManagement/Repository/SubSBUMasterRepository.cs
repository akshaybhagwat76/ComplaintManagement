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
    public class SubSBUMasterRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public SubSBUMasterRepository()
        {

        }

        public SubSBUMasterVM AddOrUpdate(SubSBUMasterVM SubSBUVM)
        {
            try
            {
                var SubSBU = db.SubSBUMasters.FirstOrDefault(p => p.Id == SubSBUVM.Id);
                if (SubSBU == null)
                {
                    SubSBUVM.IsActive = true;
                    SubSBUVM.CreatedDate = DateTime.UtcNow;
                    SubSBUVM.UserId = 1;
                    SubSBU = Mapper.Map<SubSBUMasterVM, SubSBUMaster>(SubSBUVM);
                    db.SubSBUMasters.Add(SubSBU);
                    db.SaveChanges();
                    return Mapper.Map<SubSBUMaster, SubSBUMasterVM>(SubSBU);
                }
                else
                {
                    SubSBUVM.IsActive = true;
                    SubSBUVM.UserId = 1; SubSBUVM.CreatedDate = SubSBU.CreatedDate;
                    SubSBUVM.UpdatedDate = DateTime.UtcNow;
                    db.Entry(SubSBU).CurrentValues.SetValues(SubSBUVM);
                    db.SaveChanges();
                    return Mapper.Map<SubSBUMaster, SubSBUMasterVM>(SubSBU);

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

        public List<SubSBUMasterVM> GetAll()
        {
            List<SubSBUMaster> SubSBU = new List<SubSBUMaster>();
            try
            {
                SubSBU = db.SubSBUMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<SubSBUMaster>, List<SubSBUMasterVM>>(SubSBU);
        }

        public SubSBUMasterVM Get(int id)
        {
            SubSBUMaster SubSBU = new SubSBUMaster();
            try
            {
                SubSBU = db.SubSBUMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (SubSBU == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<SubSBUMaster, SubSBUMasterVM>(SubSBU);
        }


        public bool Delete(int id)
        {
            var data = db.SubSBUMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string SubSBU)
        {
            return db.SubSBUMasters.Count(x => x.IsActive && x.SubSBU.ToUpper() == SubSBU.ToUpper()) > 0;
        }

        public bool IsExist(string SubSBU, int id)
        {
            return db.SubSBUMasters.Count(x => x.IsActive && x.SubSBU.ToUpper() == SubSBU.ToUpper() && x.Id != id) > 0;
        }
    }
}