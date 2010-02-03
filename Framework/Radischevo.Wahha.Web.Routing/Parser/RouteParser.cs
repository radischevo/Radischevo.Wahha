using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    internal static class RouteParser
    {
        #region Constants
        public const char PathSeparator = '/';
        public const char ParameterStart = '{';
        public const char ParameterEnd = '}';
        private const string DOUBLE_START = "{{";
        private const string DOUBLE_END = "}}";
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

        private static string GetLiteral(string literal)
        {
            string str = literal.Replace(DOUBLE_START, "").Replace(DOUBLE_END, "");
            if (!str.Contains(ParameterStart) && !str.Contains(ParameterEnd))
                return literal.Replace(DOUBLE_START, ParameterStart.ToString())
                    .Replace(DOUBLE_END, ParameterEnd.ToString());
            
            return null;
        }

        private static int GetParameterStartIndex(string segment, int startIndex)
        {
            while (true)
            {
                startIndex = segment.IndexOf(ParameterStart, startIndex);
                if (startIndex == -1)
                    return -1;

                if (segment.Length == startIndex + 1 || 
                    (startIndex + 1 < segment.Length && segment[startIndex + 1] != ParameterStart))
                    return startIndex;
                
                startIndex += 2;
            }
        }

        private static int GetParameterEndIndex(string segment, int startIndex)
        {
            return segment.IndexOf(ParameterEnd, startIndex);
        }

        internal static bool IsSeparator(string str)
        {
            return String.Equals(str, Char.ToString(PathSeparator), StringComparison.Ordinal);
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
            int startIndex = 0;
            string part = null;
            List<PathSubsegment> list = new List<PathSubsegment>();

            while (startIndex < segment.Length)
            {
                int parameterIndex = GetParameterStartIndex(segment, startIndex); // открывающая скобочка
                if (parameterIndex == -1)
                {
                    part = GetLiteral(segment.Substring(startIndex));
                    if (part == null)
                        throw Error.MismatchedRouteParameter(segment);
                    
                    if (part.Length > 0)
                        list.Add(new LiteralSubsegment(part));
                    
                    break;
                }

                int index = GetParameterEndIndex(segment, parameterIndex + 1); // ищем закрывающую скобочку
                if (index == -1)
                    throw Error.MismatchedRouteParameter(segment);
                
                part = GetLiteral(segment.Substring(startIndex, parameterIndex - startIndex)); // что было до параметра...
                if (part == null)
                    throw Error.MismatchedRouteParameter(segment);

                if (part.Length > 0)
                    list.Add(new LiteralSubsegment(part));
                
                string parameterName = segment.Substring(parameterIndex + 1, index - parameterIndex - 1); // остальное - имя параметра
                list.Add(new ParameterSubsegment(parameterName));

                startIndex = index + 1;
            }
            return list;
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
            Type segmentType = null;

            foreach (PathSubsegment s in segments)
            {
                if (segmentType != null && segmentType == s.GetType())
                    throw Error.ConsecutiveRouteParameters();
                
                segmentType = s.GetType();

                if (s is ParameterSubsegment)
                {
                    ParameterSubsegment parameter = (ParameterSubsegment)s;
                    if (s != null)
                    {
                        string parameterName = parameter.ParameterName;
                        if (parameter.IsCatchAll)
                            hasCatchAll = true;

                        if (!IsValidParameterName(parameterName))
                            throw Error.InvalidRouteParameterName(parameterName);

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
