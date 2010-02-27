using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public class HtmlAttributeRuleCollection : IEnumerable<HtmlAttributeRule>, IFluentAttributeRule
    {
        #region Instance Fields
        private Dictionary<string, HtmlAttributeRule> _values;
        #endregion

        #region Constructors
        public HtmlAttributeRuleCollection()
        {
            _values = new Dictionary<string, HtmlAttributeRule>(
                StringComparer.OrdinalIgnoreCase);
        }

        public HtmlAttributeRuleCollection(IEnumerable<HtmlAttributeRule> collection)
            : this()
        {
            AddRange(collection);
        }
        #endregion

        #region Instance Properties
        public HtmlAttributeRule this[string name]
        {
            get
            {
                if (String.IsNullOrEmpty(name))
                    return null;

                HtmlAttributeRule rule;
                if (_values.TryGetValue(name, out rule))
                    return rule;

                return null;
            }
            set
            {
                Precondition.Require(value, () => Error.ArgumentNull("value"));
                Precondition.Require(String.Equals(name, value.Name, StringComparison.OrdinalIgnoreCase),
					() => Error.RuleNameCannotDifferFromKey(name, value.Name));

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
        private static HtmlAttributeRule Validate(HtmlAttributeRule rule)
        {
            Precondition.Require(rule, () => Error.ArgumentNull("rule"));
            return rule;
        }
        #endregion

        #region Instance Methods
        public void Add(HtmlAttributeRule rule)
        {
            _values[rule.Name] = Validate(rule);
        }

        public void AddRange(IEnumerable<HtmlAttributeRule> collection)
        {
            Precondition.Require(collection, () => Error.ArgumentNull("collection"));
            foreach (HtmlAttributeRule rule in collection)
                Add(rule);
        }

        public void Clear()
        {
            _values.Clear();
        }

        public bool Contains(HtmlAttributeRule rule)
        {
            return _values.ContainsKey(Validate(rule).Name);
        }

        public void CopyTo(HtmlAttributeRuleCollection collection)
        {
            Precondition.Require(collection,
				() => Error.ArgumentNull("collection"));

            collection.Clear();
            collection.AddRange(this);
        }

        public bool Remove(HtmlAttributeRule rule)
        {
            return _values.Remove(Validate(rule).Name);
        }

        public bool TryGetItem(string name, out HtmlAttributeRule rule)
        {
            return _values.TryGetValue(name, out rule);
        }

        public IEnumerator<HtmlAttributeRule> GetEnumerator()
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
        private IFluentAttributeRule SetDefaultValue(string defaultValue)
        {
            foreach (IFluentAttributeRule rule in this)
                rule.Default(defaultValue);

            return this;
        }

        IFluentAttributeRule IFluentAttributeRule.As(HtmlAttributeFlags flags)
        {
            foreach (IFluentAttributeRule rule in this)
                rule.As(flags);

            return this;
        }

        IFluentAttributeRule IFluentAttributeRule.Convert(HtmlAttributeConverter converter)
        {
            foreach (IFluentAttributeRule rule in this)
                rule.Convert(converter);

            return this;
        }

        IFluentAttributeRule IFluentAttributeRule.Default(object defaultValue)
        {
            return SetDefaultValue((defaultValue == null) ?
                null : Convert.ToString(defaultValue, CultureInfo.InvariantCulture));
        }

        IFluentAttributeRule IFluentAttributeRule.Default(string defaultValue)
        {
            return SetDefaultValue(defaultValue);
        }

        IFluentAttributeRule IFluentAttributeRule.Validate(string pattern)
        {
            foreach (IFluentAttributeRule rule in this)
                rule.Validate(pattern);

            return this;
        }
        #endregion
    }
}
