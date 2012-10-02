using System;

namespace TweetStore
{
	public interface ITweet
	{
		Guid Id { get; set; }
		string StatusId { get; set; }
		DateTime Published { get; set; }
		string AuthorId { get; set; }
		string AuthorName { get; set; }
		string AuthorUrl { get; set; }
		string TweetTextContent { get; set; }
		string TweetHtmlContent { get; set; }
		string TweetUrl { get; set; }
		bool Private { get; set; }
	}
}