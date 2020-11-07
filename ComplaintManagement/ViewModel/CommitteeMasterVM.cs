using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComplaintManagement.ViewModel
{
    public class CommitteeMasterVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CommitteeName { get; set; }
        public bool Status { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public System.DateTime CreatedDate { get; set; }

       
    }
}