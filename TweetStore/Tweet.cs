using System;

namespace TweetStore
{
	public class Tweet : ITweet
	{
		public Guid Id { get; set; }
		public string StatusId { get; set; }
		public DateTime Published { get; set; }
		public string AuthorId { get; set; }
		public string AuthorName { get; set; }
		public string AuthorUrl { get; set; }
		public string TweetTextContent { get; set; }
		public string TweetHtmlContent { get; set; }
		public string TweetUrl { get; set; }
		public bool Private { get; set; }
	}
}