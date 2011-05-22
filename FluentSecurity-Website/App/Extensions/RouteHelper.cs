using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace FluentSecurity.Website.App.Extensions
{
	public static class RouteHelper
	{
		public static string Action<TController>(Expression<Func<TController, ActionResult>> actionExpression) where TController : IController
		{
			return actionExpression.GetActionName();
		}

		public static string Controller<TController>() where TController : IController
		{
			return typeof(TController).GetControllerName();
		}

		public static object DefaultsFor<TController>(Expression<Func<TController, ActionResult>> actionExpression) where TController : IController
		{
			return new
			{
				controller = Controller<TController>(),
				action = Action(actionExpression)
			};
		}
	}
}