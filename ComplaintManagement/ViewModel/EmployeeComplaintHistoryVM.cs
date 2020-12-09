using System;

namespace ComplaintManagement.ViewModel
{
    public class EmployeeComplaintHistoryVM
    {
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string Remarks { get; set; }
        public string ActionType { get; set; }
        public string UserType { get; set; }
        public int ComplaintId { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
    }
}