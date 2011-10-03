using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Data.Serialization
{
	public abstract class JavaScriptConverter<T> : JavaScriptConverter
	{
		#region Static Fields
		private static readonly IEnumerable<Type> _defaultSupportedTypes = new Type[] { typeof(T) };
		#endregion

		#region Constructors
		protected JavaScriptConverter()
		{
		}
		#endregion

		#region Instance Properties
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return _defaultSupportedTypes;
			}
		}
		#endregion

		#region Instance Methods
		public override sealed IDictionary<string, object> Serialize(object obj,
			JavaScriptSerializer serializer)
		{
			return Serialize((T)obj, serializer);
		}

		public override sealed object Deserialize(IDictionary<string, object> dictionary,
			Type type, JavaScriptSerializer serializer)
		{
			if (type.IsAssignableFrom(typeof(T)))
				return Deserialize(dictionary, serializer);

			return null;
		}

		protected abstract IDictionary<string, object> Serialize(T item, JavaScriptSerializer serializer);

		protected abstract T Deserialize(IDictionary<string, object> dictionary, JavaScriptSerializer serializer);
		#endregion
	}
}