using System;

namespace Radischevo.Wahha.Core.Serialization
{
	public struct MessageTag
	{
		#region Instance Fields
		private int _tag;
		#endregion

		#region Constructors
		public MessageTag(int tag)
		{
			_tag = tag;
		}

		public MessageTag(int number, WireType wireType)
			: this(number << 3 | (int)wireType)
		{
		}
		#endregion

		#region Instance Properties
		public int Number
		{
			get
			{
				return GetNumber(_tag);
			}
		}
		public WireType WireType
		{
			get
			{
				return GetWireType(_tag);
			}
		}

		public int Value
		{
			get
			{
				return _tag;
			}
		}
		#endregion

		#region Static Methods
		public static int GetNumber(int tag)
		{
			return (tag >> 3);
		}

		public static WireType GetWireType(int tag)
		{
			return ((WireType)tag & WireType.Mask);
		}

		public static int AsInt(int number, WireType wireType)
		{
			return new MessageTag(number, wireType).Value;
		}
		#endregion

		#region Instance Methods
		public int WithWireType(WireType wireType)
		{
			return (_tag & (int)~WireType.Mask | (int)wireType);
		}

		public override string ToString()
		{
			return String.Format("{{{0}, {1}}}", Number, WireType);
		}
		#endregion
	}
}