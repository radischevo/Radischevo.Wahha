using System;
using System.IO;

namespace Radischevo.Wahha.Core.Serialization
{
	internal class ObjectWriterStrategy : IObjectWriterStrategy
	{
		#region Constructors
		public ObjectWriterStrategy()
		{
		}
		#endregion

		#region Instance Methods
		public void Write<T>(MessageWriter target, int number, T value) 
			where T : class
		{
			target.WriteHeader(number, WireType.String);

			MemoryStream embedded = new MemoryStream();
			Serializer.Serialize(new MessageWriter(embedded), value);

			target.WriteBytes(embedded.GetBuffer(), (int)embedded.Length);
		}
		#endregion	
	}
}
