using System.Collections.Generic;

namespace FluentSecurity.Website.Models
{
	public class IndexPageModel
	{
		public IndexPageModel()
		{
			Commits = new List<CommitListModel>();
			Issues = new List<IssueListModel>();
		}

		public List<CommitListModel> Commits { get; set; }
		public List<IssueListModel> Issues { get; set; }
	}
}