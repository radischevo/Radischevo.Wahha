using System;

namespace Radischevo.Wahha.Core
{
	public interface IContextualOperation<TContext, TResult> : IContextualOperation<TContext>
	{
		#region Instance Methods
		new TResult Execute(TContext context);
		#endregion
	}
}
