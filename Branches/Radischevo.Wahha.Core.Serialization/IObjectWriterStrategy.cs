using System;

namespace Radischevo.Wahha.Core.Serialization
{
	public interface IObjectWriterStrategy
	{
		#region Instance Methods
		void Write<T>(MessageWriter target, int number, T value) 
			where T : class;
		#endregion
	}
}
