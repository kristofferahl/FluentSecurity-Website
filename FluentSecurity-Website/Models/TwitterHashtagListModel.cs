using System;

namespace FluentSecurity.Website.Models
{
	public class TwitterHashtagListModel
	{
		public string Id { get; set; }
		public string Author { get; set; }
		public string AuthorUrl { get; set; }
		public DateTime Date { get; set; }
		public string Title	 { get; set; }
		public string Content { get; set; }
		public string Url { get; set; }
	}
}