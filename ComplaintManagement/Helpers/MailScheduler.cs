using AutoMapper;
using ComplaintManagement.Models;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ComplaintManagement.Helpers
{
    public class MailScheduler
    {

        //1/4/2021
        public List<EmployeeComplaintWorkFlow> GetAllMailData()
        {


            List<EmployeeComplaintWorkFlow> emp1 = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> empds = new List<EmployeeComplaintWorkFlow>();
            EmployeeComplaintWorkFlowVM em = new EmployeeComplaintWorkFlowVM();
            List<UserMaster> User = new List<UserMaster>();
            List<EmployeeComplaintWorkFlowVM> vms = new List<EmployeeComplaintWorkFlowVM>();
            List<EmployeeComplaintWorkFlowVM> EmailsAttachement = new List<EmployeeComplaintWorkFlowVM>();
            using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
            {
                try
                {

                    emp1 = db.EmployeeComplaintWorkFlows.Where(c => !db.HR_Role.Select(b => b.ComplentId).Contains(c.ComplaintId)).ToList();




                    foreach (var item in emp1)
                    {
                        DateTime createdate = item.CreatedDate.AddDays(+2);
                        empds.AddRange(db.EmployeeComplaintWorkFlows.Where(c => c.Id == item.Id && c.IsActive && c.ActionType != "Withdrawn" && createdate < DateTime.Now && c.DueDate > DateTime.Now).ToList());


                    }
                    foreach (var item in empds)
                    {
                        EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(item);

                        catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == item.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == item.CreatedBy).EmployeeName : string.Empty;
                        catObj.ComplaintNo = item.ComplaintNo;
                        catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == item.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == item.LOSId).LOSName : string.Empty;
                        catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == item.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == item.SBUId).SBU : string.Empty;
                        catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == item.SubSBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == item.SubSBUId).SBU : string.Empty;

                        catObj.AssignedUserRoles = item.AssignedUserRoles;
                        catObj.Id = item.Id;
                        catObj.IsActive = item.IsActive;
                        catObj.ActionType = item.ActionType;
                        catObj.DueDate = item.DueDate;
                        catObj.CreatedDate = item.CreatedDate;
                        catObj.CreatedBy = item.CreatedBy;
                        catObj.ActionType = item.ActionType;
                        int UserId = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).UserId;
                        catObj.EmployeeName = db.UserMasters.FirstOrDefault(x => x.Id == UserId) != null ? db.UserMasters.FirstOrDefault(x => x.Id == UserId).EmployeeName : string.Empty;
                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).CategoryId;
                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).SubCategoryId;
                        if (categoryid != 0)
                        {
                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : string.Empty;
                        }
                        else
                        {
                            catObj.Category = "";
                        }
                        if (subcategoryid != 0)
                        {
                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : string.Empty;

                        }
                        else
                        {
                            catObj.SubCategory = "";
                        }

                        string lastperformed = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId) != null ? db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).LastPerformedBy : string.Empty;
                        var CommitteeMemberData = (from u in db.CommitteeMasters
                                                   where u.IsActive
                                                   select u).FirstOrDefault();
                        string PendingWith = string.Empty;

                        if (lastperformed != null && lastperformed != "")
                        {
                            string[] lastPerformBy = lastperformed.Split(',');
                            foreach (var Ids in lastPerformBy)
                            {
                                string UserName = string.Empty;
                                string UserRole = string.Empty;
                                int Id = Convert.ToInt32(Ids);
                                var isCommitteeUserAssigned = CommitteeMemberData.UserId.Split(',').Where(i => i.ToString() == Id.ToString()).Count() > 0;
                                if (isCommitteeUserAssigned)
                                {
                                    UserRole = Messages.COMMITTEE;
                                }
                                else
                                {
                                    UserRole = new UserMastersRepository().Get(Convert.ToInt32(Ids)).Type;
                                }
                                UserName = new UserMastersRepository().Get(Convert.ToInt32(Ids)).EmployeeName;// + ", ";
                                PendingWith += UserName + "(" + UserRole + ")" + ", ";
                            }
                        }
                        else
                        {
                            PendingWith = Messages.NotAvailable;
                        }

                        catObj.LastPerformedBy = PendingWith;


                        EmailsAttachement.Add(catObj);
                        //vms.Category= new CategoryMastersRepository().Get(item.CategoryId) != null ? new CategoryMastersRepository().Get(item.CategoryId).CategoryName : Messages.NotAvailable;
                        //vms.SubCategory = new SubCategoryMastersRepository().Get(item.SubCategoryId) != null ? new SubCategoryMastersRepository().Get(item.SubCategoryId).SubCategoryName : Messages.NotAvailable;
                        //vms.
                        //catObj.EmployeeName = usersList.FirstOrDefault(x => x.Id == catObj.UserId) != null ? usersList.FirstOrDefault(x => x.Id == catObj.UserId).EmployeeName : Messages.NotAvailable;

                    }




                    IDictionary<string, List<EmployeeComplaintWorkFlowVM>> dic = new Dictionary<string, List<EmployeeComplaintWorkFlowVM>>();

                    foreach (var items in EmailsAttachement)
                    {

                        if (items.AssignedUserRoles != null)
                        {
                            foreach (var item in items.AssignedUserRoles.Split(','))
                            {


                                List<EmployeeComplaintWorkFlowVM> value;
                                if (dic.TryGetValue(item, out value))
                                {
                                    value.Add(items);
                                    dic[item] = value;
                                }
                                else
                                {
                                    dic.Add(item, new List<EmployeeComplaintWorkFlowVM> { items });
                                }

                            }
                        }
                    }
                    foreach (var item in dic)
                    {
                        int userid = Convert.ToInt32(item.Key);
                        var Hrtype = db.UserMasters.FirstOrDefault(x => x.Id == userid && x.IsActive);

                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage package = new ExcelPackage();


                        var ws = package.Workbook.Worksheets.Add(Messages.ComplaintList);
                        //Headers
                        ws.Cells["A1"].Value = Messages.EmployeeName;
                        ws.Cells["B1"].Value = Messages.Category;
                        ws.Cells["C1"].Value = Messages.SubCategory;
                        ws.Cells["D1"].Value = Messages.CreatedDate;
                        ws.Cells["E1"].Value = Messages.CreatedBy;
                        ws.Cells["F1"].Value = Messages.PendingWith;
                        ws.Cells["G1"].Value = Messages.Remark;
                        ws.Cells["H1"].Value = Messages.Status;
                        ws.Cells["I1"].Value = Messages.ComplaintNo;



                        var rowNumber = 1;
                        ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 1].Value = Messages.EmployeeName;

                        ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 2].Value = Messages.Category;

                        ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 3].Value = Messages.SubCategory;

                        ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 4].Value = Messages.CreatedDate;

                        ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 5].Value = Messages.CreatedBy;

                        ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 6].Value = Messages.PendingWith;

                        ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 7].Value = Messages.Remark;

                        ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 8].Value = Messages.Status;

                        ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 9].Value = Messages.ComplaintNo;



                        foreach (var log in item.Value)
                        {
                            rowNumber++;
                            string createdate = log.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                            ws.Cells[rowNumber, 1].Value = log.EmployeeName;
                            ws.Cells[rowNumber, 2].Value = log.Category;
                            ws.Cells[rowNumber, 3].Value = log.SubCategory;
                            ws.Cells[rowNumber, 4].Value = createdate;
                            ws.Cells[rowNumber, 5].Value = log.CreatedByName;
                            ws.Cells[rowNumber, 6].Value = log.LastPerformedBy;
                            ws.Cells[rowNumber, 7].Value = log.Remarks;
                            ws.Cells[rowNumber, 8].Value = log.ActionType;
                            ws.Cells[rowNumber, 9].Value = log.ComplaintNo;

                        }


                        var stream = new MemoryStream();
                        package.SaveAs(stream);

                        byte[] excelData = stream.ToArray();

                        string NotificationContent = "Dear " + Hrtype.EmployeeName + "," + "Please find due complaints in the attachment for your approval. Please take urgent actions.";

                        MailSend.SendEmailJobDue(Hrtype.WorkEmail, "Complaint", NotificationContent, excelData);


                    }







                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
            return empds;

        }

        public List<EmployeeComplaintWorkFlow> GetAllMailOverdueData()
        {


            List<EmployeeComplaintWorkFlow> emp1 = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> empds = new List<EmployeeComplaintWorkFlow>();
            List<UserMaster> User = new List<UserMaster>();
            List<EmployeeComplaintWorkFlowVM> EmailsAttachement = new List<EmployeeComplaintWorkFlowVM>();
            using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
            {
                try
                {

                    emp1 = db.EmployeeComplaintWorkFlows.Where(c => !db.HR_Role.Select(b => b.ComplentId).Contains(c.ComplaintId)).ToList();

                    foreach (var item in emp1)
                    {
                        DateTime duedate = item.DueDate.AddDays(+3);

                        empds.AddRange(db.EmployeeComplaintWorkFlows.Where(c => c.Id == item.Id && c.IsActive && c.ActionType != "Withdrawn" && c.ActionType != "Completed" && c.DueDate < DateTime.Now && duedate >= DateTime.Now));
                    }

                    //1/4/2021
                    foreach (var item in empds)
                    {
                        EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(item);

                        catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == item.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == item.CreatedBy).EmployeeName : string.Empty;
                        catObj.ComplaintNo = item.ComplaintNo;
                        catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == item.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == item.LOSId).LOSName : string.Empty;
                        catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == item.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == item.SBUId).SBU : string.Empty;
                        catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == item.SubSBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == item.SubSBUId).SBU : string.Empty;

                        catObj.AssignedUserRoles = item.AssignedUserRoles;
                        catObj.Id = item.Id;
                        catObj.IsActive = item.IsActive;
                        catObj.ActionType = item.ActionType;
                        catObj.DueDate = item.DueDate;
                        catObj.CreatedDate = item.CreatedDate;
                        catObj.CreatedBy = item.CreatedBy;
                        catObj.ActionType = item.ActionType;
                        int UserId = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).UserId;
                        catObj.EmployeeName = db.UserMasters.FirstOrDefault(x => x.Id == UserId) != null ? db.UserMasters.FirstOrDefault(x => x.Id == UserId).EmployeeName : string.Empty;
                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).CategoryId;
                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).SubCategoryId;
                        if (categoryid != 0)
                        {
                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : string.Empty;
                        }
                        else
                        {
                            catObj.Category = "";
                        }
                        if (subcategoryid != 0)
                        {
                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : string.Empty;

                        }
                        else
                        {
                            catObj.SubCategory = "";
                        }

                        string lastperformed = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId) != null ? db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).LastPerformedBy : string.Empty;
                        var CommitteeMemberData = (from u in db.CommitteeMasters
                                                   where u.IsActive
                                                   select u).FirstOrDefault();
                        string PendingWith = string.Empty;

                        if (lastperformed != null && lastperformed != "")
                        {
                            string[] lastPerformBy = lastperformed.Split(',');
                            foreach (var Ids in lastPerformBy)
                            {
                                string UserName = string.Empty;
                                string UserRole = string.Empty;
                                int Id = Convert.ToInt32(Ids);
                                var isCommitteeUserAssigned = CommitteeMemberData.UserId.Split(',').Where(i => i.ToString() == Id.ToString()).Count() > 0;
                                if (isCommitteeUserAssigned)
                                {
                                    UserRole = Messages.COMMITTEE;
                                }
                                else
                                {
                                    UserRole = new UserMastersRepository().Get(Convert.ToInt32(Ids)).Type;
                                }
                                UserName = new UserMastersRepository().Get(Convert.ToInt32(Ids)).EmployeeName;// + ", ";
                                PendingWith += UserName + "(" + UserRole + ")" + ", ";
                            }
                        }
                        else
                        {
                            PendingWith = Messages.NotAvailable;
                        }

                        catObj.LastPerformedBy = PendingWith;


                        EmailsAttachement.Add(catObj);

                    }




                    IDictionary<string, List<EmployeeComplaintWorkFlowVM>> dic = new Dictionary<string, List<EmployeeComplaintWorkFlowVM>>();

                    foreach (var items in EmailsAttachement)
                    {

                        if (items.AssignedUserRoles != null)
                        {
                            foreach (var item in items.AssignedUserRoles.Split(','))
                            {


                                List<EmployeeComplaintWorkFlowVM> value;
                                if (dic.TryGetValue(item, out value))
                                {
                                    value.Add(items);
                                    dic[item] = value;
                                }
                                else
                                {
                                    dic.Add(item, new List<EmployeeComplaintWorkFlowVM> { items });
                                }

                            }
                        }
                    }
                    foreach (var item in dic)
                    {
                        int userid = Convert.ToInt32(item.Key);
                        var Hrtype = db.UserMasters.FirstOrDefault(x => x.Id == userid && x.IsActive);

                        string ManagerEmail = db.UserMasters.FirstOrDefault(x => x.Manager == Hrtype.Manager) != null ? db.UserMasters.FirstOrDefault(x => x.Manager == Hrtype.Manager).WorkEmail : string.Empty;
                        List<string> cc = new List<string> { ManagerEmail };
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage package = new ExcelPackage();


                        var ws = package.Workbook.Worksheets.Add(Messages.ComplaintList);
                        //Headers
                        ws.Cells["A1"].Value = Messages.EmployeeName;
                        ws.Cells["B1"].Value = Messages.Category;
                        ws.Cells["C1"].Value = Messages.SubCategory;
                        ws.Cells["D1"].Value = Messages.CreatedDate;
                        ws.Cells["E1"].Value = Messages.CreatedBy;
                        ws.Cells["F1"].Value = Messages.PendingWith;
                        ws.Cells["G1"].Value = Messages.Remark;
                        ws.Cells["H1"].Value = Messages.Status;
                        ws.Cells["I1"].Value = Messages.ComplaintNo;



                        var rowNumber = 1;
                        ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 1].Value = Messages.EmployeeName;

                        ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 2].Value = Messages.Category;

                        ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 3].Value = Messages.SubCategory;

                        ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 4].Value = Messages.CreatedDate;

                        ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 5].Value = Messages.CreatedBy;

                        ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 6].Value = Messages.PendingWith;

                        ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 7].Value = Messages.Remark;

                        ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 8].Value = Messages.Status;

                        ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 9].Value = Messages.ComplaintNo;



                        foreach (var log in item.Value)
                        {
                            rowNumber++;
                            string createdate = log.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                            ws.Cells[rowNumber, 1].Value = log.EmployeeName;
                            ws.Cells[rowNumber, 2].Value = log.Category;
                            ws.Cells[rowNumber, 3].Value = log.SubCategory;
                            ws.Cells[rowNumber, 4].Value = createdate;
                            ws.Cells[rowNumber, 5].Value = log.CreatedByName;
                            ws.Cells[rowNumber, 6].Value = log.LastPerformedBy;
                            ws.Cells[rowNumber, 7].Value = log.Remarks;
                            ws.Cells[rowNumber, 8].Value = log.ActionType;
                            ws.Cells[rowNumber, 9].Value = log.ComplaintNo;

                        }


                        var stream = new MemoryStream();
                        package.SaveAs(stream);

                        byte[] excelData = stream.ToArray();

                        string NotificationContent = "Dear " + Hrtype.EmployeeName + "," + "Please find overdue complaints in the attachment for your approval. Please take urgent actions.";
                        MailSend.SendEmailJobDue(Hrtype.WorkEmail, "Complaint", NotificationContent, excelData, cc);


                    }






                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
            return empds;

        }
        //Esclation Alert(1)
        public List<EmployeeComplaintWorkFlow> GetAllMailEsclationOneOverdueData()
        {


            List<EmployeeComplaintWorkFlow> emp1 = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> empds = new List<EmployeeComplaintWorkFlow>();
            List<UserMaster> User = new List<UserMaster>();
            List<EmployeeComplaintWorkFlowVM> EmailsAttachement = new List<EmployeeComplaintWorkFlowVM>();
            using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
            {
                try
                {

                    emp1 = db.EmployeeComplaintWorkFlows.Where(c => !db.HR_Role.Select(b => b.ComplentId).Contains(c.ComplaintId)).ToList();

                    foreach (var item in emp1)
                    {
                        DateTime duedate = item.DueDate.AddDays(+4);


                        empds.AddRange(db.EmployeeComplaintWorkFlows.Where(c => c.Id == item.Id && c.IsActive && c.ActionType != "Withdrawn" && c.ActionType != "Completed" && duedate <= DateTime.Now && duedate >= DateTime.Now));
                    }
                    //1/4/2021
                    foreach (var item in empds)
                    {
                        EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(item);

                        catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == item.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == item.CreatedBy).EmployeeName : string.Empty;
                        catObj.ComplaintNo = item.ComplaintNo;
                        catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == item.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == item.LOSId).LOSName : string.Empty;
                        catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == item.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == item.SBUId).SBU : string.Empty;
                        catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == item.SubSBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == item.SubSBUId).SBU : string.Empty;

                        catObj.AssignedUserRoles = item.AssignedUserRoles;
                        catObj.Id = item.Id;
                        catObj.IsActive = item.IsActive;
                        catObj.ActionType = item.ActionType;
                        catObj.DueDate = item.DueDate;
                        catObj.CreatedDate = item.CreatedDate;
                        catObj.CreatedBy = item.CreatedBy;
                        catObj.ActionType = item.ActionType;
                        int UserId = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).UserId;
                        catObj.EmployeeName = db.UserMasters.FirstOrDefault(x => x.Id == UserId) != null ? db.UserMasters.FirstOrDefault(x => x.Id == UserId).EmployeeName : string.Empty;
                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).CategoryId;
                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).SubCategoryId;
                        if (categoryid != 0)
                        {
                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : string.Empty;
                        }
                        else
                        {
                            catObj.Category = "";
                        }
                        if (subcategoryid != 0)
                        {
                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : string.Empty;

                        }
                        else
                        {
                            catObj.SubCategory = "";
                        }

                        string lastperformed = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId) != null ? db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).LastPerformedBy : string.Empty;
                        var CommitteeMemberData = (from u in db.CommitteeMasters
                                                   where u.IsActive
                                                   select u).FirstOrDefault();
                        string PendingWith = string.Empty;

                        if (lastperformed != null && lastperformed != "")
                        {
                            string[] lastPerformBy = lastperformed.Split(',');
                            foreach (var Ids in lastPerformBy)
                            {
                                string UserName = string.Empty;
                                string UserRole = string.Empty;
                                int Id = Convert.ToInt32(Ids);
                                var isCommitteeUserAssigned = CommitteeMemberData.UserId.Split(',').Where(i => i.ToString() == Id.ToString()).Count() > 0;
                                if (isCommitteeUserAssigned)
                                {
                                    UserRole = Messages.COMMITTEE;
                                }
                                else
                                {
                                    UserRole = new UserMastersRepository().Get(Convert.ToInt32(Ids)).Type;
                                }
                                UserName = new UserMastersRepository().Get(Convert.ToInt32(Ids)).EmployeeName;// + ", ";
                                PendingWith += UserName + "(" + UserRole + ")" + ", ";
                            }
                        }
                        else
                        {
                            PendingWith = Messages.NotAvailable;
                        }

                        catObj.LastPerformedBy = PendingWith;


                        EmailsAttachement.Add(catObj);

                    }




                    IDictionary<string, List<EmployeeComplaintWorkFlowVM>> dic = new Dictionary<string, List<EmployeeComplaintWorkFlowVM>>();

                    foreach (var items in EmailsAttachement)
                    {

                        if (items.AssignedUserRoles != null)
                        {
                            foreach (var item in items.AssignedUserRoles.Split(','))
                            {


                                List<EmployeeComplaintWorkFlowVM> value;
                                if (dic.TryGetValue(item, out value))
                                {
                                    value.Add(items);
                                    dic[item] = value;
                                }
                                else
                                {
                                    dic.Add(item, new List<EmployeeComplaintWorkFlowVM> { items });
                                }

                            }
                        }
                    }
                    foreach (var item in dic)
                    {
                        int userid = Convert.ToInt32(item.Key);
                        var Hrtype = db.UserMasters.FirstOrDefault(x => x.Id == userid && x.IsActive);

                        string ManagerEmail = db.UserMasters.FirstOrDefault(x => x.Manager == Hrtype.Manager) != null ? db.UserMasters.FirstOrDefault(x => x.Manager == Hrtype.Manager).WorkEmail : string.Empty;
                        List<string> cc = new List<string> { ManagerEmail };
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage package = new ExcelPackage();


                        var ws = package.Workbook.Worksheets.Add(Messages.ComplaintList);
                        //Headers
                        ws.Cells["A1"].Value = Messages.EmployeeName;
                        ws.Cells["B1"].Value = Messages.Category;
                        ws.Cells["C1"].Value = Messages.SubCategory;
                        ws.Cells["D1"].Value = Messages.CreatedDate;
                        ws.Cells["E1"].Value = Messages.CreatedBy;
                        ws.Cells["F1"].Value = Messages.PendingWith;
                        ws.Cells["G1"].Value = Messages.Remark;
                        ws.Cells["H1"].Value = Messages.Status;
                        ws.Cells["I1"].Value = Messages.ComplaintNo;



                        var rowNumber = 1;
                        ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 1].Value = Messages.EmployeeName;

                        ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 2].Value = Messages.Category;

                        ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 3].Value = Messages.SubCategory;

                        ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 4].Value = Messages.CreatedDate;

                        ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 5].Value = Messages.CreatedBy;

                        ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 6].Value = Messages.PendingWith;

                        ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 7].Value = Messages.Remark;

                        ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 8].Value = Messages.Status;

                        ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 9].Value = Messages.ComplaintNo;



                        foreach (var log in item.Value)
                        {
                            rowNumber++;
                            string createdate = log.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                            ws.Cells[rowNumber, 1].Value = log.EmployeeName;
                            ws.Cells[rowNumber, 2].Value = log.Category;
                            ws.Cells[rowNumber, 3].Value = log.SubCategory;
                            ws.Cells[rowNumber, 4].Value = createdate;
                            ws.Cells[rowNumber, 5].Value = log.CreatedByName;
                            ws.Cells[rowNumber, 6].Value = log.LastPerformedBy;
                            ws.Cells[rowNumber, 7].Value = log.Remarks;
                            ws.Cells[rowNumber, 8].Value = log.ActionType;
                            ws.Cells[rowNumber, 9].Value = log.ComplaintNo;

                        }


                        var stream = new MemoryStream();
                        package.SaveAs(stream);

                        byte[] excelData = stream.ToArray();

                        string NotificationContent = "Dear " + Hrtype.EmployeeName + "," + "Please find overdue complaints in the attachment for your approval. Please take urgent actions.";
                        MailSend.SendEmailJobDue(Hrtype.WorkEmail, "Complaint", NotificationContent, excelData, cc);


                    }



                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
            return empds;

        }
        //Esclation Alert(2)
        public List<EmployeeComplaintWorkFlow> GetAllMailEsclationTwoOverdueData()
        {


            List<EmployeeComplaintWorkFlow> emp1 = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> empds = new List<EmployeeComplaintWorkFlow>();
            List<UserMaster> User = new List<UserMaster>();
            List<EmployeeComplaintWorkFlowVM> EmailsAttachement = new List<EmployeeComplaintWorkFlowVM>();
            using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
            {
                try
                {

                    emp1 = db.EmployeeComplaintWorkFlows.Where(c => !db.HR_Role.Select(b => b.ComplentId).Contains(c.ComplaintId)).ToList();

                    foreach (var item in emp1)
                    {
                        DateTime duedate = item.DueDate.AddDays(+7);

                        empds.AddRange(db.EmployeeComplaintWorkFlows.Where(c => c.Id == item.Id && c.IsActive && c.ActionType != "Withdrawn" && c.ActionType != "Completed" && duedate <= DateTime.Now && duedate >= DateTime.Now));
                    }
                    //1/4/2021
                    foreach (var item in empds)
                    {
                        EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(item);

                        catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == item.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == item.CreatedBy).EmployeeName : string.Empty;
                        catObj.ComplaintNo = item.ComplaintNo;
                        catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == item.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == item.LOSId).LOSName : string.Empty;
                        catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == item.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == item.SBUId).SBU : string.Empty;
                        catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == item.SubSBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == item.SubSBUId).SBU : string.Empty;

                        catObj.AssignedUserRoles = item.AssignedUserRoles;
                        catObj.Id = item.Id;
                        catObj.IsActive = item.IsActive;
                        catObj.ActionType = item.ActionType;
                        catObj.DueDate = item.DueDate;
                        catObj.CreatedDate = item.CreatedDate;
                        catObj.CreatedBy = item.CreatedBy;
                        catObj.ActionType = item.ActionType;
                        int UserId = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).UserId;
                        catObj.EmployeeName = db.UserMasters.FirstOrDefault(x => x.Id == UserId) != null ? db.UserMasters.FirstOrDefault(x => x.Id == UserId).EmployeeName : string.Empty;
                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).CategoryId;
                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).SubCategoryId;
                        if (categoryid != 0)
                        {
                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : string.Empty;
                        }
                        else
                        {
                            catObj.Category = "";
                        }
                        if (subcategoryid != 0)
                        {
                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : string.Empty;

                        }
                        else
                        {
                            catObj.SubCategory = "";
                        }

                        string lastperformed = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId) != null ? db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).LastPerformedBy : string.Empty;
                        var CommitteeMemberData = (from u in db.CommitteeMasters
                                                   where u.IsActive
                                                   select u).FirstOrDefault();
                        string PendingWith = string.Empty;

                        if (lastperformed != null && lastperformed != "")
                        {
                            string[] lastPerformBy = lastperformed.Split(',');
                            foreach (var Ids in lastPerformBy)
                            {
                                string UserName = string.Empty;
                                string UserRole = string.Empty;
                                int Id = Convert.ToInt32(Ids);
                                var isCommitteeUserAssigned = CommitteeMemberData.UserId.Split(',').Where(i => i.ToString() == Id.ToString()).Count() > 0;
                                if (isCommitteeUserAssigned)
                                {
                                    UserRole = Messages.COMMITTEE;
                                }
                                else
                                {
                                    UserRole = new UserMastersRepository().Get(Convert.ToInt32(Ids)).Type;
                                }
                                UserName = new UserMastersRepository().Get(Convert.ToInt32(Ids)).EmployeeName;// + ", ";
                                PendingWith += UserName + "(" + UserRole + ")" + ", ";
                            }
                        }
                        else
                        {
                            PendingWith = Messages.NotAvailable;
                        }

                        catObj.LastPerformedBy = PendingWith;


                        EmailsAttachement.Add(catObj);

                    }




                    IDictionary<string, List<EmployeeComplaintWorkFlowVM>> dic = new Dictionary<string, List<EmployeeComplaintWorkFlowVM>>();

                    foreach (var items in EmailsAttachement)
                    {

                        if (items.AssignedUserRoles != null)
                        {
                            foreach (var item in items.AssignedUserRoles.Split(','))
                            {


                                List<EmployeeComplaintWorkFlowVM> value;
                                if (dic.TryGetValue(item, out value))
                                {
                                    value.Add(items);
                                    dic[item] = value;
                                }
                                else
                                {
                                    dic.Add(item, new List<EmployeeComplaintWorkFlowVM> { items });
                                }

                            }
                        }
                    }
                    foreach (var item in dic)
                    {
                        int userid = Convert.ToInt32(item.Key);
                        var Hrtype = db.UserMasters.FirstOrDefault(x => x.Id == userid && x.IsActive);

                        string ManagerEmail = db.UserMasters.FirstOrDefault(x => x.Manager == Hrtype.Manager) != null ? db.UserMasters.FirstOrDefault(x => x.Manager == Hrtype.Manager).WorkEmail : string.Empty;
                        List<string> cc = new List<string> { ManagerEmail };
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage package = new ExcelPackage();


                        var ws = package.Workbook.Worksheets.Add(Messages.ComplaintList);
                        //Headers
                        ws.Cells["A1"].Value = Messages.EmployeeName;
                        ws.Cells["B1"].Value = Messages.Category;
                        ws.Cells["C1"].Value = Messages.SubCategory;
                        ws.Cells["D1"].Value = Messages.CreatedDate;
                        ws.Cells["E1"].Value = Messages.CreatedBy;
                        ws.Cells["F1"].Value = Messages.PendingWith;
                        ws.Cells["G1"].Value = Messages.Remark;
                        ws.Cells["H1"].Value = Messages.Status;
                        ws.Cells["I1"].Value = Messages.ComplaintNo;



                        var rowNumber = 1;
                        ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 1].Value = Messages.EmployeeName;

                        ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 2].Value = Messages.Category;

                        ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 3].Value = Messages.SubCategory;

                        ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 4].Value = Messages.CreatedDate;

                        ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 5].Value = Messages.CreatedBy;

                        ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 6].Value = Messages.PendingWith;

                        ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 7].Value = Messages.Remark;

                        ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 8].Value = Messages.Status;

                        ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 9].Value = Messages.ComplaintNo;



                        foreach (var log in item.Value)
                        {
                            rowNumber++;
                            string createdate = log.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                            ws.Cells[rowNumber, 1].Value = log.EmployeeName;
                            ws.Cells[rowNumber, 2].Value = log.Category;
                            ws.Cells[rowNumber, 3].Value = log.SubCategory;
                            ws.Cells[rowNumber, 4].Value = createdate;
                            ws.Cells[rowNumber, 5].Value = log.CreatedByName;
                            ws.Cells[rowNumber, 6].Value = log.LastPerformedBy;
                            ws.Cells[rowNumber, 7].Value = log.Remarks;
                            ws.Cells[rowNumber, 8].Value = log.ActionType;
                            ws.Cells[rowNumber, 9].Value = log.ComplaintNo;

                        }


                        var stream = new MemoryStream();
                        package.SaveAs(stream);

                        byte[] excelData = stream.ToArray();

                        string NotificationContent = "Dear " + Hrtype.EmployeeName + "," + "Please find overdue complaints in the attachment for your approval. Please take urgent actions.";
                        MailSend.SendEmailJobDue(Hrtype.WorkEmail, "Complaint", NotificationContent, excelData, cc);


                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
            return empds;

        }



        //Esclation Alert(3)

        public List<EmployeeComplaintWorkFlow> GetAllMailEsclationThreeOverdueData()
        {


            List<EmployeeComplaintWorkFlow> emp1 = new List<EmployeeComplaintWorkFlow>();
            List<EmployeeComplaintWorkFlow> empds = new List<EmployeeComplaintWorkFlow>();
            List<UserMaster> User = new List<UserMaster>();
            List<EmployeeComplaintWorkFlowVM> EmailsAttachement = new List<EmployeeComplaintWorkFlowVM>();
            using (DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities())
            {
                try
                {

                    emp1 = db.EmployeeComplaintWorkFlows.Where(c => !db.HR_Role.Select(b => b.ComplentId).Contains(c.ComplaintId)).ToList();

                    foreach (var item in emp1)
                    {
                        DateTime duedate = item.DueDate.AddDays(+10);

                        empds.AddRange(db.EmployeeComplaintWorkFlows.Where(c => c.Id == item.Id && c.IsActive && c.ActionType != "Withdrawn" && c.ActionType != "Completed" && duedate <= DateTime.Now && duedate >= DateTime.Now));
                    }
                    //1/4/2021
                    foreach (var item in empds)
                    {
                        EmployeeComplaintWorkFlowVM catObj = Mapper.Map<EmployeeComplaintWorkFlow, EmployeeComplaintWorkFlowVM>(item);

                        catObj.CreatedByName = db.UserMasters.FirstOrDefault(x => x.Id == item.CreatedBy) != null ? db.UserMasters.FirstOrDefault(x => x.Id == item.CreatedBy).EmployeeName : string.Empty;
                        catObj.ComplaintNo = item.ComplaintNo;
                        catObj.LOSName = db.LOSMasters.FirstOrDefault(x => x.Id == item.LOSId) != null ? db.LOSMasters.FirstOrDefault(x => x.Id == item.LOSId).LOSName : string.Empty;
                        catObj.SBU = db.SBUMasters.FirstOrDefault(x => x.Id == item.SBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == item.SBUId).SBU : string.Empty;
                        catObj.SubSbU = db.SubSBUMasters.FirstOrDefault(x => x.Id == item.SubSBUId) != null ? db.SBUMasters.FirstOrDefault(x => x.Id == item.SubSBUId).SBU : string.Empty;

                        catObj.AssignedUserRoles = item.AssignedUserRoles;
                        catObj.Id = item.Id;
                        catObj.IsActive = item.IsActive;
                        catObj.ActionType = item.ActionType;
                        catObj.DueDate = item.DueDate;
                        catObj.CreatedDate = item.CreatedDate;
                        catObj.CreatedBy = item.CreatedBy;
                        catObj.ActionType = item.ActionType;
                        int UserId = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).UserId;
                        catObj.EmployeeName = db.UserMasters.FirstOrDefault(x => x.Id == UserId) != null ? db.UserMasters.FirstOrDefault(x => x.Id == UserId).EmployeeName : string.Empty;
                        int categoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).CategoryId;
                        int subcategoryid = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).SubCategoryId;
                        if (categoryid != 0)
                        {
                            catObj.Category = db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid) != null ? db.CategoryMasters.FirstOrDefault(x => x.Id == categoryid).CategoryName : string.Empty;
                        }
                        else
                        {
                            catObj.Category = "";
                        }
                        if (subcategoryid != 0)
                        {
                            catObj.SubCategory = db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid) != null ? db.SubCategoryMasters.FirstOrDefault(x => x.Id == subcategoryid).SubCategoryName : string.Empty;

                        }
                        else
                        {
                            catObj.SubCategory = "";
                        }

                        string lastperformed = db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId) != null ? db.EmployeeComplaintMasters.FirstOrDefault(x => x.Id == item.ComplaintId).LastPerformedBy : string.Empty;
                        var CommitteeMemberData = (from u in db.CommitteeMasters
                                                   where u.IsActive
                                                   select u).FirstOrDefault();
                        string PendingWith = string.Empty;

                        if (lastperformed != null && lastperformed != "")
                        {
                            string[] lastPerformBy = lastperformed.Split(',');
                            foreach (var Ids in lastPerformBy)
                            {
                                string UserName = string.Empty;
                                string UserRole = string.Empty;
                                int Id = Convert.ToInt32(Ids);
                                var isCommitteeUserAssigned = CommitteeMemberData.UserId.Split(',').Where(i => i.ToString() == Id.ToString()).Count() > 0;
                                if (isCommitteeUserAssigned)
                                {
                                    UserRole = Messages.COMMITTEE;
                                }
                                else
                                {
                                    UserRole = new UserMastersRepository().Get(Convert.ToInt32(Ids)).Type;
                                }
                                UserName = new UserMastersRepository().Get(Convert.ToInt32(Ids)).EmployeeName;// + ", ";
                                PendingWith += UserName + "(" + UserRole + ")" + ", ";
                            }
                        }
                        else
                        {
                            PendingWith = Messages.NotAvailable;
                        }

                        catObj.LastPerformedBy = PendingWith;


                        EmailsAttachement.Add(catObj);

                    }




                    IDictionary<string, List<EmployeeComplaintWorkFlowVM>> dic = new Dictionary<string, List<EmployeeComplaintWorkFlowVM>>();

                    foreach (var items in EmailsAttachement)
                    {

                        if (items.AssignedUserRoles != null)
                        {
                            foreach (var item in items.AssignedUserRoles.Split(','))
                            {


                                List<EmployeeComplaintWorkFlowVM> value;
                                if (dic.TryGetValue(item, out value))
                                {
                                    value.Add(items);
                                    dic[item] = value;
                                }
                                else
                                {
                                    dic.Add(item, new List<EmployeeComplaintWorkFlowVM> { items });
                                }

                            }
                        }
                    }
                    foreach (var item in dic)
                    {
                        int userid = Convert.ToInt32(item.Key);
                        var Hrtype = db.UserMasters.FirstOrDefault(x => x.Id == userid && x.IsActive);

                        string ManagerEmail = db.UserMasters.FirstOrDefault(x => x.Manager == Hrtype.Manager) != null ? db.UserMasters.FirstOrDefault(x => x.Manager == Hrtype.Manager).WorkEmail : string.Empty;
                        List<string> cc = new List<string> { ManagerEmail };
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        ExcelPackage package = new ExcelPackage();


                        var ws = package.Workbook.Worksheets.Add(Messages.ComplaintList);
                        //Headers
                        ws.Cells["A1"].Value = Messages.EmployeeName;
                        ws.Cells["B1"].Value = Messages.Category;
                        ws.Cells["C1"].Value = Messages.SubCategory;
                        ws.Cells["D1"].Value = Messages.CreatedDate;
                        ws.Cells["E1"].Value = Messages.CreatedBy;
                        ws.Cells["F1"].Value = Messages.PendingWith;
                        ws.Cells["G1"].Value = Messages.Remark;
                        ws.Cells["H1"].Value = Messages.Status;
                        ws.Cells["I1"].Value = Messages.ComplaintNo;



                        var rowNumber = 1;
                        ws.Cells[rowNumber, 1].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 1].Value = Messages.EmployeeName;

                        ws.Cells[rowNumber, 2].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 2].Value = Messages.Category;

                        ws.Cells[rowNumber, 3].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 3].Value = Messages.SubCategory;

                        ws.Cells[rowNumber, 4].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 4].Value = Messages.CreatedDate;

                        ws.Cells[rowNumber, 5].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 5].Value = Messages.CreatedBy;

                        ws.Cells[rowNumber, 6].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 6].Value = Messages.PendingWith;

                        ws.Cells[rowNumber, 7].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 7].Value = Messages.Remark;

                        ws.Cells[rowNumber, 8].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 8].Value = Messages.Status;

                        ws.Cells[rowNumber, 9].Style.Font.Bold = true;
                        ws.Cells[rowNumber, 9].Value = Messages.ComplaintNo;



                        foreach (var log in item.Value)
                        {
                            rowNumber++;
                            string createdate = log.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                            ws.Cells[rowNumber, 1].Value = log.EmployeeName;
                            ws.Cells[rowNumber, 2].Value = log.Category;
                            ws.Cells[rowNumber, 3].Value = log.SubCategory;
                            ws.Cells[rowNumber, 4].Value = createdate;
                            ws.Cells[rowNumber, 5].Value = log.CreatedByName;
                            ws.Cells[rowNumber, 6].Value = log.LastPerformedBy;
                            ws.Cells[rowNumber, 7].Value = log.Remarks;
                            ws.Cells[rowNumber, 8].Value = log.ActionType;
                            ws.Cells[rowNumber, 9].Value = log.ComplaintNo;

                        }


                        var stream = new MemoryStream();
                        package.SaveAs(stream);

                        byte[] excelData = stream.ToArray();

                        string NotificationContent = "Dear " + Hrtype.EmployeeName + "," + "Please find overdue complaints in the attachment for your approval. Please take urgent actions.";
                        MailSend.SendEmailJobDue(Hrtype.WorkEmail, "Complaint", NotificationContent, excelData, cc);


                    }



                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
            return empds;

        }




    }
}