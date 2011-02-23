using System.Web.Mvc;
using System.Web.Routing;

namespace ZeeBi.UI
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute("Default", "", new { controller = "UI", action = "Index" });

			
			routes.MapRoute(
				"Follow", // Route name
				"{id}", // URL with parameters
				new { controller = "UI", action = "Follow" } // Parameter defaults
			);

			routes.MapRoute(
				"Action", // Route name
				"-/{action}/{id}", // URL with parameters
				new { controller = "UI", id = UrlParameter.Optional} // Parameter defaults
			);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}
}