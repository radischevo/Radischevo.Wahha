using System;

namespace Radischevo.Wahha.Core.Serialization
{
	public class GroupEncodingObjectWriterStrategy : IObjectWriterStrategy
	{
		#region Constructors
		public GroupEncodingObjectWriterStrategy()
		{
		}
		#endregion

		#region Instance Methods
		public void Write<T>(MessageWriter target, int number, T value) 
			where T : class
		{
			target.WriteHeader(number, WireType.StartGroup);
			Serializer.Serialize(target, value);
			target.WriteHeader(number, WireType.EndGroup);
		}
		#endregion
	}
}
