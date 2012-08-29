using System;
using System.IO;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TweetStore.SelfHost
{
	public class AssetFormatter : MediaTypeFormatter
	{
		public AssetFormatter()
		{
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xhtml+xml"));
		}

		public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
		{
			var task = Task.Factory.StartNew(() =>
			{
				var asset = (Asset)value;
				var assetContent = File.ReadAllText(asset.Path);
				
				var bytes = System.Text.Encoding.Default.GetBytes(assetContent);

				writeStream.Write(bytes, 0, bytes.Length);
				writeStream.Flush();
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