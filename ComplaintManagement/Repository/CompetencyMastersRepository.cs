using AutoMapper;
using ComplaintManagement.Controllers;
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
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var Competency = db.CompetencyMasters.FirstOrDefault(p => p.Id == CompetencyVM.Id);
                    if (Competency == null)
                    {
                        CompetencyVM.IsActive = true;
                        CompetencyVM.CreatedDate = DateTime.UtcNow;
                        CompetencyVM.UserId = 1;
                        CompetencyVM.CreatedBy = Convert.ToInt32(sid);
                        Competency = Mapper.Map<CompetencyMasterVM, CompetencyMaster>(CompetencyVM);
                        if (IsExist(Competency.CompetencyName))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.CompetencyMasters.Add(Competency);
                        db.SaveChanges();
                        return Mapper.Map<CompetencyMaster, CompetencyMasterVM>(Competency);
                    }
                    else
                    {
                        CompetencyVM.IsActive = true;
                        CompetencyVM.UserId = 1; CompetencyVM.CreatedDate = Competency.CreatedDate;
                        CompetencyVM.UpdatedDate = DateTime.UtcNow;
                        CompetencyVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(Competency).CurrentValues.SetValues(CompetencyVM);
                        if (IsExist(Competency.CompetencyName,Competency.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<CompetencyMaster, CompetencyMasterVM>(Competency);

                    }
                }
                return new CompetencyMasterVM();
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
                Competency = db.CompetencyMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
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
        public bool IsExist(string CompetencyName)
        {
            return db.CompetencyMasters.Count(x => x.IsActive && x.CompetencyName.ToUpper() == CompetencyName.ToUpper()) > 0;
        }

        public bool IsExist(string CompetencyName, int id)
        {
            return db.CompetencyMasters.Count(x => x.IsActive && x.CompetencyName.ToUpper() == CompetencyName.ToUpper() && x.Id != id) > 0;
        }
    }
}