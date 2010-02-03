using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    internal sealed class ParsedRoute
    {
        #region Instance Fields
        private bool _isRelative;
        private bool _isAppRelative;
        private List<PathSegment> _segments;
        #endregion

        #region Constructors
        public ParsedRoute(IEnumerable<PathSegment> segments, 
            bool isRelative, bool isAppRelative)
        {
            Precondition.Require(segments, Error.ArgumentNull("segments"));
            _segments = new List<PathSegment>(segments);
            _isRelative = isRelative;
            _isAppRelative = (isRelative & isAppRelative);
        }
        #endregion

        #region Static Methods
        private static bool RoutePartsEqual(object a, object b)
        {
            string first = (a as string);
            string second = (b as string);
            if (first != null && second != null)
                return String.Equals(first, second, StringComparison.OrdinalIgnoreCase);

            if (a != null)
                return a.Equals(b);

            return Object.Equals(a, b);
        }

        private static bool IsRoutePartNonEmpty(object routePart)
        {
            string str = (routePart as string);
            if (str != null)
                return (str.Length > 0);
            
            return (routePart != null);
        }

        private static bool IsParameterRequired(ParameterSubsegment segment, 
            ValueDictionary defaults, out object value)
        {
            value = null;

            if (segment.IsCatchAll)
                return false;

            return !(defaults.TryGetValue(segment.ParameterName, out value));
        }

        private static bool ForEachParameter(IEnumerable<PathSegment> segments, 
            Func<ParameterSubsegment, bool> action)
        {
            foreach(PathSegment segment in segments)
            {
                if (segment is SeparatorSegment)
                    continue;
                
                ContentSegment content = (segment as ContentSegment);
                if (content != null)
                {
                    foreach (PathSubsegment subsegment in content.Segments)
                    {
                        if (subsegment is LiteralSubsegment)
                            continue;

                        ParameterSubsegment arg = (subsegment as ParameterSubsegment);
                        if (arg != null && !action(arg))
                            return false;
                    }
                }
            }
            return true;
        }

        private static ParameterSubsegment GetParameterSubsegment(
            IEnumerable<PathSegment> segments, string parameterName)
        {
            ParameterSubsegment foundSubsegment = null;

            ForEachParameter(segments, s => {
                if (String.Equals(parameterName, s.ParameterName, StringComparison.OrdinalIgnoreCase))
                {
                    foundSubsegment = s;
                    return false;
                }
                return true;
            });
            return foundSubsegment;
        }
        #endregion

        #region Instance Properties
        public bool IsRelative
        {
            get
            {
                return _isRelative;
            }
        }

        public bool IsAppRelative
        {
            get
            {
                return _isAppRelative;
            }
        }
        #endregion

        #region Instance Methods
        public ValueDictionary Match(string virtualPath, ValueDictionary defaults)
        {
            List<string> parts = new List<string>(RouteParser.SplitUrl(virtualPath));
            if (defaults == null)
                defaults = new ValueDictionary();
            
            ValueDictionary values = new ValueDictionary();

            bool hasAdditionalParameters = false;
            bool isCatchAll = false;
            
            for (int i = 0; i < _segments.Count; i++)
            {
                SeparatorSegment separator = (_segments[i] as SeparatorSegment);
                ContentSegment content = (_segments[i] as ContentSegment);

                if (parts.Count <= i)
                    hasAdditionalParameters = true;
                
                string part = (hasAdditionalParameters) ? null : parts[i];
                if (separator != null)
                {
                    if (!hasAdditionalParameters && 
                        !RouteParser.IsSeparator(part))
                        return null;
                }
                
                if (content != null)
                {
                    if (content.IsCatchAll)
                    {
                        MatchCatchAll(content, parts.Skip(i), defaults, values);
                        isCatchAll = true;
                    }
                    else if (!MatchContent(content, part, defaults, values))
                        return null;
                }
            }

            if (!isCatchAll && _segments.Count < parts.Count)
            {
                for (int j = _segments.Count; j < parts.Count; j++)
                {
                    if (!RouteParser.IsSeparator(parts[j]))
                        return null;
                }
            }

            if (defaults != null)
            {
                foreach (KeyValuePair<string, object> kvp in defaults)
                {
                    if (!values.ContainsKey(kvp.Key))
                        values.Add(kvp.Key, kvp.Value);
                }
            }

            return values;
        }

        private void MatchCatchAll(ContentSegment segment, IEnumerable<string> remainingSegments, 
            ValueDictionary defaults, ValueDictionary matchedValues)
        {
            object value;
            string remainingPart = String.Join(String.Empty, remainingSegments.ToArray());
            ParameterSubsegment parameter = (segment.Segments.FirstOrDefault() as ParameterSubsegment);

            if (remainingPart.Length > 0)
                value = remainingPart;
            else
                defaults.TryGetValue(parameter.ParameterName, out value);
            
            matchedValues.Add(parameter.ParameterName, value);
        }

        private bool MatchContent(ContentSegment segment, string pathSegment, 
            ValueDictionary defaults, ValueDictionary matchedValues)
        {
            if (String.IsNullOrEmpty(pathSegment))
            {
                if (segment.Segments.Count > 0)
                {
                    object value;
                    ParameterSubsegment ps = (segment.Segments.FirstOrDefault(
                        s => (s is ParameterSubsegment)) as ParameterSubsegment);
                    // в оригинале была проверка на первое вхождение, что неправильно. 
                    // нам нужно любое вхождение параметра.
                    
                    if (ps == null)
                        return false;
                    
                    if (defaults.TryGetValue(ps.ParameterName, out value))
                    {
                        matchedValues.Add(ps.ParameterName, value);
                        return true;
                    }
                }
                return false;
            }

            int segmentLength = pathSegment.Length;
            int segmentIndex = (segment.Segments.Count - 1);

            ParameterSubsegment lastParameter = null;
            LiteralSubsegment lastLiteral = null;

            while (segmentIndex >= 0)
            {
                int index = segmentLength;
                ParameterSubsegment parameter = (segment.Segments[segmentIndex] as ParameterSubsegment);
                LiteralSubsegment literal = (segment.Segments[segmentIndex] as LiteralSubsegment);

                if (parameter != null)
                    lastParameter = parameter;

                if (literal != null)
                {
                    lastLiteral = literal;
                    int literalIndex = pathSegment.LastIndexOf(literal.Literal, 
                        segmentLength - 1, StringComparison.OrdinalIgnoreCase);

                    if (literalIndex == -1)
                        return false;
                    
                    if ((segmentIndex == segment.Segments.Count - 1) && 
                        ((literalIndex + literal.Literal.Length) != pathSegment.Length))
                        return false;
                    
                    index = literalIndex;
                }

                if (lastParameter != null && ((lastLiteral != null && 
                    parameter == null) || segmentIndex == 0))
                {
                    int startIndex;
                    int lastIndex;

                    if (lastLiteral == null)
                    {
                        startIndex = (segmentIndex == 0) ? 0 : index + lastLiteral.Literal.Length;
                        lastIndex = segmentLength;
                    }
                    else if (segmentIndex == 0 && parameter != null)
                    {
                        startIndex = 0;
                        lastIndex = segmentLength;
                    }
                    else
                    {
                        startIndex = index + lastLiteral.Literal.Length;
                        lastIndex = segmentLength - startIndex;
                    }

                    string part = pathSegment.Substring(startIndex, lastIndex);
                    
                    if (String.IsNullOrEmpty(part))
                        return false;
                    
                    matchedValues.Add(lastParameter.ParameterName, part);

                    lastParameter = null;
                    lastLiteral = null;
                }
                segmentLength = index;
                segmentIndex--;
            }

            if (segmentLength != 0)
                return (segment.Segments[0] is ParameterSubsegment);
            
            return true;
        }

        public BoundUrl Bind(ValueDictionary currentValues, 
            ValueDictionary values, ValueDictionary defaults)
        {
            if (currentValues == null)
                currentValues = new ValueDictionary();
            
            if (values == null)
                values = new ValueDictionary();
            
            if (defaults == null)
                defaults = new ValueDictionary();
            
            ValueDictionary acceptedValues = new ValueDictionary();
            HashSet<string> unusedValues = new HashSet<string>(
                values.Keys, StringComparer.OrdinalIgnoreCase);

            ForEachParameter(_segments, segment => {
                object value;
                object currentValue;

                string parameterName = segment.ParameterName;
                bool hasValue = values.TryGetValue(parameterName, out value);
                
                if (hasValue)
                    unusedValues.Remove(parameterName);
                
                bool hasCurrentValue = currentValues.TryGetValue(parameterName, out currentValue);
                
                if (hasValue && hasCurrentValue && !RoutePartsEqual(currentValue, value))
                    return false;

                if (hasValue)
                {
                    if (IsRoutePartNonEmpty(value))
                        acceptedValues.Add(parameterName, value);
                }
                else if (hasCurrentValue)
                    acceptedValues.Add(parameterName, currentValue);
                
                return true;
            });

            foreach (KeyValuePair<string, object> kvp in values)
            {
                if (IsRoutePartNonEmpty(kvp.Value) && !acceptedValues.ContainsKey(kvp.Key))
                    acceptedValues.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<string, object> kvp in currentValues)
            {
                if (!acceptedValues.ContainsKey(kvp.Key) && 
                    GetParameterSubsegment(_segments, kvp.Key) == null)
                    acceptedValues.Add(kvp.Key, kvp.Value);
            }

            ForEachParameter(_segments, segment => {
                object value;
                
                if (!acceptedValues.ContainsKey(segment.ParameterName) && 
                    !IsParameterRequired(segment, defaults, out value))
                    acceptedValues.Add(segment.ParameterName, value);
                
                return true;
            });

            if (!ForEachParameter(_segments, segment => {
                object value;
                if (IsParameterRequired(segment, defaults, out value) && 
                    !acceptedValues.ContainsKey(segment.ParameterName))
                    return false;
                
                return true;
            })) 
            {
                return null;
            }

            ValueDictionary others = new ValueDictionary(
                (IDictionary<string, object>)defaults);
            ForEachParameter(_segments, segment => {
                others.Remove(segment.ParameterName);
                return true;
            });

            foreach (KeyValuePair<string, object> kvp in others)
            {
                object value;
                if (values.TryGetValue(kvp.Key, out value))
                {
                    unusedValues.Remove(kvp.Key);
                    if (!RoutePartsEqual(value, kvp.Value))
                        return null;
                }
            }

            return BuildUrl(defaults, acceptedValues, unusedValues);
        }

        private BoundUrl BuildUrl(ValueDictionary defaults, 
            ValueDictionary acceptedValues, HashSet<string> unusedValues)
        {
            StringBuilder pathBuilder = new StringBuilder();
            StringBuilder segmentBuilder = new StringBuilder();
            bool flush = false;
            
            foreach (PathSegment segment in _segments)
            {
                SeparatorSegment separator = (segment as SeparatorSegment);
                ContentSegment content = (segment as ContentSegment);

                if (separator != null)
                {
                    if (flush && segmentBuilder.Length > 0)
                    {
                        pathBuilder.Append(segmentBuilder.ToString());
                        segmentBuilder.Length = 0;
                    }

                    flush = false;
                    segmentBuilder.Append(RouteParser.PathSeparator);
                }
                
                if (content != null)
                {
                    bool segmentEnd = false;

                    foreach (PathSubsegment subsegment in content.Segments)
                    {
                        LiteralSubsegment literal = (subsegment as LiteralSubsegment);
                        ParameterSubsegment parameter = (subsegment as ParameterSubsegment);

                        if (literal != null)
                        {
                            flush = true;
                            segmentBuilder.Append(Uri.EscapeUriString(literal.Literal));
                        }
                        
                        if(parameter != null)
                        {
                            object acceptedValue;
                            object defaultValue;

                            if (flush && segmentBuilder.Length > 0)
                            {
                                pathBuilder.Append(segmentBuilder.ToString());
                                segmentBuilder.Length = 0;

                                segmentEnd = true;
                            }

                            flush = false;

                            if (acceptedValues.TryGetValue(parameter.ParameterName, out acceptedValue))
                                unusedValues.Remove(parameter.ParameterName);
                            
                            defaults.TryGetValue(parameter.ParameterName, out defaultValue);
                            if (RoutePartsEqual(acceptedValue, defaultValue))
                            {
                                segmentBuilder.Append(Uri.EscapeUriString(
                                    Convert.ToString(acceptedValue, CultureInfo.InvariantCulture)));
                                continue;
                            }

                            if (segmentBuilder.Length > 0)
                            {
                                pathBuilder.Append(segmentBuilder.ToString());
                                segmentBuilder.Length = 0;
                            }
                            
                            pathBuilder.Append(Uri.EscapeUriString(Convert.ToString(acceptedValue, CultureInfo.InvariantCulture)));
                            segmentEnd = true;
                        }
                    }

                    if (segmentEnd && segmentBuilder.Length > 0)
                    {
                        pathBuilder.Append(segmentBuilder.ToString());
                        segmentBuilder.Length = 0;
                    }
                }
            }

            if (flush && segmentBuilder.Length > 0)
                pathBuilder.Append(segmentBuilder.ToString());
            
            if (unusedValues.Count > 0)
            {
                bool isFirst = true;
                foreach (string key in unusedValues)
                {
                    object value;
                    if (acceptedValues.TryGetValue(key, out value))
                    {
                        pathBuilder.Append(isFirst ? '?' : '&');
                        isFirst = false;

                        pathBuilder.Append(Uri.EscapeDataString(key.ToLowerInvariant()));
                        pathBuilder.Append('=');
                        pathBuilder.Append(Uri.EscapeDataString(
                            Convert.ToString(value, CultureInfo.InvariantCulture)));
                    }
                }
            }
            return new BoundUrl(pathBuilder.ToString(), acceptedValues);
        }
        #endregion
    }
}
