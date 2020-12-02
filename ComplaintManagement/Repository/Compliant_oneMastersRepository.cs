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
    public class Compliant_oneMastersRepository
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public Compliant_oneMastersRepository()
        {

        }

        public EmployeeCompliant_oneMasterVM AddOrUpdate(EmployeeCompliant_oneMasterVM EmployeeCompliant_oneVM)
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
                            var EmployeeCompliant_one = db.EmployeeComplaintMasters.FirstOrDefault(p => p.UserId == EmployeeCompliant_oneVM.UserId);
                            if (EmployeeCompliant_one == null)
                            {
                                EmployeeCompliant_oneVM.IsActive = true;
                                EmployeeCompliant_oneVM.ComplaintStatus = true;
                                EmployeeCompliant_oneVM.CreatedDate = DateTime.UtcNow;
                                EmployeeCompliant_oneVM.CreatedBy = Convert.ToInt32(sid);
                                EmployeeCompliant_one = Mapper.Map<EmployeeCompliant_oneMasterVM, EmployeeComplaintMaster>(EmployeeCompliant_oneVM);
                              
                                db.EmployeeComplaintMasters.Add(EmployeeCompliant_one);
                                db.SaveChanges();

                                //CategoryMasters_History categoryMasters_History = Mapper.Map<CategoryMasterVM, CategoryMasters_History>(categoryVM);
                                //if (categoryMasters_History != null) { categoryMasters_History.EntityState = Messages.Added; categoryMasters_History.CategoryId = category.Id; };
                                //db.CategoryMasters_History.Add(categoryMasters_History);
                                //db.SaveChanges();

                                //dbContextTransaction.Commit();
                                //return Mapper.Map<CategoryMaster, CategoryMasterVM>(category);
                            }
                            else
                            {
                                EmployeeCompliant_oneVM.IsActive = true;
                                EmployeeCompliant_oneVM.CreatedDate = EmployeeCompliant_one.CreatedDate;
                                EmployeeCompliant_oneVM.CreatedBy = EmployeeCompliant_one.CreatedBy;
                                EmployeeCompliant_oneVM.UpdatedDate = DateTime.UtcNow;
                                EmployeeCompliant_oneVM.UpdatedBy = Convert.ToInt32(sid);
                                db.Entry(EmployeeCompliant_one).CurrentValues.SetValues(EmployeeCompliant_oneVM);
                             
                                db.SaveChanges();

                                //CategoryMasters_History categoryMasters_History = Mapper.Map<CategoryMasterVM, CategoryMasters_History>(categoryVM);
                                //if (categoryMasters_History != null) { categoryMasters_History.EntityState = Messages.Updated; categoryMasters_History.CategoryId = category.Id; };
                                //db.CategoryMasters_History.Add(categoryMasters_History);
                                //db.SaveChanges();

                                //dbContextTransaction.Commit();
                                //return Mapper.Map<CategoryMaster, CategoryMasterVM>(category);
                            }
                        }
                        catch (Exception ex)
                        {
                            //dbContextTransaction.Rollback();
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new EmployeeCompliant_oneMasterVM();
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

    }
}
    
