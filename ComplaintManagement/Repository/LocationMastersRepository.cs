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
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace ComplaintManagement.Repository
{
    public class LocationMastersRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public LocationMastersRepository()
        {

        }

        public LocationMasterVM AddOrUpdate(LocationMasterVM LocationVM)
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
                            var Location = db.LocationMasters.FirstOrDefault(p => p.Id == LocationVM.Id);
                            if (Location == null)
                            {
                                LocationVM.IsActive = true;
                                LocationVM.CreatedDate = DateTime.UtcNow;
                                LocationVM.UserId = 1;
                                LocationVM.CreatedBy = Convert.ToInt32(sid);
                                Location = Mapper.Map<LocationMasterVM, LocationMaster>(LocationVM);
                                if (IsExist(Location.LocationName))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.LocationMasters.Add(Location);
                                db.SaveChanges();


                                LocationMasters_History historyObj = Mapper.Map<LocationMasterVM, LocationMasters_History>(LocationVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Updated; historyObj.LocationId = Location.Id; };
                                db.LocationMasters_History.Add(historyObj);
                                db.SaveChanges();

                                return Mapper.Map<LocationMaster, LocationMasterVM>(Location);
                            }
                            else
                            {
                                LocationVM.IsActive = true;
                                LocationVM.CreatedDate = Location.CreatedDate;
                                LocationVM.CreatedBy = Location.CreatedBy;
                                LocationVM.UpdatedDate = DateTime.UtcNow;
                                LocationVM.ModifiedBy = Convert.ToInt32(sid);
                                db.Entry(Location).CurrentValues.SetValues(LocationVM);
                                if (IsExist(Location.LocationName, Location.Id))
                                {
                                    throw new Exception(Messages.ALREADY_EXISTS);
                                }
                                db.SaveChanges();

                                LocationMasters_History historyObj = Mapper.Map<LocationMasterVM, LocationMasters_History>(LocationVM);
                                if (historyObj != null) { historyObj.EntityState = Messages.Updated; historyObj.LocationId = Location.Id; };
                                db.LocationMasters_History.Add(historyObj);
                                db.SaveChanges();

                                return Mapper.Map<LocationMaster, LocationMasterVM>(Location);

                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
                return new LocationMasterVM();
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

        public List<LocationMasterVM> GetAll()
        {

            List<LocationMaster> Location = new List<LocationMaster>();
            List<LocationMasterVM> LocationList = new List<LocationMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                Location = db.LocationMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (Location != null && Location.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (LocationMaster item in Location)
                    {
                        LocationMasterVM catObj = Mapper.Map<LocationMaster, LocationMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            LocationList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return LocationList;
        }

        public LocationMasterVM Get(int id)
        {
            LocationMaster Location = new LocationMaster();
            try
            {
                Location = db.LocationMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Location == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<LocationMaster, LocationMasterVM>(Location);
        }

        public List<LocationMasterHistoryVM> GetAllHistory()
        {
            List<LocationMasters_History> listdto = new List<LocationMasters_History>();
            List<LocationMasterHistoryVM> lst = new List<LocationMasterHistoryVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                listdto = db.LocationMasters_History.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (listdto != null && listdto.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (LocationMasters_History item in listdto)
                    {
                        LocationMasterHistoryVM ViewModelDto = Mapper.Map<LocationMasters_History, LocationMasterHistoryVM>(item);
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
            var data = db.LocationMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string LocationName)
        {
            return db.LocationMasters.Count(x => x.IsActive && x.LocationName.ToUpper() == LocationName.ToUpper()) > 0;
        }

        public bool IsExist(string LocationName, int id)
        {
            return db.LocationMasters.Count(x => x.IsActive && x.LocationName.ToUpper() == LocationName.ToUpper() && x.Id != id) > 0;
        }

        public string UploadImportLocation(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportLocation(string file)
        {
            List<LocationMaster> importLocation = new List<LocationMaster>();
            LocationMaster LocationMasterDto = null;
            int count = 0;
            #region Indexes 
            int LocationNameIndex = 1; int StatusIndex = 2;
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
                        LocationMasterDto = new LocationMaster();

                        #region Location Name
                        //Location Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, LocationNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, LocationNameIndex }));
                        }
                        else
                        {
                            string LocationName = workSheet.Cells[i, LocationNameIndex].Value?.ToString();
                            LocationMasterVM locationMasterDto = new LocationMasterVM { LocationName = LocationName };
                            if (IsExist(LocationName))
                            {
                                throw new Exception(string.Format(Messages.DataLocationAlreadyExists, new object[] { "Name", i, LocationNameIndex }));
                            }
                            else
                            {
                                LocationMasterDto.LocationName = workSheet.Cells[i, LocationNameIndex].Value?.ToString();
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
                                    LocationMasterDto.Status = true;
                                }
                                else
                                {
                                    LocationMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion
                        LocationMasterDto.CreatedBy = Convert.ToInt32(sid);
                        LocationMasterDto.CreatedDate = DateTime.UtcNow;
                        LocationMasterDto.IsActive = true;
                        importLocation.Add(LocationMasterDto);
                    }
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (importLocation != null && importLocation.Count > 0)
                            {
                                db.LocationMasters.AddRange(importLocation);
                                db.SaveChanges();

                                List<LocationMasterHistoryVM> listVMDto = Mapper.Map<List<LocationMaster>, List<LocationMasterHistoryVM>>(importLocation);

                                List<LocationMasters_History> HistoryDto = Mapper.Map<List<LocationMasterHistoryVM>, List<LocationMasters_History>>(listVMDto);
                                if (HistoryDto != null && HistoryDto.Count > 0)
                                {
                                    HistoryDto.Select(c => { c.EntityState = Messages.Added; c.LocationId = c.Id; return c; }).ToList();
                                }

                                db.LocationMasters_History.AddRange(HistoryDto);
                                db.SaveChanges();

                                dbContextTransaction.Commit();

                                count = importLocation.Count;
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