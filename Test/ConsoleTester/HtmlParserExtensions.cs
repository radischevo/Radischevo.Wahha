using System;
using System.Collections.Generic;
using System.Xml;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Text;
using System.Text.RegularExpressions;

namespace ConsoleTester
{
	public static class HtmlParserExtensions
	{
		#region Constants
		private static HtmlElementOptions AllowFlags = HtmlElementOptions.Allowed | HtmlElementOptions.Recursive;
		private static HtmlElementOptions InternalFlags = HtmlElementOptions.Internal | HtmlElementOptions.Recursive;
		#endregion

		#region Extension Methods
		private static XmlElement AppendChild(this XmlElement element, string name)
		{
			XmlElement child = element.OwnerDocument.CreateElement(name);
			element.AppendChild(child);

			return child;
		}

		private static XmlElement AppendText(this XmlElement element, string text)
		{
			XmlText child = element.OwnerDocument.CreateTextNode(text);
			element.AppendChild(child);

			return element;
		}

		private static XmlElement AppendChild(this XmlElement element, string name, object attributes)
		{
			XmlElement child = AppendChild(element, name);
			ValueDictionary values = new ValueDictionary(attributes);

			foreach (KeyValuePair<string, object> pair in values)
				child.SetAttribute(pair.Key.ToLower(), (pair.Value == null) ? 
					String.Empty : pair.Value.ToString());
			
			return child;
		}

