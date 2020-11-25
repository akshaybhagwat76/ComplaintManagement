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
    public class CommitteeMastersRepository
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public CommitteeMastersRepository()
        {

        }

        public CommitteeMasterVM AddOrUpdate(CommitteeMasterVM CommitteeVM)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var Committee = db.CommitteeMasters.FirstOrDefault(p => p.Id == CommitteeVM.Id);
                    if (Committee == null)
                    {
                        CommitteeVM.IsActive = true;
                        CommitteeVM.CreatedDate = DateTime.UtcNow;
                        CommitteeVM.CreatedBy = Convert.ToInt32(sid);
                        Committee = Mapper.Map<CommitteeMasterVM, CommitteeMaster>(CommitteeVM);
                        if (IsExist(Committee.CommitteeName))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.CommitteeMasters.Add(Committee);
                        db.SaveChanges();
                        return Mapper.Map<CommitteeMaster, CommitteeMasterVM>(Committee);
                    }
                    else
                    {
                        CommitteeVM.IsActive = true;

                        CommitteeVM.CreatedDate = Committee.CreatedDate;
                        CommitteeVM.UpdatedDate = DateTime.UtcNow;
                        CommitteeVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(Committee).CurrentValues.SetValues(CommitteeVM);
                        if (IsExist(Committee.CommitteeName, Committee.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<CommitteeMaster, CommitteeMasterVM>(Committee);

                    }
                }
                return new CommitteeMasterVM();
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

        public List<CommitteeMasterVM> GetAll()
        {
            List<CommitteeMaster> Committee = new List<CommitteeMaster>();
            try
            {
                Committee = db.CommitteeMasters.Where(i => i.IsActive).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<CommitteeMaster>, List<CommitteeMasterVM>>(Committee);
        }

        public CommitteeMasterVM Get(int id)
        {
            CommitteeMaster Committee = new CommitteeMaster();
            try
            {
                Committee = db.CommitteeMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Committee == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<CommitteeMaster, CommitteeMasterVM>(Committee);
        }


        public bool Delete(int id)
        {
            var data = db.CommitteeMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string CommitteeName)
        {
            return db.CommitteeMasters.Count(x => x.IsActive && x.CommitteeName.ToLower() == CommitteeName.ToLower()) > 0;
        }

        public bool IsExist(string CommitteeName, int id)
        {
            return db.CommitteeMasters.Count(x => x.IsActive && x.CommitteeName.ToUpper() == CommitteeName.ToUpper() && x.Id != id) > 0;
        }
    }
}