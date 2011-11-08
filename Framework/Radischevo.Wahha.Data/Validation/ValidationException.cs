using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	/// <summary>
	/// The exception that is thrown when a validation error occurs.
	/// </summary>
	[Serializable]
	public class ValidationException : ApplicationException, ISerializable
	{
		#region Instance Fields
		private ValidationError[] _errors;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.ValidationException"/> class.
		/// </summary>
		/// <param name="context">The validation context to copy errors from.</param>
		public ValidationException(ValidationContext context)
			: this(GetValidationErrors(context))
		{
		}

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Radischevo.Wahha.Data.ValidationException"/> class.
		/// </summary>
		/// <param name="errors">The collection of errors detected.</param>
		public ValidationException(IEnumerable<ValidationError> errors)
			: base("Validation failed.")
		{
			Precondition.Require(errors, () => 
				Error.ArgumentNull("errors"));

			_errors = errors.ToArray();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Radischevo.Wahha.Data.ValidationException"/> 
		/// class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected ValidationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_errors = (ValidationError[])info.GetValue("errors", typeof(ValidationError[]));
		}
		#endregion

		#region Instance Properties
		/// <summary>
		/// Gets the collection of validation errors detected.
		/// </summary>
		public IEnumerable<ValidationError> Errors
		{
			get
			{
				return _errors;
			}
		}
		#endregion

		#region Static Methods
		private static IEnumerable<ValidationError> GetValidationErrors(ValidationContext context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			return context.Errors;
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Sets the <see cref="System.Runtime.Serialization.SerializationInfo"/> 
		/// with information about the exception.
		/// </summary>
		/// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds 
		/// the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> 
		/// that contains contextual information about the source or destination.</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errors", _errors);
		}
		#endregion
	}
}
