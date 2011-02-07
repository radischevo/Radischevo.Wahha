using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Text;

namespace Radischevo.Wahha.Web.Mvc.Html
{
	public static class SerializationExtensions
	{
		#region Constants
		private const SerializationMode _defaultMode = SerializationMode.Plaintext;
		#endregion

		#region Extension Methods
		public static string Serialize(this HtmlHelper helper, string name)
		{
			return Serialize(helper, name, null, _defaultMode);
		}

		public static string Serialize(this HtmlHelper helper, 
			string name, SerializationMode mode)
		{
			return Serialize(helper, name, null, mode);
		}

		public static string Serialize(this HtmlHelper helper, 
			string name, object data)
		{
			return Serialize(helper, name, data, _defaultMode);
		}

		public static string Serialize(this HtmlHelper helper, 
			string name, object data, SerializationMode mode)
		{
			Precondition.Require(helper, () => Error.ArgumentNull("helper"));
			Precondition.Defined(name, () => Error.ArgumentNull("name"));

			ModelStateSerializer serializer = new ModelStateSerializer();
			string elementName = helper.Context.ViewData.Template.GetHtmlElementName(name);
			string value = serializer.Serialize(data ?? helper.Context.ViewData.Model, mode);

			HtmlElementBuilder builder = new HtmlElementBuilder("input");
			builder.Attributes["type"] = "hidden";
			builder.Attributes["name"] = elementName;
			builder.Attributes["value"] = value;

			return builder.ToString();
		}
		#endregion
	}
}
