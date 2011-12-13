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
		private readonly object _attemptedValue;
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
			: this (key, null, message)
		{
		}
		
		public ValidationError(string key, Exception exception)
			: this(key, null, exception)
		{
		}

		public ValidationError(string key, object attemptedValue, string message)
			: this (key, attemptedValue, message, null)
		{
		}
		
		public ValidationError(string key, object attemptedValue, Exception exception)
			: this (key, attemptedValue, null, exception)
		{
		}
		
		public ValidationError(string key, object attemptedValue, 
			string message, Exception exception)
		{
			_key = key;
			_attemptedValue = attemptedValue;
			_message = message ?? ((exception == null) 
				? message : exception.Message);
			_exception = exception;
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
		
		public object AttemptedValue
		{
			get
			{
				return _attemptedValue;
			}
		}
		#endregion
	}
}
