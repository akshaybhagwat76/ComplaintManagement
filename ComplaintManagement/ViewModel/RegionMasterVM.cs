﻿using System;
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
        public int RegionId { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string EntityState{ get; set; }
        public int ModifiedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}