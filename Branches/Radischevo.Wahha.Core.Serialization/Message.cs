using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization
{
	public static class Message
	{
		#region Static Methods
		private static DynamicMethod StoreTypeSafeArg0(DynamicMethod method, Type type, Type arg0)
		{
			ILGenerator generator = method.GetILGenerator();
			generator.DeclareLocal(type);
			generator.Emit(OpCodes.Ldarg_0);
			if (type != arg0)
				generator.Emit(OpCodes.Castclass, type);

			generator.Emit(OpCodes.Stloc_0);
			return method;
		}
		
		public static T EndMethod<T>(DynamicMethod writer) 
			where T : class
		{
			ILGenerator generator = writer.GetILGenerator();
			generator.Emit(OpCodes.Ret);

			return writer.CreateDelegate(typeof(T)) as T;
		}
		
		public static DynamicMethod BeginReadMethod(Type type, Type arg0)
		{
			DynamicMethod reader = new DynamicMethod(String.Format("DynamicRead{0}", type.Name), 
				null, new Type[] { arg0, typeof(MessageReader) }, true);

			return StoreTypeSafeArg0(reader, type, arg0);
		}

		public static FieldWriter<T> CreateFieldWriter<T>()
		{
			return EndMethod<FieldWriter<T>>(BeginWriteMethod(typeof(T)));
		}

		public static DynamicMethod BeginWriteMethod(Type type)
		{
			DynamicMethod writer = BeginWriteMethod(type, type);
			ILGenerator il = writer.GetILGenerator();
			ForEachField(type, x => {
				if (x.CanAppendWrite)
					x.AppendWriteBody(il);
			});
			return writer;
		}

		public static DynamicMethod BeginWriteMethod(Type type, Type arg0)
		{
			DynamicMethod writer = new DynamicMethod(String.Format("DynamicWrite{0}", type.Name),
				null, new Type[] { arg0, typeof(MessageWriter) }, true);

			return StoreTypeSafeArg0(writer, type, arg0);
		}

		public static void ForEachField(Type messageType, Action<MessageField> action)
		{
			PropertyInfo[] properties = messageType.GetProperties();
			List<MessageField> fields = new List<MessageField>(properties.Length);

			properties.ForEach((field, index) => {
				TagAttribute attribute = field
					.GetCustomAttributes<TagAttribute>(false)
					.FirstOrDefault();

				if(attribute != null)
					fields.Add(MessageField.Create(attribute, field));
			});

			fields.Sort((x, y) => x.Number - y.Number);
			fields.ForEach(action);
		}

		public static int CountFields(object obj)
		{
			int count = 0;
			ForEachField(obj.GetType(), x => ++count);

			return count;
		}
		#endregion
	}
}
