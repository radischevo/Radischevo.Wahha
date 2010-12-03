using System;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization
{
	public delegate void FieldWriter<T>(T source, MessageWriter writer);
	public delegate void FieldReader<T>(T target, MessageReader reader);

	public interface IFieldIO
	{
		#region Instance Properties
		Type FieldType
		{
			get;
		}
		bool CanCreateWriter
		{
			get;
		}
		bool CanCreateReader
		{
			get;
		}
		#endregion

		#region Instance Methods
		bool CreateReader<T>(MessageField field, out FieldReader<T> reader);
        void AppendWrite(ILGenerator generator, MessageField field);
        void AppendRead(ILGenerator generator, MessageField fiel);
		#endregion
	}
}
