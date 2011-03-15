using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Radischevo.Wahha.Web.Text
{
	public class HtmlTypographerSettings
	{
		#region Instance Fields
		private List<StringReplacementRule> _replacements;
		private bool _autoLineBreak;
		private bool _insertNoBreakTags;
		private bool _extractLinks;
		private bool _encodeSpecialSymbols;
		private bool _applyPunctuationRules;
		#endregion

		#region Constructors
		public HtmlTypographerSettings()
		{
			_replacements = new List<StringReplacementRule>();

			_autoLineBreak = true;
			_extractLinks = true;
			_insertNoBreakTags = true;
			_encodeSpecialSymbols = true;
			_applyPunctuationRules = true;
		}
		#endregion

		#region Instance Properties
		public ICollection<StringReplacementRule> Replacements
		{
			get
			{
				return _replacements;
			}
		}

		public bool AutoLineBreak
		{
			get
			{
				return _autoLineBreak;
			}
			set
			{
				_autoLineBreak = value;
			}
		}

		public bool InsertNoBreakTags
		{
			get
			{
				return _insertNoBreakTags;
			}
			set
			{
				_insertNoBreakTags = value;
			}
		}

		public bool ExtractLinks
		{
			get
			{
				return _extractLinks;
			}
			set
			{
				_extractLinks = value;
			}
		}

		public bool ApplyPunctuationRules
		{
			get
			{
				return _applyPunctuationRules;
			}
			set
			{
				_applyPunctuationRules = value;
			}
		}

		public bool EncodeSpecialSymbols
		{
			get
			{
				return _encodeSpecialSymbols;
			}
			set
			{
				_encodeSpecialSymbols = value;
			}
		}
		#endregion

		#region Instance Methods
		public HtmlTypographerSettings Replace(string pattern, string replacement)
		{
			return Replace(pattern, replacement, StringReplacementMode.Default);
		}

		public HtmlTypographerSettings Replace(string pattern, string replacement,
			RegexOptions options)
		{
			return Replace(pattern, replacement, StringReplacementMode.Regex, options);
		}

		public HtmlTypographerSettings Replace(string pattern, string replacement,
			StringReplacementMode mode)
		{
			return Replace(pattern, replacement, mode, RegexOptions.None);
		}

		public HtmlTypographerSettings Replace(string pattern, string replacement,
			StringReplacementMode mode, RegexOptions options)
		{
			StringReplacementRule rule = new StringReplacementRule(mode, pattern, replacement);
			rule.Options = options;
			
			Replacements.Add(rule);
			return this;
		}
		#endregion
	}
}
