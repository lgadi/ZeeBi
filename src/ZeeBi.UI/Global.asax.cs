using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using ZeeBi.UI.Controllers;

namespace ZeeBi.UI
{
	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		private static string Name<T>()
		{
			return typeof(T).Name.Replace("Controller", string.Empty);
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute("Home", "", new { controller = Name<UrlsController>(), action = "Index" });

			routes.MapRoute("API", "api/v1/{method}", new {controller = Name<ApiController>(), action = "Index" , method = UrlParameter.Optional });
		
			routes.MapRoute("Default", "-/{controller}/{action}/{id}", new { id = UrlParameter.Optional });
			
			routes.MapRoute("Follow", "{id}", new { controller = Name<UrlsController>(), action = "Follow" });
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}
}