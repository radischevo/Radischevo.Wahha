using System;
using System.Xml;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
	public class HtmlElementContext : HtmlParsingContext
	{
		#region Instance Fields
		private HtmlElementRule _rule;
		#endregion

		#region Constructors
		public HtmlElementContext(HtmlElementRule rule)
			: this(rule, null)
		{
		}

		public HtmlElementContext(HtmlElementRule rule, 
			IValueSet parameters) 
			: base(parameters)
		{
			Precondition.Require(rule, () => Error.ArgumentNull("rule"));
			_rule = rule;
		}
		#endregion

		#region Instance Properties
		public HtmlElementRule Rule
		{
			get
			{
				return _rule;
			}
		}
		#endregion
	}
}
