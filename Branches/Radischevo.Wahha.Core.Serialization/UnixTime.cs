using System;

namespace Radischevo.Wahha.Core.Serialization
{
	public static class UnixTime
	{
		#region Constants
		public static readonly DateTime Epoch = new DateTime(1970, 1, 1);
		#endregion

		#region Static Methods
		public static long From(DateTime date)
		{
			return (date - Epoch).Ticks / TimeSpan.TicksPerMillisecond;
		}

		public static DateTime Convert(DateTime date)
		{
			return ToDateTime(From(date));
		}

		public static DateTime ToDateTime(long unixTime)
		{
			return Epoch.AddMilliseconds(unixTime);
		}
		#endregion
	}
}
