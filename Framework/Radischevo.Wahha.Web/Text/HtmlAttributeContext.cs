using System;
using System.Xml;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text
{
	public class HtmlAttributeContext : HtmlParsingContext
	{
		#region Instance Fields
		private XmlAttribute _attribute;
		private HtmlAttributeRule _rule;
		#endregion

		#region Constructors
		public HtmlAttributeContext(HtmlAttributeRule rule)
			: this(rule, null, null)
		{
		}

		public HtmlAttributeContext(HtmlAttributeRule rule, 
			XmlAttribute attribute)
			: this(rule, attribute, null)
		{
		}

		public HtmlAttributeContext(HtmlAttributeRule rule,
			XmlAttribute attribute, IValueSet parameters)
			: base(parameters)
		{
			Precondition.Require(rule, () => Error.ArgumentNull("rule"));

			_rule = rule;
			_attribute = attribute;
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

		#region Instance Methods
		public HtmlConverterResult<XmlAttribute> Cancel()
		{
			Cancelled = true;
			return HtmlConverterResult<XmlAttribute>.Empty;
		}

		public HtmlConverterResult<XmlAttribute> Result()
		{
			return Result(_attribute);
		}

		public HtmlConverterResult<XmlAttribute> Result(XmlAttribute attribute)
		{
			Precondition.Require(attribute, () =>
				Error.ArgumentNull("attribute"));

			return new HtmlConverterResult<XmlAttribute>(attribute);
		}
		#endregion
	}
}
