//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ComplaintManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeComplaintWorkFlow
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
        public System.DateTime DueDate { get; set; }
        public Nullable<System.DateTime> CommitteeDueDate { get; set; }
        public string ComplaintNo { get; set; }
    
        public virtual EmployeeComplaintMaster EmployeeComplaintMaster { get; set; }
    }
}
