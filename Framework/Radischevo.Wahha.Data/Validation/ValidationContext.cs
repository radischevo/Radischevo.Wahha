using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public sealed class ValidationContext
	{
		#region Instance Fields
		private readonly ValidationErrorCollection _errors;
		#endregion

		#region Constructors
		public ValidationContext()
		{
			_errors = new ValidationErrorCollection();
		}
		#endregion

		#region Instance Properties
		public bool IsValid
		{
			get
			{
				return (_errors.Count < 1);
			}
		}

		public ValidationErrorCollection Errors
		{
			get
			{
				return _errors;
			}
		}
		#endregion
	}
}
