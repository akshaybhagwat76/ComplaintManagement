using System.Collections.Generic;

namespace ComplaintManagement.ViewModel
{
    public class DashboardVM
    {
        public int DueComplaints { get; set; }
        public int OverDueComplaints { get; set; }
        public int AwaitingComplaints { get; set; }

    }
    public class DashboardPiChartVM
    {
        public DashboardPiChartVM()
        {
            casePiChart = new CasePiChartVM();
            categoryPiCharts = new List<ListPiChartVM>();
            subCategoryPiCharts = new List<ListPiChartVM>();
            regionPiCharts = new List<ListPiChartVM>();
            officePiCharts = new List<ListPiChartVM>();
            losPiCharts = new List<ListPiChartVM>();
            sbuPiCharts = new List<ListPiChartVM>();
            subSBUPiCharts = new List<ListPiChartVM>();
            genderofComplainant = new List<ListPiChartVM>();
            genderofRespondent = new List<ListPiChartVM>();
            designationofComplainant = new List<ListPiChartVM>();
            designationofRespondent = new List<ListPiChartVM>();
            modeofComplaint = new List<ListPiChartVM>();
            ageingPiBarCharts = new List<ListPiAndBarChartVM>();
        }
        
        public CasePiChartVM casePiChart { get; set; }
        public List<ListPiChartVM> categoryPiCharts { get; set; }
        public List<ListPiChartVM> subCategoryPiCharts { get; set; }
        public List<ListPiChartVM> regionPiCharts { get; set; }
        public List<ListPiChartVM> officePiCharts { get; set; }
        public List<ListPiChartVM> losPiCharts { get; set; }
        public List<ListPiChartVM> sbuPiCharts { get; set; }
        public List<ListPiChartVM> subSBUPiCharts { get; set; }
        public List<ListPiChartVM> genderofComplainant{ get; set; }
        public List<ListPiChartVM> genderofRespondent { get; set; }
        public List<ListPiChartVM> designationofComplainant { get; set; }
        public List<ListPiChartVM> designationofRespondent { get; set; }
        public List<ListPiChartVM> modeofComplaint { get; set; }
        public List<ListPiChartVM> caseTypeofComplaint { get; set; }
        public List<ListPiAndBarChartVM> caseTypeofComplaint1 { get; set; }

        //Bar chart
        public List<ListPiAndBarChartVM> categoryPiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> subCategoryPiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> regionPiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> officePiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> losPiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> sbuPiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> subSBUPiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> genderofComplainantPiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> genderofRespondentPiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> designationofComplainantPiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> designationofRespondentPiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> modeofComplaintPiBarCharts { get; set; }
        public List<ListPiAndBarChartVM> ageingPiBarCharts { get; set; }

       


    }
    public class CasePiChartVM
    {
        public int Actionable { get; set; }
        public int NonActionable { get; set; }

    }
    public class ListPiChartVM
    {
        public int Value { get; set; }
        public string Label { get; set; }
        public int Year { get; set; }

    }
    public class ListPiAndBarChartVM
    {
        public int Ids { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public string Label { get; set; }
        public int Year { get; set; }
        public string Url { get; set; }

    }

    public class TableListPiAndBarChartVM
    {
        public int ComplaintId { get; set; }
        public string LOSName { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string CaseNo{get;set;}
        public string SBU { get; set; }
        public string SubSBU { get; set; }
        public string Status { get; set; }
        public string Region { get; set; }


    }

    public class ImageBase64
    {
        public byte[] UrlBase64 { get; set; }
        public string Heading { get; set; }
    }

    public class DashboardMailSend
    {
        public string Comment { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string InvolveUserId { get; set; }
        public string ChartType { get; set; }
    }
}