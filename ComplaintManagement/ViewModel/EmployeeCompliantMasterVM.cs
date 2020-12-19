using System;
using System.ComponentModel;
namespace ComplaintManagement.ViewModel
{
    public class EmployeeCompliantMasterVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [DisplayName("SubCategory")]
        public int SubCategoryId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        [DisplayName("Remark")]
        public string Remark { get; set; }
        [DisplayName("Remarked")]
        public string Remarked { get; set; }
        [DisplayName("Attachments")]
        public string Attachments { get; set; }
        public string Attachments1 { get; set; }
        public bool IsActive { get; set; }
        public bool Status { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public string EmployeeName { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string ComplaintStatus { get; set; }
        public string EntityState { get; set; }
        public int EmployeeComplaintMasterId { get; set; }
        public System.DateTime DueDate { get; set; }
        public bool IsSubmitted { get; set; }
        public int HRUserId { get; set; }
        public string CaseType { get; set; }
        public string LastPerformedBy { get; set; }
        public string ComplaintNo { get; set; }
    }

}