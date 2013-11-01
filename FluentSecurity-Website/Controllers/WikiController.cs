using System;
using System.Web;
using System.Web.Mvc;
using Kiwi.Markdown;

namespace FluentSecurity.Website.Controllers
{
	[HandleError]
	public class WikiController : Controller
	{
		public const string WikiRoutePrefix = "wiki/";

		[OutputCache(CacheProfile = "WikiCache")]
		public ActionResult Doc(string docId)
		{
			if (docId.StartsWith(WikiRoutePrefix))
			{
				var newDocId = docId.Replace(WikiRoutePrefix, string.Empty);
				return RedirectPermanent("/" + WikiRoutePrefix + newDocId);
			}

			Document document;
			try
			{
				document = MvcApplication.MarkdownService.GetDocument(docId);
			}
			catch (NullReferenceException)
			{
				document = new Document
				{
					Title = "The wikipage is unavailable",
					Content = "<h1>The wikipage is unavailable at this moment!</h1><p>Github might be down and we currently have no backup of the page. </strong>Try again in a little while!</strong></p>"
				};
				Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
				Response.Cache.SetValidUntilExpires(false);
				Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
				Response.Cache.SetCacheability(HttpCacheability.NoCache);
				Response.Cache.SetNoStore();
			}

			return View(document);
		}
	}
}