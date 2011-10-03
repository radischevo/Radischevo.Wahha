using System;

namespace Radischevo.Wahha.Core
{
	public interface IContextualOperation<TContext>
	{
		#region Instance Methods
		void Execute(TContext context);
		#endregion
	}
}
