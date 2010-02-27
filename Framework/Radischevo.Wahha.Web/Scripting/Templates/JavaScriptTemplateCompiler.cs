using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Scripting.Serialization;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public class JavaScriptTemplateCompiler : TemplateCompiler
	{
		#region Nested Types
		private static class CommentTrimmer
		{
			#region Static Methods
			public static string Trim(string text)
			{
				StringBuilder sb = new StringBuilder();
				int state = 0;
				int position = 0;

				for (int i = 0; i < text.Length; i++)
				{
					char c = text[i];
					switch (c)
					{
						case '/':
							switch (state)
							{
								case 0:
								case 1:
									state = 3;
									break;
								case 3:
									sb.Append(text, position, i - position - 1);
									state = 10;
									break;
								case 10:
									break;
								case 20:
									break;
								case 21:
									position = i + 1;
									state = 1;
									break;
							}
							break;

						case '*':
							switch (state)
							{
								case 0:
								case 1:
									state = 1;
									break;
								case 3:
									sb.Append(text, position, i - position - 1);
									state = 20;
									break;
								case 10:
									break;
								case 20:
								case 21:
									state = 21;
									break;
							}
							break;

						case '\n':
						case '\r':
							switch (state)
							{
								case 0:
								case 1:
								case 3:
									state = 1;
									break;
								case 10:
									position = i;
									state = 1;
									break;
								case 20:
								case 21:
									state = 20;
									break;
							}
							break;

						case '\\':
							switch (state)
							{
								case 4:
									state = 5;
									break;
								case 6:
									state = 7;
									break;
							}
							break;

						case '\"':
							switch (state)
							{
								case 0:
								case 1:
								case 3:
								case 5:
									state = 4;
									break;

								case 4:
									state = 1;
									break;
							}
							break;

						case '\'':
							switch (state)
							{
								case 0:
								case 1:
								case 3:
								case 7:
									state = 6;
									break;

								case 6:
									state = 1;
									break;
							}
							break;

						default:
							switch (state)
							{
								case 0:
								case 1:
								case 3:
									state = 1;
									break;
								case 10:
									break;
								case 20:
								case 21:
									state = 20;
									break;
							}
							break;
					}
				}

				if (state < 10)
					sb.Append(text, position, text.Length - position);

				return sb.ToString();
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private StringBuilder _builder;
		private List<TemplateParameter> _parameters;
		private string _templateName;
		private string _codeBufferVariableName;
		private bool _inline;
		#endregion

		#region Constructors
		public JavaScriptTemplateCompiler()
			: this(false, false)
		{
		}

		public JavaScriptTemplateCompiler(bool debug)
			: this(debug, false)
		{
		}

		public JavaScriptTemplateCompiler(bool debug, bool inline)
			: base()
		{
			Debug = debug;
			_inline = inline;
			_parameters = new List<TemplateParameter>();
			_codeBufferVariableName = "$c";
		}
		#endregion

		#region Instance Properties
		public bool Inline
		{
			get
			{
				return _inline;
			}
			set
			{
				_inline = value;
			}
		}

		public string CodeBufferVariableName
		{
			get
			{
				return _codeBufferVariableName;
			}
			set
			{
				Precondition.Defined(value, () => Error.ArgumentNull("value"));
				
				_codeBufferVariableName = EnsureValidMethodName(value);
			}
		}
		#endregion

		#region Static Methods
		private static bool IsIdentifierStartCharacter(char c)
		{
			return (Char.IsLetter(c) || c == '_' || c == '$');
		}

		private static bool IsIdentifierPartCharacter(char c)
		{
			return (Char.IsLetterOrDigit(c) || c == '_' || c == '$');
		}

		private static string EnsureValidMethodName(string methodName)
		{
			StringBuilder sb = new StringBuilder();
			bool lastUnderscore = false;

			for (int i = 0; i < methodName.Length; ++i)
			{
				char c = methodName[i];

				if (i == 0 && IsIdentifierStartCharacter(c))
				{
					sb.Append(c);
					lastUnderscore = false;
				}
				else if (i > 0 && IsIdentifierPartCharacter(c))
				{
					sb.Append(c);
					lastUnderscore = false;
				}
				else if (!lastUnderscore)
				{
					sb.Append('_');
					lastUnderscore = true;
				}
			}
			return sb.ToString();
		}

		private static string GetMethodName(ParsedTemplate template)
		{
			string templateName = template.Name;

			if (String.IsNullOrEmpty(templateName))
				templateName = (String.IsNullOrEmpty(template.VirtualPath))
					? "__untitledTemplate" 
					: Path.GetFileNameWithoutExtension(template.VirtualPath);

			return EnsureValidMethodName(templateName);
		}
		#endregion

		#region Instance Methods
		protected virtual void PrependCodeBlock(string code)
		{
			if (String.IsNullOrEmpty(code))
				return;

			if (_builder == null)
				_builder = new StringBuilder();

			if (Debug) // in debug mode, we prefer to render each code block on a new line. 
				code = String.Concat(code, Environment.NewLine);

			_builder.Insert(0, code);
		}

		protected virtual void AppendCodeBlock(string code)
		{
			if (String.IsNullOrEmpty(code))
				return;

			if (_builder == null)
				_builder = new StringBuilder();

			if (Debug) // in debug mode, we prefer to render each code block on a new line. 
				_builder.AppendLine(code);
			else
				_builder.Append(code);
		}

		protected string GetGeneratedCode()
		{
			if (_builder == null)
				return String.Empty;

			return _builder.ToString();
		}

		protected override void OnPreCompile(ParsedTemplate template)
		{
			_templateName = GetMethodName(template);
			AppendCodeBlock(String.Format("var {0} = [];", 
				CodeBufferVariableName));
		}

		protected override void OnPostCompile(ParsedTemplate template)
		{
			string methodNameFormat = (Inline) 
				? "{0}: function({1}) {{" 
				: "function {0}({1}) {{";

			PrependCodeBlock(String.Format(methodNameFormat, _templateName, 
				String.Join(", ", _parameters.OrderBy(p => p.Index)
				.Select(p => p.Name).ToArray())));
			
			AppendCodeBlock(String.Format("return {0}.join('');", 
				CodeBufferVariableName));

			AppendCodeBlock("}");
		}

		protected override CompiledTemplate CreateCompiledTemplate()
		{
			JavaScriptCompiledTemplate template = 
				new JavaScriptCompiledTemplate(GetGeneratedCode());

			template.Name = _templateName;
			foreach (TemplateParameter parameter in _parameters)
				template.Parameters.Add(parameter);

			return template;
		}

		protected override void VisitDirective(DirectiveExpression directive)
		{
			base.VisitDirective(directive);

			switch (directive.Name.ToUpperInvariant())
			{
				case "PARAMETER": // нам нужно выбрать параметры
					TemplateParameter parameter = new TemplateParameter(
						directive.Attributes["name"],
						Converter.ChangeType<int>(directive.Attributes["index"], 0),
						Type.GetType(directive.Attributes["type"] ?? String.Empty, false, true)
					);
					_parameters.Add(parameter);
					break;
			}
		}

		protected override void VisitCodeBlock(CodeBlockExpression codeBlock)
		{
			base.VisitCodeBlock(codeBlock);
			switch (codeBlock.BlockType)
			{
				case CodeBlockType.Code:
					VisitCodeSnippet(codeBlock);
					break;
				case CodeBlockType.Expression:
					VisitCodeExpression(codeBlock);
					break;
				case CodeBlockType.DataBinding:
					VisitCodeExpression(codeBlock);
					break;
			}
		}

		protected override void VisitLiteral(LiteralExpression literal)
		{
			base.VisitLiteral(literal);
			AppendCodeBlock(String.Format(@"{0}.push({1});", 
				CodeBufferVariableName, 
				JavaScriptString.QuoteString(literal.Literal, true)));
		}

		private void VisitCodeSnippet(CodeBlockExpression codeBlock)
		{
			string code = (Debug)
				? codeBlock.Code
				: CommentTrimmer.Trim(codeBlock.Code);

			AppendCodeBlock(code);
		}

		private void VisitCodeExpression(CodeBlockExpression codeBlock)
		{
			AppendCodeBlock(String.Format("{0}.push({1});", 
				CodeBufferVariableName, codeBlock.Code));
		}
		#endregion
	}
}
