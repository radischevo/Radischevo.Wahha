using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public class TemplateContext
	{
		#region Instance Fields
		private TextWriter _writer;
		private ICollection<object> _parameters;
		private string _virtualPath;
		#endregion

		#region Constructors
		public TemplateContext(string virtualPath, TextWriter writer)
		{
			Precondition.Require(writer, () => Error.ArgumentNull("writer"));

			_writer = writer;
			_virtualPath = virtualPath;
			_parameters = new List<object>();
		}

		public TemplateContext(string virtualPath,
			TextWriter writer, params object[] parameters)
			: this(virtualPath, writer)
		{
			_parameters = CreateParameters(parameters);
		}

		public TemplateContext(string virtualPath,
			TextWriter writer, IEnumerable parameters)
			: this(virtualPath, writer)
		{
			_parameters = CreateParameters(parameters);
		}
		#endregion

		#region Instance Properties
		public TextWriter Writer
		{
			get
			{
				return _writer;
			}
		}

		public string VirtualPath
		{
			get
			{
				return _virtualPath ?? "/";
			}
			set
			{
				_virtualPath = value;
			}
		}

		public ICollection<object> Parameters
		{
			get
			{
				return _parameters;
			}
		}
		#endregion

		#region Static Methods
		private static ICollection<object> CreateParameters(IEnumerable parameters)
		{
			List<object> list = new List<object>();
			if (parameters == null)
				return list;

			foreach (object item in parameters)
				list.Add(item);

			return list;
		}
		#endregion
	}
}
