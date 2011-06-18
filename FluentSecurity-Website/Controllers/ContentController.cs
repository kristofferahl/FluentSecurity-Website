using System.Web.Mvc;
using FluentSecurity.Website.Models;

namespace FluentSecurity.Website.Controllers
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

		public ActionResult Contact()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Contact(ContactEditModel inModel)
		{
			if (ModelState.IsValid)
			{
				
			}
			return View();
		}

    	public ActionResult Http404()
    	{
    		Response.StatusCode = 404;
    		return View();
    	}

    	public ActionResult Http500()
    	{
			Response.StatusCode = 500;
    		return View();
    	}
    }
}
