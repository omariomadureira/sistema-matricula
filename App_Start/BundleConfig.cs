using System.Web.Optimization;
using System.Web;
using System.Web.Optimization;

namespace SistemaMatricula
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/fontawesome-free/css/all.css",
                      "~/Content/sb-admin-2.css"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Content/jquery/jquery.min.js",
                        "~/Content/bootstrap/js/bootstrap.bundle.min.js",
                        "~/Content/jquery-easing/jquery.easing.min.js",
                        "~/Content/sb-admin-2.js",
                        "~/Content/chart.js/Chart.min.js",
                        "~/Content/demo/chart-area-demo.js",
                        "~/Content/demo/chart-pie-demo.js"));
        }
    }
}
