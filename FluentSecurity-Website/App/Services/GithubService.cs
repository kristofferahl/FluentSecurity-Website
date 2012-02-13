using System;
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
			var commits = Enumerable.Empty<Commit>();

			var commitsAPI = new Commits(CacheProvider, SimpleLogProvider);
			try
			{
				commits = commitsAPI.CommitsForBranch(Username, RepositoryName, "develop") ?? Enumerable.Empty<Commit>();
			}
			catch (Exception) {}
			
			return commits;
		}

		private IEnumerable<Issue> GetIssues()
		{
			var issues = Enumerable.Empty<Issue>();

			//var issuesAPI = new Issues(_cacheProvider, _simpleLogProvider);
			//issuesAPI.Authenticate(_githubUser);
			//var issues = issuesAPI.List("FluentSecurity", "kristofferahl", IssueState.Open) ?? new List<Issue>();

			var api = new BaseAPI(CacheProvider, SimpleLogProvider);
			var url = string.Format("issues/list/{0}/{1}/{2}", Username, RepositoryName, "open");
			
			try
			{
				var result = api.ConsumeJsonUrl<GithubSharp.Core.Models.Internal.IssuesCollection>(url);
				issues = result.Issues ?? Enumerable.Empty<Issue>();
			}
			catch (Exception) {}

			return issues;
		}
	}
}