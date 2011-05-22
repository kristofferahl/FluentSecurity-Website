using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace FluentSecurity_Website.App.Extensions
{
	public static class RouteCollectionExtensions
	{
		public static RouteCollection RouteFor<TController>(this RouteCollection routeCollection, Expression<Func<TController, ActionResult>> actionExpression, string url) where TController : IController
		{
			var routeName = typeof(TController).GetControllerName() + "_" + actionExpression.GetActionName();
			routeCollection.MapRoute(routeName, url, RouteHelper.DefaultsFor(actionExpression));
			return routeCollection;
		}

		public static RouteCollection RouteFor<TController>(this RouteCollection routeCollection, Expression<Func<TController, ActionResult>> actionExpression, object defaults, string url) where TController : IController
		{
			return routeCollection.RouteFor(actionExpression, defaults, null, url);
		}

		public static RouteCollection RouteFor<TController>(this RouteCollection routeCollection, Expression<Func<TController, ActionResult>> actionExpression, object defaults, object constraints, string url) where TController : IController
		{
			var routeName = typeof(TController).GetControllerName() + "_" + actionExpression.GetActionName();
			routeCollection.MapRoute(routeName, url, defaults, constraints);

			var route = (Route)routeCollection[routeName];
			route.Defaults.Add("controller", typeof(TController).GetControllerName());
			route.Defaults.Add("action", actionExpression.GetActionName());

			return routeCollection;
		}
	}
}