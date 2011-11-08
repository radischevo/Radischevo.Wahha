using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	[Serializable]
	public class ValidationError
	{
		#region Instance Fields
		private readonly string _key;
		private readonly string _message;
		private readonly Exception _exception;
		#endregion

		#region Constructors
		public ValidationError(string message)
			: this(null, message)
		{
		}

		public ValidationError(Exception exception)
			: this(null, exception)
		{
		}

		public ValidationError(string key, string message)
		{
			Precondition.Defined(message, () =>
				Error.ArgumentNull("message"));

			_key = key;
			_message = message;
		}

		public ValidationError(string key, Exception exception)
		{
			Precondition.Require(exception, () => 
				Error.ArgumentNull("exception"));

			_key = key;
			_exception = exception;
			_message = exception.Message;
		}
		#endregion

		#region Instance Properties
		public string Key
		{
			get
			{
				return _key;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
		}

		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}
		#endregion
	}
}
