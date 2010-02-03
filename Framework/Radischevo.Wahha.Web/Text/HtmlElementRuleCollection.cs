using System;
using System.Collections;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public class HtmlElementRuleCollection : IEnumerable<HtmlElementRule>, IFluentElementRule
    {
        #region Instance Fields
        private Dictionary<string, HtmlElementRule> _values;
        #endregion

        #region Constructors
        public HtmlElementRuleCollection()
        {
            _values = new Dictionary<string, HtmlElementRule>(
                StringComparer.OrdinalIgnoreCase);
        }

        public HtmlElementRuleCollection(IEnumerable<HtmlElementRule> collection)
            : this()
        {
            AddRange(collection);
        }
        #endregion

        #region Instance Properties
        public HtmlElementRule this[string name]
        {
            get
            {
                if (String.IsNullOrEmpty(name))
                    return null;

                HtmlElementRule rule;
                if (_values.TryGetValue(name, out rule))
                    return rule;

                return null;
            }
            set
            {
                Precondition.Require(value, Error.ArgumentNull("value"));
                Precondition.Require(String.Equals(name, value.Name, StringComparison.OrdinalIgnoreCase),
                    new InvalidOperationException("The rule name must be equal to the key."));

                _values[name] = value;
            }
        }

        public int Count
        {
            get
            {
                return _values.Count;
            }
        }
        #endregion

        #region Static Methods
        private static HtmlElementRule Validate(HtmlElementRule rule)
        {
            Precondition.Require(rule, Error.ArgumentNull("rule"));
            return rule;
        }
        #endregion

        #region Instance Methods
        public void Add(HtmlElementRule rule)
        {
            _values[rule.Name] = Validate(rule);
        }

        public void AddRange(IEnumerable<HtmlElementRule> collection)
        {
            Precondition.Require(collection, Error.ArgumentNull("collection"));
            foreach (HtmlElementRule rule in collection)
                Add(rule);
        }

        public void Clear()
        {
            _values.Clear();
        }

        public bool Contains(HtmlElementRule rule)
        {
            return _values.ContainsKey(Validate(rule).Name);
        }

        public void CopyTo(HtmlElementRuleCollection collection)
        {
            Precondition.Require(collection, 
                Error.ArgumentNull("collection"));

            collection.Clear();
            collection.AddRange(this);
        }

        public bool TryGetItem(string name, out HtmlElementRule rule)
        {
            return _values.TryGetValue(name, out rule);
        }

        public bool Remove(HtmlElementRule rule)
        {
            return _values.Remove(Validate(rule).Name);
        }

        public IEnumerator<HtmlElementRule> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Fluent Interface Implementation
        IFluentElementRule IFluentElementRule.As(HtmlElementFlags flags)
        {
            foreach (IFluentElementRule rule in this)
                rule.As(flags);

            return this;
        }

        IFluentElementRule IFluentElementRule.Convert(HtmlElementConverter converter)
        {
            foreach (IFluentElementRule rule in this)
                rule.Convert(converter);

            return this;
        }

        IRuleAppender IRuleAppender.With(Func<IRuleSelector, IRuleBuilder> inner)
        {
            foreach (IFluentElementRule rule in this)
                rule.With(inner);

            return this;
        }
        #endregion
    }
}