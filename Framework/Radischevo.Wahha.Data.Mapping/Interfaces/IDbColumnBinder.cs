using System;

namespace Radischevo.Wahha.Data.Mapping
{
	public interface IDbColumnBinder
	{
		#region Instance Methods
		object ToPropertyValue (object value);
		
		object ToColumnValue (object value);
		#endregion
	}
}

