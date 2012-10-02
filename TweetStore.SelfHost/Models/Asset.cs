using System.IO;

namespace TweetStore.SelfHost.Models
{
	public class Asset
	{
		public string Path { get; set; }

		public Asset(string path)
		{
			Path = path;
		}

		public bool Exists()
		{
			return File.Exists(Path);
		}
	}
}