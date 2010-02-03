using System;

namespace Radischevo.Wahha.Core.Async
{
	public delegate void EndInvokeDelegate(IAsyncResult asyncResult);

	public delegate TResult EndInvokeDelegate<TResult>(IAsyncResult asyncResult);
}
