using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SistemaMatricula.Helpers;

namespace SistemaMatricula
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                AreaRegistration.RegisterAllAreas();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(null, e.Message);
                Models.Log.Add(Models.Log.TYPE_ERROR, "SistemaMatricula.Global.Application_Start", notes);
            }
        }
    }
}
