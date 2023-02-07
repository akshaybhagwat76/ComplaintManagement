using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComplaintManagement.ViewModel
{
    public class CompanyVM
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public bool IsActive { get; set; }
    }
}