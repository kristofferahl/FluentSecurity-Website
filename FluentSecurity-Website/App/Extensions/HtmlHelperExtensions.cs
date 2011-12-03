using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using ColorCode;

namespace FluentSecurity.Website.App.Extensions
{
	public static class HtmlHelperExtensions
	{
		public static ILanguage DefaultCodeLanguage = Languages.CSharp;
		public static ICodeColorizer SyntaxHightlighter = new CodeColorizer();

		public static MvcHtmlString Code(this HtmlHelper htmlHelper, string code, ILanguage language = null)
		{
			if (language == null)
				language = DefaultCodeLanguage;

			var hightLightedCode = SyntaxHightlighter.Colorize(code, language);

			return MvcHtmlString.Create(hightLightedCode);
		}

		public static MvcHtmlString Navigation(this HtmlHelper htmlHelper, Func<IEnumerable<MvcHtmlString>> items, object attributes = null, string listElement = "ul")
		{
			var nonEmptyItems = items().Where(x => !String.IsNullOrWhiteSpace(x.ToHtmlString()));
			if (nonEmptyItems.Any() == false) return MvcHtmlString.Empty;

			var innerHtml = nonEmptyItems.Aggregate(String.Empty, (current, item) => String.Concat(current, item.ToHtmlString()));

			var list = new TagBuilder(listElement);
			list.AddAttributes(attributes);
			list.InnerHtml = innerHtml;

			return MvcHtmlString.Create(list.ToString());
		}

		public static MvcHtmlString NavigationLink(this HtmlHelper htmlHelper,
			string url,
			string innerHtml,
			string wrapperElement = null,
			object htmlAttributes = null,
			bool markSelectedWhenActive = true,
			bool markSelectedWhenExactMatchOnly = false,
			bool alwaysDisplayInnerHtml = false
			)
		{
			if (string.IsNullOrEmpty(url))
				return alwaysDisplayInnerHtml ? MvcHtmlString.Create(innerHtml) : MvcHtmlString.Empty;

			var tagBuilder = new TagBuilder("a");
			tagBuilder.MergeAttribute("href", url);
			tagBuilder.AddAttributes(htmlAttributes);

			Func<string, HtmlHelper, bool> urlIsCurrentPathMatch = UrlMatchesCurrentPath;
			if (markSelectedWhenExactMatchOnly) urlIsCurrentPathMatch = UrlMatchesCurrentPathExactly;

			if (markSelectedWhenActive && urlIsCurrentPathMatch(url, htmlHelper) && string.IsNullOrEmpty(wrapperElement))
				tagBuilder.InnerHtml = String.Format("<strong>{0}</strong>", innerHtml);
			else
				tagBuilder.InnerHtml = innerHtml;

			var navigationLink = tagBuilder.ToString(TagRenderMode.Normal);

			if (!string.IsNullOrEmpty(wrapperElement))
			{
				var wrapperElementBuilder = new TagBuilder(wrapperElement);
				if (markSelectedWhenActive && urlIsCurrentPathMatch(url, htmlHelper))
					wrapperElementBuilder.InnerHtml = String.Format("<strong>{0}</strong>", navigationLink);
				else
					wrapperElementBuilder.InnerHtml = navigationLink;
				navigationLink = wrapperElementBuilder.ToString(TagRenderMode.Normal);
			}

			return MvcHtmlString.Create(navigationLink);
		}

		private static bool UrlMatchesCurrentPath(string url, HtmlHelper htmlHelper)
		{
			var currentPath = GetCurrentPath(htmlHelper);
			return (url == currentPath || currentPath.StartsWith(url + "/"));
		}

		private static bool UrlMatchesCurrentPathExactly(string url, HtmlHelper htmlHelper)
		{
			var currentPath = GetCurrentPath(htmlHelper);
			return (url == currentPath);
		}

		private static string GetCurrentPath(HtmlHelper htmlHelper)
		{
			if (htmlHelper == null ||
				htmlHelper.ViewContext == null ||
				htmlHelper.ViewContext.HttpContext == null ||
				htmlHelper.ViewContext.HttpContext.Request == null)
			{
				return null;
			}

			return htmlHelper.ViewContext.HttpContext.Request.Path;
		}

		public static MvcHtmlString DisplayCurrentVersion(this HtmlHelper htmlHelper)
		{
			const string format = @"<div id=""current-version"">Version {0}</div>";
			const string currentVersionCacheKey = "FluentSecurity-CurrentVersion";

			string version;
			var cachedVersion = HttpRuntime.Cache.Get(currentVersionCacheKey);
			if (cachedVersion == null)
			{
				version = GetVersionFromGithub() ?? GetVersionFromAppSettings();
				HttpRuntime.Cache.Insert(
					currentVersionCacheKey,
					version,
					null,
					DateTime.Now.AddDays(1),
					TimeSpan.Zero);
			}
			else
			{
				version = cachedVersion.ToString();
			}

			var versionHtml = String.Format(format, version);
			return MvcHtmlString.Create(versionHtml);
		}

		private static string GetVersionFromAppSettings()
		{
			return ConfigurationManager.AppSettings["FluentSecurity.LastKnownVersion"];
		}

		private static string GetVersionFromGithub()
		{
			try
			{
				var xml = XDocument.Load("https://raw.github.com/kristofferahl/FluentSecurity/master/Build/Scripts/Build.build");
				var root = xml.Root;
				if (root != null)
				{
					var properties = root.Elements().Where(e => e.Name.LocalName == "property" && e.HasAttributes);
					if (properties.Any())
					{
						var versionProperty = properties.SingleOrDefault(p => p.FirstAttribute.Value == "project.version.label");
						if (versionProperty != null)
						{
							var versionAttribute = versionProperty.Attribute("value");
							if (versionAttribute != null)
								return versionAttribute.Value;
						}
					}
				}
			}
			catch {}
			return null;
		}
	}
}