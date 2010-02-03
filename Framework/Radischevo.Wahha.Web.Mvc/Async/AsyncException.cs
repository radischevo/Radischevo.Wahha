using System;
using System.Runtime.Serialization;
using System.Web;

namespace Radischevo.Wahha.Web.Mvc.Async
{
	[Serializable]
	public sealed class AsyncException : HttpException
	{
		#region Constructors
		public AsyncException()
		{
		}

		private AsyncException(SerializationInfo info, 
			StreamingContext context)
			: base(info, context)
		{
		}

		public AsyncException(string message)
			: base(message)
		{
		}

		public AsyncException(string message, Exception inner)
			: base(message, inner)
		{
		}
		#endregion
	}
}
