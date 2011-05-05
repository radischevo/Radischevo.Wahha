using System;

namespace Radischevo.Wahha.Core
{
	public interface IOperation<TResult> : IOperation
	{
		#region Instance Methods
		new TResult Execute();
		#endregion
	}
}
