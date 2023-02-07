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
    
    public partial class EmployeeComplaintMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EmployeeComplaintMaster()
        {
            this.EmployeeComplaintHistories = new HashSet<EmployeeComplaintHistory>();
            this.EmployeeComplaintWorkFlows = new HashSet<EmployeeComplaintWorkFlow>();
        }
    
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
        public System.DateTime DueDate { get; set; }
        public bool IsSubmitted { get; set; }
        public string LastPerformedBy { get; set; }
    
        public virtual UserMaster UserMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeComplaintHistory> EmployeeComplaintHistories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeComplaintWorkFlow> EmployeeComplaintWorkFlows { get; set; }
    }
}
