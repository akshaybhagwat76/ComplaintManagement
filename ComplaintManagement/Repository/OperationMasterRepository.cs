using AutoMapper;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using ComplaintManagement.ViewModel;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComplaintManagement.Repository
{
    public class OperationMasterRepository
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();

        public OperationMasterRepository()
        {
                    
        }
        #region To get All operation data
        public List<OperationMasterVM> GetAll()
        {
            List<OperationMaster> operation = new List<OperationMaster>();
            List<OperationMasterVM> operationList = new List<OperationMasterVM>();
            try
            {
                List<UserMasterVM> usersList = new UserMastersRepository().GetAll();
                operation = db.OperationMasters.OrderByDescending(x => x.CreatedDate).OrderByDescending(x => x.Id).ToList();


                if (operation != null && operation.Count > 0 && usersList != null && usersList.Count > 0)
                {
                    foreach (OperationMaster item in operation)
                    {
                        OperationMasterVM opeObj = Mapper.Map<OperationMaster, OperationMasterVM>(item);
                        if (opeObj != null)
                        {
                            opeObj.CreatedByName = usersList.FirstOrDefault(x => x.Id == opeObj.CreatedBy) != null ? usersList.FirstOrDefault(x => x.Id == opeObj.CreatedBy).EmployeeName : string.Empty;
                            opeObj.UpdatedByName = usersList.FirstOrDefault(x => x.Id == opeObj.ModifiedBy) != null ? usersList.FirstOrDefault(x => x.Id == opeObj.ModifiedBy).EmployeeName : Messages.NotAvailable;
                            operationList.Add(opeObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (HttpContext.Current != null) ErrorSignal.FromCurrentContext().Raise(ex);
                throw new Exception(ex.Message.ToString());
            }
            return operationList;
        }
        #endregion
    }
}