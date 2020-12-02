using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ComplaintManagement.ViewModel
{
    public class RoleMasterHistoryVM
    {
        public int Id { get; set; }
        [DisplayName("User ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public int UserId { get; set; }
        [DisplayName("LOS ")]
        public string EntityState{ get; set; }
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string LOSId { get; set; }
        [DisplayName("SBU ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string SBUId { get; set; }
        [DisplayName("SubSBU ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string SubSBUId { get; set; }
        [DisplayName("Competency ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string CompetencyId { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public int RegionId { get; set; }
        public int RoleId { get; set; }
        public bool Status { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}