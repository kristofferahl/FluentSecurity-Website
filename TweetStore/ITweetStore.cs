using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TweetStore
{
	public interface ITweetStore
	{
		void Store(ITweet tweet);
		void Delete(Guid id);
		IEnumerable<T> GetAll<T>(bool includePrivate = false) where T : class, ITweet;
		IEnumerable<T> Query<T>(bool includePrivate = false, params Expression<Func<ITweet, bool>>[] expressions) where T : class, ITweet;
	}
}