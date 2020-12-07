using System;
namespace ComplaintManagement.ViewModel
{
    public class EmployeeComplaintWorkFlowVM
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
        public string RoleId { get; set; }
        public int LOSId { get; set; }
        public int SBUId { get; set; }
        public int SubSBUId { get; set; }
        public int CompentencyId { get; set; }
        public string AssignedUserRoles { get; set; }

    }
}