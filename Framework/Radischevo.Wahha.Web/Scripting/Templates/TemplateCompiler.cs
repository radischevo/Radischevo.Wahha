using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public abstract class TemplateCompiler
	{
		#region Instance Fields
		private bool _debug;
		#endregion

		#region Constructors
		protected TemplateCompiler()
		{
		}
		#endregion

		#region Instance Properties
		public bool Debug 
		{
			get
			{
				return _debug;
			}
			set
			{
				_debug = value;
			}
		}
		#endregion

		#region Instance Methods
		public CompiledTemplate Build(ParsedTemplate template)
		{
			Precondition.Require(template, () => Error.ArgumentNull("template"));
			OnPreCompile(template);

			foreach (TemplateExpression expression in template.ExpressionTree)
				VisitExpression(expression);

			OnPostCompile(template);
			return CreateCompiledTemplate();
		}

		protected abstract CompiledTemplate CreateCompiledTemplate();

		protected virtual void OnPreCompile(ParsedTemplate template)
		{
		}

		protected virtual void OnPostCompile(ParsedTemplate template)
		{
		}

		protected virtual void VisitExpression(TemplateExpression expression)
		{
			switch (expression.Type)
			{
				case TemplateExpressionType.Literal:
					VisitLiteral((LiteralExpression)expression);
					break;
				case TemplateExpressionType.CodeBlock:
					VisitCodeBlock((CodeBlockExpression)expression);
					break;
				case TemplateExpressionType.Directive:
					VisitDirective((DirectiveExpression)expression);
					break;
			}
		}

		protected virtual void VisitLiteral(LiteralExpression literal)
		{
		}

		protected virtual void VisitCodeBlock(CodeBlockExpression codeBlock)
		{
		}

		protected virtual void VisitDirective(DirectiveExpression directive)
		{
		}
		#endregion
	}
}
