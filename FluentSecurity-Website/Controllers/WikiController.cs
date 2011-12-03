using System.Web.Mvc;

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

			return View(MvcApplication.MarkdownService.GetDocument(docId));
		}
	}
}