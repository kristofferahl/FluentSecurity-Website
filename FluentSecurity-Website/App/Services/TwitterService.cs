using System.Collections.Generic;
using System.Linq;
using FluentSecurity.Website.Models;
using LinqToTwitter;

namespace FluentSecurity.Website.App.Services
{
	public class TwitterService : ITwitterService
	{
		public IEnumerable<TwitterHashtagListModel> Hashtag(string hashtag, int amount)
		{
			var list = new List<TwitterHashtagListModel>();

			using (var twitterCtx = new TwitterContext())
			{
				var searchResults = (twitterCtx.Search.Where(search =>
					search.Type == SearchType.Search &&
					search.PageSize == amount &&
					search.Hashtag == hashtag
					)).SingleOrDefault() ?? new Search();

				foreach (var searchResult in searchResults.Entries)
				{
					list.Add(new TwitterHashtagListModel
					{
						Id = searchResult.ID,
						Url = searchResult.Source,
						Content = searchResult.Content,
						Title = searchResult.Title,
						Author = searchResult.Author.Name,
						AuthorUrl = searchResult.Author.URI,
						Date = searchResult.Published
					});
				}
			}

			return list;
		}
	}
}