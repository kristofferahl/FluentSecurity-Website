using System;
using System.Net;
using System.Web.Http;

namespace TweetStore.SelfHost
{
	public class AssetsController : ApiController
	{
		public Asset Get(string type, string asset)
		{
			var model = new Asset(String.Format(@"..\..\Assets\{0}\{1}", type, asset));
			if (model.Exists()) return model;
			throw new HttpResponseException(HttpStatusCode.NotFound);
		}
	}
}