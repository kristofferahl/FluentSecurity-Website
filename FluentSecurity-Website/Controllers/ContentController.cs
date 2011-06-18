using System.Web.Mvc;
using FluentSecurity.Website.Models;
using Postal;

namespace FluentSecurity.Website.Controllers
{
    public class ContentController : Controller
    {
    	private const string TempdataKey = "Model";

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
			var outModel = TempData.ContainsKey(TempdataKey) ?
				(ContactEditModel) TempData[TempdataKey] : new ContactEditModel();
			
			return View(outModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Contact(ContactEditModel inModel)
		{
			if (ModelState.IsValid)
			{
				dynamic email = new Email("Contact");
				email.To = "mail@fluentsecurity.net";
				email.From = inModel.Email;
				email.FromName = inModel.Name;
				email.Subject = inModel.Subject;
				email.Message = inModel.Message;
				email.Send();
				inModel.EmailSent = true;
				TempData[TempdataKey] = inModel;
				return RedirectToAction("Contact");
			}
			return View(inModel);
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
