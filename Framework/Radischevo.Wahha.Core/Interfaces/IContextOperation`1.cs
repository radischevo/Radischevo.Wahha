using System;

namespace Radischevo.Wahha.Core
{
	public interface IContextOperation<TContext, TResult> : IContextOperation<TContext>
	{
		#region Instance Methods
		new TResult Execute(TContext context);
		#endregion
	}
}
