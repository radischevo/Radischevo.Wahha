using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization
{
	public abstract class FieldIOBase : IFieldIO
	{
		#region Instance Fields
		protected PropertyInfo _property;
		#endregion

		#region Constructors
		protected FieldIOBase(PropertyInfo property)
		{
			Precondition.Require(property, () =>
				Error.ArgumentNull("property"));

			_property = property;
		}
		#endregion

		#region Instance Properties
		protected PropertyInfo Property
		{
			get
			{
				return _property;
			}
		}

		public virtual Type FieldType
		{
			get
			{
				return _property.PropertyType;
			}
		}

		public bool CanCreateWriter
		{
			get
			{
				return _property.CanWrite;
			}
		}
		public bool CanCreateReader
		{
			get
			{
				return _property.CanRead;
			}
		}
		#endregion

		#region Instance Methods
		public abstract void AppendWrite(ILGenerator generator, MessageField field);
		
		public abstract void AppendRead(ILGenerator generator, MessageField field);

		public bool CreateReader<T>(MessageField field, out FieldReader<T> reader)
		{
			DynamicMethod builder = Message.BeginReadMethod(_property.DeclaringType, typeof(T));
			AppendRead(builder.GetILGenerator(), field);
			reader = Message.EndMethod<FieldReader<T>>(builder);

			return true;
		}

		public void AppendGetField(ILGenerator generator)
		{
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Call, _property.GetGetMethod());
		}
		#endregion		
	}
}
