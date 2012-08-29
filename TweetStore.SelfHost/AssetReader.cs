using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Web.Http;

namespace TweetStore.SelfHost
{
	public class AssetReader
	{
		private readonly ConcurrentDictionary<string, byte[]> _cache = new ConcurrentDictionary<string, byte[]>();

		public byte[] Read(Asset asset)
		{
#if !DEBUG
			if (!_cache.ContainsKey(asset.Path))
			{
				var bytes = ReadFileBytes(asset);
				_cache.TryAdd(asset.Path, bytes);
			}
			return _cache[asset.Path];
#else
			var bytes = ReadFileBytes(asset);
			return bytes;
#endif
		}

		private static byte[] ReadFileBytes(Asset asset)
		{
			var assetContent = File.ReadAllText(asset.Path);
			var bytes = System.Text.Encoding.Default.GetBytes(assetContent);
			return bytes;
		}
	}
}