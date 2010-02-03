using System;
using System.Collections.Generic;
using System.Xml;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public class HtmlElementRule : IFluentElementRule, IRuleSelector, IRuleAppender
    {
        #region Instance Fields
        private string _name;
        private HtmlElementFlags _flags;
        private HtmlElementRule _parent;
        private HtmlElementRuleCollection _children;
        private HtmlAttributeRuleCollection _attributes;
        private HtmlElementConverter _converter;
        #endregion

        #region Constructors
        public HtmlElementRule(HtmlElementRule parent,
            string name, HtmlElementFlags flags)
        {
            Precondition.Require(!String.IsNullOrEmpty(name),
                Error.ArgumentNull("name"));

            _name = name;
            _flags = flags;
            _children = new HtmlElementRuleCollection();
            _attributes = new HtmlAttributeRuleCollection();
            _parent = parent;
        }
        #endregion

        #region Instance Properties
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public HtmlElementFlags Flags
        {
            get
            {
                return _flags;
            }
            set
            {
                _flags = value;
            }
        }

        public HtmlElementRuleCollection Children
        {
            get
            {
                return _children;
            }
        }

        public HtmlElementRule Parent
        {
            get
            {
                return _parent;
            }
        }

        public HtmlAttributeRuleCollection Attributes
        {
            get
            {
                return _attributes;
            }
        }

        public HtmlElementConverter Converter
        {
            get
            {
                return _converter;
            }
            set
            {
                _converter = value;
            }
        }
        #endregion

        #region Fluent Interface Implementation
        IRuleAppender IRuleAppender.With(Func<IRuleSelector, IRuleBuilder> inner)
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

        IFluentElementRule IFluentElementRule.As(HtmlElementFlags flags)
        {
            _flags = flags;
            return this;
        }

        IFluentElementRule IFluentElementRule.Convert(HtmlElementConverter converter)
        {
            _converter = converter;
            return this;
        }

        IFluentElementRule IRuleSelector.Element(string name)
        {
            return new HtmlElementRule(this, name, HtmlElementFlags.Allowed |
                HtmlElementFlags.AllowContent | HtmlElementFlags.SelfClosing);
        }

        IFluentElementRule IRuleSelector.Elements(params string[] names)
        {
            Precondition.Require(names, Error.ArgumentNull("names"));
            HtmlElementRuleCollection collection = new HtmlElementRuleCollection();

            foreach (string name in names)
                collection.Add(new HtmlElementRule(this, name, HtmlElementFlags.Allowed |
                    HtmlElementFlags.AllowContent | HtmlElementFlags.SelfClosing));
            
            return collection;
        }

        IFluentAttributeRule IRuleSelector.Attribute(string name)
        {
            return new HtmlAttributeRule(this, name, HtmlAttributeFlags.Allowed);
        }

        IFluentAttributeRule IRuleSelector.Attributes(params string[] names)
        {
            Precondition.Require(names, Error.ArgumentNull("names"));
            HtmlAttributeRuleCollection collection = new HtmlAttributeRuleCollection();

            foreach (string name in names)
                collection.Add(new HtmlAttributeRule(this, name, HtmlAttributeFlags.Allowed));

            return collection;
        }
        #endregion

        #region Instance Methods
        public virtual HtmlElementRule AddElementRule(HtmlElementRule rule)
        {
            Precondition.Require(rule, Error.ArgumentNull("rule"));
            HtmlElementRule current = (rule.Parent == this) ? rule : rule.CreateCopy(this);
            _children[current.Name] = current;

            return current;
        }

        public virtual HtmlAttributeRule AddAttributeRule(HtmlAttributeRule rule)
        {
            Precondition.Require(rule, Error.ArgumentNull("rule"));
            HtmlAttributeRule current = (rule.Element == this) ? rule : rule.CreateCopy(this);
            _attributes[current.Name] = current;

            return current;
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

        internal HtmlElementRule CreateCopy(HtmlElementRule parent)
        {
            HtmlElementRule current = new HtmlElementRule(parent, _name, _flags);
            _children.CopyTo(current._children);
            _attributes.CopyTo(current._attributes);
            current._converter = _converter;

            return current;
        }

        public override int GetHashCode()
        {
            return (_name.GetHashCode() ^ base.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;

            HtmlElementRule rule = (obj as HtmlElementRule);
            if (Object.ReferenceEquals(rule, null))
                return false;

            if (rule.Name.Equals(_name, StringComparison.OrdinalIgnoreCase))
                return Object.Equals(_parent, rule._parent);

            return false;
        }

        public override string ToString()
        {
            return String.Format(@"{0}, ""{1}""", GetType().Name, _name);
        }
        #endregion
    }
}
