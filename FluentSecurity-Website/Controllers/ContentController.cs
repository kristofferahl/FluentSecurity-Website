using System;
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
