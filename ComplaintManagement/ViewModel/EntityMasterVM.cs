using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.ViewModel
{
    public class EntityMasterVM
    {
        public int Id { get; set; }
        [DisplayName("Category Name")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string EntityName { get; set; }
        [DisplayName("Category Name")]
        public bool Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}