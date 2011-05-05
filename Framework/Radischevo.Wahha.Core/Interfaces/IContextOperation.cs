using System;

namespace Radischevo.Wahha.Core
{
	public interface IContextOperation<TContext>
	{
		#region Instance Methods
		void Execute(TContext context);
		#endregion
	}
}
