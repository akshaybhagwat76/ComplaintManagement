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
    public class CompliantMastersRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public EmployeeCompliantMasterVM AddOrUpdate(EmployeeCompliantMasterVM EmployeeComplaintVM)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var EmployeeCompliant = db.EmployeeComplaintMasters.FirstOrDefault(p => p.UserId == EmployeeComplaintVM.UserId);
                            if (EmployeeCompliant == null)
                            {
                                EmployeeComplaintVM.IsActive = true;
                                EmployeeComplaintVM.ComplaintStatus = Messages.Opened;
                                EmployeeComplaintVM.CreatedDate = DateTime.UtcNow;
                                EmployeeComplaintVM.CreatedBy = Convert.ToInt32(sid);
                                EmployeeCompliant = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMaster>(EmployeeComplaintVM);

                                db.EmployeeComplaintMasters.Add(EmployeeCompliant);
                                db.SaveChanges();

                                EmployeeComplaintMastersHistory EmployeeComplaintMasters_History = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMastersHistory>(EmployeeComplaintVM);
                                if (EmployeeComplaintMasters_History != null) { EmployeeComplaintMasters_History.EntityState = Messages.Added; EmployeeComplaintMasters_History.EmployeeComplaintMasterId = EmployeeComplaintMasters_History.Id; };
                                db.EmployeeComplaintMastersHistories.Add(EmployeeComplaintMasters_History);
                                db.SaveChanges();
                                dbContextTransaction.Commit();

                            }
                            else
                            {
                                EmployeeComplaintVM.IsActive = true;
                                EmployeeComplaintVM.CreatedDate = EmployeeCompliant.CreatedDate;
                                EmployeeComplaintVM.CreatedBy = EmployeeCompliant.CreatedBy;
                                EmployeeComplaintVM.UpdatedDate = DateTime.UtcNow;
                                EmployeeComplaintVM.UpdatedBy = Convert.ToInt32(sid);
                                db.Entry(EmployeeCompliant).CurrentValues.SetValues(EmployeeComplaintVM);

                                db.SaveChanges();

                                EmployeeComplaintMastersHistory EmployeeComplaintMasters_History = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMastersHistory>(EmployeeComplaintVM);
                                if (EmployeeComplaintMasters_History != null) { EmployeeComplaintMasters_History.EntityState = Messages.Updated; EmployeeComplaintMasters_History.EmployeeComplaintMasterId = EmployeeComplaintMasters_History.Id; };
                                db.EmployeeComplaintMastersHistories.Add(EmployeeComplaintMasters_History);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new EmployeeCompliantMasterVM();
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
        public void Removefile(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                var user = db.EmployeeComplaintMasters.Where(x => x.Attachments.Contains(fileName)).FirstOrDefault();
                if (user != null && !string.IsNullOrEmpty(user.Attachments))
                {
                    string[] Attachments = user.Attachments.Split(new string[] { "," },
                                  StringSplitOptions.None);
                    int index = Array.IndexOf(Attachments, Attachments.Where(x => x == fileName).FirstOrDefault());
                    Attachments[index] = string.Empty;

                    user.Attachments = String.Join(",", Attachments.Select(p => p));
                    db.SaveChanges();
                }
                new Common().RemoveDoc(fileName);
            }
        }

        //public List<CategoryMasterVM> GetAll()
        //{
        //    List<CategoryMaster> category = new List<CategoryMaster>();
        //    List<CategoryMasterVM> categoryList = new List<CategoryMasterVM>();
        //    try
        //    {
        //        List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
        //        category = db.CategoryMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
        //        if (category != null && category.Count > 0 && usersList != null && usersList.Count > 0)
        //        {
        //            foreach (CategoryMaster item in category)
        //            {
        //                CategoryMasterVM catObj = Mapper.Map<CategoryMaster, CategoryMasterVM>(item);
        //                if (catObj != null)
        //                {
        //                    catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
        //                    catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
        //                    categoryList.Add(catObj);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
        //        throw new Exception(ex.Message.ToString());
        //    }
        //    return categoryList;
        //}

        //public List<CategoryMasterHistoryVM> GetAllHistory()
        //{
        //    List<CategoryMasters_History> category = new List<CategoryMasters_History>();
        //    List<CategoryMasterHistoryVM> categoryList = new List<CategoryMasterHistoryVM>();
        //    try
        //    {
        //        List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
        //        category = db.CategoryMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
        //        if (category != null && category.Count > 0 && usersList != null && usersList.Count > 0)
        //        {
        //            foreach (CategoryMasters_History item in category)
        //            {
        //                CategoryMasterHistoryVM catObj = Mapper.Map<CategoryMasters_History, CategoryMasterHistoryVM>(item);
        //                if (catObj != null)
        //                {
        //                    catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
        //                    catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
        //                    categoryList.Add(catObj);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
        //        throw new Exception(ex.Message.ToString());
        //    }
        //    return categoryList;
        //}

        //public CategoryMasterVM Get(int id)
        //{
        //    CategoryMaster category = new CategoryMaster();
        //    try
        //    {
        //        category = db.CategoryMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
        //        if (category == null)
        //        {
        //            throw new Exception(Messages.BAD_DATA);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
        //        throw new Exception(ex.Message.ToString());
        //    }
        //    return Mapper.Map<CategoryMaster, CategoryMasterVM>(category);
        //}

        //public bool Delete(int id)
        //{
        //    var data = db.CategoryMasters.FirstOrDefault(p => p.Id == id);
        //    if (data != null)
        //    {
        //        data.IsActive = false;
        //    }
        //    return db.SaveChanges() > 0;
        //}

        public DashboardVM GetDashboardCounts()
        {
            DashboardVM Dashboard = new DashboardVM();
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                var Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
               .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid) && !string.IsNullOrEmpty(Role))
                {
                    List<EmployeeComplaintMaster> employeeComplaintMaster = db.EmployeeComplaintMasters.Where(x => x.IsActive).ToList();
                    if (employeeComplaintMaster != null)
                    {
                        if (Role.ToLower() == Messages.HRUser.ToLower())
                        {
                            //employeeComplaintMaster = employeeComplaintMaster.Where(i => i.CreatedBy == Convert.ToInt32(sid)).ToList();
                            Dashboard.OverDueComplaints = employeeComplaintMaster.Where(x => x.DueDate >= DateTime.UtcNow).Count();
                            Dashboard.DueComplaints = employeeComplaintMaster.Where(x => x.ComplaintStatus == Messages.SUBMITTED && x.DueDate <= DateTime.Now).Count();
                            //das
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Dashboard;
        }
    }
}
