using System;
using System.Web;
using System.Web.Mvc;

namespace FluentSecurity.Website.App.Extensions
{
	public static class DateTimeExtensions
	{
		public static MvcHtmlString TimePastSince(this DateTime dateTime, int timeZoneDifferenceInHours, string daysTextFormat, string dayTextFormat, string hoursTextFormat, string hourTextFormat, string justNowText)
		{
			var twitterTimeZoneNow = DateTime.Now.AddHours(timeZoneDifferenceInHours);
			var timePastSinceTweet = twitterTimeZoneNow.Subtract(dateTime);
			var timePastString = string.Empty;

			var totalHours = timePastSinceTweet.TotalHours;
			
			if (totalHours < 1)
				timePastString = justNowText;

			if (totalHours >= 1 && totalHours < 2)
				timePastString = string.Format(hourTextFormat, Math.Ceiling(totalHours));

			if (totalHours >= 2)
				timePastString = string.Format(hoursTextFormat, Math.Ceiling(totalHours));

			if (totalHours >= 24)
				timePastString = string.Format(dayTextFormat, timePastSinceTweet.Days);

			if (totalHours >= 48)
				timePastString = string.Format(daysTextFormat, timePastSinceTweet.Days);

			return MvcHtmlString.Create(HttpUtility.HtmlEncode(timePastString));
		}
	}
}