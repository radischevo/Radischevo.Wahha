using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    /// <summary>
    /// Provides an XML document processing 
    /// using the specified parsing rules.
    /// </summary>
    public class HtmlFilterSettings : IRuleSelector, IRuleAppender
    {
        #region Instance Fields
        private HtmlElementRule _document;
        private HtmlElementOptions _defaultOptions;
        private HtmlFilteringMode _mode;
        private bool _preserveWhitespace;
        #endregion

        #region Constructors
        public HtmlFilterSettings()
            : this(HtmlFilteringMode.DenyByDefault)
        {
        }

        public HtmlFilterSettings(HtmlFilteringMode processingMode)
        {
            _document = new HtmlElementRule(null, "html",
                HtmlElementOptions.Allowed | HtmlElementOptions.AllowContent |
                HtmlElementOptions.UseTypography);

            _preserveWhitespace = true;
            _mode = processingMode;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets or sets the <see cref="Radischevo.Wahha.Web.Text.HtmlFilteringMode"/> 
        /// which will be used to determine whether the particular HTML node is allowed 
        /// within the document.
        /// </summary>
        public HtmlFilteringMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }

        /// <summary>
        /// Gets or sets the default rule set, which will be applied 
        /// to an element, if no specific rule is found.
        /// </summary>
        public HtmlElementOptions DefaultOptions
        {
            get
            {
                return _defaultOptions;
            }
            set
            {
                _defaultOptions = value;
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

        #region Rule Specific Methods
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
        #endregion

        #region Fluent Interface Implementation
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
