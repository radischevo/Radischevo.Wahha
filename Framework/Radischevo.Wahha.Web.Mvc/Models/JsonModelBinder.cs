using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Scripting.Serialization;

namespace Radischevo.Wahha.Web.Mvc
{
    public class JsonModelBinder : ComplexTypeModelBinder
    {
		#region Static Methods
		private static string MakeArrayKey(string prefix, int index)
		{
			return prefix + "-" + index.ToString(CultureInfo.InvariantCulture);
		}

		private static string MakePropertyKey(string prefix, string propertyName)
		{
			return (String.IsNullOrEmpty(prefix)) ? propertyName : prefix + "-" + propertyName;
		}

		private static void AppendBindingData(IDictionary<string, object> store, string prefix, object value)
		{
			IDictionary<string, object> dict = (value as IDictionary<string, object>);
			if (dict != null)
			{
				foreach (KeyValuePair<string, object> entry in dict)
					AppendBindingData(store, MakePropertyKey(prefix, entry.Key), entry.Value);

				return;
			}

			IList list = (value as IList);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
					AppendBindingData(store, MakeArrayKey(prefix, i), list[i]);

				return;
			}
			store[prefix] = value;
		}

		private static ValueDictionary CreateBindingData(string serializedValue)
		{
			if (String.IsNullOrEmpty(serializedValue))
				return null;

			ValueDictionary data = new ValueDictionary();
			try
			{
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
		protected override object ExecuteBind(BindingContext context)
        {
            object value;
			if (!context.TryGetValue(out value))
				return null;

			ValueDictionary data = CreateBindingData(Convert.ToString(value, 
				CultureInfo.InvariantCulture)) ?? new ValueDictionary();

			BindingContext inner = new BindingContext(context, context.ModelType,
				String.Empty, context.Source, data, context.AllowMemberUpdate, context.Errors);

			return base.ExecuteBind(inner);
        }
        #endregion
    }
}
