using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class InputStreamValueProvider : IValueProvider
	{
		#region Instance Fields
		private string _contents;
		#endregion

		#region Constructors
		public InputStreamValueProvider(ControllerContext context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			_contents = ReadInputStream(context.Context);
		}
		#endregion

		#region Instance Properties
		public IEnumerable<string> Keys
		{
			get
			{
				return Enumerable.Empty<string>();
			}
		}
		#endregion

		#region Static Methods
		private static string ReadInputStream(HttpContextBase context)
		{
			using (StreamReader reader = new StreamReader(
				context.Request.InputStream, 
				context.Request.ContentEncoding))
			{
				return reader.ReadToEnd();
			}
		}
		#endregion

		#region Instance Methods
		public bool Contains(string prefix)
		{
			return true;
		}

		public ValueProviderResult GetValue(string key)
		{
			return new ValueProviderResult(_contents, CultureInfo.InvariantCulture);
		}
		#endregion
	}

	public sealed class InputStreamValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		public InputStreamValueProviderFactory()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public IValueProvider Create(ControllerContext context)
		{
			return new InputStreamValueProvider(context);
		}
		#endregion
	}
}
