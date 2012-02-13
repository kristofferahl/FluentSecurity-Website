using System.Collections.Generic;
using System.Linq;
using FluentSecurity.Website.App.Extensions;
using FluentSecurity.Website.Models;
using GithubSharp.Core.API;
using GithubSharp.Core.Base;
using GithubSharp.Core.Models;
using GithubSharp.Plugins.CacheProviders.WebCache;
using GithubSharp.Plugins.LogProviders.SimpleLogProvider;

namespace FluentSecurity.Website.App.Services
{
	public class GithubService : IGithubService
	{
		private static readonly WebCacher CacheProvider = new WebCacher();
		private static readonly SimpleLogProvider SimpleLogProvider = new SimpleLogProvider();

		private const string Username = "kristofferahl";
		private const string RepositoryName = "FluentSecurity";

		public IEnumerable<CommitListModel> Commits(int amount)
		{
			var commits = GetCommits();
			foreach (var commit in commits.Take(amount))
			{
				yield return new CommitListModel
				{
					Author = commit.Author.Name,
					Date = commit.AuthoredDate,
					Message = commit.Message.Truncate(150),
					Id = commit.Id,
					Url = "http://github.com" + commit.URL
				};
			}
		}

		public IEnumerable<IssueListModel> Issues(int amount)
		{
			var issues = GetIssues();
			foreach (var issue in issues.OrderByDescending(x => x.Number).Take(amount))
			{
				yield return new IssueListModel
				{
					Id = issue.Number,
					Title = issue.Title,
					Text = issue.Body.Truncate(150),
					User = issue.User,
					Created = issue.CreatedAt,
					Url = issue.HtmlUrl
				};
			}
		}

		private IEnumerable<Commit> GetCommits()
		{
			var commitsAPI = new Commits(CacheProvider, SimpleLogProvider);
			var commits = commitsAPI.CommitsForBranch(Username, RepositoryName, "develop") ?? Enumerable.Empty<Commit>();
			return commits;
		}

		private IEnumerable<Issue> GetIssues()
		{
			var api = new BaseAPI(CacheProvider, SimpleLogProvider);
			var url = string.Format("issues/list/{0}/{1}/{2}", Username, RepositoryName, "open");
			var result = api.ConsumeJsonUrl<GithubSharp.Core.Models.Internal.IssuesCollection>(url);
			var issues = result.Issues ?? Enumerable.Empty<Issue>();
			return issues;
		}
	}
}