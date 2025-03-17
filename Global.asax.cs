using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace dbsd_cw2_00017747 {
    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            string path = Server.MapPath("~/App_Data");
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }
    }
}
