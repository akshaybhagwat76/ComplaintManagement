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
    public class EntityMasterRepository
    {

        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public EntityMasterRepository()
        {

        }

        public EntityMasterVM AddOrUpdate(EntityMasterVM EntityVM)
        {
            try
            {
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    var Entity = db.EntityMasters.FirstOrDefault(p => p.Id == EntityVM.Id);
                    if (Entity == null)
                    {
                        EntityVM.IsActive = true;
                        EntityVM.CreatedDate = DateTime.UtcNow;
                        EntityVM.UserId = 1;
                        EntityVM.CreatedBy = Convert.ToInt32(sid);
                        Entity = Mapper.Map<EntityMasterVM, EntityMaster>(EntityVM);
                        if (IsExist(Entity.EntityName))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.EntityMasters.Add(Entity);
                        db.SaveChanges();
                        return Mapper.Map<EntityMaster, EntityMasterVM>(Entity);
                    }
                    else
                    {
                        EntityVM.IsActive = true;
                         EntityVM.CreatedDate = Entity.CreatedDate;
                        EntityVM.CreatedBy = Entity.CreatedBy;
                        EntityVM.UpdatedDate = DateTime.UtcNow;
                        EntityVM.ModifiedBy = Convert.ToInt32(sid);
                        db.Entry(Entity).CurrentValues.SetValues(EntityVM);
                        if (IsExist(Entity.EntityName,Entity.Id))
                        {
                            throw new Exception(Messages.ALREADY_EXISTS);
                        }
                        db.SaveChanges();
                        return Mapper.Map<EntityMaster, EntityMasterVM>(Entity);

                    }
                }
                return new EntityMasterVM();
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

        public List<EntityMasterVM> GetAll()
        {

            List<EntityMaster> Entity = new List<EntityMaster>();
            List<EntityMasterVM> EntityList = new List<EntityMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                Entity = db.EntityMasters.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                if (Entity != null && Entity.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (EntityMaster item in Entity)
                    {
                        EntityMasterVM catObj = Mapper.Map<EntityMaster, EntityMasterVM>(item);
                        if (catObj != null)
                        {

                            catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                            catObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            EntityList.Add(catObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return EntityList;
        }

        public EntityMasterVM Get(int id)
        {
            EntityMaster Entity = new EntityMaster();
            try
            {
                Entity = db.EntityMasters.FirstOrDefault(i => i.Id == id && i.IsActive);
                if (Entity == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return Mapper.Map<EntityMaster, EntityMasterVM>(Entity);
        }


        public bool Delete(int id)
        {
            var data = db.EntityMasters.FirstOrDefault(p => p.Id == id);
            if (data != null)
            {
                data.IsActive = false;
            }
            return db.SaveChanges() > 0;
        }
        public bool IsExist(string EntityName)
        {
            return db.EntityMasters.Count(x => x.IsActive && x.EntityName.ToUpper() == EntityName.ToUpper()) > 0;
        }

        public bool IsExist(string EntityName, int id)
        {
            return db.EntityMasters.Count(x => x.IsActive && x.EntityName.ToUpper() == EntityName.ToUpper() && x.Id != id) > 0;
        }
        public string UploadImportEntity(string file)
        {
            return new Common().SaveExcelFromBase64(file);
        }

        public int ImportEntity(string file)
        {
            List<EntityMaster> importEntity = new List<EntityMaster>();
            EntityMaster EntityMasterDto = null;
            int count = 0;
            #region Indexes 
            int EntityNameIndex = 1; int StatusIndex = 2;
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
                        EntityMasterDto = new EntityMaster();

                        #region Category Name
                        //Competency Name check
                        if (string.IsNullOrEmpty(workSheet.Cells[i, EntityNameIndex].Value?.ToString()))
                        {
                            throw new Exception(string.Format(Messages.FieldIsRequired, new object[] { "Name", i, EntityNameIndex }));
                        }
                        else
                        {
                            string EntityName = workSheet.Cells[i, EntityNameIndex].Value?.ToString();
                            EntityMasterVM entityMasterDto = new EntityMasterVM { EntityName = EntityName };
                            if (IsExist(EntityName))
                            {
                                throw new Exception(string.Format(Messages.DataEntityAlreadyExists, new object[] { "Name", i, EntityNameIndex }));
                            }
                            else
                            {
                                EntityMasterDto.EntityName = workSheet.Cells[i, EntityNameIndex].Value?.ToString();
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
                                    EntityMasterDto.Status = true;
                                }
                                else
                                {
                                    EntityMasterDto.Status = false;
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format(Messages.StatusInvalid, new object[] { i, StatusIndex }));
                            }
                        }
                        #endregion
                        EntityMasterDto.CreatedBy = Convert.ToInt32(sid);
                        EntityMasterDto.CreatedDate = DateTime.UtcNow;
                        EntityMasterDto.IsActive = true;
                        importEntity.Add(EntityMasterDto);
                    }
                    if (importEntity != null && importEntity.Count > 0)
                    {
                        db.EntityMasters.AddRange(importEntity);
                        db.SaveChanges();
                        count = importEntity.Count;
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