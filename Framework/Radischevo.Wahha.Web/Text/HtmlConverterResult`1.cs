using System;
using System.Xml;

namespace Radischevo.Wahha.Web.Text
{
	public struct HtmlConverterResult<TNode>
		where TNode : XmlNode
	{
		#region Static Fields
		public static readonly HtmlConverterResult<TNode> Empty =
			new HtmlConverterResult<TNode>();
		#endregion

		#region Instance Fields
		private TNode _element;
		#endregion

		#region Constructors
		internal HtmlConverterResult(TNode element)
		{
			_element = element;
		}
		#endregion

		#region Instance Properties
		public TNode Element
		{
			get
			{
				return _element;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return (_element == null);
			}
		}
		#endregion
	}
}
