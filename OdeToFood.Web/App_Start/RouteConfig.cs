using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OdeToFood.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //{*pathInfo} oznacza cokolwiek, wszystko. Czyli przykładowo dla "trace.axd/1/2/3/4" parametr "resource" to trace"
            //natomiast parametr "pathInfo" to 1/2/3/4. Przekierowanie pasuje do szablonu, więc zostanie zignorowane, nie zostanie obsłużone.
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
