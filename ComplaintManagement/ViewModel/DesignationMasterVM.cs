using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.ViewModel
{
    public class DesignationMasterVM
    {
        public int Id { get; set; }
        [DisplayName("Designation ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string Designation { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }

        internal DesignationMasterVM Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}