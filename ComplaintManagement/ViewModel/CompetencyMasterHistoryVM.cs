﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.ViewModel
{
    public class CompetencyMasterHistoryVM
    {
        public int Id { get; set; }
        [DisplayName("Competency ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string CompetencyName { get; set; }
        public int CompetencyId { get; set; }

        [DisplayName("Status")]
        public string EntityState{ get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
    }
}