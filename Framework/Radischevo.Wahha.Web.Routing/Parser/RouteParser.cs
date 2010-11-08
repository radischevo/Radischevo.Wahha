using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    internal static class RouteParser
	{
		#region Nested Types
		private enum State : int
		{
			Literal = 0,
			PossibleEscapedVariable,
			EscapedVariable,
			Variable,
			AfterVariable,
			AfterEscapedVariable,
			PossibleEscapedParameter,
			EscapedParameter,
			Parameter,
			AfterParameter,
			AfterEscapedParameter
		}
		#endregion

		#region Constants
		public const char PathSeparator = '/';
		public const char ParameterStart = '{';
		public const char ParameterEnd = '}';
		public const char VariableStart = '[';
		public const char VariableEnd = ']';
		private const string ESCAPED_PARAMETER_START = "{{";
		private const string ESCAPED_PARAMETER_END = "}}";
		private const string ESCAPED_VARIABLE_START = "[[";
		private const string ESCAPED_VARIABLE_END = "]]";
        #endregion

		#region Static Methods
		public static ParsedRoute Parse(string url)
        {
            if (url == null)
                url = String.Empty;

            bool isAppRelative = (url.StartsWith("~/"));
            bool isRelative = (isAppRelative || url.StartsWith("/"));
            url = url.TrimStart('~');

            IEnumerable<string> segments = SplitUrl(url);
            ValidateUrlParts(segments);

            return new ParsedRoute(SplitSegments(segments), isRelative, isAppRelative);
        }

		internal static bool IsSeparator(string str)
		{
			return String.Equals(str, Char.ToString(PathSeparator), StringComparison.Ordinal);
		}

        private static string GetLiteral(string literal)
        {
			if (String.IsNullOrEmpty(literal))
				return String.Empty;

			return literal
				.Replace(ESCAPED_PARAMETER_START, ParameterStart.ToString())
				.Replace(ESCAPED_PARAMETER_END, ParameterEnd.ToString())
				.Replace(ESCAPED_VARIABLE_START, VariableStart.ToString())
				.Replace(ESCAPED_VARIABLE_END, VariableEnd.ToString());
        }

        private static bool IsValidParameterName(string parameterName)
        {
            if (parameterName.Length == 0)
                return false;
            
            for (int i = 0; i < parameterName.Length; i++)
            {
                char ch = parameterName[i];
                if ((i < 1 && !Char.IsLetter(ch)) || (i > 0 && !Char.IsLetterOrDigit(ch)))
                    return false;
            }
            return true;
        }

        private static IEnumerable<PathSubsegment> ParseUrlSegment(string segment)
        {
			List<PathSubsegment> segments = new List<PathSubsegment>();

			State state = State.Literal;
			int startIndex = 0;
			string content = String.Empty;

			for (int i = 0; i < segment.Length; ++i)
			{
				char c = segment[i];
				switch (c)
				{
					case ParameterStart:
						switch (state)
						{
							case State.Literal:
							case State.AfterVariable:
								state = State.PossibleEscapedParameter;
								break;
							case State.PossibleEscapedParameter:
								state = State.EscapedParameter;
								break;
							case State.AfterParameter:
								throw Error.ConsecutiveRouteParameters();
							default:
								throw Error.UnexpectedSymbolInRoute(ParameterStart, segment);
						}
						break;
					case VariableStart:
						switch (state)
						{
							case State.Literal:
							case State.AfterParameter:
							case State.AfterVariable:
								state = State.PossibleEscapedVariable;
								break;
							case State.PossibleEscapedVariable:
								state = State.EscapedVariable;
								break;
							default:
								throw Error.UnexpectedSymbolInRoute(VariableStart, segment);
						}
						break;
					case ParameterEnd:
						switch (state)
						{
							case State.Parameter:
								content = segment.Substring(startIndex, i - startIndex);
								segments.Add(new ParameterSubsegment(content));
								startIndex = i + 1;
								state = State.AfterParameter;
								break;
							case State.EscapedParameter:
								state = State.AfterEscapedParameter;
								break;
							case State.AfterEscapedParameter:
								state = State.Literal;
								break;
							default:
								throw Error.UnexpectedSymbolInRoute(ParameterEnd, segment);
						}
						break;
					case VariableEnd:
						switch (state)
						{
							case State.Variable:
								content = segment.Substring(startIndex, i - startIndex);
								if (!IsValidParameterName(content))
									throw Error.InvalidRouteVariableName(content);

								segments.Add(new VariableSubsegment(content));
								startIndex = i + 1;
								state = State.AfterVariable;
								break;
							case State.EscapedVariable:
								state = State.AfterEscapedVariable;
								break;
							case State.AfterEscapedVariable:
								state = State.Literal;
								break;
							default:
								throw Error.UnexpectedSymbolInRoute(VariableEnd, segment);
						}
						break;
					default:
						switch (state)
						{
							case State.PossibleEscapedParameter:
								content = GetLiteral(segment.Substring(startIndex, i - startIndex - 1));
								if (content.Length > 0)
									segments.Add(new LiteralSubsegment(content));
								
								startIndex = i;
								state = State.Parameter;
								break;
							case State.PossibleEscapedVariable:
								content = GetLiteral(segment.Substring(startIndex, i - startIndex - 1));
								if (content.Length > 0)
									segments.Add(new LiteralSubsegment(content));

								startIndex = i;
								state = State.Variable;
								break;
							case State.AfterEscapedParameter:
							case State.AfterEscapedVariable:
								throw Error.IncompleteEscapeSequenceInRoute(segment);
							case State.AfterVariable:
							case State.AfterParameter:
								state = State.Literal;
								break;
						}
						break;
				}
			}
			switch (state)
			{
				case State.Literal:
				case State.AfterVariable:
				case State.AfterParameter:
					content = GetLiteral(segment.Substring(startIndex));
					if (content.Length > 0)
						segments.Add(new LiteralSubsegment(content));
					
					break;
				default:
					throw Error.IncompleteEscapeSequenceInRoute(segment);
			}
			return LiteralComposer.Compose(segments);
        }

        private static IList<PathSegment> SplitSegments(IEnumerable<string> parts)
        {
            List<PathSegment> list = new List<PathSegment>();
            foreach (string part in parts)
            {
                if (IsSeparator(part))
                    list.Add(new SeparatorSegment());
                else
                {
                    IEnumerable<PathSubsegment> subsegments = ParseUrlSegment(part);
                    list.Add(new ContentSegment(subsegments));
                }
            }
            return list;
        }

        internal static IEnumerable<string> SplitUrl(string url)
        {
            List<string> list = new List<string>();
            if (!String.IsNullOrEmpty(url))
            {
                int index;
                for (int i = 0; i < url.Length; i = index + 1)
                {
                    index = url.IndexOf(PathSeparator, i);
                    if (index == -1)
                    {
                        string str = url.Substring(i);
                        if (str.Length > 0)
                            list.Add(str);
                        
                        return list;
                    }
                    string item = url.Substring(i, index - i);
                    if (item.Length > 0)
                        list.Add(item);
                    
                    list.Add(Char.ToString(PathSeparator));
                }
            }
            return list;
        }

        private static void ValidateUrlParts(IEnumerable<string> segments)
        {
            HashSet<string> usedParameterNames = 
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            bool? hasSeparators = null;
            bool hasCatchAll = false;

            foreach (string part in segments)
            {
                bool isSeparator;

                if (hasCatchAll)
                    throw Error.CatchAllMustBeLast();
                
                if (!hasSeparators.HasValue)
                {
                    hasSeparators = IsSeparator(part);
                    isSeparator = hasSeparators.Value;
                }
                else
                {
                    isSeparator = IsSeparator(part);
                    if (isSeparator && hasSeparators.Value)
                        throw Error.ConsecutiveSeparators();
                    
                    hasSeparators = isSeparator;
                }

                if (!isSeparator)
                {
                    IEnumerable<PathSubsegment> subsegments = ParseUrlSegment(part);
                    ValidateUrlSegment(subsegments, usedParameterNames);
                    hasCatchAll = subsegments.Any(s => ((s is ParameterSubsegment) && ((ParameterSubsegment)s).IsCatchAll));
                }
            }
        }

        private static void ValidateUrlSegment(IEnumerable<PathSubsegment> segments, 
            HashSet<string> usedParameterNames)
        {
            bool hasCatchAll = false;
            foreach (PathSubsegment s in segments)
            {
                if (s is ParameterSubsegment)
                {
                    ParameterSubsegment parameter = (ParameterSubsegment)s;
                    if (s != null)
                    {
                        string parameterName = parameter.ParameterName;
						if (!IsValidParameterName(parameterName))
							throw Error.InvalidRouteParameterName(parameterName);

                        if (parameter.IsCatchAll)
                            hasCatchAll = true;

                        if (usedParameterNames.Contains(parameterName))
                            throw Error.DuplicateRouteParameterName(parameterName);
                        
                        usedParameterNames.Add(parameterName);
                    }
                }
            }
            
            if (hasCatchAll && segments.Count() != 1)
                throw Error.CatchAllInMultiSegment();
        }
        #endregion
    }
}