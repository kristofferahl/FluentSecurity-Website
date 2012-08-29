using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb;

namespace TweetStore
{
	public abstract class SisoDbTweetStore : ITweetStore
	{
		public const string DefaultDatabaseConnectionString = @"Data Source=|DataDirectory|TweetStore.sdf;";

		private static ISisoDatabase _sisoDatabase;
		private readonly Func<ISisoDatabase> _sisoDatabaseProvider;

		protected SisoDbTweetStore(Func<ISisoDatabase> sisoDatabaseProvider)
		{
			_sisoDatabaseProvider = sisoDatabaseProvider;
		}

		public void Store(ITweet tweet)
		{
			EnsureInitialized();
			using (var session = _sisoDatabase.BeginSession())
			{
				var tweetExists = session.Query<ITweet>().Any(t => t.StatusId == tweet.StatusId);
				if (!tweetExists) session.Insert(tweet);
			}
		}

		public void Delete(Guid id)
		{
			_sisoDatabase.UseOnceTo().DeleteById<ITweet>(id);
		}

		public IEnumerable<T> GetAll<T>(bool includePrivate = false) where T : class, ITweet
		{
			EnsureInitialized();
			var query = _sisoDatabase.UseOnceTo().Query<ITweet>();
			if (!includePrivate) query = query.Where(t => t.Private == false);
			return query.ToListOf<T>();
		}

		public IEnumerable<T> Query<T>(bool includePrivate = false, params Expression<Func<ITweet, bool>>[] expressions) where T : class, ITweet
		{
			if (expressions == null) throw new ArgumentNullException("expressions");
			if (expressions.Length < 1) throw new ArgumentException("At least one expression must be provided","expressions");

			EnsureInitialized();
			var query = _sisoDatabase.UseOnceTo().Query<ITweet>();
			if (!includePrivate) query = query.Where(t => t.Private == false);
			return query.Where(expressions).ToListOf<T>();
		}

		private void EnsureInitialized()
		{
			if (_sisoDatabase == null)
			{
				_sisoDatabase = _sisoDatabaseProvider.Invoke();
				_sisoDatabase.CreateIfNotExists();
			}
		}
	}
}