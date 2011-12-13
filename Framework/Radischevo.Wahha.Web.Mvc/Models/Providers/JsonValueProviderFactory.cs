using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Serialization;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class JsonValueProviderFactory : InputStreamValueProviderFactory
	{
		#region Constructors
		public JsonValueProviderFactory ()
			: base()
		{
		}
		#endregion
		
		#region Static Methods
		private static void AppendBindingData (IDictionary<string, object> store, string prefix, object value)
		{
			IDictionary<string, object > dict = (value as IDictionary<string, object>);
			IList list = (value as IList);
			
			if (dict != null)
			{
				foreach (KeyValuePair<string, object> entry in dict)
					AppendBindingData (store, MakePropertyKey (prefix, entry.Key), entry.Value);
			}
			else
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
					AppendBindingData (store, MakeArrayKey (prefix, i), list [i]);
			}
			store [prefix] = value;
		}

		private static ValueDictionary Deserialize (string input)
		{
			try
			{
				ValueDictionary data = new ValueDictionary ();
				
				JavaScriptSerializer serializer = new JavaScriptSerializer ();
				object value = serializer.DeserializeObject (input);

				AppendBindingData (data, String.Empty, value);
				return data;
			}
			catch
			{
				return null;
			}
		}
		#endregion
		
		#region Instance Methods
		protected override bool ValidateRequest (HttpRequestBase request)
		{
			return request.ContentType.StartsWith ("application/json", 
				StringComparison.OrdinalIgnoreCase);		
		}
		
		protected override IValueSet CreateBindingStore (Stream input, Encoding encoding)
		{
			using (StreamReader reader = new StreamReader(input, encoding))
			{
				string serializedValue = reader.ReadToEnd ();
				
				if (String.IsNullOrEmpty (serializedValue))
					return null;

				return Deserialize (serializedValue);
			}
		}
		#endregion
	}
}

