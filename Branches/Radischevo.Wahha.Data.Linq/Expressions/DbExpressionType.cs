using System;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Extended node types for custom expressions
    /// </summary>
    public enum DbExpressionType
    {
        Table = 1024,
        ClientJoin,
        Column,
        Select,
        Projection,
        Entity,
        Join,
        Aggregate,
        Scalar,
        Exists,
        In,
        Grouping,
        AggregateSubquery,
        IsNull,
        Between,
        RowCount,
        NamedValue,
        OuterJoined,
        Insert,
        Update,
        Delete,
        Batch,
        Function,
        Block,
        If,
        Declaration,
        Variable
    }
}
