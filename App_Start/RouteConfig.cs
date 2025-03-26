using System.Web.Mvc;
using System.Web.Routing;

namespace dbsd_cw2_00017747 {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
           name: "EntityRoute",
           url: "EntityBooks/{action}/{id}",
           defaults: new { controller = "EntityBooks", action = "Index", id = UrlParameter.Optional }
       );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Books", action = "Index", id = UrlParameter.Optional }
            );




        }
    }
}
