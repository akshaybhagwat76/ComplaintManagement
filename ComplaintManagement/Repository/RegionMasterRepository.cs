using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace ComplaintManagement.Repository
{
    public class RegionMasterRepository
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public RegionMasterRepository()
        {

        }

        public RegionMasterVM AddOrUpdate(RegionMasterVM RegionVM)
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
                            var Region = db.RegionMasters.FirstOrDefault(p => p.Id == RegionVM.Id);
                            if (Region == null)
                            {
                                RegionVM.IsActive = true;
                                RegionVM.CreatedDate = DateTime.UtcNow;
                                RegionVM.UserId = 1;
                                RegionVM.CreatedBy = Convert.ToInt32(sid);
                                Region = Mapper.Map<RegionMasterVM, RegionMaster>(RegionVM);
                                if (IsExist(Region.Region))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.RegionMasters.Add(Region);
                                db.SaveChanges();

                                RegionMasters_History historyObj = Mapper.Map<RegionMasterVM, RegionMasters_History>(RegionVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Added; historyObj.RegionId = Region.Id; };
                                db.RegionMasters_History.Add(historyObj);
                                db.SaveChanges();

                                return Mapper.Map<RegionMaster, RegionMasterVM>(Region);
                            }
                            else
                            {
                                RegionVM.IsActive = true;
                                RegionVM.CreatedDate = Region.CreatedDate;
                                RegionVM.CreatedBy = Region.CreatedBy;
                                RegionVM.UpdatedDate = DateTime.UtcNow;
                                RegionVM.ModifiedBy = Convert.ToInt32(sid);
                                db.Entry(Region).CurrentValues.SetValues(RegionVM);
                                if (IsExist(Region.Region, Region.Id))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SaveChanges();

                                RegionMasters_History historyObj = Mapper.Map<RegionMasterVM, RegionMasters_History>(RegionVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Updated; historyObj.RegionId = Region.Id; };
                                db.RegionMasters_History.Add(historyObj);
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                return Mapper.Map<RegionMaster, RegionMasterVM>(Region);

                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new RegionMasterVM();
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

        public List<RegionMasterVM> GetAll()
        {

            List<RegionMaster> Region = new List<RegionMaster>();
            List<RegionMasterVM> RegionList = new List<RegionMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                Region = db.RegionMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (Region != null && Region.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (RegionMaster item in Region)
                    {
                        RegionMasterVM catObj = Mapper.Map<RegionMaster, RegionMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            RegionList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return RegionList;
        }

        public RegionMasterVM Get(int id)
        {
            RegionMaster Region = new RegionMaster();
            try
            {
                Region = db.RegionMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Region == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<RegionMaster, RegionMasterVM>(Region);
        }

        public List<RegionMasterHistoryVM> GetAllHistory()
        {
            List<RegionMasters_History> listdto = new List<RegionMasters_History>();
            List<RegionMasterHistoryVM> lst = new List<RegionMasterHistoryVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                listdto = db.RegionMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (listdto != null && listdto.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (RegionMasters_History item in listdto)
                    {
                        RegionMasterHistoryVM ViewModelDto = Mapper.Map<RegionMasters_History, RegionMasterHistoryVM>(item);
                        if (ViewModelDto != null)
                        {
                            ViewModelDto.CreatedByName = usersList.FirstOrDefault(x => x.Id == ViewModelDto.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == ViewModelDto.CreatedBy).EmployeeName : string.Empty;
                            ViewModelDto.UpdatedByName = usersList.FirstOrDefault(x => x.Id == ViewModelDto.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == ViewModelDto.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            lst.Add(ViewModelDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return lst;
        }

        public bool Delete(int id)
        {
            var data = db.RegionMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }

        public bool IsExist(string Region)
        {
            return db.RegionMasters.Count(x => x.IsActive && x.Region.ToUpper() == Region.ToUpper()) > 0;
        }

        public bool IsExist(string Region, int id)
        {
            return db.RegionMasters.Count(x => x.IsActive && x.Region.ToUpper() == Region.ToUpper() && x.Id != id) > 0;
        }

        public string UploadImportRegion(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportRegion(string file)
        {
            List<RegionMaster> importRegion = new List<RegionMaster>();
            RegionMaster RegionMasterDto = null;
            int count = 0;
            #region Indexes 
            int RegionNameIndex = 1; int StatusIndex = 2;
            #endregion

            string[] statuses = { "active", "inactive" };
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (Path.GetExtension(file) == ".xlsx" && !string.IsNullOrEmpty(sid))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelPackage package = new ExcelPackage(new FileInfo(file));
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    for (int i = 1; i <= workSheet.Dimension.Rows; i++)
                    {
                        if (i == 1) //skip header row if its there
                        {
                            continue;
                        }
                        RegionMasterDto = new RegionMaster();

                        #region Region Name
                        //Region Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, RegionNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, RegionNameIndex }));
                        }
                        else
                        {
                            string RegionName = workSheet.Cells[i, RegionNameIndex].Value?.ToString();
                            RegionMasterVM regionMasterDto = new RegionMasterVM { Region = RegionName };
                            if (IsExist(RegionName))
                            {
                                throw new Exception(string.Format(Messages.DataRegionAlreadyExists, new object[] { "Name", i, RegionNameIndex }));
                            }
                            else
                            {
                                RegionMasterDto.Region = workSheet.Cells[i, RegionNameIndex].Value?.ToString();
                            }
                        }
                        #endregion

                        #region Status
                        //Status check
                        if (!string.IsNullOrEmpty(workSheet.Cells[i, StatusIndex].Value?.ToString()))
                        {
                            string Status = workSheet.Cells[i, StatusIndex].Value?.ToString();
                            if (statuses.Any(Status.ToLower().Contains))
                            {
                                if (workSheet.Cells[i, StatusIndex].Value?.ToString().ToLower() == Messages.Active.ToLower())
                                {
                                    RegionMasterDto.Status = true;
                                }
                                else
                                {
                                    RegionMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion
                        RegionMasterDto.CreatedBy = Convert.ToInt32(sid);
                        RegionMasterDto.CreatedDate = DateTime.UtcNow;
                        RegionMasterDto.IsActive = true;
                        importRegion.Add(RegionMasterDto);
                    }
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (importRegion != null && importRegion.Count > 0)
                            {
                                db.RegionMasters.AddRange(importRegion);
                                db.SaveChanges();


                                List<RegionMasterHistoryVM> listVMDto = Mapper.Map<List<RegionMaster>, List<RegionMasterHistoryVM>>(importRegion);

                                List<RegionMasters_History> HistoryDto = Mapper.Map<List<RegionMasterHistoryVM>, List<RegionMasters_History>>(listVMDto);
                                if (HistoryDto != null && HistoryDto.Count > 0)
                                {
                                    HistoryDto.Select(c => { c.EntityState = Messages.Added; c.RegionId = c.Id; return c; }).ToList();
                                }

                                db.RegionMasters_History.AddRange(HistoryDto);
                                db.SaveChanges();

                                dbContextTransaction.Commit();

                                count = importRegion.Count;
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                count = 0;
                throw ex;
            }
            return count;
        }
    }
}