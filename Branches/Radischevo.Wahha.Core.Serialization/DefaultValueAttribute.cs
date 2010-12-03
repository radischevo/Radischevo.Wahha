using System;

namespace Radischevo.Wahha.Core.Serialization
{
	public sealed class DefaultValueAttribute : Attribute
	{
		#region Instance Fields
		private string _value;
		#endregion

		#region Constructors
		public DefaultValueAttribute(string value)
		{
			_value = value;
		}
		#endregion

		#region Instance Properties
		public string Value
		{
			get
			{
				return _value;
			}
		}
		#endregion
	}
}
