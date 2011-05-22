using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace FluentSecurity.Website.App.Extensions
{
	public static class MvcExtensions
	{
		public static string GetControllerName(this Type controllerType)
		{
			return controllerType.Name.Replace("Controller", string.Empty);
		}

		public static string GetActionName<TController>(this Expression<Func<TController, ActionResult>> actionExpression) where TController : IController
		{
			return ((MethodCallExpression)actionExpression.Body).Method.Name;
		}
	}
}