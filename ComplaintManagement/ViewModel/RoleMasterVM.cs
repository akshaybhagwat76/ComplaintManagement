using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ComplaintManagement.ViewModel
{
    public class RoleMasterVM
    {
            public int Id { get; set; }
            [DisplayName("User Name")]
            [StringLength(100, ErrorMessage = "Max 128 characters")]
            public int UserId { get; set; }
            [DisplayName("LOS Name")]
            [StringLength(100, ErrorMessage = "Max 128 characters")]
            public int LOSId { get; set; }
            [DisplayName("SBU Name")]
            [StringLength(100, ErrorMessage = "Max 128 characters")]
            public string SBUId { get; set; }
            [DisplayName("SubSBU Name")]
            [StringLength(100, ErrorMessage = "Max 128 characters")]
            public string SubSBUId { get; set; }
            [DisplayName("Competency Name")]
            [StringLength(100, ErrorMessage = "Max 128 characters")]
            public string CompetencyId { get; set; }
            public bool Status { get; set; }
            public System.DateTime CreatedDate { get; set; }
            public bool IsActive { get; set; }
            public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}