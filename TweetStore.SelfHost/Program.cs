﻿using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.SelfHost;
using TweetStore.SelfHost.App.Formatters;
using TweetStore.SqlCE;

namespace TweetStore.SelfHost
{
	class Program
	{
		public static ITweetStore Store = new SqlCeTweetStore(ConfigurationManager.ConnectionStrings["TweetStore"].ConnectionString);
		
		static void Main(string[] args)
		{
			const string address = "http://localhost:8080";
			
			var config = new HttpSelfHostConfiguration(address);

			config.Formatters.Insert(0, new AssetFormatter());

			config.Routes.MapHttpRoute(
				"Admin Web", "admin", new { controller = "Admin" }
				);

			config.Routes.MapHttpRoute(
				"Assets", "assets/{type}/{asset}", new { controller = "Assets" }
				);

			config.Routes.MapHttpRoute(
				"API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional }
				);

			using (var server = new HttpSelfHostServer(config))
			{
				server.OpenAsync().Wait();
				Console.WriteLine("Started host on " + address + "/admin/");
				Console.WriteLine("Press Enter to quit.");
				Console.ReadLine();
			}
		}
	}
}
