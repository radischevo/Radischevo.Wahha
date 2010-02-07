using System;

namespace Radischevo.Wahha.Web.Mvc.Validation
{
	public class ModelValidationContext : ControllerContext
	{
		#region Instance Fields
		private ModelMetadata _metadata;
		private object _container;
		private object _value;
		#endregion

		#region Constructors
		public ModelValidationContext(ControllerContext context, 
			ModelMetadata metadata, object value)
			: this(context, metadata, null, value)
		{
		}

		public ModelValidationContext(ControllerContext context, 
			ModelMetadata metadata, object container, object value)
			: base(context)
		{
			_metadata = metadata;
			_container = container;
			_value = value;
		}
		#endregion

		#region Instance Properties
		public ModelMetadata Metadata
		{
			get
			{
				return _metadata;
			}
			set
			{
				_metadata = value;
			}
		}

		public object Container
		{
			get
			{
				return _container;
			}
			set
			{
				_container = value;
			}
		}

		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
		#endregion
	}
}
