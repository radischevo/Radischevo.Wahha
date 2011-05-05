using System;
using System.Runtime.Serialization;

namespace Radischevo.Wahha.Data
{
	public sealed class DbOperationValidationException : ApplicationException
	{
		#region Constructors
		public DbOperationValidationException()
			: base()
		{
		}

		public DbOperationValidationException(string message)
			: base(message)
		{
		}

		public DbOperationValidationException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected DbOperationValidationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion
	}
}
