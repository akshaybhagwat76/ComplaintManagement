namespace ComplaintManagement.ViewModel
{
    public class PolicyDetailsMasterVM
    {
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public string SecurityRequirementsCommodity { get; set; }
        public string Requirement { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
    }
}