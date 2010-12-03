using System;

namespace Radischevo.Wahha.Core.Serialization
{
	internal class UnknownEnumException : ArgumentOutOfRangeException
	{
		#region Instance Fields
		private int _value;
		#endregion

		#region Constructors
		public UnknownEnumException(int value)
		{
			_value = value;
		}
		#endregion

		#region Instance Properties
		public int Value
		{
			get
			{
				return _value;
			}
		}
		#endregion
	}
}
