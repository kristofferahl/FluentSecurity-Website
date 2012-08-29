using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Linq;

namespace TweetStore.SelfHost.Controllers
{
	public class TweetsController : ApiController
	{
		public IEnumerable<Tweet> GetAll()
		{
			LogRequest();
			return Program.Store.GetAll<Tweet>(true);
		}

		public Tweet Delete(Guid id)
		{
			LogRequest();
			var tweet = Program.Store.Query<Tweet>(true, t => t.Id == id).SingleOrDefault();
			if (tweet != null)
			{
				Program.Store.Delete(id);
				return tweet;
			}
			throw new HttpResponseException(HttpStatusCode.NotFound);
		}

		private void LogRequest()
		{
			Console.WriteLine("Request: " + Request.RequestUri);
		}
	}
}