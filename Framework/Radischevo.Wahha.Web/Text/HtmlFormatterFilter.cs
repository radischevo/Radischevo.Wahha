using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Radischevo.Wahha.Web.Text
{
	internal sealed class HtmlFormatterFilter : HtmlFilter
	{
		#region Instance Fields
		private StringBuilder _contents;
		private HtmlElementRule _container;
		private HtmlElementBuilder _element;
		private HtmlElementRenderMode _renderMode;
		#endregion

		#region Constructors
		public HtmlFormatterFilter(HtmlFilterSettings settings, 
			HtmlElementRule container, HtmlElementBuilder element, 
			StringWriter output)
			: base(TextReader.Null, output, settings)
		{
			_contents = output.GetStringBuilder();
			_container = container;
			_element = element;
		}
		#endregion

		#region Instance Properties
		public HtmlElementRenderMode RenderMode
		{
			get
			{
				return _renderMode;
			}
			set
			{
				_renderMode = value;
			}
		}
		#endregion

		#region Instance Methods
		private XmlElement CreateContainer()
		{
			XmlDocument dummy = new XmlDocument();
			XmlElement container = dummy.CreateElement(_container.Name);
			XmlElement element = dummy.CreateElement(_element.Name);

			foreach (KeyValuePair<string, object> attr in _element.Attributes)
				element.SetAttribute(attr.Key, (attr.Value == null) ? 
					String.Empty : attr.Value.ToString());

			element.InnerXml = _element.InnerHtml ?? String.Empty;
			container.AppendChild(element);

			return element;
		}

		public override void Execute()
		{
			XmlElement element = CreateContainer();
			HtmlElementResult result = ConvertElement(_container, element);
			if (result == null)
				return;

			HtmlElementRule rule = result.Rule;
			WriteStartElement(rule, result.Element);

			if (_renderMode == HtmlElementRenderMode.StartTag)
			{
				_contents.Append('>');
				return;
			}

			WriteElementContents(rule, result.Element);

			if (_renderMode == HtmlElementRenderMode.EndTag)
			{
				_contents.Length = 0;

				HtmlElementOptions options = rule.Options;
				if ((rule.Options & HtmlElementOptions.SelfClosing) ==
					HtmlElementOptions.SelfClosing)
					rule.Options = (HtmlElementOptions)((byte)rule.Options - 0x04);

				WriteEndElement(rule, result.Element);
				rule.Options = options;

				if (_contents.Length > 0 && _contents[0] == '>')
					_contents.Remove(0, 1);
			}
			else
				WriteEndElement(rule, result.Element);
		}
		#endregion
	}
}
