using System.Web.Http;

namespace TweetStore.SelfHost
{
	public class AdminController : ApiController
	{
		public Asset Get()
		{
			return new Asset(@"..\..\Assets\Templates\Admin.html");
		}
	}
}