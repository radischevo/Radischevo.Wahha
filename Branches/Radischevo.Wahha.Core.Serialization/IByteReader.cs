using System;

namespace Radischevo.Wahha.Core.Serialization
{
	public interface IByteReader
	{
		#region Instance Properties
		bool EndOfStream
		{
			get;
		}
		#endregion

		#region Instance Methods
		byte GetByte();
		float GetSingle();
		ArraySegment<byte> GetBytes();
		ArraySegment<byte> GetBytes(int count);
		IByteReader GetReader(int length);
		#endregion
	}
}
