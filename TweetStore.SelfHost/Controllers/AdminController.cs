using System;
using System.Web.Http;

namespace TweetStore.SelfHost.Controllers
{
	public class AdminController : ApiController
	{
		public Asset Get()
		{
			Console.WriteLine("Request: " + Request.RequestUri);
			return new Asset(@"..\..\Assets\Templates\Admin.html");
		}
	}
}