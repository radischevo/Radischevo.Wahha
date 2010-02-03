using System;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public class CodeBlockExpression : TemplateExpression
	{
		#region Instance Fields
		private string _code;
		private CodeBlockType _blockType;
		#endregion

		#region Constructors
		public CodeBlockExpression(CodeBlockType type, string code)
			: base(TemplateExpressionType.CodeBlock)
		{
			_blockType = type;
			_code = code;
		}
		#endregion

		#region Instance Properties
		public CodeBlockType BlockType
		{
			get
			{
				return _blockType;
			}
		}

		public string Code
		{
			get
			{
				return _code ?? String.Empty;
			}
			set
			{
				_code = value;
			}
		}
		#endregion

		#region Instance Methods
		public override string ToString()
		{
			return String.Format("{{ Code Block: {0}, Type: {1} }}", Code, BlockType);
		}
		#endregion
	}
}
