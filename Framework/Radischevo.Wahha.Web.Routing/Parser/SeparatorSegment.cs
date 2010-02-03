using System;

namespace Radischevo.Wahha.Web.Routing
{
    internal sealed class SeparatorSegment : PathSegment
    {
        #region Constructors
        public SeparatorSegment()
        {   }
        #endregion

        #region Instance Methods
        public override string ToString()
        {
            return new String(RouteParser.PathSeparator, 1);
        }
        #endregion
    }
}
