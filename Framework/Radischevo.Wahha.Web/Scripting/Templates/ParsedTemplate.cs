using System;
using System.Collections.Generic;
using System.Text;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public class ParsedTemplate
	{
		#region Constants
		public const string MainDirectiveName = "template";
		#endregion

		#region Instance Fields
		private string _virtualPath;
		private string _name;
		private Encoding _encoding;
		private List<TemplateExpression> _expressionTree;
		#endregion

		#region Constructors
		public ParsedTemplate(string virtualPath)
			: this(virtualPath, Encoding.UTF8, null)
		{
		}

		public ParsedTemplate(string virtualPath, 
			Encoding encoding)
			: this(virtualPath, encoding, null) 
		{
		}

		public ParsedTemplate(string virtualPath,
			IEnumerable<TemplateExpression> tree)
			: this(virtualPath, Encoding.UTF8, tree)
		{
		}

		public ParsedTemplate(string virtualPath,
			Encoding encoding, 
			IEnumerable<TemplateExpression> tree)
		{
			_virtualPath = virtualPath;
			_encoding = encoding;

			_expressionTree = (tree == null) 
				? new List<TemplateExpression>()
				: new List<TemplateExpression>(tree);
		}
		#endregion

		#region Instance Properties
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public string VirtualPath
		{
			get
			{
				return _virtualPath ?? String.Empty;
			}
			set
			{
				_virtualPath = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return _encoding ?? Encoding.UTF8;
			}
			set
			{
				_encoding = value;
			}
		}

		public ICollection<TemplateExpression> ExpressionTree
		{
			get
			{
				return _expressionTree;
			}
		}
		#endregion
	}
}
