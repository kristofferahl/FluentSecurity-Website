using System;
using System.IO;

namespace FluentSecurity.Website.App.Services
{
	public class FileBackedGithubUrlContentProvider : GithubUrlContentProvider
	{
		public string BackupDirectory { get; private set; }

		public FileBackedGithubUrlContentProvider(string baseUrl, string backupDirectory) : base(baseUrl)
		{
			BackupDirectory = String.Concat(AppDomain.CurrentDomain.BaseDirectory, @"\", backupDirectory);
			if (!Directory.Exists(BackupDirectory))
			{
				Directory.CreateDirectory(BackupDirectory);
			}
		}

		public override string GetContent(string docId)
		{
			var backupFilePath = Path.Combine(BackupDirectory, docId + ".md");
			var content = base.GetContent(docId);
			if (content != null)
			{
				try
				{
					File.WriteAllText(backupFilePath, content);
				}
				catch {}
			}
			else
			{
				content = File.Exists(backupFilePath)
					? File.ReadAllText(backupFilePath)
					: null;
			}
			return content;
		}
	}
}