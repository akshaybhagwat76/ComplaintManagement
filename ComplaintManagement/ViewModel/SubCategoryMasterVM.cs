using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ComplaintManagement.ViewModel
{
    public class SubCategoryMasterVM
    {
        public int Id { get; set; }
        [DisplayName("Sub Category ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string SubCategoryName { get; set; }
        public bool Status { get; set; }
        public string EntityState{ get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public int SubCategoryId { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}