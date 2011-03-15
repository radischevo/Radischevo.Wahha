using System;
using System.Xml;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
	public sealed class HtmlElementResult
	{
		#region Instance Fields
		private HtmlElementRule _rule;
		private XmlElement _element;
		#endregion

		#region Constructors
		public HtmlElementResult(XmlElement element, 
			HtmlElementRule rule)
		{
			Precondition.Require(element, () => 
				Error.ArgumentNull("element"));
			Precondition.Require(rule, () => 
				Error.ArgumentNull("rule"));

			_element = element;
			_rule = rule;
		}
		#endregion

		#region Instance Properties
		public XmlElement Element
		{
			get
			{
				return _element;
			}
		}

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