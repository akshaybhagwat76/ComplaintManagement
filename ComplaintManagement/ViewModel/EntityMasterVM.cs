using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComplaintManagement.ViewModel
{
    public class EntityMasterVM
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}