using System;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// A kind of SQL join
    /// </summary>
    public enum DbJoinType
    {
        CrossJoin,
        InnerJoin,
        CrossApply,
        OuterApply,
        LeftOuter,
        SingletonLeftOuter
    }
}
