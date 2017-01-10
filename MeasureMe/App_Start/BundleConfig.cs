using System.Web;
using System.Web.Optimization;

namespace MeasureMe {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            BundleTable.EnableOptimizations = true;
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"
                      , "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/JQueryUI/external/jquery.js"
                      , "~/JQueryUI/jquery-ui.min.js"
                      , "~/D3/d3.min.js"
                      , "~/C3/c3.min.js"
                      , "~/Scripts/site.js"));

            bundles.Add(new ScriptBundle("~/bundles/auth").Include(
                      "~/Scripts/login.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/bootstrap.css"
                      , "~/Content/site.css"
                      , "~/Content/scroll.css"
                      , "~/Content/snackbar.css"
                      , "~/JQueryUI/jquery-ui.min.css"
                      , "~/JQueryUI/jquery-ui.structure.min.css"
                      , "~/JQueryUI/jquery-ui.theme.min.css"
                      , "~/C3/c3.css"
                      ));
        }
    }
}
