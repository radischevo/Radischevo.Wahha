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
        private HtmlElementFlags _defaultElementFlags;
        private HtmlProcessingMode _processingMode;
        private HtmlStringTypographer _typographer;
        private bool _preserveWhitespace;
        #endregion

        #region Constructors
        public HtmlStringParser()
            : this(HtmlProcessingMode.DenyByDefault)
        {
        }

        public HtmlStringParser(HtmlProcessingMode processingMode)
        {
            _document = new HtmlElementRule(null, "html",
                HtmlElementFlags.Allowed | HtmlElementFlags.AllowContent |
                HtmlElementFlags.UseTypography);

            _preserveWhitespace = true;
            _processingMode = processingMode;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets or sets the <see cref="Radischevo.Wahha.Web.Text.HtmlProcessingMode"/> 
        /// which will be used to determine whether the particular HTML node is allowed 
        /// within the document.
        /// </summary>
        public HtmlProcessingMode ProcessingMode
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
        public HtmlElementFlags DefaultElementFlags
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
        protected HtmlElementRule GetElementRule(HtmlElementRule parent, string tag)
        {
            HtmlElementFlags defaultFlags = DefaultElementFlags | ((_processingMode == HtmlProcessingMode.AllowByDefault) ?
                HtmlElementFlags.SelfClosing | HtmlElementFlags.Allowed | HtmlElementFlags.AllowContent :
                HtmlElementFlags.Denied);

            return GetRuleOrDefault(parent, tag, defaultFlags);
        }

        protected HtmlAttributeRule GetAttributeRule(HtmlElementRule parent, string attribute)
        {
            HtmlAttributeFlags flags = (_processingMode == HtmlProcessingMode.AllowByDefault) ?
                HtmlAttributeFlags.Allowed : HtmlAttributeFlags.Denied;

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
            if (tagRule != null && (tagRule.Flags & HtmlElementFlags.Recursive)
                == HtmlElementFlags.Recursive) // и ищем рекурсивные правила
                return tagRule;

            return null;
        }

        protected HtmlElementRule GetRuleOrDefault(string tag, HtmlElementFlags flags)
        {
            return GetRule(tag) ?? new HtmlElementRule(Document, tag, flags);
        }

        protected HtmlElementRule GetRuleOrDefault(HtmlElementRule parent, string tag, HtmlElementFlags flags)
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
            Precondition.Require(writer, Error.ArgumentNull("writer"));
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
                    if ((parent.Flags & HtmlElementFlags.Container) == HtmlElementFlags.Container)
                        WriteElement(parent, (XmlElement)node, writer);
                    break;
                case XmlNodeType.CDATA:
                    if ((parent.Flags & HtmlElementFlags.Text) == HtmlElementFlags.Text)
                        WriteCData(parent, node.Value, writer);
                    break;
                case XmlNodeType.Comment:
                    if ((parent.Flags & HtmlElementFlags.Text) == HtmlElementFlags.Text)
                        WriteComment(parent, node.Value, writer);
                    break;
                case XmlNodeType.Text:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    if ((parent.Flags & HtmlElementFlags.Text) == HtmlElementFlags.Text)
                        WriteText(parent, node.Value, writer);
                    break;
            }
        }

        protected XmlElement ConvertElement(HtmlElementRule parent, 
            XmlElement element, out HtmlElementRule rule)
        {
            Precondition.Require(element, Error.ArgumentNull("element"));

            rule = GetElementRule(parent, element.LocalName);
            if (rule.Converter != null)
            {
                element = rule.Converter(element);
                if (!String.Equals(element.LocalName, rule.Name, StringComparison.OrdinalIgnoreCase))
                    rule = GetElementRule(parent, element.LocalName);
            }

            return element;
        }

        protected virtual void WriteElement(HtmlElementRule parent, 
            XmlElement element, XmlWriter writer)
        {
            HtmlElementRule rule;
            element = ConvertElement(parent, element, out rule);

            WriteStartElement(rule, element, writer);
            WriteElementContents(rule, element, writer);
            WriteEndElement(rule, element, writer);
        }

        protected void WriteStartElement(HtmlElementRule rule, XmlElement element, XmlWriter writer)
        {
            if ((rule.Flags & HtmlElementFlags.Allowed) == HtmlElementFlags.Allowed)
            {
                writer.WriteStartElement(element.LocalName);
                WriteAttributes(rule, element.Attributes, writer);
            }
        }

        protected void WriteEndElement(HtmlElementRule rule, XmlElement element, XmlWriter writer)
        {
            if ((rule.Flags & HtmlElementFlags.Allowed) == HtmlElementFlags.Allowed)
            {
                if ((rule.Flags & HtmlElementFlags.SelfClosing) == HtmlElementFlags.SelfClosing)
                    writer.WriteEndElement();
                else
                    writer.WriteFullEndElement();
            }
        }

        protected void WriteElementContents(HtmlElementRule rule,
            XmlElement element, XmlWriter writer)
        {
            if ((rule.Flags & HtmlElementFlags.Preformatted) == HtmlElementFlags.Preformatted)
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
                if ((attr.Flags & HtmlAttributeFlags.Required) == HtmlAttributeFlags.Required
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
                if(!String.Equals(attribute.LocalName, attrRule.Name))
                    attrRule = GetAttributeRule(rule, attribute.LocalName);
            }
            
            string value = attribute.Value;

            if ((attrRule.Flags & HtmlAttributeFlags.Allowed) == HtmlAttributeFlags.Allowed)
            {
                if ((attrRule.Flags & HtmlAttributeFlags.Default) == HtmlAttributeFlags.Default &&
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
            if ((rule.Flags & HtmlElementFlags.UseTypography) == HtmlElementFlags.UseTypography &&
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
            XmlElement xe = dummy.CreateElement(element.Name);
            
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

                        HtmlElementFlags flags = rule.Flags;
                        if ((rule.Flags & HtmlElementFlags.SelfClosing) ==
                            HtmlElementFlags.SelfClosing)
                            rule.Flags = (HtmlElementFlags)((byte)rule.Flags - 0x04);

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
            Precondition.Require(rule, Error.ArgumentNull("rule"));
            return Document.AddElementRule(rule);
        }

        /// <summary>
        /// Adds a top-level rule for the attribute. In most cases, the rule is ignored.
        /// </summary>
        /// <param name="rule">A new sub-rule to add.</param>
        protected virtual HtmlAttributeRule AddAttributeRule(HtmlAttributeRule rule)
        {
            Precondition.Require(rule, Error.ArgumentNull("rule"));
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
        /// <param name="inner">The selector function.</param>
        public IRuleAppender Add(Func<IRuleSelector, IRuleBuilder> inner)
        {
            Precondition.Require(inner, Error.ArgumentNull("inner"));
            IRuleBuilder rule = inner(this);

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
                throw Error.UnsupportedElementRule("inner");

            return this;
        }
        #endregion

        #region Fluent Interface Implementation
        IRuleAppender IRuleAppender.With(Func<IRuleSelector, IRuleBuilder> inner)
        {
            return Add(inner);
        }

        IFluentElementRule IRuleSelector.Element(string name)
        {
            return new HtmlElementRule(Document, name, HtmlElementFlags.Allowed |
                HtmlElementFlags.AllowContent | HtmlElementFlags.SelfClosing);
        }

        IFluentAttributeRule IRuleSelector.Attribute(string name)
        {
            return new HtmlAttributeRule(Document, name, HtmlAttributeFlags.Allowed);
        }

        IFluentElementRule IRuleSelector.Elements(params string[] names)
        {
            Precondition.Require(names, Error.ArgumentNull("names"));
            HtmlElementRuleCollection collection = new HtmlElementRuleCollection();

            foreach (string name in names)
                collection.Add(new HtmlElementRule(Document, name, HtmlElementFlags.Allowed |
                    HtmlElementFlags.AllowContent | HtmlElementFlags.SelfClosing));

            return collection;
        }

        IFluentAttributeRule IRuleSelector.Attributes(params string[] names)
        {
            Precondition.Require(names, Error.ArgumentNull("names"));
            HtmlAttributeRuleCollection collection = new HtmlAttributeRuleCollection();

            foreach (string name in names)
                collection.Add(new HtmlAttributeRule(Document, name, HtmlAttributeFlags.Allowed));

            return collection;
        }
        #endregion
    }
}
