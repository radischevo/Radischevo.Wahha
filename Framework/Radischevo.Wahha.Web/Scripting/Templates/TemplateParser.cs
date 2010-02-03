using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.RegularExpressions;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public class TemplateParser
	{
		#region Static Fields
		protected static Regex _directiveRegex = new DirectiveRegex();
		protected static Regex _aspCodeRegex = new AspCodeRegex();
		protected static Regex _aspExprRegex = new AspExprRegex();
		protected static Regex _databindExprRegex = new DatabindExprRegex();
		protected static Regex _commentRegex = new CommentRegex();
		protected static Regex _textRegex = new TextRegex();
		protected static Regex _includeRegex = new IncludeRegex();
		#endregion

		#region Instance Fields
		private StringBuilder _literalBuilder;
		private List<TemplateExpression> _expressions;
		private bool _hasMainDirective;
		private string _templateName;
		private string _virtualPath;
		private int _lineNumber;
		#endregion	

		#region Constructors
		public TemplateParser()
		{
		}
		#endregion

		#region Instance Properties
		protected virtual ICollection<TemplateExpression> Expressions
		{
			get
			{
				if (_expressions == null)
					_expressions = new List<TemplateExpression>();

				return _expressions;
			}
		}
		#endregion

		#region Static Methods
		private static int LineCount(string text, int offset, int newOffset)
		{
			int count = 0;
			while (offset < newOffset)
			{
				if ((text[offset] == '\r' || text[offset] == '\n') && 
					(offset == 0 || text[offset - 1] != '\r'))
					count++;
				
				offset++;
			}
			return count;
		}

		private static bool IsAbsolutePhysicalPath(string path)
		{
			if (path == null || path.Length < 3)
				return false;
			
			return ((path[1] == Path.VolumeSeparatorChar && 
				IsDirectorySeparatorChar(path[2])) || IsUncSharePath(path));
		}

		private static bool IsDirectorySeparatorChar(char ch)
		{
			if (ch != Path.DirectorySeparatorChar)
				return (ch == Path.AltDirectorySeparatorChar);
			
			return true;
		}

		private static bool IsUncSharePath(string path)
		{
			return (path.Length > 2 && IsDirectorySeparatorChar(path[0]) 
				&& IsDirectorySeparatorChar(path[1]));
		}

		protected static bool IsWhiteSpaceString(string s)
		{
			return (s.Trim().Length == 0);
		}

		protected static string TrimWhiteSpaceChars(string s)
		{
			StringBuilder inner = new StringBuilder();
			for (int i = 0; i < s.Length; ++i)
			{
				if (s[i] == '\r' || s[i] == '\n' || s[i] == '\t')
					continue;

				inner.Append(s[i]);
			}
			return inner.ToString();
		}
		#endregion

		#region Helper Methods
		protected virtual string ConstructPhysicalPath(string path, out string virtualPath)
		{
			string appPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;
			if (!IsAbsolutePhysicalPath(path))
			{
				try
				{
					path = MapVirtualPath(path);
				}
				catch (ArgumentException)
				{
					path = Path.GetFullPath(Path.Combine(
						Path.GetDirectoryName(appPhysicalPath),
						path.Replace('/', '\\').Trim('\\')));
				}
			}
			
			if (IsUncSharePath(path))
			{
				virtualPath = Path.GetFileName(path);
			}
			else
			{
				string relativePath = "~/" + path.Replace(appPhysicalPath,
					String.Empty).Replace('\\', '/');

				virtualPath = VirtualPathUtility.ToAbsolute(relativePath,
					HostingEnvironment.ApplicationVirtualPath);
			}
			return path;
		}

		protected virtual string MapVirtualPath(string virtualPath)
		{
			return HostingEnvironment.MapPath(virtualPath);
		}

		protected virtual void AddLiteral(string literal)
		{
			if (_literalBuilder == null)
				_literalBuilder = new StringBuilder();

			_literalBuilder.Append(TrimWhiteSpaceChars(literal));
		}

		protected virtual void ProcessLiteral()
		{
			if (_literalBuilder != null)
			{
				string literal = _literalBuilder.ToString();

				if (!IsWhiteSpaceString(literal))
					Expressions.Add(new LiteralExpression(literal));
			}
			_literalBuilder = null;
		}

		protected virtual void AddDirective(Match match, string text)
		{
			ProcessLiteral();

			CaptureCollection names = match.Groups["attrname"].Captures;
			CaptureCollection values = match.Groups["attrval"].Captures;

			Precondition.Require(names.Count > 0, 
				Error.TemplateDirectiveCannotBeEmpty(_virtualPath, text, _lineNumber + 1));

			string directiveName = names[0].Value;
			DirectiveExpression directive = new DirectiveExpression(directiveName);
			for (int i = 1; i < names.Count; ++i)
				directive.Attributes.Add(names[i].Value, values[i].Value);

			if (String.Equals(directiveName, ParsedTemplate.MainDirectiveName,
				StringComparison.InvariantCultureIgnoreCase))
			{
				if (_hasMainDirective)
					throw Error.DuplicateMainDirective(ParsedTemplate.MainDirectiveName, 
						_virtualPath, text, _lineNumber + 1);

				_hasMainDirective = true;
				_templateName = directive.Attributes["name"];
			}			
			Expressions.Add(directive);
		}

		protected void AddInclude(Match match, string text)
		{
			ProcessLiteral();

			string type = match.Groups["pathtype"].Value ?? "file";
			string fileName = match.Groups["filename"].Value;

			if (String.IsNullOrEmpty(fileName))
				throw Error.ArgumentNull("fileName");
			
			switch (type.ToUpperInvariant())
			{
				case "VIRTUAL":
					fileName = MapVirtualPath(fileName);
					break;
				case "FILE":
					if (!IsAbsolutePhysicalPath(fileName))
					{
						try
						{
							fileName = MapVirtualPath(fileName);
						}
						catch (ArgumentException)
						{
							string currentPath = (String.IsNullOrEmpty(_virtualPath)) ? 
								MapVirtualPath("~/") : MapVirtualPath(_virtualPath);

							fileName = Path.GetFullPath(Path.Combine(
								Path.GetDirectoryName(currentPath),
								fileName.Replace('/', '\\').Trim('\\')));
						}
					}
					break;
			}
			ProcessInclude(fileName);
		}

		protected virtual void ProcessInclude(string path)
		{
			TemplateParser subParser = new TemplateParser();
			try
			{
				subParser.ParseFile(path);

				foreach (TemplateExpression expr in subParser.Expressions)
				{
					if (expr is DirectiveExpression)
						continue;

					Expressions.Add(expr);
				}
			}
			catch(Exception ex)
			{
				if (ex is HttpParseException) // если произошла ошибка разбора, кидаем, как есть.
					throw;

				throw Error.CouldNotParseNestedTemplate(ex, _virtualPath, _lineNumber + 1);
			}
		}

		protected virtual void AddCodeBlock(Match match, CodeBlockType type, string text)
		{
			ProcessLiteral();

			Group group = match.Groups["code"];
			string s = group.Value.Replace(@"%\>", "%>");
			int lineNumber = _lineNumber;
			
			if (type != CodeBlockType.Code)
			{
				int length = -1;
				for (int i = 0; (i < s.Length && Char.IsWhiteSpace(s[i])); ++i)
				{
					if ((s[i] == '\r' || s[i] == '\n') && 
						(i == 0 || s[i - 1] != '\r'))
					{
						lineNumber++;
						length = i;
					}
					else if (s[i] == '\n')
						length = i;
				}

				if (length >= 0)
					s = s.Substring(length + 1);
				
				length = -1;
				for (int j = s.Length - 1; j >= 0 && Char.IsWhiteSpace(s[j]); j--)
				{
					if (s[j] == '\r' || s[j] == '\n')
						length = j;
				}
				
				if (length >= 0)
					s = s.Substring(0, length);

				if (IsWhiteSpaceString(s))
					throw Error.EmptyCodeRenderExpression(_virtualPath, text, _lineNumber + 1);
			}
			Expressions.Add(new CodeBlockExpression(type, s));
		}

		protected virtual void HandlePreParse()
		{
		}

		protected virtual void ParseInternal(string text, Encoding encoding)
		{
			Precondition.Require(text, Error.ArgumentNull("text"));

			int startIndex = 0;
			int length = text.Length;

			Match match;
			while (startIndex < text.Length)
			{
				if ((match = _textRegex.Match(text, startIndex)).Success)
				{
					AddLiteral(match.ToString());

					_lineNumber += LineCount(text, startIndex, match.Index + match.Length);
					startIndex = match.Index + match.Length;
				}
				if ((match = _directiveRegex.Match(text, startIndex)).Success)
				{
					AddDirective(match, text);
				}
				else
				{
					if (!(match = _commentRegex.Match(text, startIndex)).Success)
					{
						if ((match = _includeRegex.Match(text, startIndex)).Success)
							AddInclude(match, text);
						else if ((match = _aspExprRegex.Match(text, startIndex)).Success)
							AddCodeBlock(match, CodeBlockType.Expression, text);
						else if ((match = _databindExprRegex.Match(text, startIndex)).Success)
							AddCodeBlock(match, CodeBlockType.DataBinding, text);
						else if ((match = _aspCodeRegex.Match(text, startIndex)).Success)
							AddCodeBlock(match, CodeBlockType.Code, text);
					}
				}
				if (match == null || !match.Success)
				{
					if(++startIndex < length)
						AddLiteral("<");
				}
				else
				{
					_lineNumber += LineCount(text, startIndex, match.Index + match.Length);
					startIndex = match.Index + match.Length;
				}
			}
			ProcessLiteral();
		}

		protected virtual void HandlePostParse()
		{
		}
		#endregion

		#region Instance Methods
		public ParsedTemplate ParseFile(string fileName)
		{
			Precondition.Require(!String.IsNullOrEmpty(fileName),
				Error.ArgumentNull("fileName"));

			using (StreamReader reader = new StreamReader(
				ConstructPhysicalPath(fileName, out _virtualPath)))
			{
				return Parse(reader);
			}
		}

		public ParsedTemplate Parse(TextReader reader)
		{
			Precondition.Require(reader, Error.ArgumentNull("reader"));
			string text = reader.ReadToEnd();

			return Parse(text);
		}

		public ParsedTemplate Parse(string text)
		{
			return Parse(text, Encoding.UTF8);
		}

		public ParsedTemplate Parse(string text, Encoding encoding)
		{
			HandlePreParse();
			ParseInternal(text, encoding);
			HandlePostParse();

			ParsedTemplate template = 
				new ParsedTemplate(_virtualPath, encoding, Expressions);

			template.Name = _templateName;
			return template;
		}
		#endregion
	}
}