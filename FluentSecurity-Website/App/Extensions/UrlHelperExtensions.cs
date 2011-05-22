using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace FluentSecurity.Website.App.Extensions
{
	public static class UrlHelperExtensions
	{
		public static string Action<TController>(this UrlHelper urlHelper, Expression<Func<TController, ActionResult>> actionExpression, bool allowRestrictedUrls = false) where TController : IController
		{
			return urlHelper.Action(actionExpression, null, allowRestrictedUrls);
		}

		public static string Action<TController>(this UrlHelper urlHelper, Expression<Func<TController, ActionResult>> actionExpression, object values, bool allowRestrictedUrls = false) where TController : IController
		{
			var controllerName = typeof(TController).GetControllerName();
			var actionName = actionExpression.GetActionName();
			return urlHelper != null ? urlHelper.Action(actionName, controllerName, values) : "~/";
		}

		public static bool CurrentSectionMatchUrlOf<TController>(this UrlHelper urlHelper, Expression<Func<TController, ActionResult>> actionExpression) where TController : IController
		{
			var sectionUrl = urlHelper.Action(actionExpression, null);
			if (urlHelper.RequestContext.HttpContext.Request.Url != null && sectionUrl != null)
			{
				var currentUrl = urlHelper.RequestContext.HttpContext.Request.Url.PathAndQuery;
				return (currentUrl.StartsWith(sectionUrl));
			}
			return false;
		}
	}
}