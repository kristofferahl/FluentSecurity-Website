using System;
using System.IO;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TweetStore.SelfHost.Models;

namespace TweetStore.SelfHost.App.Formatters
{
	public class AssetFormatter : MediaTypeFormatter
	{
		private readonly AssetReader _reader = new AssetReader();

		public AssetFormatter()
		{
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/css"));
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xhtml+xml"));
		}

		public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
		{
			var task = Task.Factory.StartNew(() =>
			{
				var asset = (Asset)value;
				var bytes = _reader.Read(asset);
				if (bytes != null)
				{
					writeStream.Write(bytes, 0, bytes.Length);
					writeStream.Flush();
				}
			});
			return task;
		}

		public override bool CanReadType(Type type)
		{
			return false;
		}

		public override bool CanWriteType(Type type)
		{
			return type == typeof(Asset);
		}
	}
}