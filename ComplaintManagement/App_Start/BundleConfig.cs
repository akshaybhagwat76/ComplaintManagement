using System.Web;
using System.Web.Optimization;

namespace ComplaintManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/common.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            // Font Awesome All
            bundles.Add(new StyleBundle("~/font-awesome/all").Include(
                      "~/Scripts/vendor/fontawesome-free/css/all.min.css"));

            // SB-admin-2
            bundles.Add(new StyleBundle("~/sb-admin2/css").Include(
                     "~/Content/css/sb-admin-2.min.css"));

            // SB-admin-2-style
            bundles.Add(new StyleBundle("~/sb-admin2/styles").Include(
                     "~/Content/css/styles.css"));

            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.4.1.min.js"));

            //Jquery 
            bundles.Add(new ScriptBundle("~/sb-admin2/jquery").Include(
                    "~/Scripts/vendor/jquery/jquery.js"));

            //Bootstrap bundle
            bundles.Add(new ScriptBundle("~/sb-admin2/bootstrapbundle").Include(
                    "~/Scripts/vendor/bootstrap/js/bootstrap.bundle.min.js"));

            //Core plugin JavaScript
            bundles.Add(new ScriptBundle("~/sb-admin2/Jqueryeasing").Include(
                    "~/Scripts/vendor/jquery-easing/jquery.easing.min.js"));

            //Custom scripts for all pages
            bundles.Add(new ScriptBundle("~/sb-admin2/js").Include(
                    "~/Scripts/js/sb-admin-2.min.js"));

            // jQueryUI CSS
            bundles.Add(new ScriptBundle("~/Scripts/vendor/jquery-ui/jqueryuiStyles").Include(
                        "~/Scripts/vendor/jquery-ui/jquery-ui.min.css"));


            // jQueryUI 
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/vendor/jquery-ui/jquery-ui.min.js"));

            // toastr notification 
            bundles.Add(new ScriptBundle("~/plugins/toastr").Include(
                      "~/Scripts/vendor/toastr/toastr.min.js"));

            // toastr notification styles
            bundles.Add(new StyleBundle("~/plugins/toastrStyles").Include(
                      "~/Content/toastr/toastr.min.css"));

            // Bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            // dataTables css styles
            bundles.Add(new StyleBundle("~/Content/plugins/dataTables/dataTablesStyles").Include(
                      "~/Content/plugins/dataTables/datatables.min.css",
                      "~/Content/plugins/dataTables/responsive.min.css"));

            // dataTables 
            bundles.Add(new ScriptBundle("~/plugins/dataTables").Include(
                      "~/Scripts/vendor/datatables/datatables.min.js",
                      "~/Scripts/vendor/datatables/responsive.min.js"));

            // dataPicker styles
            bundles.Add(new StyleBundle("~/plugins/dataPickerStyles").Include(
                      "~/Content/plugins/datepicker/datepicker3.css"));

            // dataPicker 
            bundles.Add(new ScriptBundle("~/plugins/datePicker").Include(
                      "~/Scripts/plugins/datepicker/bootstrap-datepicker.js"));

            //Common JS
            bundles.Add(new Bundle("~/Assets/Common").Include(
                     "~/Assets/common.js"));

            // Category Master
            bundles.Add(new ScriptBundle("~/Assets/CategoryMasters").Include(
                      "~/Assets/CategoryMasters.js"));

            //Edit Category Master
            bundles.Add(new ScriptBundle("~/Assets/EditCategoryMasters").Include(
                      "~/Assets/EditCategoryMaster.js"));

            // SubCategory Master
            bundles.Add(new ScriptBundle("~/Assets/SubCategoryMasters").Include(
                      "~/Assets/SubCategoryMasters.js"));
            //Edit SubCategory Master
            bundles.Add(new ScriptBundle("~/Assets/EditSubCategoryMasters").Include(
                      "~/Assets/EditSubCategoryMasters.js"));

            // Designation Master
            bundles.Add(new ScriptBundle("~/Assets/DesignationMasters").Include(
                      "~/Assets/DesignationMasters.js"));

            //Edit Designation Master
            bundles.Add(new ScriptBundle("~/Assets/EditDesignationMasters").Include(
                      "~/Assets/EditDesignationMasters.js"));

            // SBU Master
            bundles.Add(new ScriptBundle("~/Assets/SBUMasters").Include(
                      "~/Assets/SBUMasters.js"));

            //Edit SBU Master
            bundles.Add(new ScriptBundle("~/Assets/EditSBUMasters").Include(
                      "~/Assets/EditSBUMasters.js"));

            // SubSBU Master
            bundles.Add(new ScriptBundle("~/Assets/SubSBUMasters").Include(
                      "~/Assets/SubSBUMasters.js"));

            //Edit SubSBU Master
            bundles.Add(new ScriptBundle("~/Assets/EditSubSBUMasters").Include(
                      "~/Assets/EditSubSBUMasters.js"));


            // Competency Master
            bundles.Add(new ScriptBundle("~/Assets/CompetencyMasters").Include(
                      "~/Assets/CompetencyMasters.js"));

            //Edit Competency Master
            bundles.Add(new ScriptBundle("~/Assets/EditCompetencyMasters").Include(
                      "~/Assets/EditCompetencyMasters.js"));


            // Committee Master
            bundles.Add(new ScriptBundle("~/Assets/CommitteeMasters").Include(
                      "~/Assets/CommitteeMasters.js"));
        }
    }
}
