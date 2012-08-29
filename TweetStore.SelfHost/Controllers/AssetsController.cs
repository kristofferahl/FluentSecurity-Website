using System;
using System.Net;
using System.Web.Http;
using TweetStore.SelfHost.Models;

namespace TweetStore.SelfHost.Controllers
{
	public class AssetsController : ApiController
	{
		public Asset Get(string type, string asset)
		{
			Console.WriteLine("Request: " + Request.RequestUri);
			var model = new Asset(String.Format(@"..\..\Assets\{0}\{1}", type, asset));
			if (model.Exists()) return model;
			throw new HttpResponseException(HttpStatusCode.NotFound);
		}
	}
}