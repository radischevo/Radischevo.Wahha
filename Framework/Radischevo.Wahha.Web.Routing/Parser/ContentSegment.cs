using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    internal sealed class ContentSegment : PathSegment
    {
        #region Instance Fields
        private List<PathSubsegment> _segments;
        #endregion

        #region Constructors
        public ContentSegment(IEnumerable<PathSubsegment> segments)
        {
            Precondition.Require(segments, () => Error.ArgumentNull("segments"));
            _segments = new List<PathSubsegment>(segments);
        }
        #endregion

        #region Instance Properties
        public List<PathSubsegment> Segments
        {
            get
            {
                return _segments;
            }
        }

        public bool IsCatchAll
        {
            get
            {
                return _segments.Any(p => (p is ParameterSubsegment && 
                    ((ParameterSubsegment)p).IsCatchAll));
            }
        }
        #endregion

        #region Instance Methods
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (PathSubsegment segment in _segments)
                sb.Append(segment.ToString());

            return sb.ToString();
        }
        #endregion
    }
}
