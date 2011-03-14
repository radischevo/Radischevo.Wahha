using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    /// <summary>
    /// Provides an XML document processing 
    /// using the specified parsing rules.
    /// </summary>
    public class HtmlStringParser : IRuleSelector, IRuleAppender
    {
        #region Instance Fields
        private HtmlElementRule _document;
        private HtmlElementOptions _defaultElementFlags;
        private HtmlFilteringMode _processingMode;
        private HtmlStringTypographer _typographer;
        private bool _preserveWhitespace;
        #endregion

        #region Constructors
        public HtmlStringParser()
            : this(HtmlFilteringMode.DenyByDefault)
        {
        }

        public HtmlStringParser(HtmlFilteringMode processingMode)
        {
            _document = new HtmlElementRule(null, "html",
                HtmlElementOptions.Allowed | HtmlElementOptions.AllowContent |
                HtmlElementOptions.UseTypography);

            _preserveWhitespace = true;
            _processingMode = processingMode;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets or sets the <see cref="Radischevo.Wahha.Web.Text.HtmlFilteringMode"/> 
        /// which will be used to determine whether the particular HTML node is allowed 
        /// within the document.
        /// </summary>
        public HtmlFilteringMode ProcessingMode
        {
            get
            {
                return _processingMode;
            }
            set
            {
                _processingMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the default rule set, which will be applied 
        /// to an element, if no specific rule is found.
        /// </summary>
        public HtmlElementOptions DefaultElementFlags
        {
            get
            {
                return _defaultElementFlags;
            }
            set
            {
                _defaultElementFlags = value;
            }
        }

        /// <summary>
        /// Specifies how whitespace is handled.
        /// </summary>
        public bool PreserveWhitespace
        {
            get
            {
                return _preserveWhitespace;
            }
            set
            {
                _preserveWhitespace = value;
            }
        }

        /// <summary>
        /// Gets the root node of the element rule tree.
        /// </summary>
        public HtmlElementRule Document
        {
            get
            {
                return _document;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Radischevo.Wahha.Web.Text.HtmlStringTypographer"/> 
        /// which is used to process the contents of the text nodes.
        /// </summary>
        public HtmlStringTypographer Typographer
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

        /// <summary>
        /// Gets the top-level element rule collection 
        /// used by this instance.
        /// </summary>
        public HtmlElementRuleCollection Elements
        {
            get
            {
                return _document.Children;
            }
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
			HtmlElementOptions defaultFlags = DefaultElementFlags | ((_processingMode == HtmlFilteringMode.AllowByDefault) ?
                HtmlElementOptions.SelfClosing | HtmlElementOptions.Allowed | HtmlElementOptions.AllowContent :
                HtmlElementOptions.Denied);

            return GetRuleOrDefault(parent, tag, defaultFlags);
        }

        protected HtmlAttributeRule GetAttributeRule(HtmlElementRule parent, string attribute)
        {
			bool isInternal = ((parent.Flags & HtmlElementOptions.Internal) == HtmlElementOptions.Internal);

			HtmlAttributeOptions flags = (_processingMode == HtmlFilteringMode.AllowByDefault || isInternal) ?
                HtmlAttributeOptions.Allowed : HtmlAttributeOptions.Denied;

            return parent.Attributes[attribute] ?? _document.Attributes[attribute] ?? 
                new HtmlAttributeRule(_document, attribute, flags);
        }

        protected HtmlElementRule GetRule(string tag)
        {
            if (String.Equals(tag, "html",
                StringComparison.OrdinalIgnoreCase))
                return Document;

            return Document.Children[tag];
        }

        protected HtmlElementRule GetRule(HtmlElementRule parent, string tag)
        {
            HtmlElementRule tagRule;

            // если родитель есть, ищем непосредственного потомка у него
            if (parent != null)
            {
                tagRule = parent.Children[tag];
                if (tagRule != null) // если он есть, возвращаем
                    return tagRule;
            }
            // если мы тут, нет ни родителя, ни потомка, зрим в корень...
            tagRule = GetRule(tag);
            if (tagRule != null && (tagRule.Flags & HtmlElementOptions.Recursive)
                == HtmlElementOptions.Recursive) // и ищем рекурсивные правила
                return tagRule;

            return null;
        }

        protected HtmlElementRule GetRuleOrDefault(string tag, HtmlElementOptions flags)
        {
            return GetRule(tag) ?? new HtmlElementRule(Document, tag, flags);
        }

        protected HtmlElementRule GetRuleOrDefault(HtmlElementRule parent, string tag, HtmlElementOptions flags)
        {
            parent = parent ?? _document;
            return GetRule(parent, tag) ?? new HtmlElementRule(parent, tag, flags);
        }
        #endregion

        #region Processing Methods
        /// <summary>
        /// Processes the provided XML document using the specified 
        /// parsing rules and writes the resulting text into the provided 
        /// <see cref="System.Xml.XmlWriter"/>.
        /// </summary>
        /// <param name="document">An <see cref="System.Xml.XmlDocument"/> to process.</param>
        /// <param name="writer">An <see cref="System.Xml.XmlWriter"/> to write into.</param>
        public virtual void ProcessDocument(XmlElement document, XmlWriter writer)
        {
            Precondition.Require(writer, () => Error.ArgumentNull("writer"));
            if (document == null) // в случае загрузки пустого XML
                return;

            foreach (XmlNode node in document)
                WriteNode(Document, node, writer);
        }

        protected void WriteNode(HtmlElementRule parent, XmlNode node, XmlWriter writer)
        {
            if (parent == null)
                return;

            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    if ((parent.Flags & HtmlElementOptions.Container) == HtmlElementOptions.Container)
                        WriteElement(parent, (XmlElement)node, writer);
                    break;
                case XmlNodeType.CDATA:
                    if ((parent.Flags & HtmlElementOptions.Text) == HtmlElementOptions.Text)
                        WriteCData(parent, node.Value, writer);
                    break;
                case XmlNodeType.Comment:
                    if ((parent.Flags & HtmlElementOptions.Text) == HtmlElementOptions.Text)
                        WriteComment(parent, node.Value, writer);
                    break;
                case XmlNodeType.Text:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    if ((parent.Flags & HtmlElementOptions.Text) == HtmlElementOptions.Text)
                        WriteText(parent, node.Value, writer);
                    break;
            }
        }

        protected XmlElement ConvertElement(HtmlElementRule parent, 
            XmlElement element, out HtmlElementRule rule)
        {
            Precondition.Require(element, () => Error.ArgumentNull("element"));

            rule = GetElementRule(parent, element.LocalName);
            if (rule.Converter != null)
            {
                element = rule.Converter(element);
				if (element == null)
					return null;

                if (!MatchName(element, rule))
                    rule = GetElementRule(parent, element.LocalName);

				if ((rule.Flags & HtmlElementOptions.Internal) == HtmlElementOptions.Internal)
				{
					rule = rule.Clone();
					rule.Flags |= HtmlElementOptions.Allowed;
				}
            }
            return element;
        }

        protected virtual void WriteElement(HtmlElementRule parent, 
            XmlElement element, XmlWriter writer)
        {
            HtmlElementRule rule;
            element = ConvertElement(parent, element, out rule);

			if (element == null)
				return;
			
			WriteStartElement(rule, element, writer);
			WriteElementContents(rule, element, writer);
			WriteEndElement(rule, element, writer);
        }

        protected void WriteStartElement(HtmlElementRule rule, XmlElement element, XmlWriter writer)
        {
            if ((rule.Flags & HtmlElementOptions.Allowed) == HtmlElementOptions.Allowed)
            {
                writer.WriteStartElement(element.LocalName);
                WriteAttributes(rule, element.Attributes, writer);
            }
        }

        protected void WriteEndElement(HtmlElementRule rule, XmlElement element, XmlWriter writer)
        {
            if ((rule.Flags & HtmlElementOptions.Allowed) == HtmlElementOptions.Allowed)
            {
                if ((rule.Flags & HtmlElementOptions.SelfClosing) == HtmlElementOptions.SelfClosing)
                    writer.WriteEndElement();
                else
                    writer.WriteFullEndElement();
            }
        }

        protected void WriteElementContents(HtmlElementRule rule,
            XmlElement element, XmlWriter writer)
        {
            if ((rule.Flags & HtmlElementOptions.Preformatted) == HtmlElementOptions.Preformatted)
                WriteText(rule, element.InnerXml, writer);
            else
                foreach (XmlNode node in element)
                    WriteNode(rule, node, writer);
        }

        protected void WriteAttributes(HtmlElementRule rule,
            XmlAttributeCollection attributes, XmlWriter writer)
        {
            HashSet<string> usedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string attr = null;

            foreach (XmlAttribute attribute in attributes)
                if ((attr = WriteAttribute(rule, attribute, writer)) != null)
                    usedAttributes.Add(attr);

            EnsureRequiredAttributes(rule, usedAttributes, writer);
        }

        protected void EnsureRequiredAttributes(HtmlElementRule rule,
            HashSet<string> usedAttributes, XmlWriter writer)
        {
            foreach (HtmlAttributeRule attr in rule.Attributes)
            {
                if ((attr.Flags & HtmlAttributeOptions.Required) == HtmlAttributeOptions.Required
                    && !usedAttributes.Contains(attr.Name))
                    writer.WriteAttributeString(attr.Name, attr.DefaultValue);
            }
        }

        protected virtual string WriteAttribute(HtmlElementRule rule,
            XmlAttribute attribute, XmlWriter writer)
        {
            HtmlAttributeRule attrRule = GetAttributeRule(rule, attribute.LocalName);
            if (attrRule.Converter != null)
            {
                attribute = attrRule.Converter(attribute);
				if (attribute == null)
					return null;

                if(!MatchName(attribute, attrRule))
                    attrRule = GetAttributeRule(rule, attribute.LocalName);

				if ((attrRule.Flags & HtmlAttributeOptions.Internal) == HtmlAttributeOptions.Internal)
				{
					attrRule = attrRule.Clone();
					attrRule.Flags |= HtmlAttributeOptions.Allowed;
				}
            }
            string value = attribute.Value;

            if ((attrRule.Flags & HtmlAttributeOptions.Allowed) == HtmlAttributeOptions.Allowed)
            {
                if ((attrRule.Flags & HtmlAttributeOptions.Default) == HtmlAttributeOptions.Default &&
                    String.IsNullOrEmpty(value))
                    value = attrRule.DefaultValue;

                if (attrRule.ValidateValue(value))
                {
                    writer.WriteAttributeString(attribute.LocalName, value);
                    return attribute.LocalName;
                }
            }
            return null;
        }

        protected virtual void WriteCData(HtmlElementRule rule, string text, XmlWriter writer)
        {
            writer.WriteCData(text);
        }

        protected virtual void WriteComment(HtmlElementRule rule, string text, XmlWriter writer)
        {
            writer.WriteComment(text);
        }

        protected virtual void WriteText(HtmlElementRule rule, string text, XmlWriter writer)
        {
            if ((rule.Flags & HtmlElementOptions.UseTypography) == HtmlElementOptions.UseTypography &&
                _typographer != null)
            {
                _typographer.Formatter = (element, mode) => FormatElement(rule, element, mode);
                
                // an ugly hack ;)
                bool lastValue = _typographer.InsertNoBreakTags;
                if (String.Equals(rule.Name, "nobr", StringComparison.OrdinalIgnoreCase))
                    _typographer.InsertNoBreakTags = false;

                writer.WriteRaw(_typographer.Parse(text));
                _typographer.InsertNoBreakTags = lastValue;
            }
            else
                writer.WriteString(text);
        }

        protected string FormatElement(HtmlElementRule parent, 
            HtmlElementBuilder element, HtmlElementRenderMode renderMode)
        {
            XmlDocument dummy = new XmlDocument();
			XmlElement container = dummy.CreateElement(parent.Name);

            XmlElement xe = dummy.CreateElement(element.Name);
			container.AppendChild(xe);
            
            foreach(KeyValuePair<string, object> attr in element.Attributes)
                xe.Attributes.Append(dummy.CreateAttribute(attr.Key.ToLowerInvariant())).Value = 
                    (attr.Value == null) ? String.Empty : attr.Value.ToString();

            xe.InnerXml = element.InnerHtml ?? String.Empty;

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(sw))
                {
                    HtmlElementRule rule;
                    xe = ConvertElement(parent, xe, out rule);
                    StringBuilder sb = sw.GetStringBuilder();

                    WriteStartElement(rule, xe, writer);
                    if (renderMode == HtmlElementRenderMode.StartTag)
                        return sb.Append('>').ToString();

                    WriteElementContents(rule, xe, writer);

                    if (renderMode == HtmlElementRenderMode.EndTag)
                    {
                        sb.Length = 0;

                        HtmlElementOptions flags = rule.Flags;
                        if ((rule.Flags & HtmlElementOptions.SelfClosing) ==
                            HtmlElementOptions.SelfClosing)
                            rule.Flags = (HtmlElementOptions)((byte)rule.Flags - 0x04);

                        WriteEndElement(rule, xe, writer);
                        rule.Flags = flags;
                        
                        if(sb.Length > 0 && sb[0] == '>')
                            sb.Remove(0, 1);
                    }
                    else
                        WriteEndElement(rule, xe, writer);
                }
                return sw.ToString();
            }
        }
        #endregion

        #region Rule Specific Methods
        /// <summary>
        /// Adds a top-level rule for the element.
        /// </summary>
        /// <param name="rule">A new sub-rule to add.</param>
        protected virtual HtmlElementRule AddElementRule(HtmlElementRule rule)
        {
            Precondition.Require(rule, () => Error.ArgumentNull("rule"));
            return Document.AddElementRule(rule);
        }

        /// <summary>
        /// Adds a top-level rule for the attribute. In most cases, the rule is ignored.
        /// </summary>
        /// <param name="rule">A new sub-rule to add.</param>
        protected virtual HtmlAttributeRule AddAttributeRule(HtmlAttributeRule rule)
        {
            Precondition.Require(rule, () => Error.ArgumentNull("rule"));
            return Document.AddAttributeRule(rule);
        }

        private void AddElementRules(IEnumerable<HtmlElementRule> collection)
        {
            foreach (HtmlElementRule rule in collection)
                AddElementRule(rule);
        }

        private void AddAttributeRules(IEnumerable<HtmlAttributeRule> collection)
        {
            foreach (HtmlAttributeRule rule in collection)
                AddAttributeRule(rule);
        }
        
        /// <summary>
        /// Adds a top-level rule for the element.
        /// </summary>
        /// <param name="builder">The selector function.</param>
        public IRuleAppender Treat(Func<IRuleSelector, IRuleBuilder> builder)
        {
            Precondition.Require(builder, () => Error.ArgumentNull("inner"));
            IRuleBuilder rule = builder(this);

            HtmlElementRule elem = (rule as HtmlElementRule);
            HtmlAttributeRule attr = (rule as HtmlAttributeRule);
            HtmlElementRuleCollection ec = (rule as HtmlElementRuleCollection);
            HtmlAttributeRuleCollection ac = (rule as HtmlAttributeRuleCollection);

            if (elem != null)
                AddElementRule(elem);
            else if (ec != null)
                AddElementRules(ec);
            else if (attr != null)
                AddAttributeRule(attr);
            else if (ac != null)
                AddAttributeRules(ac);
            else
                throw Error.UnsupportedElementRule("builder");

            return this;
        }
        #endregion

        #region Fluent Interface Implementation
		IRuleAppender IRuleAppender.With(Func<IRuleSelector, IRuleBuilder> builder)
		{
			return Treat(builder);
		}

        IFluentElementRule IRuleSelector.Element(string name)
        {
            return new HtmlElementRule(Document, name, HtmlElementOptions.Allowed |
                HtmlElementOptions.AllowContent | HtmlElementOptions.SelfClosing);
        }

        IFluentAttributeRule IRuleSelector.Attribute(string name)
        {
            return new HtmlAttributeRule(Document, name, HtmlAttributeOptions.Allowed);
        }

        IFluentElementRule IRuleSelector.Elements(params string[] names)
        {
            Precondition.Require(names, () => Error.ArgumentNull("names"));
            HtmlElementRuleCollection collection = new HtmlElementRuleCollection();

            foreach (string name in names)
                collection.Add(new HtmlElementRule(Document, name, HtmlElementOptions.Allowed |
                    HtmlElementOptions.AllowContent | HtmlElementOptions.SelfClosing));

            return collection;
        }

        IFluentAttributeRule IRuleSelector.Attributes(params string[] names)
        {
            Precondition.Require(names, () => Error.ArgumentNull("names"));
            HtmlAttributeRuleCollection collection = new HtmlAttributeRuleCollection();

            foreach (string name in names)
                collection.Add(new HtmlAttributeRule(Document, name, HtmlAttributeOptions.Allowed));

            return collection;
        }
        #endregion
    }
}
