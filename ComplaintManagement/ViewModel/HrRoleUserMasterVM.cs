using System;
using System.ComponentModel;
namespace ComplaintManagement.ViewModel
{
    public class HrRoleUserMasterVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ComplentId { get; set; }
        public string CaseType { get; set; }
        public string Attachement { get; set; }
        public string Remark { get; set; }
        public int InvolvedUsersId { get; set; }
        public int Status { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public int IsActive { get; set; }
        public int HRUserId { get; set; }

    }
}