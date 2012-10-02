using System.Collections.Generic;
using FluentSecurity.Website.Models;

namespace FluentSecurity.Website.App.Services
{
	public interface ITwitterService
	{
		IEnumerable<TweetListModel> Search(string searchterm, int amount);
	}
}