using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace ComplaintManagement.Repository
{
    public class EmployeeComplaintMastersRepository
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
                            var EmployeeComplaint = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == EmployeeComplaintVM.Id);
                            if (EmployeeComplaint == null)
                            {
                                EmployeeComplaintVM.IsActive = true;
                                EmployeeComplaintVM.CreatedDate = DateTime.UtcNow;
                                EmployeeComplaintVM.CreatedBy = Convert.ToInt32(sid);
                                EmployeeComplaintVM.DueDate = DateTime.UtcNow.AddDays(5);
                                EmployeeComplaint = Mapper.Map<EmployeeCompliantMasterVM, EmployeeComplaintMaster>(EmployeeComplaintVM);
                                EmployeeComplaint.UserId = EmployeeComplaint.UserId == 0 ? Convert.ToInt32(sid) : EmployeeComplaint.UserId;
                                EmployeeComplaint.ComplaintStatus = Messages.Opened;
                                    EmployeeComplaint.Status = true;
                                db.EmployeeComplaintMasters.Add(EmployeeComplaint);
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
                                EmployeeComplaintVM.CreatedDate = EmployeeComplaint.CreatedDate;
                                EmployeeComplaintVM.CreatedBy = EmployeeComplaint.CreatedBy;
                                EmployeeComplaintVM.UpdatedDate = DateTime.UtcNow;
                                EmployeeComplaintVM.UpdatedBy = Convert.ToInt32(sid);
                                db.Entry(EmployeeComplaint).CurrentValues.SetValues(EmployeeComplaintVM);
                                db.SaveChanges();
                                dbContextTransaction.Commit();

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

        public List<EmployeeCompliantMasterVM> GetAll()
        {
            List<EmployeeComplaintMaster> EmployeeComplaint = new List<EmployeeComplaintMaster>();
            List<EmployeeCompliantMasterVM> EmployeeCompliant_oneList = new List<EmployeeCompliantMasterVM>();
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                    EmployeeComplaint = db.EmployeeComplaintMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                    EmployeeComplaint = EmployeeComplaint.Where(x => x.CreatedBy == Convert.ToInt32(sid)).ToList();
                    if (EmployeeComplaint != null && EmployeeComplaint.Count > 0 && usersList != null && usersList.Count > 0)
                    {
                        foreach (EmployeeComplaintMaster item in EmployeeComplaint)
                        {
                            EmployeeCompliantMasterVM catObj = Mapper.Map<EmployeeComplaintMaster, EmployeeCompliantMasterVM>(item);
                            if (catObj != null)
                            {
                                catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.UpdatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.UpdatedBy).EmployeeName : Messages.NotAvailable;
                                catObj.CategoryName = new CategoryMastersRepository().Get(item.CategoryId) != null ? new CategoryMastersRepository().Get(item.CategoryId).CategoryName : Messages.NotAvailable;
                                catObj.SubCategoryName = new SubCategoryMastersRepository().Get(item.SubCategoryId) != null ? new SubCategoryMastersRepository().Get(item.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                                catObj.EmployeeName = usersList.FirstOrDefault(x => x.Id == catObj.UserId) != null ? usersList.FirstOrDefault(x => x.Id == catObj.UserId).EmployeeName : Messages.NotAvailable;

                                EmployeeCompliant_oneList.Add(catObj);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return EmployeeCompliant_oneList;
        }

        public List<CategoryMasterHistoryVM> GetAllHistory()
        {
            List<CategoryMasters_History> category = new List<CategoryMasters_History>();
            List<CategoryMasterHistoryVM> categoryList = new List<CategoryMasterHistoryVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                category = db.CategoryMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (category != null && category.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (CategoryMasters_History item in category)
                    {
                        CategoryMasterHistoryVM catObj = Mapper.Map<CategoryMasters_History, CategoryMasterHistoryVM>(item);
                        if (catObj != null)
                        {
                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            categoryList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return categoryList;
        }

        public EmployeeCompliantMasterVM Get(int id)
        {
            EmployeeComplaintMaster EmployeeComplaint = new EmployeeComplaintMaster();
            try
            {
                EmployeeComplaint = db.EmployeeComplaintMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (EmployeeComplaint == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<EmployeeComplaintMaster, EmployeeCompliantMasterVM>(EmployeeComplaint);
        }
        public bool Delete(int id)
        {
            var data = db.EmployeeComplaintMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }


    }
}