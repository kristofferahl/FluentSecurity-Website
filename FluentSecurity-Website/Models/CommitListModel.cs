using System;

namespace FluentSecurity.Website.Models
{
	public class CommitListModel
	{
		public string Id { get; set; }
		public string Author { get; set; }
		public DateTime Date { get; set; }
		public string Message { get; set; }
		public string Url { get; set; }
	}
}