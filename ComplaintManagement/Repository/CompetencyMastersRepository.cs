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
    public class CompetencyMastersRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public CompetencyMastersRepository()
        {

        }

        public CompetencyMasterVM AddOrUpdate(CompetencyMasterVM CompetencyVM)
        {
            try
            {
                var Competency = db.CompetencyMasters.FirstOrDefault(p => p.Id == CompetencyVM.Id);
                if (Competency == null)
                {
                    CompetencyVM.IsActive = true;
                    CompetencyVM.CreatedDate = DateTime.UtcNow;
                    CompetencyVM.UserId = 1;
                    Competency = Mapper.Map<CompetencyMasterVM, CompetencyMaster>(CompetencyVM);
                    db.CompetencyMasters.Add(Competency);
                    db.SaveChanges();
                    return Mapper.Map<CompetencyMaster, CompetencyMasterVM>(Competency);
                }
                else
                {
                    CompetencyVM.IsActive = true;
                    CompetencyVM.UserId = 1; CompetencyVM.CreatedDate = Competency.CreatedDate;
                    CompetencyVM.UpdatedDate = DateTime.UtcNow;
                    db.Entry(Competency).CurrentValues.SetValues(CompetencyVM);
                    db.SaveChanges();
                    return Mapper.Map<CompetencyMaster, CompetencyMasterVM>(Competency);

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

        public List<CompetencyMasterVM> GetAll()
        {
            List<CompetencyMaster> Competency= new List<CompetencyMaster>();
            try
            {
                Competency = db.CompetencyMasters.Where(i => i.IsActive).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<CompetencyMaster>, List<CompetencyMasterVM>>(Competency);
        }

        public CompetencyMasterVM Get(int id)
        {
            CompetencyMaster Competency = new CompetencyMaster();
            try
            {
                Competency = db.CompetencyMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Competency == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<CompetencyMaster, CompetencyMasterVM>(Competency);
        }


        public bool Delete(int id)
        {
            var data = db.CompetencyMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
    }
}