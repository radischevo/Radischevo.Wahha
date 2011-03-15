using System;
using System.Xml;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
	public sealed class HtmlAttributeResult
	{
		#region Instance Fields
		private HtmlAttributeRule _rule;
		private XmlAttribute _attribute;
		#endregion

		#region Constructors
		public HtmlAttributeResult(XmlAttribute attribute, 
			HtmlAttributeRule rule)
		{
			Precondition.Require(attribute, () =>
				Error.ArgumentNull("attribute"));
			Precondition.Require(rule, () =>
				Error.ArgumentNull("rule"));

			_attribute = attribute;
			_rule = rule;
		}
		#endregion

		#region Instance Properties
		public XmlAttribute Attribute
		{
			get
			{
				return _attribute;
			}
		}

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