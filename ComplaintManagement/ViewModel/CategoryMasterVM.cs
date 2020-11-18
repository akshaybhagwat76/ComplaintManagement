using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.ViewModel
{
    public class CategoryMasterVM
    {
        public int Id { get; set; }
        [DisplayName("Category ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string CategoryName { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}