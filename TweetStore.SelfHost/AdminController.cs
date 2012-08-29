using System.Web.Http;

namespace TweetStore.SelfHost
{
	public class AdminController : ApiController
	{
		public Asset Get()
		{
			return new Asset(@"..\..\Admin.html");
		}
	}
}