using System.Collections.Generic;
using FluentSecurity.Website.Models;

namespace FluentSecurity.Website.App.Services
{
	public interface IGithubService
	{
		IEnumerable<CommitListModel> Commits(int amount);
		IEnumerable<IssueListModel> Issues(int amount);
	}
}