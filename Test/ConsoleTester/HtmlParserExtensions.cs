using System;
using System.Collections.Generic;
using System.Xml;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Text;

namespace ConsoleTester
{
	public static class HtmlParserExtensions
	{
		#region Constants
		private static HtmlElementOptions AllowFlags = HtmlElementOptions.Allowed | HtmlElementOptions.Recursive;
		private static HtmlElementOptions InternalFlags = HtmlElementOptions.Internal | HtmlElementOptions.Recursive;
		#endregion

		#region Extension Methods
		private static XmlElement AppendChild(this XmlElement element, string localName)
		{
			XmlElement child = element.OwnerDocument.CreateElement(localName);
			element.AppendChild(child);

			return child;
		}

		private static XmlElement AppendChild(this XmlElement element, string localName, object attributes)
		{
			XmlElement child = AppendChild(element, localName);
			ValueDictionary values = new ValueDictionary(attributes);

			foreach (KeyValuePair<string, object> pair in values)
				child.SetAttribute(pair.Key.ToLower(), (pair.Value == null) ? 
					String.Empty : pair.Value.ToString());
			
			return child;
		}

		public static XmlElement FlashPlayer(this XmlDocument document, string movie, int width, int height, string variables)
		{
			XmlElement video = document.CreateElement("object");

			video.SetAttribute("classid", "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000");
			video.SetAttribute("width", width.ToString());
			video.SetAttribute("height", height.ToString());

			video.AppendChild("param", new {
				name = "movie",
				value = movie
			});
			video.AppendChild("param", new {
				name = "allowfullscreen",
				value = "true"
			});
			video.AppendChild("param", new {
				name = "wmode",
				value = "transparent"
			});
			video.AppendChild("param", new {
				name = "allowscriptaccess",
				value = "always"
			});
			video.AppendChild("param", new {
				name = "flashvars",
				value = variables
			});
			video.AppendChild("embed", new {
				type = "application/x-shockwave-flash",
				src = movie,
				width = width,
				height = height,
				wmode = "transparent",
				allowscriptaccess = "always",
				allowfullscreen = "true",
				flashVars = variables
			});
			return video;
		}

