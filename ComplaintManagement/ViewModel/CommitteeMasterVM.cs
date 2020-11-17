using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.ViewModel
{
    public class CommitteeMasterVM
    {
        public int Id { get; set; }
        [DisplayName("User")]
        public int UserId { get; set; }
        [DisplayName("Committee Name")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string CommitteeName { get; set; }
        public bool Status { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public System.DateTime CreatedDate { get; set; }

       
    }
}