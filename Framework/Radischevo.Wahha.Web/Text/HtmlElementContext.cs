using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Radischevo.Wahha.Web.Text
{
	public class HtmlElementContext
	{
		#region Instance Fields
		private XmlElement _element;
		private bool _skip;
		#endregion

		#region Constructors
		public HtmlElementContext(XmlElement element)
		{
			_element = element;
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

		public bool Skip
		{
			get
			{
				return _skip;
			}
			set
			{
				_skip = value;
			}
		}
		#endregion

		#region Instance Methods
		public XmlElement Rewrite(string name)
		{
			return (_element = _element.OwnerDocument.CreateElement(name));
		}

		public XmlElement Wrap(string name)
		{
			if (String.Equals(_element.ParentNode.Name, name, 
				StringComparison.OrdinalIgnoreCase))
				return _element;

			XmlElement wrapper = _element.OwnerDocument.CreateElement(name);
			wrapper.AppendChild(_element);

			return (_element = wrapper);
		}
		#endregion
	}
}
