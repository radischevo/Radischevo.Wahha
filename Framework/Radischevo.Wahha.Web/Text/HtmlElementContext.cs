using System;
using System.Xml;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
	public class HtmlElementContext : HtmlParsingContext
	{
		#region Instance Fields
		private XmlElement _element;
		private HtmlElementRule _rule;
		#endregion

		#region Constructors
		public HtmlElementContext(HtmlElementRule rule)
			: this(rule, null, null)
		{
		}

		public HtmlElementContext(HtmlElementRule rule,
			XmlElement element)
			: this(rule, element, null)
		{
		}

		public HtmlElementContext(HtmlElementRule rule, 
			XmlElement element, IValueSet parameters) 
			: base(parameters)
		{
			Precondition.Require(rule, () => Error.ArgumentNull("rule"));

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

		#region Instance Methods
		public HtmlConverterResult<XmlElement> Cancel()
		{
			Cancelled = true;
			return HtmlConverterResult<XmlElement>.Empty;
		}

		public HtmlConverterResult<XmlElement> Result()
		{
			return Result(_element);
		}

		public HtmlConverterResult<XmlElement> Result(XmlElement element)
		{
			Precondition.Require(element, () => 
				Error.ArgumentNull("element"));

			return new HtmlConverterResult<XmlElement>(element);
		}
		#endregion
	}
}
