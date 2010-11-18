using System;

namespace Radischevo.Wahha.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public sealed class DeserializeAttribute : ModelBinderAttribute
	{
		#region Nested Types
		private sealed class DeserializingModelBinder : ModelBinderBase
		{
			#region Instance Fields
			private readonly SerializationMode _mode;
			#endregion

			#region Constructors
			public DeserializingModelBinder(SerializationMode mode)
				: base()
			{
				_mode = mode;
			}
			#endregion

			#region Instance Methods
			protected override object ExecuteBind(BindingContext context)
			{
				ValueProviderResult value;
				if (!context.TryGetValue(out value))
					return null;

				string stringValue = value.GetValue<string>();
				if (String.IsNullOrEmpty(stringValue))
					return null;

				ModelStateSerializer serializer = new ModelStateSerializer();
				return serializer.Deserialize(stringValue, _mode);
			}
			#endregion
		}
		#endregion

		#region Constants
		private const SerializationMode _defaultMode = SerializationMode.Plaintext;
		#endregion

		#region Instance Fields
		private SerializationMode _mode;
		#endregion

		#region Constructors
		public DeserializeAttribute()
			: this(_defaultMode)
		{
		}

		public DeserializeAttribute(SerializationMode mode)
		{
			_mode = mode;
		}
		#endregion

		#region Instance Properties
		public SerializationMode Mode
		{
			get
			{
				return _mode;
			}
		}
		#endregion

		#region Instance Methods
		public override IModelBinder GetBinder()
		{
			return new DeserializingModelBinder(Mode);
		}
		#endregion
	}
}
