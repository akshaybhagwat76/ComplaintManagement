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
    
    public partial class CommitteeMaster
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CommitteeName { get; set; }
        public bool Status { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int modifiedby { get; set; }
    }
}