		public static XmlElement CrossDomainLink(this XmlElement element, string domain, string redirectUrl)
		{
			string location = element.GetAttribute("href");
			element.RemoveAttribute("target");

			Uri uri;
			if (!Uri.TryCreate(location, UriKind.Absolute, out uri))
				if (!Uri.TryCreate("http://" + location, UriKind.Absolute, out uri))
					return null;

			if (!uri.Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
			{
				if (!String.IsNullOrEmpty(redirectUrl))
					element.SetAttribute("href", String.Format("{0}?url={1}", redirectUrl, 
						Uri.EscapeDataString(uri.AbsoluteUri)));

				element.SetAttribute("target", "_blank");
				element.SetAttribute("rel", "nofollow");
			}
			return element;
		}

		public static IRuleAppender Youtube(this IRuleAppender appender)
		{
			return appender.With(e => e.Elements("object").As(InternalFlags | HtmlElementOptions.Container)
				.With(p => p.Element("param").As(HtmlElementOptions.Allowed | HtmlElementOptions.SelfClosing)
					.With(a => a.Attributes("name", "value").As(HtmlAttributeOptions.Allowed | HtmlAttributeOptions.Required))
					)
				.With(i => i.Element("embed").As(HtmlElementOptions.Allowed | HtmlElementOptions.SelfClosing | HtmlElementOptions.Internal))
				.With(a => a.Attributes("classid", "name").As(HtmlAttributeOptions.Allowed))
				.With(a => a.Attributes("width", "height").As(HtmlAttributeOptions.Allowed).Validate("#int")))
				.With(e => e.Element("youtube").As(AllowFlags | HtmlElementOptions.SelfClosing)
				.Convert(elem => {
					return elem.OwnerDocument.FlashPlayer(elem.GetAttribute("src"), 640, 360, "");
				}));
		}

		public static IRuleAppender Abstract(this IRuleAppender appender, string url)
		{
			return appender.With(e => e.Element("cut").As(AllowFlags | HtmlElementOptions.SelfClosing)
				.Convert(elem => {
					XmlElement link = elem.OwnerDocument.CreateElement("a");
					link.SetAttribute("href", url);
					link.AppendChild(elem.OwnerDocument.CreateTextNode(
						String.Format("{0} →", elem.GetAttribute("title").Define("читать далее"))
					));
					return link;
				}));
		}

		public static IRuleAppender Images(this IRuleAppender appender)
		{
			return appender.With(e => e.Element("img")
				.As(AllowFlags | HtmlElementOptions.SelfClosing)
				.Convert(elem => {
					if (elem.HasAttribute("alt"))
						elem.SetAttribute("title", elem.GetAttribute("alt"));

					return elem;
				})
				.With(a => a.Attribute("src").Validate("#url"))
				.With(a => a.Attribute("width").Validate("#int"))
				.With(a => a.Attribute("height").Validate("#int"))
				.With(a => a.Attribute("title")));
		}

		public static IRuleAppender Links(this IRuleAppender appender, string domain, string redirectUrl)
		{
			return appender.With(e => e.Element("a")
				.As(AllowFlags | HtmlElementOptions.Text)
				.Convert(elem => {
					if (elem.InnerText.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
						elem.InnerText.Length > 30)
					{
						elem.SetAttribute("title", elem.InnerText);
						elem.InnerXml = elem.InnerText.Substring(0, 30) + "…";
					}
					return elem.CrossDomainLink(domain, redirectUrl);
				})
				.With(a => a.Attribute("href").Validate("#url"))
				.With(a => a.Attributes("rel", "target"))
				.With(a => a.Attribute("title")));
		}

		public static IRuleAppender RegularContent(this IRuleAppender appender)
		{
			return appender.With(e => e.Elements("i", "b", "u", "em", "strong", "acronym", "h1", "h2", "h3", "h4", "h5", "h6")
				.As(AllowFlags | HtmlElementOptions.Text | HtmlElementOptions.UseTypography))
				.With(e => e.Element("p").As(AllowFlags | HtmlElementOptions.AllowContent | HtmlElementOptions.UseTypography)
					.With(a => a.Attribute("align").Validate("left", "right", "center")))
				.With(e => e.Element("nobr").As(AllowFlags | HtmlElementOptions.Text | HtmlElementOptions.UseTypography))
				.With(e => e.Element("ul").As(AllowFlags | HtmlElementOptions.Container)
					.With(l => l.Element("li").As(HtmlElementOptions.Allowed | HtmlElementOptions.Text | HtmlElementOptions.UseTypography)))
				.With(e => e.Elements("code", "pre").As(AllowFlags | HtmlElementOptions.Preformatted))
				.With(e => e.Element("br").As(AllowFlags | HtmlElementOptions.SelfClosing))
				.With(e => e.Elements("script", "iframe").As(HtmlElementOptions.Denied | HtmlElementOptions.Recursive));
		}

		public static HtmlStringTypographer Replaces(this HtmlStringTypographer item)
		{
			return item.Replace("(c)", "&copy;")
				.Replace("(r)", "&reg;")
				.Replace("(tm)", "&trade;")
				.Replace("+/-", "&plusmn;")
				.Replace("+-", "&plusmn;")
				.Replace(@"(?'number'\d+)\s*\^\s*(?'power'\-?\d+((\.|\,)\d+)?)",
					"${number}<sup>${power}</sup>", StringReplacementMode.Regex)
				.Replace(@"(?'before'\d+)\s*x\s*(?'after'\-?\d+)",
					"${before}&times;${after}", StringReplacementMode.Regex)
				.Replace(@"(?'before'\d+)\s*\*\s*(?'after'\-?\d+)",
					"${before}&times;${after}", StringReplacementMode.Regex);
		}
		#endregion
	}
}
