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
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var LOS = db.LOSMasters.FirstOrDefault(p => p.Id == LOSVM.Id);
                    if (LOS == null)
                    {
                        LOSVM.IsActive = true;
                        LOSVM.CreatedDate = DateTime.UtcNow;
                        LOSVM.UserId = 1;
                        LOSVM.CreatedBy = Convert.ToInt32(sid);
                        LOS = Mapper.Map<LOSMasterVM, LOSMaster>(LOSVM);
                        if (IsExist(LOS.LOSName))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.LOSMasters.Add(LOS);
                        db.SaveChanges();
                        return Mapper.Map<LOSMaster, LOSMasterVM>(LOS);
                    }
                    else
                    {
                        LOSVM.IsActive = true;
                         LOSVM.CreatedDate = LOS.CreatedDate;
                        LOSVM.CreatedBy = LOS.CreatedBy;
                        LOSVM.UpdatedDate = DateTime.UtcNow;
                        LOSVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(LOS).CurrentValues.SetValues(LOSVM);
                        if (IsExist(LOS.LOSName,LOS.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<LOSMaster, LOSMasterVM>(LOS);
                    }
                }
                return new LOSMasterVM();
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
            List<LOSMaster> LOS = new List<LOSMaster>();
            List<LOSMasterVM> LOSList = new List<LOSMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                LOS = db.LOSMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (LOS != null && LOS.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (LOSMaster item in LOS)
                    {
                        LOSMasterVM catObj = Mapper.Map<LOSMaster, LOSMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            LOSList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return LOSList;
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