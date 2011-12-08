using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Serialization;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class JsonValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		public JsonValueProviderFactory ()
		{
		}
		#endregion
		
		#region Static Methods
		private static bool ValidateRequest(HttpRequestBase request)
		{
			return request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
		}
		
		private static string MakeArrayKey(string prefix, int index)
		{
			return prefix + "-" + index.ToString(CultureInfo.InvariantCulture);
		}

		private static string MakePropertyKey(string prefix, string propertyName)
		{
			return (String.IsNullOrEmpty(prefix)) ? propertyName : prefix + "-" + propertyName;
		}
		
		private static string ReadInputStream(HttpContextBase context)
		{
			using (StreamReader reader = new StreamReader(
				context.Request.InputStream, 
				context.Request.ContentEncoding))
			{
				return reader.ReadToEnd();
			}
		}
		
		private static void AppendBindingData(IDictionary<string, object> store, string prefix, object value)
		{
			IDictionary<string, object> dict = (value as IDictionary<string, object>);
			IList list = (value as IList);
			
			if (dict != null)
			{
				foreach (KeyValuePair<string, object> entry in dict)
					AppendBindingData(store, MakePropertyKey(prefix, entry.Key), entry.Value);
			}
			else if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
					AppendBindingData(store, MakeArrayKey(prefix, i), list[i]);
			}
			store[prefix] = value;
		}

		private static ValueDictionary CreateBindingData(HttpContextBase context)
		{
			string serializedValue = ReadInputStream(context);
			
			if (String.IsNullOrEmpty(serializedValue))
				return null;

			try
			{
				ValueDictionary data = new ValueDictionary();
				
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				object value = serializer.DeserializeObject(serializedValue);

				AppendBindingData(data, String.Empty, value);
				return data;
			}
			catch
			{
				return null;
			}
		}
		#endregion
		
		#region Instance Methods
		public IValueProvider Create (ControllerContext context)
		{
			if (ValidateRequest(context.Context.Request)) 
			{
				ValueDictionary data = CreateBindingData(context.Context) ?? new ValueDictionary();
				return new DictionaryValueProvider(data);
			}
			return null;
		}
		#endregion
	}
}

