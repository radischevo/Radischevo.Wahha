using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public interface IDbOperation<TResult> : IContextualOperation<DbOperationContext, TResult>, IDbOperation
	{
	}
}
