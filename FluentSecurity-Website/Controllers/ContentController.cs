using System.Web.Mvc;

namespace FluentSecurity_Website.Controllers
{
    public class ContentController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult GettingStarted()
		{
			return View();
		}
    }
}
