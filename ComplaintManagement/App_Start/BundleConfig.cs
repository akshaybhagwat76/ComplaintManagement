using System.Web;
using System.Web.Optimization;

namespace ComplaintManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            // Font Awesome All
            bundles.Add(new StyleBundle("~/font-awesome/all").Include(
                      "~/Scripts/vendor/fontawesome-free/css/all.min.css"));

            // SB-admin-2
            bundles.Add(new StyleBundle("~/sb-admin2/css").Include(
                     "~/Content/css/sb-admin-2.min.css"));

            // SB-admin-2-style
            bundles.Add(new StyleBundle("~/sb-admin2/styles").Include(
                     "~/Content/css/styles.css"));

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
        }
    }
}
