using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core.Serialization.Fields;

namespace Radischevo.Wahha.Core.Serialization
{
	internal class Serializer<T>
	{
		#region Static Fields
		private static readonly KeyValuePair<int, FieldReader<T>>[] _fields = GetFields();
		public static readonly FieldWriter<T> FieldWriter = Message.CreateFieldWriter<T>();
		#endregion

		#region Instance Fields
		private int _position;
		private KeyValuePair<int, FieldReader<T>> _current;
		#endregion

		#region Constructors
		public Serializer()
		{
			_position = -1;
		}
		#endregion

		#region Instance Properties
		public int Current
		{
			get
			{
				return _current.Key;
			}
		}
		#endregion

		#region Static Methods
		private static KeyValuePair<int, FieldReader<T>>[] GetFields()
		{
			List<KeyValuePair<int, FieldReader<T>>> fields = 
				new List<KeyValuePair<int, FieldReader<T>>>();

			Message.ForEachField(typeof(T),
				field => fields.Add(new KeyValuePair<int, FieldReader<T>>(
					field.Header, field.GetFieldReader<T>())));

			fields.Sort((x, y) => x.Key - y.Key);
			return fields.ToArray();
		}
		#endregion

		#region Instance Methods
		private bool FindReader(int header)
		{
			if (header == Current)
				return true;

			if (header > Current)
			{
				for (int i = _position + 1; i != _fields.Length; ++i)
				{
					var current = _fields[i];
					if (current.Key == header)
					{
						_position = i;
						_current = current;
						return true;
					}
				}
			}
			for (int i = _position; i-- > 0; )
			{
				var current = _fields[i];
				if (current.Key == header)
				{
					_position = i;
					_current = current;
					return true;
				}
			}
			return false;
		}

		public bool TryGetFieldReader(MessageTag tag)
		{
			if (FindReader(tag.Value))
				return true;

			return (tag.WireType == WireType.StartGroup && 
				FindReader(tag.WithWireType(WireType.String)));
		}

		public T Deserialize(MessageReader reader, T target, 
			UnknownFieldCollection missing)
		{
			MessageTag tag = new MessageTag();
			while (reader.TryReadMessageTag(ref tag))
			{
				if (TryGetFieldReader(tag))
				{
					if (tag.WireType == WireType.String)
						_current.Value(target, reader.CreateSubReader());

					else
						try
						{
							_current.Value(target, reader);
						}
						catch (UnknownEnumException e)
						{
							missing.Add(new UnknownFieldVarint(tag, e.Value));
						}
				}
				else if (tag.WireType == WireType.EndGroup)
					break;

				else if (tag.WireType < WireType.MaxValid)
					missing.Add(tag, reader);

				else
					throw new NotSupportedException();
			}
			return target;
		}
		#endregion		
	}
}
