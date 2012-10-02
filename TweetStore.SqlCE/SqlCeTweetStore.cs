using SisoDb.SqlCe4;

namespace TweetStore.SqlCE
{
	public class SqlCeTweetStore : SisoDbTweetStore
	{
		public SqlCeTweetStore() : this(DefaultDatabaseConnectionString) {}

		public SqlCeTweetStore(string connectionString) : base(connectionString.CreateSqlCe4Db) {}
	}
}