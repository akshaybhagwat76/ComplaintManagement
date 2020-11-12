using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ComplaintManagement.ViewModel
{
    public class LOSMasterVM
    {
        public int Id { get; set; }
        [DisplayName("LOS Name")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string LOSName { get; set; }
        [DisplayName("SBU Name")]
        public string SBUId { get; set; }
        [DisplayName("SubSBU Name")]
        public string SubSBUId { get; set; }
        [DisplayName("Competency Name")]
        public string CompetencyId { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}