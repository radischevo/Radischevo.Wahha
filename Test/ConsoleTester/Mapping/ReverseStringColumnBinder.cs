using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Mapping;

namespace ConsoleTester
{
	[Serializable]
	public class ReverseStringColumnBinder : IDbColumnBinder
	{
		#region Constructors
		public ReverseStringColumnBinder ()
		{
		}
		#endregion
		
		#region Instance Methods
		public object ToPropertyValue (object value)
		{
			char[] symbols = Converter.ChangeType<string>(value, String.Empty).ToCharArray();
			Array.Reverse(symbols);
			
			return new String(symbols);
		}

		public object ToColumnValue (object value)
		{
			return ToPropertyValue(value); // we reverse them again
		}
		#endregion
	}
}

