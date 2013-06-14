using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FluentSecurity.Website.Models;
using LinqToTwitter;
using TweetStore;
using TweetStore.SqlCE;

namespace FluentSecurity.Website.App.Services
{
	public class TwitterService : ITwitterService
	{
		private readonly ITweetStore _tweetStore;
		private readonly ITwitterAuthorizer _auth;

		public TwitterService() : this(new SqlCeTweetStore()) {}

		public TwitterService(ITweetStore tweetStore)
		{
			_tweetStore = tweetStore;
			_auth = new SingleUserAuthorizer
			{
				Credentials = new InMemoryCredentials
				{
					ConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"],
					ConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"],
					OAuthToken = ConfigurationManager.AppSettings["TwitterOAuthToken"],
					AccessToken = ConfigurationManager.AppSettings["TwitterAccessToken"]
				}
			};
		}

		public IEnumerable<TweetListModel> Search(string searchterm, int amount)
		{
			return searchterm.StartsWith("@")
				? GetTweetsFor(searchterm, amount)
				: GetByQuery(searchterm, amount);
		}

		public IEnumerable<TweetListModel> GetByQuery(string searchterm, int amount)
		{
			using (var twitterCtx = new TwitterContext(_auth))
			{
				var searchResults = (twitterCtx.Search.Where(search =>
				  search.Type == SearchType.Search &&
				  search.Count == amount &&
				  search.Query == searchterm &&
				  search.ResultType == ResultType.Mixed
				  )).SingleOrDefault() ?? new Search();

				foreach (var searchResult in searchResults.Statuses)
				{
					var tweet = new TweetListModel
					{
						StatusId = searchResult.StatusID,
						Published = searchResult.CreatedAt,
						AuthorId = searchResult.User.Identifier.ScreenName,
						AuthorName = searchResult.User.Name,
						AuthorUrl = "http://twitter.com/" + searchResult.User.Identifier.ScreenName,
						TweetUrl = "http://twitter.com/" + searchResult.User.Identifier.ScreenName + "/status/" + searchResult.StatusID,
						TweetTextContent = searchResult.Text,
						TweetHtmlContent = null
					};
					_tweetStore.Store(tweet);
				}
			}
			return _tweetStore.Query<TweetListModel>(false, t => t.TweetTextContent.Contains(searchterm));
		}

		public IEnumerable<TweetListModel> GetTweetsFor(string twitterId, int amount)
		{
			var trimmedTwitterId = twitterId.Replace("@", "");
			using (var twitterCtx = new TwitterContext(_auth))
			{
				var searchResults = (twitterCtx.Status.Where(search =>
				  search.Type == StatusType.User &&
				  search.ID == trimmedTwitterId
				  )).Take(amount);

				foreach (var searchResult in searchResults)
				{
					var tweet = new TweetListModel
					{
						StatusId = searchResult.StatusID,
						Published = searchResult.CreatedAt,
						AuthorId = searchResult.ID,
						AuthorName = searchResult.User.Name,
						AuthorUrl = "http://twitter.com/" + searchResult.ID,
						TweetTextContent = searchResult.Text,
						TweetHtmlContent = null,
						TweetUrl = "http://twitter.com/" + searchResult.ID + "/status/" + searchResult.StatusID,
					};
					_tweetStore.Store(tweet);
				}
			}
			return _tweetStore.Query<TweetListModel>(false, t => t.AuthorId.Contains(trimmedTwitterId));
		}
	}
}