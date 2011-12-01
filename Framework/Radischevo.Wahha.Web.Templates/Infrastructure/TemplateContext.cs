using System;
using System.Collections.Generic;
using System.IO;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Templates
{
	public class TemplateContext
	{
		#region Instance Fields
		private HttpContextBase _context;
		private ValueDictionary _data;
		private object _model;
		private TemplateRenderingBase _template;
		private Stack<TextWriter> _output;
		private Action<TextWriter> _bodyRenderer;
		private Stack<Dictionary<string, SectionWriter>> _sectionWriters;
		#endregion

		#region Constructors
		public TemplateContext()
			: this(null, null, null)
		{
		}

		public TemplateContext(HttpContextBase context,
			TemplateRenderingBase template, object model)
		{
			_context = context;
			_template = template;
			_model = model;
		}
		#endregion

		#region Static Properties
		public static TemplateContext Current
		{
			get
			{
				System.Web.HttpContext context = System.Web.HttpContext.Current;
				if (context != null)
				{
					HttpContextBase wrapper = new HttpContextWrapper(context);
					ITemplateFile template = TemplateStack.CurrentTemplate(wrapper);
					TemplateRenderingBase current = (template as TemplateRenderingBase);

					return (current == null) ? null : current.TemplateContext;
				}
				return null;
			}
		}
		#endregion

		#region Instance Properties
		public HttpContextBase Context
		{
			get
			{
				return _context;
			}
		}

		public ValueDictionary Data
		{
			get
			{
				if (_data == null)
					_data = new ValueDictionary();

				return _data;
			}
			set
			{
				_data = value;
			}
		}

		public object Model
		{
			get
			{
				return _model;
			}
		}

		public TemplateRenderingBase Template
		{
			get
			{
				return _template;
			}
			set
			{
				_template = value;
			}
		}

		internal Action<TextWriter> BodyRenderer
		{
			get
			{
				return _bodyRenderer;
			}
			set
			{
				_bodyRenderer = value;
			}
		}

		internal Stack<TextWriter> Output
		{
			get
			{
				if (_output == null)
					_output = new Stack<TextWriter>();

				return _output;
			}
			set
			{
				_output = value;
			}
		}

		internal Stack<Dictionary<string, SectionWriter>> SectionWriters
		{
			get
			{
				if (_sectionWriters == null)
					_sectionWriters = new Stack<Dictionary<string, SectionWriter>>();
				
				return _sectionWriters;
			}
			set
			{
				_sectionWriters = value;
			}
		}
		#endregion
	}
}
