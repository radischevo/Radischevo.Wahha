using System;

namespace Radischevo.Wahha.Core.Serialization
{
	public enum WireType
	{
		Unknown = -1,
		Varint = 0,
		Fixed64 = 1,
		String = 2,
		StartGroup = 3,
		EndGroup = 4,
		Fixed32 = 5,
		MaxValid = 5,
		Mask = 0x7
	}
}
