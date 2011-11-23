namespace FluentSecurity.Website.App.Extensions
{
	public static class StringExtensions
	{
		public static string Truncate(this string text, int maxLength)
		{
			const string suffix = "...";
			var truncatedString = text;

			if (maxLength <= 0) return truncatedString;
			var strLength = maxLength - suffix.Length;

			if (strLength <= 0) return truncatedString;

			if (text == null || text.Length <= maxLength) return truncatedString;

			truncatedString = text.Substring(0, strLength);
			truncatedString = truncatedString.TrimEnd();
			truncatedString += suffix;
			return truncatedString;
		} 
	}
}