using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public class HtmlAttributeRule : IFluentAttributeRule
    {
        #region Instance Fields
        private string _name;
        private HtmlElementRule _element;
        private HtmlAttributeFlags _flags;
        private string _pattern;
        private string _defaultValue;
        private Regex _regexPatternCache;
        private HtmlAttributeConverter _converter;
        #endregion

        #region Constructors
        public HtmlAttributeRule(HtmlElementRule element,
            string name, HtmlAttributeFlags flags)
            : this(element, name, flags, null)
        {
        }

        public HtmlAttributeRule(HtmlElementRule element,
            string name, HtmlAttributeFlags flags, string pattern)
            : this(element, name, flags, pattern, null)
        {
        }

        public HtmlAttributeRule(HtmlElementRule element,
            string name, HtmlAttributeFlags flags, string pattern,
            string defaultValue)
        {
            Precondition.Require(element, () => Error.ArgumentNull("element"));
            Precondition.Defined(name,
				() => Error.ArgumentNull("name"));

            _element = element;
            _name = name;
            _flags = flags;
            _pattern = pattern;
            _defaultValue = defaultValue;
        }
        #endregion

        #region Instance Properties
        public HtmlElementRule Element
        {
            get
            {
                return _element;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public HtmlAttributeFlags Flags
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

        public string Pattern
        {
            get
            {
                return _pattern;
            }
            set
            {
                _pattern = value;
                _regexPatternCache = null;
            }
        }

        public string DefaultValue
        {
            get
            {
                return _defaultValue ?? String.Empty;
            }
            set
            {
                _defaultValue = value;
            }
        }

        public HtmlAttributeConverter Converter
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

        protected Regex RegexPattern
        {
            get
            {
                if (_regexPatternCache == null && !String.IsNullOrEmpty(_pattern))
                    _regexPatternCache = new Regex(_pattern, RegexOptions.IgnoreCase |
                        RegexOptions.CultureInvariant | RegexOptions.Multiline |
                        RegexOptions.Singleline);

                return _regexPatternCache;
            }
        }
        #endregion

        #region Static Methods
        private static bool ContainsOnlyDigits(string value)
        {
            if (String.IsNullOrEmpty(value))
                return true;

            for (int i = 0; i < value.Length; ++i)
                if (!Char.IsDigit(value, i))
                    return false;

            return true;
        }

		private static string RemoveSpecialChars(string value)
		{
			if (String.IsNullOrEmpty(value))
				return null;

			int length = value.Length;
			char[] array = new char[length];
			int index = 0;

			for (int i = 0; i < length; ++i)
			{
				if (Char.IsLetterOrDigit(value[i]) ||
					Char.IsPunctuation(value[i]))
					array[index++] = value[i];
			}
			return new String(array, 0, index);
		}

		private static string ExtractUrlScheme(string value)
		{
			Uri uri;
			string scheme = null;
			int index = -1;

			if (Uri.TryCreate(value, UriKind.Absolute, out uri))
				scheme = uri.Scheme;
			else if ((index = value.IndexOf(':')) > -1)
				scheme = value.Substring(0, index);

			return RemoveSpecialChars(scheme);
		}

		private static bool ContainsUrl(string value)
        {
            if (String.IsNullOrEmpty(value))
                return true;

			string scheme = ExtractUrlScheme(value);

			if (String.Equals("javascript", scheme,
				StringComparison.InvariantCultureIgnoreCase))
				return false;

            return (value[0] == '/' || Char.IsLetterOrDigit(value, 0));            
        }
        #endregion

        #region Instance Methods
        internal HtmlAttributeRule CreateCopy(HtmlElementRule parent)
        {
            HtmlAttributeRule current = new HtmlAttributeRule(parent, _name, _flags);
            current._converter = _converter;
            current._defaultValue = _defaultValue;
            current._pattern = _pattern;

            return current;
        }

        public virtual bool ValidateValue(string value)
        {
            if (String.IsNullOrEmpty(_pattern))
                return true;

            switch (_pattern.ToLower())
            {
                case "#int":
                    return ContainsOnlyDigits(value);
                case "#url":
                    return ContainsUrl(value);
                default:
                    return RegexPattern.IsMatch(value);
            }
        }

        public override int GetHashCode()
        {
            return (_name.GetHashCode() ^ base.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;

            HtmlAttributeRule rule = (obj as HtmlAttributeRule);
            if (Object.ReferenceEquals(rule, null))
                return false;

            if (rule.Name.Equals(_name, StringComparison.OrdinalIgnoreCase))
                return _element.Equals(rule._element);

            return false;
        }

        public override string ToString()
        {
            return String.Format(@"{0}, ""{1}""", GetType().Name, _name);
        }
        #endregion

        #region Fluent Interface Implementation
        private IFluentAttributeRule SetDefaultValue(string defaultValue)
        {
            _defaultValue = defaultValue;
            if (!String.IsNullOrEmpty(defaultValue))
                _flags |= HtmlAttributeFlags.Default;

            return this;
        }

        IFluentAttributeRule IFluentAttributeRule.As(HtmlAttributeFlags flags)
        {
            _flags = flags;
            return this;
        }

        IFluentAttributeRule IFluentAttributeRule.Convert(HtmlAttributeConverter converter)
        {
            _converter = converter;
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
            _pattern = pattern;
            return this;
        }
        #endregion
    }
}
