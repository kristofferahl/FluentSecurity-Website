using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentSecurity.Website.App.Extensions;
using FluentSecurity.Website.Controllers;

namespace FluentSecurity.Website
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

			routes.RouteFor<ContentController>(x => x.GettingStarted(), "getting-started");
			routes.RouteFor<ContentController>(x => x.Index(), "");

			routes.RouteFor<ContentController>(x => x.Http500(), "system/http-500");
			routes.RouteFor<ContentController>(x => x.Http404(), "system/http-404");

			routes.MapRoute("CatchAll", "{*url}", new { controller = "Content", action = "Http404" });
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}

		protected void Application_Error()
		{
			var httpException = Server.GetLastError() as HttpException;
			if (httpException != null && httpException.GetHttpCode() == 404)
			{
				Response.Redirect("~/system/http-404", true);
			}
		}
	}
}