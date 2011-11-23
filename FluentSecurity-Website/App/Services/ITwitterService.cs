using System.Collections.Generic;
using FluentSecurity.Website.Models;

namespace FluentSecurity.Website.App.Services
{
	public interface ITwitterService
	{
		IEnumerable<TwitterHashtagListModel> Hashtag(string hashtag, int amount);
	}
}