		public static XmlElement FlashPlayer(this HtmlElementContext context, 
			XmlElement element, string movie, int width, int height, string variables)
		{
			XmlElement video = element.OwnerDocument.CreateElement("object");

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

		public static XmlElement CrossDomainLink(this HtmlElementContext context, XmlElement element)
		{
			string domain = context.Parameters.GetValue<string>("link-domain", String.Empty);
			string redirect = context.Parameters.GetValue<string>("link-redirect");

			string location = element.GetAttribute("href");
			element.RemoveAttribute("target");

			Uri uri;
			if (!Uri.TryCreate(location, UriKind.Absolute, out uri))
				if (!Uri.TryCreate("http://" + location, UriKind.Absolute, out uri))
					return null;

			if (!uri.Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
			{
				if (!String.IsNullOrEmpty(redirect))
					element.SetAttribute("href", String.Format("{0}?url={1}", redirect, 
						Uri.EscapeDataString(uri.AbsoluteUri)));

				element.SetAttribute("target", "_blank");
				element.SetAttribute("rel", "nofollow");
			}
			return element;
		}

		public static IRuleAppender Youtube(this IRuleAppender appender)
		{
			return appender.Treat(e => e.Elements("object").As(InternalFlags | HtmlElementOptions.Container)
				.Treat(p => p.Element("param").As(HtmlElementOptions.Allowed | HtmlElementOptions.SelfClosing)
					.Treat(a => a.Attributes("name", "value").As(HtmlAttributeOptions.Allowed | HtmlAttributeOptions.Required))
					)
				.Treat(i => i.Element("embed").As(HtmlElementOptions.Allowed | HtmlElementOptions.SelfClosing | HtmlElementOptions.Internal))
				.Treat(a => a.Attributes("classid", "name").As(HtmlAttributeOptions.Allowed))
				.Treat(a => a.Attributes("width", "height").As(HtmlAttributeOptions.Allowed).Validate("#int")))
				.Treat(e => e.Element("youtube").As(AllowFlags | HtmlElementOptions.SelfClosing)
				.Convert((context, element) => {
					string location = element.GetAttribute("src");
					if (String.IsNullOrEmpty(location))
						return null;

					return context.FlashPlayer(element, location, 640, 360, "");
				}));
		}

		public static IRuleAppender Abstract(this IRuleAppender appender)
		{
			return appender.Treat(e => e.Element("cut").As(AllowFlags | HtmlElementOptions.SelfClosing)
				.Convert((context, element) => {
					XmlElement link = element.OwnerDocument.CreateElement("a");
					string url = context.Parameters.GetValue<string>("url", String.Empty);

					link.SetAttribute("href", url);
					link.AppendText(String.Format("{0} →", 
						element.GetAttribute("title").Define(s => !String.IsNullOrEmpty(s), "читать далее")
					));
					return link;
				}));
		}

		public static IRuleAppender Images(this IRuleAppender appender)
		{
			return appender.Treat(e => e.Element("img")
				.As(AllowFlags | HtmlElementOptions.SelfClosing)
				.Convert((context, element) => {
					if (element.HasAttribute("alt"))
						element.SetAttribute("title", element.GetAttribute("alt"));

					return element;
				})
				.Treat(a => a.Attribute("src").Validate("#url"))
				.Treat(a => a.Attribute("width").Validate("#int"))
				.Treat(a => a.Attribute("height").Validate("#int"))
				.Treat(a => a.Attribute("title")));
		}

		public static IRuleAppender Links(this IRuleAppender appender)
		{
			return appender.Treat(e => e.Element("a")
				.As(AllowFlags | HtmlElementOptions.Text)
				.Convert((context, element) => {
					if (element.InnerText.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
						element.InnerText.Length > 30)
					{
						element.SetAttribute("title", element.InnerText);
						element.InnerXml = element.InnerText.Substring(0, 30) + "…";
					}
					return context.CrossDomainLink(element);
				})
				.Treat(a => a.Attribute("href").Validate("#url"))
				.Treat(a => a.Attributes("rel", "target"))
				.Treat(a => a.Attribute("title")));
		}

		public static IRuleAppender RegularContent(this IRuleAppender appender)
		{
			return appender.Treat(e => e.Elements("i", "b", "u", "em", "strong", "acronym", "h1", "h2", "h3", "h4", "h5", "h6")
				.As(AllowFlags | HtmlElementOptions.Text | HtmlElementOptions.UseTypography))
				.Treat(e => e.Element("p").As(AllowFlags | HtmlElementOptions.AllowContent | HtmlElementOptions.UseTypography)
					.Treat(a => a.Attribute("align").Validate("left", "right", "center")))
				.Treat(e => e.Element("nobr").As(AllowFlags | HtmlElementOptions.Text | HtmlElementOptions.UseTypography))
				.Treat(e => e.Element("ul").As(AllowFlags | HtmlElementOptions.Container)
					.Treat(l => l.Element("li").As(HtmlElementOptions.Allowed | HtmlElementOptions.Text | HtmlElementOptions.UseTypography)))
				.Treat(e => e.Elements("code", "pre").As(AllowFlags | HtmlElementOptions.Preformatted))
				.Treat(e => e.Element("br").As(AllowFlags | HtmlElementOptions.SelfClosing))
				.Treat(e => e.Elements("script", "iframe").As(HtmlElementOptions.Denied | HtmlElementOptions.Recursive));
		}

		public static HtmlTypographerSettings Replaces(this HtmlTypographerSettings settings)
		{
			return settings.Replace(@"\(c\)", "&copy;", RegexOptions.IgnoreCase)
				.Replace(@"\(r\)", "®", RegexOptions.IgnoreCase)
				.Replace(@"\(tm\)", "™", RegexOptions.IgnoreCase)
				.Replace("+/-", "&plusmn;")
				.Replace("+-", "&plusmn;")
				.Replace(@"(?'number'\d+)\s*\^\s*(?'power'\-?\d+((\.|\,)\d+)?)",
					"${number}<sup>${power}</sup>", RegexOptions.IgnoreCase)
				.Replace(@"(?'before'\d+)\s*x\s*(?'after'\-?\d+)",
					"${before}&times;${after}", RegexOptions.IgnoreCase)
				.Replace(@"(?'before'\d+)\s*\*\s*(?'after'\-?\d+)",
					"${before}&times;${after}", RegexOptions.IgnoreCase);
		}
		#endregion
	}
}
