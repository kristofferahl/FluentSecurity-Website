using System.Collections.Generic;
using System.Web.Http;

namespace TweetStore.SelfHost
{
	public class TweetsController : ApiController
	{
		public IEnumerable<Tweet> GetAll()
		{
			return Program.Store.GetAll<Tweet>();
		}
	}
}