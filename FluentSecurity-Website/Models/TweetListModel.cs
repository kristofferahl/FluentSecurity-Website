using System;
using TweetStore;

namespace FluentSecurity.Website.Models
{
	public class TweetListModel : Tweet
	{
		public string GetAuthorName(string defaultValue = "Unknown")
		{
			if (!String.IsNullOrEmpty(base.AuthorName))
				return AuthorName;

			if (!String.IsNullOrEmpty(AuthorId))
				return AuthorId;

			return defaultValue;
		}
	}
}