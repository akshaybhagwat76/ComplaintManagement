﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.ViewModel
{
    public class SubSBUMasterVM
    {
        public int Id { get; set; }
        [DisplayName("Sub SBU ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string SubSBU { get; set; }
        public string EntityState{ get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int SubSBUId { get; set; }
        public int ModifiedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string InvolvedUsersId { get; set; }
        public string InvolvedUser { get; set; }
    }
}