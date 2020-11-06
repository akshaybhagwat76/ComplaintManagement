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
    
    public partial class RoleMaster
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LOSId { get; set; }
        public int SBUId { get; set; }
        public int SubSBUId { get; set; }
        public int CompetencyId { get; set; }
        public bool Status { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual CompetencyMaster CompetencyMaster { get; set; }
        public virtual LOSMaster LOSMaster { get; set; }
        public virtual SBUMaster SBUMaster { get; set; }
        public virtual SubSBUMaster SubSBUMaster { get; set; }
        public virtual UserMaster UserMaster { get; set; }
    }
}