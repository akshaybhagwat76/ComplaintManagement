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
    public class UserMastersRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public UserMastersRepository()
        {

        }

        public UserMasterVM AddOrUpdate(UserMasterVM UserVM)
        {
            try
            {
                var User = db.UserMasters.FirstOrDefault(p => p.Id == UserVM.Id);
                if (User == null)
                {
                    UserVM.IsActive = true;
                    UserVM.CreatedDate = DateTime.UtcNow;
                    if(!string.IsNullOrEmpty(UserVM.ImagePath) && UserVM.ImagePath != null)
                    {
                        UserVM.ImagePath = UserVM.ImagePath != null ? new Common().SaveImageFromBase64(UserVM.ImagePath):"";
                    }
                    User = Mapper.Map<UserMasterVM, UserMaster>(UserVM);
                    db.UserMasters.Add(User);
                    db.SaveChanges();
                    return Mapper.Map<UserMaster, UserMasterVM>(User);
                }
                else
                {
                    if (!string.IsNullOrEmpty(UserVM.ImagePath) && UserVM.ImagePath != null)
                    {
                        new Common().RemoveFile(User.ImagePath);
                        UserVM.ImagePath = UserVM.ImagePath != null ? new Common().SaveImageFromBase64(UserVM.ImagePath) : "";
                    }
                    UserVM.IsActive = true;
                    UserVM.CreatedDate = User.CreatedDate;
                    UserVM.UpdatedDate = DateTime.UtcNow;
                    db.Entry(User).CurrentValues.SetValues(UserVM);
                    db.SaveChanges();
                    return Mapper.Map<UserMaster, UserMasterVM>(User);

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

        public void RemoveProfilePic(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                var user = db.UserMasters.Where(x => x.ImagePath == fileName).FirstOrDefault();
                if (user != null)
                {
                    user.ImagePath = string.Empty;
                    db.SaveChanges();
                }
                new Common().RemoveFile(fileName);
            }
        }

        public List<UserMasterVM> GetAll()
        {
            List<UserMaster> User = new List<UserMaster>();
            try
            {
                User = db.UserMasters.Where(i => i.IsActive).ToList();
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<List<UserMaster>, List<UserMasterVM>>(User);
        }

        public UserMasterVM Get(int id)
        {
            UserMaster User = new UserMaster();
            try
            {
                User = db.UserMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (User == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<UserMaster, UserMasterVM>(User);
        }


        public bool Delete(int id)
        {
            var data = db.UserMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
    }
}