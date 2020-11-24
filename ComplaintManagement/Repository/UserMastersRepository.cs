using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        public UserMaster Login(LoginVM LoginVM)
        {
            UserMaster user = new UserMaster();
            try
            {
                if (string.IsNullOrWhiteSpace(LoginVM.Email) && string.IsNullOrWhiteSpace(LoginVM.Password))
                {
                    throw new Exception(Messages.BAD_DATA);
                }
                user = (from u in db.UserMasters
                        where u.EmployeeName == LoginVM.Email || u.WorkEmail == LoginVM.Email && u.IsActive 
                        select u).FirstOrDefault();
                if (user == null)
                {
                    throw new Exception(Messages.INVALID_USER_PASS);
                }
                if (!user.IsActive )
                {
                    throw new Exception(Messages.NOT_ACTIVE);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            
            return user;
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
                User = db.UserMasters.Where(i => i.IsActive).OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
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
                        else if (!string.IsNullOrEmpty(workEmail))
                        {
                            bool workEmailExist = lstUsers.Count(x => x.WorkEmail == workEmail) > 0;
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
        public string UploadImportUsers(string file)
        {
            string response = string.Empty;
            return new Common().SaveExcelFromBase64(file);
        }
        public int ImportUsers(string file)
        {
            List<UserMaster> importUsers = new List<UserMaster>();
            UserMaster UserMasterDto = null;
            int count = 0;
            #region Indexes 
            int EmployeeNameIndex = 1; int EmployeeIdIndex = 2; int GenderIndex = 3; int AgeIndex = 4;
            int WorkEmailIndex = 5; int TimeTypeIndex = 6; int BusinessTitleIndex = 7; int CompanyIndex = 8; int LOSIdIndex = 9;
            int SBUIdIndex = 10; int SubSBUIdIndex = 11; int CompentencyIdIndex = 12; int LocationIdIndex = 13; int RegionIdIndex = 14;
            int DateOfJoiningIndex = 15; int MobileNoIndex = 16; int ManagerIndex = 17; int TypeIndex = 18; int ImagePathIndex = 19; int StatusIndex = 20;

            #endregion


            string[] genders = { "male", "female" };
            string[] timeType = { "full time", "part time" };
            string[] managers = { "normal", "admin", "hr" };
            string[] statuses = { "active", "inactive" };

            int number;

            if (Path.GetExtension(file) == ".xlsx")
            {
                ExcelPackage package = new ExcelPackage(new FileInfo(file));
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                var lstDesignation = new DesignationMasterRepository().GetAll();
                var lstEntity = new EntityMasterRepository().GetAll();
                var lstSBU = new SBUMasterRepository().GetAll();
                var lstSubSBU = new SubSBUMasterRepository().GetAll();
                var lstLOS = new LOSMasterRepository().GetAll();
                var lstCompetency = new CompetencyMastersRepository().GetAll();
                var lstLocation = new LocationMastersRepository().GetAll();
                var lstRegion = new RegionMasterRepository().GetAll();
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

                for (int i = 1; i <= workSheet.Dimension.Rows; i++)
                {
                    if (i == 1) //skip header row if its there
                    {
                        continue;
                    }
                    UserMasterDto = new UserMaster();

                    //Employee Name check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, EmployeeNameIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, EmployeeNameIndex }));
                    }
                    else
                    {
                        UserMasterDto.EmployeeName = workSheet.Cells[i, EmployeeNameIndex].Value?.ToString();
                    }

                    //EmpId check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, EmployeeIdIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "ID", i, EmployeeIdIndex }));
                    }
                    else
                    {
                        string EmployeeId = workSheet.Cells[i, EmployeeIdIndex].Value?.ToString();
                        UserMasterVM userMasterDto = new UserMasterVM { EmployeeId = EmployeeId };
                        if (!string.IsNullOrEmpty(IsExist(userMasterDto)))
                        {
                            throw new Exception(string.Format(Messages.DataEmpAlreadyExists, new object[] { "ID", i, EmployeeIdIndex }));
                        }
                        else
                        {
                            UserMasterDto.EmployeeName = workSheet.Cells[i, EmployeeIdIndex].Value?.ToString();
                        }
                    }

                    //Gender check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, GenderIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "ID", i, GenderIndex }));
                    }
                    else
                    {
                        string Gender = workSheet.Cells[i, EmployeeIdIndex].Value?.ToString();
                        if (genders.Any(Gender.ToLower().Contains))
                        {
                            UserMasterDto.Gender = workSheet.Cells[i, GenderIndex].Value?.ToString();
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.GenderInvalid, new object[] { i, GenderIndex }));
                        }
                    }

                    //Age check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, AgeIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Age", i, AgeIndex }));
                    }
                    else
                    {
                        if (int.TryParse(workSheet.Cells[i, AgeIndex].Value?.ToString(), out number))
                        {

                            UserMasterDto.Age = Convert.ToInt32(workSheet.Cells[i, AgeIndex].Value?.ToString());
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.AgeInvalid, new object[] { i, AgeIndex }));
                        }
                    }

                    //Email check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, WorkEmailIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "ID", i, WorkEmailIndex }));
                    }
                    else
                    {
                        string Email = workSheet.Cells[i, WorkEmailIndex].Value?.ToString();
                        Match match = regex.Match(Email);

                        UserMasterVM userMasterDto = new UserMasterVM { WorkEmail = Email };
                        if (!string.IsNullOrEmpty(IsExist(userMasterDto)))
                        {
                            throw new Exception(string.Format(Messages.DataEmpAlreadyExists, new object[] { "email", i, WorkEmailIndex }));
                        }
                        else if (!match.Success)
                        {
                            throw new Exception(string.Format(Messages.EmailFormat, new object[] { i, WorkEmailIndex }));
                        }
                        else
                        {
                            UserMasterDto.WorkEmail = workSheet.Cells[i, WorkEmailIndex].Value?.ToString();
                        }
                    }

                    //Type check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, TimeTypeIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "ID", i, TimeTypeIndex }));
                    }
                    else
                    {
                        string Type = workSheet.Cells[i, TimeTypeIndex].Value?.ToString();
                        if (timeType.Any(Type.ToLower().Contains))
                        {
                            UserMasterDto.TimeType = workSheet.Cells[i, TimeTypeIndex].Value?.ToString();
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.TimeTypeInvalid, new object[] { i, AgeIndex }));
                        }
                    }

                    //Businesss check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, BusinessTitleIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "ID", i, BusinessTitleIndex }));
                    }
                    else
                    {
                        string Business = workSheet.Cells[i, BusinessTitleIndex].Value?.ToString();
                        var Designation = lstDesignation.FirstOrDefault(x => x.Designation.ToLower() == Business.ToLower());
                        if (Designation != null)
                        {
                            UserMasterDto.BusinessTitle = Designation.Id;
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "Business title", i, BusinessTitleIndex }));
                        }
                    }

                    //Company check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, CompanyIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Company", i, CompanyIndex }));
                    }
                    else
                    {
                        string Entity = workSheet.Cells[i, CompanyIndex].Value?.ToString();
                        var CompanyDto = lstEntity.FirstOrDefault(x => x.EntityName.ToLower() == Entity.ToLower());
                        if (CompanyDto != null)
                        {
                            UserMasterDto.Company = CompanyDto.Id;
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "Company", i, CompanyIndex }));
                        }
                    }

                    //LOS check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, LOSIdIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "LOS", i, LOSIdIndex }));
                    }
                    else
                    {
                        string LOS = workSheet.Cells[i, LOSIdIndex].Value?.ToString();
                        var LOSDto = lstLOS.FirstOrDefault(x => x.LOSName.ToLower() == LOS.ToLower());
                        if (LOSDto != null)
                        {
                            UserMasterDto.LOSId = LOSDto.Id;
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "LOS", i, LOSIdIndex }));
                        }
                    }

                    //SBU check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, SBUIdIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "SBU", i, SBUIdIndex }));
                    }
                    else
                    {
                        string SBU = workSheet.Cells[i, SBUIdIndex].Value?.ToString();
                        var SBUDto = lstSBU.FirstOrDefault(x => x.SBU.ToLower() == SBU.ToLower());
                        if (SBUDto != null)
                        {
                            UserMasterDto.SBUId = SBUDto.Id;
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "SBU", i, SBUIdIndex }));
                        }
                    }

                    //SubSBU check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, SubSBUIdIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "SubSBU", i, SubSBUIdIndex }));
                    }
                    else
                    {
                        string SubSBU = workSheet.Cells[i, SubSBUIdIndex].Value?.ToString();
                        var SubSBUDto = lstSubSBU.FirstOrDefault(x => x.SubSBU.ToLower() == SubSBU.ToLower());
                        if (SubSBUDto != null)
                        {
                            UserMasterDto.SubSBUId = SubSBUDto.Id;
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "SubSBU", i, SubSBUIdIndex }));
                        }
                    }

                    //Compentency check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, CompentencyIdIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Compentency", i, CompentencyIdIndex }));
                    }
                    else
                    {
                        string Compentency = workSheet.Cells[i, CompentencyIdIndex].Value?.ToString();
                        var CompentencyDto = lstCompetency.FirstOrDefault(x => x.CompetencyName.ToLower() == Compentency.ToLower());
                        if (CompentencyDto != null)
                        {
                            UserMasterDto.CompentencyId = CompentencyDto.Id;
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "Compentency", i, CompentencyIdIndex }));
                        }
                    }

                    //Location check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, LocationIdIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Location", i, LocationIdIndex }));
                    }
                    else
                    {
                        string Location = workSheet.Cells[i, LocationIdIndex].Value?.ToString();
                        var LocationDto = lstCompetency.FirstOrDefault(x => x.CompetencyName.ToLower() == Location.ToLower());
                        if (LocationDto != null)
                        {
                            UserMasterDto.LocationId = LocationDto.Id;
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "Location", i, LocationIdIndex }));
                        }
                    }

                    //Region check
                    if (string.IsNullOrEmpty(workSheet.Cells[i, RegionIdIndex].Value?.ToString()))
                    {
                        throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Region", i, RegionIdIndex }));
                    }
                    else
                    {
                        string Region = workSheet.Cells[i, RegionIdIndex].Value?.ToString();
                        var RegionDto = lstRegion.FirstOrDefault(x => x.Region.ToLower() == Region.ToLower());
                        if (RegionDto != null)
                        {
                            UserMasterDto.RegionId = RegionDto.Id;
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.DataEmpNOTExists, new object[] { "Region", i, RegionIdIndex }));
                        }
                    }

                    //DOJ check
                    if (!string.IsNullOrEmpty(workSheet.Cells[i, DateOfJoiningIndex].Value?.ToString()))
                    {
                        string DOJ = workSheet.Cells[i, DateOfJoiningIndex].Value?.ToString();
                        try
                        {
                            UserMasterDto.DateOfJoining = Convert.ToDateTime(DOJ);
                        }
                        catch (FormatException)
                        {
                            throw new Exception(string.Format(Messages.DOJInvalid, new object[] { i, DateOfJoiningIndex }));
                        }
                    }

                    //Mobile check
                    if (!string.IsNullOrEmpty(workSheet.Cells[i, MobileNoIndex].Value?.ToString()))
                    {
                        string Mobile = workSheet.Cells[i, MobileNoIndex].Value?.ToString();

                        if (Mobile.Length > 17)
                        {
                            throw new Exception(string.Format(Messages.MobileNumberInvalid, new object[] { i, DateOfJoiningIndex }));
                        }
                        if (Mobile.Any(x => !char.IsLetter(x)))
                        {
                            UserMasterDto.MobileNo = Mobile;
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.MobileNumberInvalidFormat, new object[] { i, DateOfJoiningIndex }));
                        }
                    }

                    //Manager check
                    if (!string.IsNullOrEmpty(workSheet.Cells[i, ManagerIndex].Value?.ToString()))
                    {
                        string mgr = workSheet.Cells[i, ManagerIndex].Value?.ToString();
                        UserMasterDto.Manager = workSheet.Cells[i, ManagerIndex].Value?.ToString();
                        ///Already created employees check.

                        //if (managers.Any(mgr.ToLower().Contains))
                        //{
                        //}
                        //else
                        //{
                        //    throw new Exception(string.Format(Messages.ManagerInvalid, new object[] { i, ManagerIndex }));
                        //}
                    }

                    //Type check
                    if (!string.IsNullOrEmpty(workSheet.Cells[i, TypeIndex].Value?.ToString()))
                    {
                        string Type = workSheet.Cells[i, TypeIndex].Value?.ToString();
                        if (managers.Any(Type.ToLower().Contains))
                        {
                            UserMasterDto.Type = workSheet.Cells[i, TypeIndex].Value?.ToString();
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.TypeInvalid, new object[] { i, TypeIndex }));
                        }
                    }

                    //Image check
                    if (!string.IsNullOrEmpty(workSheet.Cells[i, ImagePathIndex].Value?.ToString()))
                    {
                        string IMG = workSheet.Cells[i, ImagePathIndex].Value?.ToString();
                        UserMasterDto.ImagePath = IMG;
                    }

                    //Status check
                    if (!string.IsNullOrEmpty(workSheet.Cells[i, StatusIndex].Value?.ToString()))
                    {
                        string Status = workSheet.Cells[i, StatusIndex].Value?.ToString();
                        if (statuses.Any(Status.ToLower().Contains))
                        {
                            UserMasterDto.Status = Convert.ToBoolean(workSheet.Cells[i, StatusIndex].Value?.ToString());
                        }
                        else
                        {
                            throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                        }
                    }
                }
            }
            return count;
        }
    }
}
