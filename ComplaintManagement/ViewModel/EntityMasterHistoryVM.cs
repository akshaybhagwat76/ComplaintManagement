using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ComplaintManagement.ViewModel
{
    public class EntityMasterHistoryVM
    {
        public int Id { get; set; }
        [DisplayName("Entity ")]
        [StringLength(100, ErrorMessage = "Max 128 characters")]
        public string EntityName { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public int UserId { get; set; }
        public int EntityId { get; set; }
        public string EntityState { get; set; }

        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
    }
}