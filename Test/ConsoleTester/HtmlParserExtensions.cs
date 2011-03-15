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
		private static XmlElement CreateElement(this HtmlElementContext context, string name)
		{
			XmlDocument document = context.Element.OwnerDocument;
			return document.CreateElement(name);
		}

		private static XmlAttribute CreateAttribute(this HtmlElementContext context, string name)
		{
			XmlDocument document = context.Element.OwnerDocument;
			return document.CreateAttribute(name);
		}

		private static XmlText CreateTextNode(this HtmlElementContext context, string text)
		{
			XmlDocument document = context.Element.OwnerDocument;
			return document.CreateTextNode(text);
		}

		private static XmlElement AppendChild(this XmlElement element, string name)
		{
			XmlElement child = element.OwnerDocument.CreateElement(name);
			element.AppendChild(child);

			return child;
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

		public static HtmlConverterResult<XmlElement> FlashPlayer(
			this HtmlElementContext context, string movie, 
			int width, int height, string variables)
		{
			XmlElement video = context.CreateElement("object");

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
			return context.Result(video);
		}

		public static HtmlConverterResult<XmlElement> CrossDomainLink(this HtmlElementContext context)
		{
			string domain = context.Parameters.GetValue<string>("link-domain", String.Empty);
			string redirect = context.Parameters.GetValue<string>("link-redirect");

			string location = context.Element.GetAttribute("href");
			context.Element.RemoveAttribute("target");

			Uri uri;
			if (!Uri.TryCreate(location, UriKind.Absolute, out uri))
				if (!Uri.TryCreate("http://" + location, UriKind.Absolute, out uri))
					return context.Cancel();

			if (!uri.Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
			{
				if (!String.IsNullOrEmpty(redirect))
					context.Element.SetAttribute("href", String.Format("{0}?url={1}", redirect, 
						Uri.EscapeDataString(uri.AbsoluteUri)));

				context.Element.SetAttribute("target", "_blank");
				context.Element.SetAttribute("rel", "nofollow");
			}
			return context.Result();
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
				.Convert(context => {
					string location = context.Element.GetAttribute("src");
					if (String.IsNullOrEmpty(location))
						return context.Cancel();

					return context.FlashPlayer(location, 640, 360, "");
				}));
		}

		public static IRuleAppender Abstract(this IRuleAppender appender)
		{
			return appender.Treat(e => e.Element("cut").As(AllowFlags | HtmlElementOptions.SelfClosing)
				.Convert(context => {
					XmlElement link = context.CreateElement("a");

					string url = context.Parameters.GetValue<string>("url", String.Empty);

					link.SetAttribute("href", url);
					link.AppendChild(context.CreateTextNode(String.Format("{0} →", 
						context.Element.GetAttribute("title").Define("читать далее"))
					));
					return context.Result(link);
				}));
		}

		public static IRuleAppender Images(this IRuleAppender appender)
		{
			return appender.Treat(e => e.Element("img")
				.As(AllowFlags | HtmlElementOptions.SelfClosing)
				.Convert(context => {
					if (context.Element.HasAttribute("alt"))
						context.Element.SetAttribute("title", context.Element.GetAttribute("alt"));

					return context.Result();
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
				.Convert(context => {
					XmlElement element = context.Element;
					if (element.InnerText.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
						element.InnerText.Length > 30)
					{
						element.SetAttribute("title", element.InnerText);
						element.InnerXml = element.InnerText.Substring(0, 30) + "…";
					}
					return context.CrossDomainLink();
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

		public static HtmlTypographer Replaces(this HtmlTypographer item)
		{
			return item.Replace(@"\(c\)", "&copy;", RegexOptions.IgnoreCase)
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
