using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Mapping;

namespace ConsoleTester
{
	public class TimestampDbColumnBinder : IDbColumnBinder
	{
		#region Constructors
		public TimestampDbColumnBinder ()
		{
		}
		#endregion
		
		#region Instance Methods
		public object ToPropertyValue (object value)
		{
			return DateTime.FromFileTimeUtc(Converter.ChangeType<long>(value, 0));
		}

		public object ToColumnValue (object value)
		{
			return Converter.ChangeType<DateTime>(value).ToFileTimeUtc();
		}
		#endregion
	}
}