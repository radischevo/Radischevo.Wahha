using System;
using System.IO;
using System.Text;
using System.Xml;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class XmlValueProviderFactory : InputStreamValueProviderFactory
	{
		#region Constructors
		public XmlValueProviderFactory ()
			: base()
		{
		}
		#endregion
		
		#region Static Methods
		protected override bool ValidateRequest(HttpRequestBase request)
		{
			return request.ContentType.StartsWith("text/xml", StringComparison.OrdinalIgnoreCase);
		}
		
		protected override IValueSet CreateBindingStore (Stream input, Encoding encoding)
		{
			// TODO: Написать читалку из XML в Dictionary.
			return base.CreateBindingStore(input, encoding);
		}
		#endregion
	}
}

