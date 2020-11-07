using System;
namespace ComplaintManagement.ViewModel
{
    public class SubCategoryMasterVM
    {
        public int Id { get; set; }
        public string SubCategoryName { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}