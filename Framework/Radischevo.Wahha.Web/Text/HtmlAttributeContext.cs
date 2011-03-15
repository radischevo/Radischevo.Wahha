using System;
using System.Xml;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
	public class HtmlAttributeContext : HtmlParsingContext
	{
		#region Instance Fields
		private HtmlAttributeRule _rule;
		#endregion

		#region Constructors
		public HtmlAttributeContext(HtmlAttributeRule rule)
			: this(rule, null)
		{
		}

		public HtmlAttributeContext(HtmlAttributeRule rule,
			IValueSet parameters)
			: base(parameters)
		{
			Precondition.Require(rule, () => Error.ArgumentNull("rule"));
			_rule = rule;
		}
		#endregion

		#region Instance Properties
		public HtmlAttributeRule Rule
		{
			get
			{
				return _rule;
			}
		}
		#endregion
	}
}
