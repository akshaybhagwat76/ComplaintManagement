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
using Dapper;
using System.Data.SqlClient;
using System.Configuration;

namespace ComplaintManagement.Repository
{
    public class EmployeeComplaintHistoryRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();
        
       
        public void AddComplaintHistory(string remarks, int complaintId, string actionType, DB_A6A061_complaintuserEntities db)
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                string usertypeValue = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    EmployeeComplaintHistory employeeComplaintHistory = new EmployeeComplaintHistory();
                    employeeComplaintHistory.IsActive = true;
                    employeeComplaintHistory.CreatedDate = DateTime.UtcNow;
                    employeeComplaintHistory.CreatedBy = Convert.ToInt32(sid);
                    employeeComplaintHistory.Remarks = remarks;
                    employeeComplaintHistory.ComplaintId = complaintId;
                    employeeComplaintHistory.ActionType = actionType;
                    employeeComplaintHistory.UserType = usertypeValue;
                    db.EmployeeComplaintHistories.Add(employeeComplaintHistory);
                    db.SaveChanges();
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

        public List<EmployeeComplaintHistoryVM> GetAll()
        {
            List<EmployeeComplaintHistory> EmployeeComplaintHistory = new List<EmployeeComplaintHistory>();
            List<EmployeeComplaintHistoryVM> EmployeeComplaintHistoryList = new List<EmployeeComplaintHistoryVM>();

            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(sid))
                    {
                        List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                        EmployeeComplaintHistory = db.EmployeeComplaintHistories.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                        if (EmployeeComplaintHistory != null && EmployeeComplaintHistory.Count > 0 && usersList != null && usersList.Count > 0)
                        {
                            foreach (EmployeeComplaintHistory item in EmployeeComplaintHistory)
                            {
                                EmployeeComplaintHistoryVM catObj = Mapper.Map<EmployeeComplaintHistory, EmployeeComplaintHistoryVM>(item);
                                if (catObj != null)
                                {
                                    catObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == catObj.CreatedBy).EmployeeName : string.Empty;
                                    EmployeeComplaintHistoryList.Add(catObj);
                                }
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
            return EmployeeComplaintHistoryList;
        }

        public List<EmployeeComplaintWorkFlowVM> GetAllAwaitingComplaints()
        {
            List<EmployeeComplaintHistory> EmployeeComplaintHistory = new List<EmployeeComplaintHistory>();
            List<EmployeeComplaintHistoryVM> EmployeeComplaintHistoryList = new List<EmployeeComplaintHistoryVM>();
           // List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowList = new EmployeeWorkFlowRepository().GetAll().ToList();
            List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowListDto = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlow> empcomplaintworks = new List<EmployeeComplaintWorkFlow>();
            //List<EmployeeCompliantMasterVM> EmployeeComplaintList = new EmployeeComplaintMastersRepository().GetAll();
            List<EmployeeCompliantMasterVM> EmployeeComplaintList = new List<EmployeeCompliantMasterVM>();
            //List<CategoryMasterVM> categoryMasters = new CategoryMastersRepository().GetAll();
            List<CategoryMasterVM> categoryMasters = new List<CategoryMasterVM>();
            //List<SubCategoryMasterVM> SubcategoryMasters = new SubCategoryMastersRepository().GetAll();
            List<SubCategoryMasterVM> SubcategoryMasters = new List<SubCategoryMasterVM>();


            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                //using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                //{
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                           .Select(c => c.Value).SingleOrDefault();
                var Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    //List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                    //EmployeeComplaintHistory = db.EmployeeComplaintHistories.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                    //if (EmployeeComplaintHistory != null && EmployeeComplaintHistory.Count > 0 && usersList != null && usersList.Count > 0)
                    //{
                    //foreach (EmployeeComplaintWorkFlowVM employeeComplaintWorkFlowDto in EmployeeComplaintWorkFlowList)
                    //{
                    //    if (employeeComplaintWorkFlowDto.ActionType.ToLower() == Messages.SUBMITTED.ToLower() && employeeComplaintWorkFlowDto.CreatedBy == Convert.ToInt32(sid) && employeeComplaintWorkFlowDto.IsActive)
                    //    {
                    //        employeeComplaintWorkFlowDto.Id = employeeComplaintWorkFlowDto.ComplaintId;
                    //        employeeComplaintWorkFlowDto.CreatedByName = usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy).EmployeeName : string.Empty;
                    //        employeeComplaintWorkFlowDto.CreatedDate = employeeComplaintWorkFlowDto.CreatedDate;
                    //        employeeComplaintWorkFlowDto.LOSName = new LOSMasterRepository().Get(employeeComplaintWorkFlowDto.LOSId).LOSName;
                    //        employeeComplaintWorkFlowDto.ComplaintNo = employeeComplaintWorkFlowDto.ComplaintNo;
                    //        EmployeeCompliantMasterVM employeeCompliantMasterDto = EmployeeComplaintList.Where(x => x.Id == employeeComplaintWorkFlowDto.ComplaintId).FirstOrDefault();
                    //        if (employeeCompliantMasterDto != null && employeeCompliantMasterDto.CategoryId > 0 && employeeCompliantMasterDto.SubCategoryId > 0)
                    //        {
                    //            employeeComplaintWorkFlowDto.Category = categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId) != null ? categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId).CategoryName : Messages.NotAvailable;
                    //            employeeComplaintWorkFlowDto.SubCategory = SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId) != null ? SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                    //        }
                    //        if (employeeComplaintWorkFlowDto.Id > 0 && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.CreatedByName) && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.LOSName))
                    //        {
                    //            EmployeeComplaintWorkFlowListDto.Add(employeeComplaintWorkFlowDto);
                    //        }
                    //    }
                    //}
                    int ssid = Convert.ToInt32(sid);
                    //empcomplaintworks = db.EmployeeComplaintWorkFlows.Where(x => x.ActionType.ToLower() == Messages.SUBMITTED.ToLower() && x.CreatedBy == ssid && x.IsActive).ToList();

                    //if (empcomplaintworks != null)
                    //{

                        // empcomplaintworks.Id = empcomplaintworks.ComplaintId;
                        //    employeeComplaintWorkFlowDto.CreatedByName = usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy).EmployeeName : string.Empty;
                        //    employeeComplaintWorkFlowDto.CreatedDate = employeeComplaintWorkFlowDto.CreatedDate;
                        //    employeeComplaintWorkFlowDto.LOSName = new LOSMasterRepository().Get(employeeComplaintWorkFlowDto.LOSId).LOSName;
                        //    employeeComplaintWorkFlowDto.ComplaintNo = employeeComplaintWorkFlowDto.ComplaintNo;
                        //    EmployeeCompliantMasterVM employeeCompliantMasterDto = EmployeeComplaintList.Where(x => x.Id == employeeComplaintWorkFlowDto.ComplaintId).FirstOrDefault();
                        //    if (employeeCompliantMasterDto != null && employeeCompliantMasterDto.CategoryId > 0 && employeeCompliantMasterDto.SubCategoryId > 0)
                        //   {
                        //        employeeComplaintWorkFlowDto.Category = categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId) != null ? categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId).CategoryName : Messages.NotAvailable;
                        //        employeeComplaintWorkFlowDto.SubCategory = SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId) != null ? SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                        //    }
                        //   if (employeeComplaintWorkFlowDto.Id > 0 && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.CreatedByName) && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.LOSName))
                        //   {
                        //       EmployeeComplaintWorkFlowListDto.Add(employeeComplaintWorkFlowDto);
                        //    }

                        using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["CMS"].ToString()))
                        {
                            con.Open();
                            var query = $@"select distinct wfl.ComplaintId as Id,usl.EmployeeName as CreatedByName,wfl.CreatedDate,lo.LOSName,wfl.ComplaintNo,(select cats.CategoryName from CategoryMasters cats where emps.CategoryId=cats.Id) as Category, 
                                           (select subcats.SubCategoryName from SubCategoryMasters subcats where emps.SubCategoryId=subcats.Id) as SubCategory 
                                            FROM EmployeeComplaintWorkFlow wfl
                                            left join UserMasters usl on usl.Id= wfl.CreatedBy
                                            left join LOSMasters lo on lo.Id=wfl.LOSId
                                            left join EmployeeComplaintMasters emps on emps.Id=wfl.ComplaintId
                                            where wfl.ActionType = 'SUBMITTED' and  wfl.IsActive =1 and wfl.CreatedBy={ssid}
                                            order by id desc";

                           EmployeeComplaintWorkFlowListDto= con.Query<EmployeeComplaintWorkFlowVM>(query).ToList();

                        }
                    }



                //}
            //}
                //}
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return EmployeeComplaintWorkFlowListDto;
        }

        public List<EmployeeComplaintWorkFlowVM> GetAllDueComplaints()
        {
            List<EmployeeComplaintHistory> EmployeeComplaintHistory = new List<EmployeeComplaintHistory>();
            List<EmployeeComplaintHistoryVM> EmployeeComplaintHistoryList = new List<EmployeeComplaintHistoryVM>();
            List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowList = new EmployeeWorkFlowRepository().GetAll().ToList();
           
            //List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowList = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowListDto = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeCompliantMasterVM> EmployeeComplaintList = new EmployeeComplaintMastersRepository().GetAllList();
            //List<EmployeeCompliantMasterVM> EmployeeComplaintList = new List<EmployeeCompliantMasterVM>();
            List<CategoryMasterVM> categoryMasters = new CategoryMastersRepository().GetAll();
            List<SubCategoryMasterVM> SubcategoryMasters = new SubCategoryMastersRepository().GetAll();
            //List<CategoryMasterVM> categoryMasters = new List<CategoryMasterVM>();
            //List<SubCategoryMasterVM> SubcategoryMasters = new List<SubCategoryMasterVM>();

            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
                    var Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(sid))
                    {
                        List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                        EmployeeComplaintHistory = db.EmployeeComplaintHistories.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                        if (EmployeeComplaintHistory != null && EmployeeComplaintHistory.Count > 0 && usersList != null && usersList.Count > 0)
                        {
                            foreach (EmployeeComplaintWorkFlowVM employeeComplaintWorkFlowDto in EmployeeComplaintWorkFlowList)
                            {
                                if (Role == Messages.Committee)
                                {
                                    if (employeeComplaintWorkFlowDto.ActionType.ToLower() == Messages.Committee.ToLower() && employeeComplaintWorkFlowDto.DueDate >= DateTime.UtcNow)
                                    {
                                        employeeComplaintWorkFlowDto.UserType = employeeComplaintWorkFlowDto.UserType;
                                        employeeComplaintWorkFlowDto.Id = employeeComplaintWorkFlowDto.Id;
                                        employeeComplaintWorkFlowDto.ComplaintId = employeeComplaintWorkFlowDto.ComplaintId;
                                        employeeComplaintWorkFlowDto.CreatedByName = usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy).EmployeeName : string.Empty;
                                        employeeComplaintWorkFlowDto.CreatedDate = employeeComplaintWorkFlowDto.CreatedDate;
                                        employeeComplaintWorkFlowDto.LOSName = new LOSMasterRepository().Get(employeeComplaintWorkFlowDto.LOSId).LOSName;
                                        employeeComplaintWorkFlowDto.ComplaintNo = employeeComplaintWorkFlowDto.ComplaintNo;
                                        EmployeeCompliantMasterVM employeeCompliantMasterDto = EmployeeComplaintList.Where(x => x.Id == employeeComplaintWorkFlowDto.ComplaintId).FirstOrDefault();
                                        if (employeeCompliantMasterDto != null && employeeCompliantMasterDto.CategoryId > 0 && employeeCompliantMasterDto.SubCategoryId > 0)
                                        {
                                            employeeComplaintWorkFlowDto.Category = categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId) != null ? categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId).CategoryName : Messages.NotAvailable;
                                            employeeComplaintWorkFlowDto.SubCategory = SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId) != null ? SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                                        }
                                        if (employeeComplaintWorkFlowDto.Id > 0 && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.CreatedByName) && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.LOSName))
                                        {
                                            EmployeeComplaintWorkFlowListDto.Add(employeeComplaintWorkFlowDto);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(employeeComplaintWorkFlowDto.AssignedUserRoles))
                                    {
                                        var isHrUserAssigned = employeeComplaintWorkFlowDto.AssignedUserRoles.Split(',').Where(i => i.ToString() == sid).Count() > 0;


                                        if (employeeComplaintWorkFlowDto.ActionType.ToLower() == Messages.SUBMITTED.ToLower() && employeeComplaintWorkFlowDto.DueDate >= DateTime.UtcNow && isHrUserAssigned)
                                        {
                                            employeeComplaintWorkFlowDto.UserType = employeeComplaintWorkFlowDto.UserType;
                                            employeeComplaintWorkFlowDto.Id = employeeComplaintWorkFlowDto.Id;
                                            employeeComplaintWorkFlowDto.ComplaintId = employeeComplaintWorkFlowDto.ComplaintId;
                                            employeeComplaintWorkFlowDto.CreatedByName = usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy).EmployeeName : string.Empty;
                                            employeeComplaintWorkFlowDto.CreatedDate = employeeComplaintWorkFlowDto.CreatedDate;
                                            employeeComplaintWorkFlowDto.LOSName = new LOSMasterRepository().Get(employeeComplaintWorkFlowDto.LOSId).LOSName;
                                            employeeComplaintWorkFlowDto.ComplaintNo = employeeComplaintWorkFlowDto.ComplaintNo;
                                            EmployeeCompliantMasterVM employeeCompliantMasterDto = EmployeeComplaintList.Where(x => x.Id == employeeComplaintWorkFlowDto.ComplaintId).FirstOrDefault();
                                            if (employeeCompliantMasterDto != null && employeeCompliantMasterDto.CategoryId > 0 && employeeCompliantMasterDto.SubCategoryId > 0)
                                            {
                                                employeeComplaintWorkFlowDto.Category = categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId) != null ? categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId).CategoryName : Messages.NotAvailable;
                                                employeeComplaintWorkFlowDto.SubCategory = SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId) != null ? SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                                            }
                                            if (employeeComplaintWorkFlowDto.Id > 0 && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.CreatedByName) && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.LOSName))
                                            {
                                                EmployeeComplaintWorkFlowListDto.Add(employeeComplaintWorkFlowDto);
                                            }
                                        }
                                    }
                                }
                            }
                        }




                        //if (Role == Messages.Committee)
                        //{
                        //    using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["CMS"].ToString()))
                        //    {
                        //        con.Open();
                        //        var query = $@"select distinct wfl.ComplaintId as Id,usl.EmployeeName as CreatedByName,wfl.CreatedDate,lo.LOSName,wfl.ComplaintNo,(select cats.CategoryName from CategoryMasters cats where emps.CategoryId=cats.Id) as Category, 
                        //                   (select subcats.SubCategoryName from SubCategoryMasters subcats where emps.SubCategoryId=subcats.Id) as SubCategory 
                        //                    FROM EmployeeComplaintWorkFlow wfl
                        //                   left join UserMasters usl on usl.Id= wfl.CreatedBy
                        //                    left join LOSMasters lo on lo.Id=wfl.LOSId
                        //                    left join EmployeeComplaintMasters emps on emps.Id=wfl.ComplaintId
                        //                    where wfl.ActionType = 'SUBMITTED' and wfl.DueDate >= DateTime.UtcNow
                        //                   order by id desc";

                        //        EmployeeComplaintWorkFlowListDto = con.Query<EmployeeComplaintWorkFlowVM>(query).ToList();

                        //    }
                        //}
                        //else
                        //{
                        //    using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["CMS"].ToString()))
                        //    {
                        //        string[] assignedUsers = db.EmployeeComplaintWorkFlows.Select(s => s.AssignedUserRoles).ToArray();
                        //        foreach (string userId in assignedUsers)
                        //        {
                        //            string[] assignedUsersId = userId.Split(',');
                        //            foreach (string Id in assignedUsersId)
                        //            {
                        //                //var isHrUserAssigned = db.EmployeeComplaintWorkFlows.Select(AssignedUserRoles.Split(',').Where(i => i.ToString() == sid).Count() > 0;


                        //                //if (employeeComplaintWorkFlowDto.ActionType.ToLower() == Messages.SUBMITTED.ToLower() && employeeComplaintWorkFlowDto.DueDate >= DateTime.UtcNow && isHrUserAssigned)
                        //                //{
                        //                //    employeeComplaintWorkFlowDto.UserType = employeeComplaintWorkFlowDto.UserType;
                        //                //    employeeComplaintWorkFlowDto.Id = employeeComplaintWorkFlowDto.Id;
                        //                //    employeeComplaintWorkFlowDto.ComplaintId = employeeComplaintWorkFlowDto.ComplaintId;
                        //                //    employeeComplaintWorkFlowDto.CreatedByName = usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy).EmployeeName : string.Empty;
                        //                //    employeeComplaintWorkFlowDto.CreatedDate = employeeComplaintWorkFlowDto.CreatedDate;
                        //                //    employeeComplaintWorkFlowDto.LOSName = new LOSMasterRepository().Get(employeeComplaintWorkFlowDto.LOSId).LOSName;
                        //                //    employeeComplaintWorkFlowDto.ComplaintNo = employeeComplaintWorkFlowDto.ComplaintNo;
                        //                //    EmployeeCompliantMasterVM employeeCompliantMasterDto = EmployeeComplaintList.Where(x => x.Id == employeeComplaintWorkFlowDto.ComplaintId).FirstOrDefault();
                        //                //    if (employeeCompliantMasterDto != null && employeeCompliantMasterDto.CategoryId > 0 && employeeCompliantMasterDto.SubCategoryId > 0)
                        //                //    {
                        //                //        employeeComplaintWorkFlowDto.Category = categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId) != null ? categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId).CategoryName : Messages.NotAvailable;
                        //                //        employeeComplaintWorkFlowDto.SubCategory = SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId) != null ? SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                        //                //    }
                        //                //    if (employeeComplaintWorkFlowDto.Id > 0 && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.CreatedByName) && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.LOSName))
                        //                //    {
                        //                //        EmployeeComplaintWorkFlowListDto.Add(employeeComplaintWorkFlowDto);
                        //                //    }
                        //                //}
                        //                //if (Convert.ToInt32(Id) == Convert.ToInt32(sid))
                        //                //{

                        //                //    var query = $@"select wfl.ComplaintId as Id,
                        //                //     wfl.AssignedUserRoles
                        //                //     FROM EmployeeComplaintWorkFlow wfl
                        //                //     where wfl.ActionType = 'SUBMITTED'
                        //                //     and  FIND_IN_set(wfl.AssignedUserRoles, '{Id}') != 0 ";

                        //                //    EmployeeComplaintWorkFlowListDto = con.Query<EmployeeComplaintWorkFlowVM>(query).ToList();

                        //                //}


                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
            }

            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
            return EmployeeComplaintWorkFlowListDto;
        }
        public List<EmployeeComplaintWorkFlowVM> GetAllOverDueComplaints()
        {
            List<EmployeeComplaintHistory> EmployeeComplaintHistory = new List<EmployeeComplaintHistory>();
            List<EmployeeComplaintHistoryVM> EmployeeComplaintHistoryList = new List<EmployeeComplaintHistoryVM>();
            List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowList = new EmployeeWorkFlowRepository().GetAll().ToList();
            List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowListDto = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeCompliantMasterVM> EmployeeComplaintList = new EmployeeComplaintMastersRepository().GetAllList();
            List<CategoryMasterVM> categoryMasters = new CategoryMastersRepository().GetAll();
            List<SubCategoryMasterVM> SubcategoryMasters = new SubCategoryMastersRepository().GetAll();

            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
                    var Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(sid))
                    {
                        List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                        EmployeeComplaintHistory = db.EmployeeComplaintHistories.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                        if (EmployeeComplaintHistory != null && EmployeeComplaintHistory.Count > 0 && usersList != null && usersList.Count > 0)
                        {
                            foreach (EmployeeComplaintWorkFlowVM employeeComplaintWorkFlowDto in EmployeeComplaintWorkFlowList)
                            {
                                if (Role == Messages.Committee)
                                {
                                    if (employeeComplaintWorkFlowDto.ActionType.ToLower() == Messages.Committee.ToLower() && employeeComplaintWorkFlowDto.DueDate <= DateTime.UtcNow)
                                    {
                                        employeeComplaintWorkFlowDto.Id = employeeComplaintWorkFlowDto.Id;
                                        employeeComplaintWorkFlowDto.CreatedByName = usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy).EmployeeName : string.Empty;
                                        employeeComplaintWorkFlowDto.CreatedDate = employeeComplaintWorkFlowDto.CreatedDate;
                                        employeeComplaintWorkFlowDto.LOSName = new LOSMasterRepository().Get(employeeComplaintWorkFlowDto.LOSId).LOSName;
                                        employeeComplaintWorkFlowDto.ComplaintNo = employeeComplaintWorkFlowDto.ComplaintNo;
                                        EmployeeCompliantMasterVM employeeCompliantMasterDto = EmployeeComplaintList.Where(x => x.Id == employeeComplaintWorkFlowDto.ComplaintId).FirstOrDefault();
                                        if (employeeCompliantMasterDto != null && employeeCompliantMasterDto.CategoryId > 0 && employeeCompliantMasterDto.SubCategoryId > 0)
                                        {
                                            employeeComplaintWorkFlowDto.Category = categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId) != null ? categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId).CategoryName : Messages.NotAvailable;
                                            employeeComplaintWorkFlowDto.SubCategory = SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId) != null ? SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                                        }
                                        if (employeeComplaintWorkFlowDto.Id > 0 && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.CreatedByName) && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.LOSName))
                                        {
                                            EmployeeComplaintWorkFlowListDto.Add(employeeComplaintWorkFlowDto);
                                        }
                                    }
                                }
                                else
                                {

                                    if (!string.IsNullOrEmpty(employeeComplaintWorkFlowDto.AssignedUserRoles))
                                    {
                                        var isHrUserAssigned = employeeComplaintWorkFlowDto.AssignedUserRoles.Split(',').Where(i => i.ToString() == sid).Count() > 0;

                                        if (employeeComplaintWorkFlowDto.ActionType.ToLower() == Messages.SUBMITTED.ToLower() && employeeComplaintWorkFlowDto.DueDate <= DateTime.UtcNow && isHrUserAssigned)
                                        {
                                            employeeComplaintWorkFlowDto.Id = employeeComplaintWorkFlowDto.Id;
                                            employeeComplaintWorkFlowDto.CreatedByName = usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy).EmployeeName : string.Empty;
                                            employeeComplaintWorkFlowDto.CreatedDate = employeeComplaintWorkFlowDto.CreatedDate;
                                            employeeComplaintWorkFlowDto.LOSName = new LOSMasterRepository().Get(employeeComplaintWorkFlowDto.LOSId).LOSName;
                                            employeeComplaintWorkFlowDto.ComplaintNo = employeeComplaintWorkFlowDto.ComplaintNo;
                                            EmployeeCompliantMasterVM employeeCompliantMasterDto = EmployeeComplaintList.Where(x => x.Id == employeeComplaintWorkFlowDto.ComplaintId).FirstOrDefault();
                                            if (employeeCompliantMasterDto != null && employeeCompliantMasterDto.CategoryId > 0 && employeeCompliantMasterDto.SubCategoryId > 0)
                                            {
                                                employeeComplaintWorkFlowDto.Category = categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId) != null ? categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId).CategoryName : Messages.NotAvailable;
                                                employeeComplaintWorkFlowDto.SubCategory = SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId) != null ? SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                                            }
                                            if (employeeComplaintWorkFlowDto.Id > 0 && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.CreatedByName) && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.LOSName))
                                            {
                                                EmployeeComplaintWorkFlowListDto.Add(employeeComplaintWorkFlowDto);
                                            }
                                        }
                                    }
                                }
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
            return EmployeeComplaintWorkFlowListDto;
        }

        public List<EmployeeComplaintWorkFlowVM> GetAllComplaintsThree()
        {
            List<EmployeeComplaintHistory> EmployeeComplaintHistory = new List<EmployeeComplaintHistory>();
            List<EmployeeComplaintHistoryVM> EmployeeComplaintHistoryList = new List<EmployeeComplaintHistoryVM>();
            List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowList = new EmployeeWorkFlowRepository().GetAll().ToList();
            List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowListDto = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeCompliantMasterVM> EmployeeComplaintList = new EmployeeComplaintMastersRepository().GetAllList();
            List<CategoryMasterVM> categoryMasters = new CategoryMastersRepository().GetAll();
            List<SubCategoryMasterVM> SubcategoryMasters = new SubCategoryMastersRepository().GetAll();

            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
                    var Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(sid))
                    {
                        List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                        List<CommitteeMasterVM> committeesList = new CommitteeMastersRepository().GetAll();
                        EmployeeComplaintHistory = db.EmployeeComplaintHistories.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                        if (EmployeeComplaintHistory != null && EmployeeComplaintHistory.Count > 0 && usersList != null && usersList.Count > 0)
                        {
                            foreach (EmployeeComplaintWorkFlowVM employeeComplaintWorkFlowDto in EmployeeComplaintWorkFlowList)
                            {
                                var IsInCommitteeList = db.CommitteeRoles.Where(a => a.ComplaintId == employeeComplaintWorkFlowDto.ComplaintId).Count() > 0;

                                if (employeeComplaintWorkFlowDto.ActionType.ToLower() == Messages.Committee.ToLower() || IsInCommitteeList)
                                {
                                    employeeComplaintWorkFlowDto.Id = employeeComplaintWorkFlowDto.Id;
                                    employeeComplaintWorkFlowDto.CreatedByName = usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy).EmployeeName : string.Empty;
                                    employeeComplaintWorkFlowDto.CreatedDate = employeeComplaintWorkFlowDto.CreatedDate;
                                    employeeComplaintWorkFlowDto.LOSName = new LOSMasterRepository().Get(employeeComplaintWorkFlowDto.LOSId).LOSName;
                                    employeeComplaintWorkFlowDto.ComplaintNo = employeeComplaintWorkFlowDto.ComplaintNo;
                                    employeeComplaintWorkFlowDto.ActionType = employeeComplaintWorkFlowDto.ActionType;
                                    EmployeeCompliantMasterVM employeeCompliantMasterDto = EmployeeComplaintList.Where(x => x.Id == employeeComplaintWorkFlowDto.ComplaintId).FirstOrDefault();
                                    if (employeeCompliantMasterDto != null && employeeCompliantMasterDto.CategoryId > 0 && employeeCompliantMasterDto.SubCategoryId > 0)
                                    {
                                        employeeComplaintWorkFlowDto.Category = categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId) != null ? categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId).CategoryName : Messages.NotAvailable;
                                        employeeComplaintWorkFlowDto.SubCategory = SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId) != null ? SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                                    }
                                    var CommitteeMemberData = (from u in db.CommitteeMasters
                                                               where u.IsActive
                                                               select u).FirstOrDefault();
                                    string PendingWith = string.Empty;
                                    if (employeeCompliantMasterDto.LastPerformedBy != null && employeeCompliantMasterDto.LastPerformedBy != "")
                                    {

                                        if(employeeComplaintWorkFlowDto.ActionType == Messages.COMMITTEE)
                                        {
                                            PendingWith = Messages.COMMITTEE;
                                        }
                                        else
                                        {
                                            PendingWith = Messages.HRUser;
                                        }

                                        //string[] lastPerformBy = employeeCompliantMasterDto.LastPerformedBy.Split(',');
                                        //foreach (var Ids in lastPerformBy)
                                        //{
                                        //    string UserName = string.Empty;
                                        //    string UserRole = string.Empty;
                                        //    int Id = Convert.ToInt32(Ids);
                                        //    var isCommitteeUserAssigned = CommitteeMemberData.UserId.Split(',').Where(i => i.ToString() == Id.ToString()).Count() > 0;
                                        //    if (isCommitteeUserAssigned && employeeComplaintWorkFlowDto.ActionType == Messages.COMMITTEE)
                                        //    {
                                        //        UserRole = Messages.COMMITTEE;
                                        //    }
                                        //    else
                                        //    {
                                        //        UserRole = new UserMastersRepository().Get(Convert.ToInt32(Ids)).Type;
                                        //    }
                                        //    //UserName = new UserMastersRepository().Get(Convert.ToInt32(Ids)).EmployeeName;// + ", ";
                                        //    PendingWith = UserRole;//+= UserName + "(" + UserRole + ")" + ", ";
                                        //}
                                    }
                                    else
                                    {
                                        PendingWith = Messages.NotAvailable;
                                    }
                                    employeeComplaintWorkFlowDto.LastPerformedBy = PendingWith;
                                    if (employeeComplaintWorkFlowDto.Id > 0 && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.CreatedByName) && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.LOSName))
                                    {
                                        EmployeeComplaintWorkFlowListDto.Add(employeeComplaintWorkFlowDto);
                                    }

                                }
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
            return EmployeeComplaintWorkFlowListDto;
        }
        public List<EmployeeComplaintWorkFlowVM> GetAllComplaintsTwo()
        {
            List<EmployeeComplaintHistory> EmployeeComplaintHistory = new List<EmployeeComplaintHistory>();
            List<EmployeeComplaintHistoryVM> EmployeeComplaintHistoryList = new List<EmployeeComplaintHistoryVM>();
            List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowList = new EmployeeWorkFlowRepository().GetAll().ToList();
            List<EmployeeComplaintWorkFlowVM> EmployeeComplaintWorkFlowListDto = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeCompliantMasterVM> EmployeeComplaintList = new EmployeeComplaintMastersRepository().GetAllList();
            List<CategoryMasterVM> categoryMasters = new CategoryMastersRepository().GetAll();
            List<SubCategoryMasterVM> SubcategoryMasters = new SubCategoryMastersRepository().GetAll();

            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

                using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
                {
                    var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                               .Select(c => c.Value).SingleOrDefault();
                    var Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(sid))
                    {
                        List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                        EmployeeComplaintHistory = db.EmployeeComplaintHistories.Where(i => i.IsActive).ToList().OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
                        if (EmployeeComplaintHistory != null && EmployeeComplaintHistory.Count > 0 && usersList != null && usersList.Count > 0)
                        {
                            foreach (EmployeeComplaintWorkFlowVM employeeComplaintWorkFlowDto in EmployeeComplaintWorkFlowList)
                            {
                                if (!string.IsNullOrEmpty(employeeComplaintWorkFlowDto.AssignedUserRoles))
                                {
                                    var isHrUserAssigned = employeeComplaintWorkFlowDto.AssignedUserRoles.Split(',').Where(i => i.ToString() == sid).Count() > 0;


                                    //if (employeeComplaintWorkFlowDto.ActionType.ToLower() == Messages.SUBMITTED.ToLower() && employeeComplaintWorkFlowDto.DueDate <= DateTime.UtcNow && isHrUserAssigned)
                                    if (employeeComplaintWorkFlowDto.ActionType.ToLower() != Messages.Opened.ToLower() && isHrUserAssigned)
                                    {
                                        employeeComplaintWorkFlowDto.Id = employeeComplaintWorkFlowDto.Id;
                                        employeeComplaintWorkFlowDto.ComplaintId = employeeComplaintWorkFlowDto.ComplaintId;
                                        employeeComplaintWorkFlowDto.CreatedByName = usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == employeeComplaintWorkFlowDto.CreatedBy).EmployeeName : string.Empty;
                                        employeeComplaintWorkFlowDto.CreatedDate = employeeComplaintWorkFlowDto.CreatedDate;
                                        employeeComplaintWorkFlowDto.LOSName = new LOSMasterRepository().Get(employeeComplaintWorkFlowDto.LOSId).LOSName;
                                        employeeComplaintWorkFlowDto.ComplaintNo = employeeComplaintWorkFlowDto.ComplaintNo;
                                        employeeComplaintWorkFlowDto.ActionType = employeeComplaintWorkFlowDto.ActionType;



                                        EmployeeCompliantMasterVM employeeCompliantMasterDto = EmployeeComplaintList.Where(x => x.Id == employeeComplaintWorkFlowDto.ComplaintId).FirstOrDefault();

                                        if (employeeCompliantMasterDto != null && employeeCompliantMasterDto.CategoryId > 0 && employeeCompliantMasterDto.SubCategoryId > 0)
                                        {
                                            employeeComplaintWorkFlowDto.Category = categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId) != null ? categoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.CategoryId).CategoryName : Messages.NotAvailable;
                                            employeeComplaintWorkFlowDto.SubCategory = SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId) != null ? SubcategoryMasters.FirstOrDefault(x => x.Id == employeeCompliantMasterDto.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                                        }
                                        var CommitteeMemberData = (from u in db.CommitteeMasters
                                                                   where u.IsActive
                                                                   select u).FirstOrDefault();
                                        string PendingWith = string.Empty;
                                        if (employeeCompliantMasterDto.LastPerformedBy != null && employeeCompliantMasterDto.LastPerformedBy != "")
                                        {
                                            if (employeeComplaintWorkFlowDto.ActionType == Messages.COMMITTEE)
                                            {
                                                PendingWith = Messages.COMMITTEE;
                                            }
                                            else
                                            {
                                                PendingWith = Messages.HRUser;
                                            }

                                            //string[] lastPerformBy = employeeCompliantMasterDto.LastPerformedBy.Split(',');
                                            //foreach (var Ids in lastPerformBy)
                                            //{
                                            //    string UserName = string.Empty;
                                            //    string UserRole = string.Empty;
                                            //    int Id = Convert.ToInt32(Ids);
                                            //    var isCommitteeUserAssigned = CommitteeMemberData.UserId.Split(',').Where(i => i.ToString() == Id.ToString()).Count() > 0;
                                            //    if (isCommitteeUserAssigned)
                                            //    {
                                            //        UserRole = Messages.COMMITTEE;
                                            //    }
                                            //    else
                                            //    {
                                            //        UserRole = new UserMastersRepository().Get(Convert.ToInt32(Ids)).Type;
                                            //    }
                                            //    //UserName = new UserMastersRepository().Get(Convert.ToInt32(Ids)).EmployeeName;// + ", ";
                                            //    PendingWith = UserRole;//+= UserName + "(" + UserRole + ")" + ", ";
                                            //}
                                        }
                                        else
                                        {
                                            PendingWith = Messages.NotAvailable;
                                        }
                                        employeeComplaintWorkFlowDto.LastPerformedBy = PendingWith;
                                        if (employeeComplaintWorkFlowDto.Id > 0 && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.CreatedByName) && !string.IsNullOrEmpty(employeeComplaintWorkFlowDto.LOSName))
                                        {
                                            EmployeeComplaintWorkFlowListDto.Add(employeeComplaintWorkFlowDto);
                                        }
                                    }
                                }
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
            return EmployeeComplaintWorkFlowListDto;
        }

        public void AddEmailHistory(string EmailFrom, string EmailTo, int? complaintId, string ErrorDescription,string status,string ComplaintStatus)
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                string usertypeValue = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    EmailTrack emailHistory = new EmailTrack();
                    emailHistory.EmailFrom = EmailFrom;
                    emailHistory.EmailTo = EmailTo;
                    emailHistory.CreatedBy = Convert.ToInt32(sid);
                    emailHistory.ErrorDescription = ErrorDescription;
                    emailHistory.ComplaintId = complaintId;
                    emailHistory.CreateDate = DateTime.UtcNow;
                    emailHistory.Status = status;
                    emailHistory.ComplaintStatus = ComplaintStatus;
                    db.EmailTracks.Add(emailHistory);
                    db.SaveChanges();
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


        public void ErrorLogHistory(int? complaintId, string ErrorDescription, string status)
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                string usertypeValue = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();
                var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
                if (!string.IsNullOrEmpty(sid))
                {
                    ErrorLogHistoryEntry errorHistory = new ErrorLogHistoryEntry();
                    errorHistory.CreatedBy = Convert.ToInt32(sid);
                    errorHistory.ErrorDescription = ErrorDescription;
                    errorHistory.ComplaintId = complaintId;
                    errorHistory.CreateDate = DateTime.UtcNow;
                    errorHistory.SreenName = status;
                    db.ErrorLogHistoryEntries.Add(errorHistory);
                    db.SaveChanges();
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
        
    }
}