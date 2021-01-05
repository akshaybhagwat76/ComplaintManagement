using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ComplaintManagement.ViewModel
{
    public class LOSMasterVM
    {
        public int Id { get; set; }
        [DisplayName("LOS ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string LOSName { get; set; }
        [DisplayName("SBU ")]
        public string SBUId { get; set; }
        [DisplayName("SubSBU ")]
        public string SubSBUId { get; set; }
        public string EntityState{ get; set; }
        [DisplayName("Competency ")]
        public string CompetencyId { get; set; }
        public bool Status { get; set; }
        public int LOSId { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string InvolvedUsersId { get; set; }
    public string CaseType { get; set; }
        public string typevalues { get; set; }

        public string UsersInvolved { get; set; }
        public string Comments { get; set; }
        public string fromDate { get; set; }
public string toDate { get; set; }
        public int losid { get; set; }


    }
}