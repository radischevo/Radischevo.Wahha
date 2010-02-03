using System;
using System.ComponentModel.DataAnnotations;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
	public class DataTypeValidationAttribute : ValidationAttribute
	{
		#region Instance Fields
		private Type _type;
		#endregion

		#region Constructors
		public DataTypeValidationAttribute()
			: this(null)
		{
		}

		public DataTypeValidationAttribute(Type type)
		{
			Type = type;
		}
		#endregion

		#region Instance Properties
		public Type Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value.MakeNonNullableType();
			}
		}
		#endregion

		#region Instance Methods
		public override bool IsValid(object value)
		{
			// Having type = null means no server-side validation.
			if (_type == null)
				return true;

			// If the provided value is null, 
			// check if the required type allows null values.
			if (Object.ReferenceEquals(value, null))
				return _type.IsNullable();

			// Check if the type can be assigned.
			return _type.IsAssignableFrom(value.GetType());
		}
		#endregion
	}
}
