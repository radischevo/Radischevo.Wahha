using System;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public sealed class TemplateParameter
	{
		#region Instance Fields
		private int _index;
		private string _name;
		private Type _type;
		#endregion

		#region Constructors
		public TemplateParameter(string name)
			: this(name, 0, null)
		{
		}

		public TemplateParameter(string name, int index)
			: this(name, index, null)
		{
		}

		public TemplateParameter(string name, int index,
			Type type)
		{
			_index = index;
			_name = name;
			_type = type;
		}
		#endregion

		#region Instance Properties
		public string Name
		{
			get
			{
				return _name ?? String.Empty;
			}
			set
			{
				_name = value;
			}
		}

		public int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = value;
			}
		}

		public Type Type
		{
			get
			{
				return _type ?? typeof(object);
			}
			set
			{
				_type = value;
			}
		}
		#endregion
	}
}
