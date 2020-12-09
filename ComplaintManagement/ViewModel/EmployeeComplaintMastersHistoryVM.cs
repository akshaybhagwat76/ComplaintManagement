using System;
namespace ComplaintManagement.ViewModel
{
    public class EmployeeComplaintMastersHistoryVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string Remark { get; set; }
        public string Attachments { get; set; }
        public bool IsActive { get; set; }
        public bool Status { get; set; }
        public string ComplaintStatus { get; set; }
        public string EntityState { get; set; }
        public int EmployeeComplaintMasterId { get; set; }
        public System.DateTime DueDate { get; set; }
        public bool IsSubmitted { get; set; }
    }
}