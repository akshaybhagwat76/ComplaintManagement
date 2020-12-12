using System;
using System.ComponentModel;
namespace ComplaintManagement.ViewModel
{
    public class UserMasterVM
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        
        public string EmployeeId { get; set; }
        [DisplayName("Gender")]
        public string Gender { get; set; }
        public string EntityState{ get; set; }
        [DisplayName("Age")]
        public int Age { get; set; }
        [DisplayName("Work Email")]
        public string WorkEmail { get; set; }
        [DisplayName("Time Type")]
        public string TimeType { get; set; }
        [DisplayName("Business Title")]
        public int BusinessTitle { get; set; }
        [DisplayName("Company")]
        public int Company { get; set; }
        [DisplayName("LOS")]
        public int LOSId { get; set; }
        [DisplayName("SBU")]
        public int SBUId { get; set; }
        [DisplayName("SubSBU")]
        public int SubSBUId { get; set; }

        [DisplayName("Compentency")]
        public int CompentencyId { get; set; }
        [DisplayName("Location")]
        public int LocationId { get; set; }
        [DisplayName("Region")]
        public int RegionId { get; set; }
        [DisplayName("Date Of Joining")]
        public Nullable<System.DateTime> DateOfJoining { get; set; }
        [DisplayName("Mobile No")]
        public string MobileNo { get; set; }
        [DisplayName("Manager")]
        public string Manager { get; set; }
        [DisplayName("Type")]
        public string Type { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
        [DisplayName("Updated Date")]
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        [DisplayName("Image Path")]
        public string ImagePath { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public string ComplaintStatus { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public int UserMasterId { get; set; }
        public string UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [DisplayName("SubCategory")]
        public int SubCategoryId { get; set; }
        [DisplayName("Remarks")]
        public string Remark { get; set; }
        [DisplayName("Remarked")]
        public string Ramarked { get; set; }

        [DisplayName("Attachments")]
        public string Attachments { get; set; }
        [DisplayName("Case Type")]
        public string CashTypeId { get; set; }
        [DisplayName("Involved Users")]
        public string InvolvedUsersId { get; set; }
        [DisplayName("Remarks")]
        public string RemarkCommittee { get; set; }
        [DisplayName("Attachments")]
        public string AttachmentsCommittee { get; set; }
        public int ComplaintId { get; set; }
    }
}