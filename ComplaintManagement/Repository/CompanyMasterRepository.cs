    using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComplaintManagement.Repository
{
    public class CompanyMasterRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();


        public CompanyMasterRepository()
        {
                    
        }
        #region To get All company data
        public List<CompanyVM> GetAll()
        {
            List<CompanyMaster> company = new List<CompanyMaster>();
            List<CompanyVM> companyList = new List<CompanyVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                company = db.CompanyMasters.OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();
            
                if (company != null && company.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (CompanyMaster item in company)
                    {
                        CompanyVM comObj = Mapper.Map<CompanyMaster, CompanyVM>(item);
                        if (comObj != null)
                        {
                            comObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == comObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == comObj.CreatedBy).EmployeeName : string.Empty;
                            comObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == comObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == comObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            companyList.Add(comObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return companyList;
        } 
        #endregion

    }
}