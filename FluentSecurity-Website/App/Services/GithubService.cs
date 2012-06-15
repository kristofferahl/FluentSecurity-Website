using System;
using System.Collections.Generic;
using System.Linq;
using FluentSecurity.Website.App.Extensions;
using FluentSecurity.Website.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;

namespace FluentSecurity.Website.App.Services
{
	public class GithubService : IGithubService
	{
		private const string Username = "kristofferahl";
		private const string RepositoryName = "FluentSecurity";

		public IEnumerable<CommitListModel> Commits(int amount)
		{
			var commits = Get("commits?sha=develop&per_page=" + amount,
				item => new CommitListModel
				{
					Author = item.commit.author.name,
					Date = item.commit.author.date,
					Message = item.commit.message,
					Id = item.sha
				},
				model =>
				{
					model.Message = model.Message.Truncate(150);
					model.Url = String.Format("http://github.com/{0}/{1}/commit/{2}", Username, RepositoryName, model.Id);
				});

			return commits.ToList();
		}

		public IEnumerable<IssueListModel> Issues(int amount)
		{
			var issues = Get("issues?per_page=" + amount,
				item => new IssueListModel
				{
					Id = item.number,
					Title = item.title,
					Text = item.body,
					User = item.user.login,
					Created = item.created_at,
					Url = item.html_url
				},
				model =>
				{
					model.Text = model.Text.Truncate(150);
				});

			return issues.OrderByDescending(x => x.Id).ToList();
		}

		private static IEnumerable<T> Get<T>(string url, Func<dynamic, T> map, Action<T> modify = null)
		{
			var client = GetClient();
			var request = new RestRequest(url, Method.GET);
			var response = client.Execute<dynamic>(request);
			if (response.ResponseStatus != ResponseStatus.Completed)
			{
				yield break;
			}
			dynamic data = response.Data;
			foreach (var item in data)
			{
				var mappedItem = map.Invoke(item);
				if (modify != null) modify.Invoke(mappedItem);
				yield return mappedItem;
			}
		}

		private static IRestClient GetClient()
		{
			var restClient = new RestClient();
			restClient.AddHandler("application/json", new DynamicJsonDeserializer());
			var uri = new Uri(String.Format("https://api.github.com/repos/{0}/{1}/", Username, RepositoryName));
			restClient.BaseUrl = uri.ToString();
			return restClient;
		}

		public class DynamicJsonDeserializer : IDeserializer
		{
			public T Deserialize<T>(IRestResponse response)
			{
				return JsonConvert.DeserializeObject<dynamic>(response.Content);
			}

			public string RootElement { get; set; }
			public string Namespace { get; set; }
			public string DateFormat { get; set; }
		}
	}
}