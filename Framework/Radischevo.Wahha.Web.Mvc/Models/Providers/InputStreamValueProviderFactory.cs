using System;
using System.Globalization;
using System.IO;
using System.Text;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
	public abstract class InputStreamValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		protected InputStreamValueProviderFactory ()
		{
		}

		#endregion
		
		#region Static Methods
		protected static string MakeArrayKey (string prefix, int index)
		{
			return prefix + "-" + index.ToString (CultureInfo.InvariantCulture);
		}

		protected static string MakePropertyKey (string prefix, string propertyName)
		{
			return (String.IsNullOrEmpty (prefix)) ? propertyName : prefix + "-" + propertyName;
		}

		#endregion
		
		#region Instance Methods
		protected abstract bool ValidateRequest (HttpRequestBase request);
		
		protected virtual IValueSet CreateBindingStore (Stream input, Encoding encoding)
		{
			return new ValueDictionary ();
		}
		
		public IValueProvider Create (ControllerContext context)
		{
			if (ValidateRequest (context.Context.Request))
			{
				Stream input = context.Context.Request.InputStream;
				Encoding encoding = context.Context.Request.ContentEncoding;
				IValueSet data = CreateBindingStore (input, encoding) ?? new ValueDictionary ();
				
				return new DictionaryValueProvider (data);
			}
			return null;
		}
		#endregion
	}
}

