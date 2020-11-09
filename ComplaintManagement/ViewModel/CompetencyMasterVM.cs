using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComplaintManagement.ViewModel
{
    public class CompetencyMasterVM
    {
        public int Id { get; set; }
        public string CompetencyName { get; set; }
        public bool Status { get; set; }
        public int Userid { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}