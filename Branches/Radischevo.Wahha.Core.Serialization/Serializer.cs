using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Radischevo.Wahha.Core.Serialization
{
	public static class Serializer
	{
		#region Static Methods
		private static object CreateDefault(object obj)
		{
			Array.ForEach(obj.GetType().GetProperties(), item => {
				DefaultValueAttribute attribute = item
					.GetCustomAttributes<DefaultValueAttribute>(false)
					.FirstOrDefault();

				if (attribute != null)
					item.SetValue(obj, CreateDefaultItem(item.PropertyType, attribute.Value), null);
			});
			return obj;
		}

		private static object CreateDefaultItem(Type type, string str)
		{
			if (type.IsEnum)
				return Enum.Parse(type, str);

			Type[] stringMethodArg = new Type[] { typeof(string) };

			MethodInfo parse = type.GetMethod("Parse", stringMethodArg);
			if (parse != null)
				return parse.Invoke(null, new object[] { str });

			MethodInfo tryParse = type.GetMethod("TryParse", new Type[] { 
				typeof(string), type.MakeByRefType() 
			});
			if (tryParse != null)
			{
				object[] args = new object[] { str, null };
				if ((bool)tryParse.Invoke(null, args))
					return args[1];
			}

			ConstructorInfo constructor = type.GetConstructor(stringMethodArg);
			if (constructor != null)
				return constructor.Invoke(new object[] { str });

			return null;
		}

		public static T CreateDefault<T>() where T : class, new()
		{
			return CreateDefault(new T()) as T;
		}

		public static T CreateDefaultItem<T>(string s)
		{
			return (T)CreateDefaultItem(typeof(T), s);
		}

		public static void Serialize<T>(MessageWriter writer, T value)
		{
			Serializer<T>.FieldWriter(value, writer);
		}

		public static void Serialize<T>(Stream stream, T value)
		{
			Serialize(new MessageWriter(stream), value);
		}

		public static T Deserialize<T>(Stream stream, UnknownFieldCollection missing) 
			where T : new()
		{
			return Deserialize<T>(new MessageReader(stream), missing);
		}

		public static T Deserialize<T>(Stream stream) 
			where T : new()
		{
			return Deserialize<T>(new MessageReader(stream), new UnknownFieldCollection());
		}

		public static T Deserialize<T>(MessageReader reader, UnknownFieldCollection missing) 
			where T : new()
		{
			return new Serializer<T>().Deserialize(reader, new T(), missing);
		}

		public static T Deserialize<T>(MessageReader reader) 
			where T : new()
		{
			return new Serializer<T>().Deserialize(reader, new T(), new UnknownFieldCollection());
		}
		#endregion
	}
}
