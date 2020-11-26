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
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var Designation = db.DesignationMasters.FirstOrDefault(p => p.Id == DesignationVM.Id);
                    if (Designation == null)
                    {
                        DesignationVM.IsActive = true;
                        DesignationVM.CreatedDate = DateTime.UtcNow;
                        DesignationVM.CreatedBy = Convert.ToInt32(sid);
                       
                        Designation = Mapper.Map<DesignationMasterVM, DesignationMaster>(DesignationVM);
                        if (IsExist(Designation.Designation))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.DesignationMasters.Add(Designation);
                        db.SaveChanges();
                        return Mapper.Map<DesignationMaster, DesignationMasterVM>(Designation);
                    }
                    else
                    {
                        DesignationVM.IsActive = true;
                       DesignationVM.CreatedDate = Designation.CreatedDate;
                        DesignationVM.UpdatedDate = DateTime.UtcNow;
                        DesignationVM.CreatedBy = Designation.CreatedBy;
                        DesignationVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(Designation).CurrentValues.SetValues(DesignationVM);
                        if (IsExist(Designation.Designation,Designation.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<DesignationMaster, DesignationMasterVM>(Designation);

                    }
                }
                return new DesignationMasterVM();
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
            List<DesignationMasterVM> DesignationList = new List<DesignationMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                Designation = db.DesignationMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (Designation != null && Designation.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (DesignationMaster item in Designation)
                    {
                        DesignationMasterVM catObj = Mapper.Map<DesignationMaster, DesignationMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            DesignationList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return DesignationList;
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
        public bool IsExist(string Designation)
        {
            return db.DesignationMasters.Count(x => x.IsActive && x.Designation.ToUpper() == Designation.ToUpper()) > 0;
        }

        public bool IsExist(string Designation, int id)
        {
            return db.DesignationMasters.Count(x => x.IsActive && x.Designation.ToUpper() == Designation.ToUpper() && x.Id != id) > 0;
        }
    }
}