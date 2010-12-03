using System;

namespace Radischevo.Wahha.Core.Serialization
{
	public sealed class TagAttribute : Attribute
	{
		#region Instance Fields
		private int _number;
		private bool _fixed;
		private bool _zigZag;
		private bool _packed;
		#endregion

		#region Constructors
		public TagAttribute(int number)
		{
			_number = number;
		}
		#endregion

		#region Instance Properties
		public int Number
		{
			get
			{
				return _number;
			}
		}

		public bool Fixed
		{
			get
			{
				return _fixed;
			}
			set
			{
				_fixed = value;
			}
		}

		public bool ZigZag
		{
			get
			{
				return _zigZag;
			}
			set
			{
				_zigZag = value;
			}
		}

		public bool Packed
		{
			get
			{
				return _packed;
			}
			set
			{
				_packed = value;
			}
		}
		#endregion
	}
}
