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
                    if (!string.IsNullOrEmpty(UserVM.ImagePath) && UserVM.ImagePath != null)
                    {
                        UserVM.ImagePath = UserVM.ImagePath != null ? new Common().SaveImageFromBase64(UserVM.ImagePath) : "";
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

        public string IsExist(UserMasterVM userMasterVM)
        {
            string response = string.Empty;
            try
            {
                var lstUsers = GetAll();
                if (userMasterVM != null)
                {
                    string workEmail = userMasterVM.WorkEmail; string empId = userMasterVM.EmployeeId;
                    int UserId = userMasterVM.Id;
                    if (!string.IsNullOrEmpty(workEmail) || !string.IsNullOrEmpty(empId))
                    {
                        if (UserId > 0 && !string.IsNullOrEmpty(workEmail))
                        {
                            bool workEmailExist = lstUsers.Count(x => x.Id != UserId && x.WorkEmail == workEmail) > 0;
                            if (workEmailExist)
                            {
                                response += Messages.WORK_EMAIL_EXIST;
                            }
                        }
                        else if(!string.IsNullOrEmpty(workEmail))
                        {
                            bool workEmailExist = lstUsers.Count(x=>x.WorkEmail == workEmail) > 0;
                            if (workEmailExist)
                            {
                                response += Messages.WORK_EMAIL_EXIST;
                            }
                        }

                         if (UserId > 0 && !string.IsNullOrEmpty(empId))
                        {
                            bool empIdExist = lstUsers.Count(x => x.Id != UserId && x.EmployeeId == empId) > 0;
                            if (empIdExist)
                            {
                                response += Messages.EMP_ID_EXIST;
                            }
                        }
                        else if (!string.IsNullOrEmpty(empId))
                        {
                            bool empIdExist = lstUsers.Count(x => x.EmployeeId == empId) > 0;
                            if (empIdExist)
                            {
                                response += Messages.EMP_ID_EXIST;
                            }
                        }
                        else
                        {
                            //Nothing to do here.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return response;
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