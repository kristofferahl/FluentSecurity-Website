using System;

namespace FluentSecurity.Website.Models
{
	public class IssueListModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string User { get; set; }
		public DateTime Created { get; set; }
		public string Url { get; set; }
		public string Text { get; set; }
	}
}