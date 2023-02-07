using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ComplaintManagement.ViewModel
{   
    public class PolicyMasterVM
    {
        public int PolicyId { get; set; }
        [DisplayName("Company ")]
        public Nullable<int> CompanyId { get; set; }    
        [DisplayName("Operation ")]
        public Nullable<int> OperationId { get; set; }
        [DisplayName("Policy Number ")]
        public string PolicyNumber { get; set; }
        [DisplayName("Policy Name ")]

        public string PolicyName { get; set; }

        [DisplayName ("Time Code")]
        public string TimeCode { get; set; }
        [DisplayName("Last certificate number")]

        public Nullable<int> Lastcertificatenumber { get; set; }

        [DisplayName("Valid since")]

        public System.DateTime Validsince { get; set; }
        [DisplayName("Valid until")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]

        public System.DateTime Validuntil { get; set; }
        [DisplayName("Able")]

        public bool Able { get; set; }
        [DisplayName("Dry")]
        public Nullable<int> Dry { get; set; }
        [DisplayName("Reefer")]
        public Nullable<int> Reefer { get; set; }
        [DisplayName("InternationalCostDry")]
        public Nullable<int> InternationalCostDry { get; set; }
        [DisplayName("InternationalCostReefer")]
        public Nullable<int> InternationalCostReefer { get; set; }
        [DisplayName("Observations")]

        public string Observations { get; set; }
        
        public bool IsActive { get; set; }
        public List<PolicyDetailsMasterVM> PolicyDetailsMaster { get; set; }

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

        public string CompanyName { get; set; }


        [NotMapped]

        public string OperationName { get; set; }

    }
}