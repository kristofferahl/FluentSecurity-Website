using System;
using System.Net;
using Kiwi.Markdown;

namespace FluentSecurity.Website.App.Services
{
	public class GithubUrlContentProvider : IContentProvider
	{
		public string BaseUrl { get; private set; }

		public GithubUrlContentProvider(string baseUrl)
		{
			BaseUrl = baseUrl.EndsWith("/")
				? baseUrl
				: String.Concat(baseUrl, "/");
		}

		public virtual string GetContent(string docId)
		{
			try
			{
				var url = String.Concat(BaseUrl, docId, ".md");
				using (var client = new WebClient())
				{
					var document = client.DownloadString(url);
					return String.IsNullOrWhiteSpace(document) || document.Contains("<!DOCTYPE html>")
						? null
						: document;
				}
			}
			catch
			{
				return null;
			}
		}
	}
}