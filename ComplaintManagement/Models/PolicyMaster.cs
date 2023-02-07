//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ComplaintManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class PolicyMaster
    {
        public int PolicyId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> OperationId { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyName { get; set; }
        public string TimeCode { get; set; }
        public Nullable<int> Lastcertificatenumber { get; set; }
        public Nullable<System.DateTime> Validsince { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime Validuntil { get; set; }
        public bool Able { get; set; }
        public Nullable<int> Dry { get; set; }
        public Nullable<int> Reefer { get; set; }
        public Nullable<int> InternationalCostDry { get; set; }
        public Nullable<int> InternationalCostReefer { get; set; }
        public string Observations { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]

        public bool Status { get; set; }

        [NotMapped]
       public string CreatedByName { get; set; }

        [NotMapped]

        public string UpdatedByName { get; set; }

        [NotMapped]

        public int CreatedBy { get; set; }

        [NotMapped]

        public int ModifiedBy { get; set; }

        [NotMapped]

        public string CompanyName  { get; set; }


        [NotMapped]

        public string OperationName { get; set; }
    }
}