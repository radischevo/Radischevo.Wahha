﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Text.Sgml;

namespace Radischevo.Wahha.Web.Text
{
	/// <summary>
	/// Transforms the provided HTML string using 
	/// the specified parsing rules and 
	/// adds typographic beautification.
	/// </summary>
	public class HtmlFilter : IDisposable
	{
		#region Instance Fields
		private HtmlFilterSettings _settings;
		private HtmlTypographer _typographer;
		private IValueSet _parameters;
		private XmlReader _reader;
		private XmlWriter _writer;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Web.Text.HtmlFilter"/> class.
		/// </summary>
		public HtmlFilter(HtmlFilterSettings settings, 
			TextReader reader, TextWriter writer)
		{
			Precondition.Require(settings, () => Error.ArgumentNull("settings"));
			Precondition.Require(reader, () => Error.ArgumentNull("reader"));
			Precondition.Require(writer, () => Error.ArgumentNull("writer"));

			_settings = settings;
			_reader = CreateHtmlReader(reader, settings);
			_writer = new XmlTextWriter(writer);
		}
		#endregion

		#region Instance Properties
		public HtmlTypographer Typographer
		{
			get
			{
				return _typographer;
			}
			set
			{
				_typographer = value;
			}
		}

		public IValueSet Parameters
		{
			get
			{
				if (_parameters == null)
					_parameters = new ValueDictionary();

				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}

		public HtmlFilterSettings Settings
		{
			get
			{
				return _settings;
			}
		}

		protected XmlReader Reader
		{
			get
			{
				return _reader;
			}
		}

		protected XmlWriter Writer
		{
			get
			{
				return _writer;
			}
		}
		#endregion

		#region Static Methods
		private static XmlReader CreateHtmlReader(TextReader reader, HtmlFilterSettings settings)
		{
			SgmlReader sgml = new SgmlReader();
			sgml.DocType = "html";
			sgml.WhitespaceHandling = (settings.PreserveWhitespace) ?
				WhitespaceHandling.All : WhitespaceHandling.None;
			sgml.CaseFolding = CaseFolding.ToLower;
			sgml.InputStream = reader;

			return sgml;
		}
		#endregion

		#region Helper Methods
		protected bool MatchName(XmlElement element, HtmlElementRule rule)
		{
			return String.Equals(element.LocalName, rule.Name, StringComparison.OrdinalIgnoreCase);
		}

		protected bool MatchName(XmlAttribute attribute, HtmlAttributeRule rule)
		{
			return String.Equals(attribute.LocalName, rule.Name, StringComparison.OrdinalIgnoreCase);
		}

		protected HtmlElementRule GetElementRule(HtmlElementRule parent, string tag)
		{
			HtmlElementOptions defaultFlags = _settings.DefaultOptions | 
				((_settings.Mode == HtmlFilteringMode.AllowByDefault) 
				? HtmlElementOptions.SelfClosing | HtmlElementOptions.Allowed | HtmlElementOptions.AllowContent 
				: HtmlElementOptions.Denied);

			return GetRuleOrDefault(parent, tag, defaultFlags);
		}

		protected HtmlAttributeRule GetAttributeRule(HtmlElementRule parent, string attribute)
		{
			bool isInternal = ((parent.Options & HtmlElementOptions.Internal) == HtmlElementOptions.Internal);

			HtmlAttributeOptions flags = (_settings.Mode == HtmlFilteringMode.AllowByDefault || isInternal) 
				? HtmlAttributeOptions.Allowed : HtmlAttributeOptions.Denied;

			return parent.Attributes[attribute] ?? _settings.Document.Attributes[attribute] ??
				new HtmlAttributeRule(_settings.Document, attribute, flags);
		}

		protected HtmlElementRule GetRule(string tag)
		{
			if (String.Equals(tag, "html",
				StringComparison.OrdinalIgnoreCase))
				return _settings.Document;

			return _settings.Document.Children[tag];
		}

		protected HtmlElementRule GetRule(HtmlElementRule parent, string tag)
		{
			HtmlElementRule tagRule;
			if (parent != null)
			{
				tagRule = parent.Children[tag];
				if (tagRule != null)
					return tagRule;
			}
			tagRule = GetRule(tag);
			if (tagRule != null && (tagRule.Options & HtmlElementOptions.Recursive)
				== HtmlElementOptions.Recursive)
				return tagRule;

			return null;
		}

		protected HtmlElementRule GetRuleOrDefault(string tag, HtmlElementOptions options)
		{
			return GetRule(tag) ?? new HtmlElementRule(_settings.Document, tag, options);
		}

		protected HtmlElementRule GetRuleOrDefault(HtmlElementRule parent, string tag, HtmlElementOptions options)
		{
			parent = parent ?? _settings.Document;
			return GetRule(parent, tag) ?? new HtmlElementRule(parent, tag, options);
		}
		#endregion

		#region Processing Methods
		/// <summary>
		/// Processes an input string using the specified 
		/// parsing and typographic rules.
		/// </summary>
		public virtual void Execute()
		{
			XmlDocument document = new XmlDocument();
			document.PreserveWhitespace = _settings.PreserveWhitespace;
			document.Load(Reader);

			if (document.DocumentElement == null)
				return;

			WriteElementContents(_settings.Document, document.DocumentElement);
		}

		protected void WriteElementContents(HtmlElementRule rule, XmlElement element)
		{
			if ((rule.Options & HtmlElementOptions.Preformatted) == HtmlElementOptions.Preformatted)
				WriteText(rule, element.InnerXml);
			else 
				foreach (XmlNode node in element)
					WriteNode(rule, node);
		}

		protected void WriteNode(HtmlElementRule parent, XmlNode node)
		{
			if (parent == null)
				return;

			switch (node.NodeType)
			{
				case XmlNodeType.Element:
					if ((parent.Options & HtmlElementOptions.Container) == HtmlElementOptions.Container)
						WriteElement(parent, (XmlElement)node);
					break;
				case XmlNodeType.CDATA:
					if ((parent.Options & HtmlElementOptions.Text) == HtmlElementOptions.Text)
						WriteCData(parent, node.Value);
					break;
				case XmlNodeType.Comment:
					if ((parent.Options & HtmlElementOptions.Text) == HtmlElementOptions.Text)
						WriteComment(parent, node.Value);
					break;
				case XmlNodeType.Text:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					if ((parent.Options & HtmlElementOptions.Text) == HtmlElementOptions.Text)
						WriteText(parent, node.Value);
					break;
			}
		}

		protected virtual void WriteElement(HtmlElementRule parent, XmlElement element)
		{
			HtmlElementContext context = ConvertElement(parent, element);
			if (context == null || context.Cancelled)
				return;
			
			WriteStartElement(context.Rule, context.Element);
			WriteElementContents(context.Rule, context.Element);
			WriteEndElement(context.Rule, context.Element);
		}

		protected HtmlElementContext ConvertElement(HtmlElementRule parent, XmlElement element)
		{
			Precondition.Require(element, () => Error.ArgumentNull("element"));
			HtmlElementRule rule = GetElementRule(parent, element.LocalName);
			HtmlElementContext context = new HtmlElementContext(rule, element, Parameters);

			if (rule.HasConverter)
			{
				HtmlConverterResult<XmlElement> result = rule.Converter(context);
				if (result.IsEmpty)
					return null;

				if (!MatchName(result.Element, context.Rule))
					rule = GetElementRule(parent, result.Element.LocalName);

				if ((rule.Options & HtmlElementOptions.Internal) == HtmlElementOptions.Internal)
				{
					rule = rule.Clone();
					rule.Options |= HtmlElementOptions.Allowed;
				}
				return new HtmlElementContext(rule, result.Element);
			}
			return context;
		}

		protected void WriteStartElement(HtmlElementRule rule, XmlElement element)
		{
			if ((rule.Options & HtmlElementOptions.Allowed) == HtmlElementOptions.Allowed)
			{
				_writer.WriteStartElement(element.LocalName);
				WriteAttributes(rule, element.Attributes);
			}
		}

		protected void WriteEndElement(HtmlElementRule rule, XmlElement element)
		{
			if ((rule.Options & HtmlElementOptions.Allowed) == HtmlElementOptions.Allowed)
			{
				if ((rule.Options & HtmlElementOptions.SelfClosing) == HtmlElementOptions.SelfClosing)
					_writer.WriteEndElement();
				else
					_writer.WriteFullEndElement();
			}
		}

		protected void WriteAttributes(HtmlElementRule rule, XmlAttributeCollection attributes)
		{
			HashSet<string> usedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			XmlAttribute attr = null;

			foreach (XmlAttribute attribute in attributes)
				if ((attr = WriteAttribute(rule, attribute)) != null)
					usedAttributes.Add(attr.LocalName);

			EnsureRequiredAttributes(rule, usedAttributes);
		}

		protected virtual XmlAttribute WriteAttribute(HtmlElementRule element, XmlAttribute attribute)
		{
			HtmlAttributeContext context = ConvertAttribute(element, attribute);
			if (context == null || context.Cancelled)
				return null;

			string value = context.Attribute.Value;
			if ((context.Rule.Options & HtmlAttributeOptions.Allowed) == HtmlAttributeOptions.Allowed)
			{
				if ((context.Rule.Options & HtmlAttributeOptions.Default) == HtmlAttributeOptions.Default &&
					String.IsNullOrEmpty(value))
					value = context.Rule.DefaultValue;

				if (context.Rule.ValidateValue(value))
				{
					_writer.WriteAttributeString(context.Attribute.LocalName, value);
					return context.Attribute;
				}
			}
			return null;
		}

		protected HtmlAttributeContext ConvertAttribute(HtmlElementRule element, XmlAttribute attribute)
		{
			HtmlAttributeRule attrRule = GetAttributeRule(element, attribute.LocalName);
			HtmlAttributeContext context = new HtmlAttributeContext(attrRule, attribute, Parameters);

			if (attrRule.HasConverter)
			{
				HtmlConverterResult<XmlAttribute> result = attrRule.Converter(context);
				if (result.IsEmpty)
					return null;

				if (!MatchName(result.Element, attrRule))
					attrRule = GetAttributeRule(element, result.Element.LocalName);

				if ((attrRule.Options & HtmlAttributeOptions.Internal) == HtmlAttributeOptions.Internal)
				{
					attrRule = attrRule.Clone();
					attrRule.Options |= HtmlAttributeOptions.Allowed;
				}
				return new HtmlAttributeContext(attrRule, result.Element);
			}
			return context;
		}

		protected void EnsureRequiredAttributes(HtmlElementRule rule, HashSet<string> usedAttributes)
		{
			foreach (HtmlAttributeRule attr in rule.Attributes)
			{
				if ((attr.Options & HtmlAttributeOptions.Required) == HtmlAttributeOptions.Required
					&& !usedAttributes.Contains(attr.Name))
					_writer.WriteAttributeString(attr.Name, attr.DefaultValue);
			}
		}

		protected virtual void WriteCData(HtmlElementRule rule, string text)
		{
			_writer.WriteCData(text);
		}

		protected virtual void WriteComment(HtmlElementRule rule, string text)
		{
			_writer.WriteComment(text);
		}

		protected virtual void WriteText(HtmlElementRule rule, string text)
		{
			if ((rule.Options & HtmlElementOptions.UseTypography) == 
				HtmlElementOptions.UseTypography && _typographer != null)
			{
				_typographer.Formatter = (element, mode) => FormatElement(rule, element, mode);

				bool lastValue = _typographer.InsertNoBreakTags;
				if (String.Equals(rule.Name, "nobr", StringComparison.OrdinalIgnoreCase))
					_typographer.InsertNoBreakTags = false;

				_writer.WriteRaw(_typographer.Parse(text));
				_typographer.InsertNoBreakTags = lastValue;
			}
			else
				_writer.WriteString(text);
		}

		protected string FormatElement(HtmlElementRule parent,
			HtmlElementBuilder element, HtmlElementRenderMode renderMode)
		{
			using (StringWriter writer = new StringWriter())
			{
				HtmlFormatterFilter filter = new HtmlFormatterFilter(
					_settings, parent, element, writer);
				
				filter.RenderMode = renderMode;
				filter.Execute();

				return writer.ToString();
			}
		}
		#endregion

		#region IDisposable Members
		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			IDisposable reader = (_reader as IDisposable);
			IDisposable writer = (_writer as IDisposable);

			if (reader != null)
				reader.Dispose();
			
			if (writer != null)
				writer.Dispose();
		}
		#endregion
	}
}
