using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ComplaintManagement.ViewModel
{
    public class RegionMasterVM
    {

        public int Id { get; set; }
        [DisplayName("Region ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string Region { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}