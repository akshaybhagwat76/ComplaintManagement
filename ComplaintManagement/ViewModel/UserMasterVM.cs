using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComplaintManagement.ViewModel
{
    public class UserMasterVM
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string WorkEmail { get; set; }
        public string TimeType { get; set; }
        public string BusinessTitle { get; set; }
        public string Company { get; set; }
        public int LOSId { get; set; }
        public int SBUId { get; set; }
        public int SubSBUId { get; set; }
        public int CompentencyId { get; set; }
        public int LocationId { get; set; }
        public int RegionId { get; set; }
        public System.DateTime DateOfJoining { get; set; }
        public string MobileNo { get; set; }
        public string Manager { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string ImagePath { get; set; }

    }
}