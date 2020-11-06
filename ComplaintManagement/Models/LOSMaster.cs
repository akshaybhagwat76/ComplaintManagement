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
    
    public partial class LOSMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LOSMaster()
        {
            this.RoleMasters = new HashSet<RoleMaster>();
            this.UserMasters = new HashSet<UserMaster>();
        }
    
        public int Id { get; set; }
        public string LOSName { get; set; }
        public int SBUId { get; set; }
        public int SubSBUId { get; set; }
        public int CompetencyId { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual CompetencyMaster CompetencyMaster { get; set; }
        public virtual SBUMaster SBUMaster { get; set; }
        public virtual SubSBUMaster SubSBUMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoleMaster> RoleMasters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserMaster> UserMasters { get; set; }
    }
}
