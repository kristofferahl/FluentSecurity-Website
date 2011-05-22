using System.Web.Mvc;
using System.Web.Routing;

namespace FluentSecurity_Website.App.Extensions
{
	///<summary>
	/// Extends TagBuilder
	///</summary>
	public static class TagBuilderExtensions
	{
		/// <summary>
		/// Adds an attribute to the tagbuilder for each property in the object. Will not replace existing attributes.
		/// </summary>
		public static TagBuilder AddAttributes(this TagBuilder tagBuilder, object htmlAttributes)
		{
			return tagBuilder.AddAttributes(htmlAttributes, false);
		}

		/// <summary>
		/// Adds an attribute to the tagbuilder for each property in the object.
		/// </summary>
		public static TagBuilder AddAttributes(this TagBuilder tagBuilder, object htmlAttributes, bool replaceExistingAttributes)
		{
			var attributes = new RouteValueDictionary(htmlAttributes);
			tagBuilder.MergeAttributes(attributes, replaceExistingAttributes);
			return tagBuilder;
		}
	}
}