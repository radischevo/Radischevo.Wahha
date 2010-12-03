using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Radischevo.Wahha.Core.Serialization
{
	public class UnknownFieldCollection : IEnumerable<UnknownField>
	{
		#region Instance Fields
		private List<UnknownField> _fields;
		#endregion

		#region Constructors
		public UnknownFieldCollection()
		{
			_fields = new List<UnknownField>();
		}
		#endregion

		#region Instance Properties
		public int Count
		{
			get
			{
				return _fields.Count;
			}
		}

		public UnknownField this[int index]
		{
			get
			{
				return _fields[index];
			}
		}
		#endregion

		#region Instance Methods
		internal void Add(UnknownField field)
		{
			_fields.Add(field);
		}

		public void Add(MessageTag tag, MessageReader reader)
		{
			Add(UnknownField.Create(tag, reader));
		}

		public void Serialize(Stream stream)
		{
			Serialize(new MessageWriter(stream));
		}

		public void Serialize(MessageWriter writer)
		{
			_fields.ForEach(x => x.Serialize(writer));
		}

		public IEnumerator<UnknownField> GetEnumerator()
		{
			return _fields.GetEnumerator();
		}
		#endregion

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}
