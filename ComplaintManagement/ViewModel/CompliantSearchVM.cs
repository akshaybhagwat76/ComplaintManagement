namespace ComplaintManagement.ViewModel
{
    public class CompliantSearchVM
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int currentPage { get; set; }
    }
}