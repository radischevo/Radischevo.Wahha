using System;
using System.Text.RegularExpressions;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
    public class StringReplacementRule
    {
        #region Instance Fields
        private string _pattern;
        private string _replacement;
        private StringReplacementMode _mode;
        private RegexOptions _options;
        #endregion

        #region Constructors
        public StringReplacementRule(StringReplacementMode mode,
            string pattern, string replacement)
        {
            Precondition.Defined(pattern, () => Error.ArgumentNull("pattern"));

            _mode = mode;
            _pattern = pattern;
            _replacement = replacement;
            _options = RegexOptions.IgnoreCase | RegexOptions.Multiline |
                RegexOptions.Singleline;
        }
        #endregion

        #region Instance Properties
        public string Pattern
        {
            get
            {
                return _pattern;
            }
            set
            {
                Precondition.Defined(value, () => Error.ArgumentNull("value"));
                _pattern = value;
            }
        }

        public string Replacement
        {
            get
            {
                return _replacement ?? String.Empty;
            }
            set
            {
                _replacement = value;
            }
        }

        public StringReplacementMode Mode
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

        public RegexOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
            }
        }
        #endregion
    }
}
