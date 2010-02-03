using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Determines if two expressions are equivalent. 
    /// Supports DbExpression nodes.
    /// </summary>
    public class DbExpressionComparer : ExpressionComparer
    {
        #region Instance Fields
        private ScopedDictionary<DbTableAlias, DbTableAlias> _aliasScope;
        #endregion

        #region Constructors
        protected DbExpressionComparer(ScopedDictionary<ParameterExpression,
            ParameterExpression> parameterScope,
            ScopedDictionary<DbTableAlias, DbTableAlias> aliasScope)
            : base(parameterScope)
        {
            _aliasScope = aliasScope;
        }
        #endregion

        #region Static Methods
        public new static bool AreEqual(Expression a, Expression b)
        {
            return AreEqual(null, null, a, b);
        }

        public static bool AreEqual(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, 
            ScopedDictionary<DbTableAlias, DbTableAlias> aliasScope, Expression a, Expression b)
        {
            return new DbExpressionComparer(parameterScope, aliasScope).Compare(a, b);
        }
        #endregion

        #region Instance Methods
        protected override bool Compare(Expression a, Expression b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.NodeType != b.NodeType)
                return false;
            if (a.Type != b.Type)
                return false;

            switch ((DbExpressionType)a.NodeType)
            {
                case DbExpressionType.Table:
                    return CompareTable((DbTableExpression)a, (DbTableExpression)b);
                case DbExpressionType.Column:
                    return CompareColumn((DbColumnExpression)a, (DbColumnExpression)b);
                case DbExpressionType.Select:
                    return CompareSelect((DbSelectExpression)a, (DbSelectExpression)b);
                case DbExpressionType.Join:
                    return CompareJoin((DbJoinExpression)a, (DbJoinExpression)b);
                case DbExpressionType.Aggregate:
                    return CompareAggregate((DbAggregateExpression)a, (DbAggregateExpression)b);
                case DbExpressionType.Scalar:
                case DbExpressionType.Exists:
                case DbExpressionType.In:
                    return CompareSubquery((DbSubqueryExpression)a, (DbSubqueryExpression)b);
                case DbExpressionType.AggregateSubquery:
                    return CompareAggregateSubquery((DbAggregateSubqueryExpression)a, (DbAggregateSubqueryExpression)b);
                case DbExpressionType.IsNull:
                    return CompareIsNull((DbIsNullExpression)a, (DbIsNullExpression)b);
                case DbExpressionType.Between:
                    return CompareBetween((DbBetweenExpression)a, (DbBetweenExpression)b);
                case DbExpressionType.RowCount:
                    return CompareRowNumber((DbRowNumberExpression)a, (DbRowNumberExpression)b);
                case DbExpressionType.Projection:
                    return CompareProjection((DbProjectionExpression)a, (DbProjectionExpression)b);
                case DbExpressionType.NamedValue:
                    return CompareNamedValue((DbNamedValueExpression)a, (DbNamedValueExpression)b);
                case DbExpressionType.Insert:
                    return CompareInsert((DbInsertExpression)a, (DbInsertExpression)b);
                case DbExpressionType.Update:
                    return CompareUpdate((DbUpdateExpression)a, (DbUpdateExpression)b);
                case DbExpressionType.Delete:
                    return CompareDelete((DbDeleteExpression)a, (DbDeleteExpression)b);
                case DbExpressionType.Batch:
                    return CompareBatch((BatchExpression)a, (BatchExpression)b);
                case DbExpressionType.Function:
                    return CompareFunction((DbFunctionExpression)a, (DbFunctionExpression)b);
                case DbExpressionType.Entity:
                    return CompareEntity((DbEntityExpression)a, (DbEntityExpression)b);
                case DbExpressionType.If:
                    return CompareIf((DbConditionalExpression)a, (DbConditionalExpression)b);
                case DbExpressionType.Block:
                    return CompareBlock((DbExpressionSet)a, (DbExpressionSet)b);
                default:
                    return base.Compare(a, b);
            }
        }

        protected virtual bool CompareTable(DbTableExpression a, DbTableExpression b)
        {
            return (a.Name == b.Name);
        }

        protected virtual bool CompareColumn(DbColumnExpression a, DbColumnExpression b)
        {
            return (CompareAlias(a.Alias, b.Alias) && a.Name == b.Name);
        }

        protected virtual bool CompareAlias(DbTableAlias a, DbTableAlias b)
        {
            if (_aliasScope != null)
            {
                DbTableAlias mapped;
                if (_aliasScope.TryGetValue(a, out mapped))
                    return (mapped == b);
            }
            return (a == b);
        }

        protected virtual bool CompareSelect(DbSelectExpression a, DbSelectExpression b)
        {
            ScopedDictionary<DbTableAlias, DbTableAlias> save = _aliasScope;
            try
            {
                if (!Compare(a.From, b.From))
                    return false;

                _aliasScope = new ScopedDictionary<DbTableAlias, DbTableAlias>(save);
                MapAliases(a.From, b.From);

                return Compare(a.Where, b.Where)
                    && CompareOrderList(a.OrderBy, b.OrderBy)
                    && CompareExpressionList(a.GroupBy, b.GroupBy)
                    && Compare(a.Skip, b.Skip)
                    && Compare(a.Take, b.Take)
                    && a.IsDistinct == b.IsDistinct
                    && CompareColumnDeclarations(a.Columns, b.Columns);
            }
            finally
            {
                _aliasScope = save;
            }
        }

        private void MapAliases(Expression a, Expression b)
        {
            DbTableAlias[] prodA = DeclaredAliasGatherer.Gather(a).ToArray();
            DbTableAlias[] prodB = DeclaredAliasGatherer.Gather(b).ToArray();

            for (int i = 0, n = prodA.Length; i < n; i++)
                _aliasScope.Add(prodA[i], prodB[i]);
        }

        protected virtual bool CompareOrderList(
            ReadOnlyCollection<DbOrderExpression> a, 
            ReadOnlyCollection<DbOrderExpression> b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.Count != b.Count)
                return false;
            for (int i = 0, n = a.Count; i < n; i++)
            {
                if (a[i].OrderType != b[i].OrderType ||
                    !Compare(a[i].Expression, b[i].Expression))
                    return false;
            }
            return true;
        }

        protected virtual bool CompareColumnDeclarations(
            ReadOnlyCollection<DbColumnDeclaration> a, 
            ReadOnlyCollection<DbColumnDeclaration> b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.Count != b.Count)
                return false;
            for (int i = 0, n = a.Count; i < n; i++)
            {
                if (!CompareColumnDeclaration(a[i], b[i]))
                    return false;
            }
            return true;
        }

        protected virtual bool CompareColumnDeclaration(
            DbColumnDeclaration a, DbColumnDeclaration b)
        {
            return (a.Name == b.Name && Compare(a.Expression, b.Expression));
        }

        protected virtual bool CompareJoin(DbJoinExpression a, DbJoinExpression b)
        {
            if (a.Join != b.Join || !Compare(a.Left, b.Left))
                return false;

            if (a.Join == DbJoinType.CrossApply || a.Join == DbJoinType.OuterApply)
            {
                ScopedDictionary<DbTableAlias, DbTableAlias> save = _aliasScope;
                try
                {
                    _aliasScope = new ScopedDictionary<DbTableAlias, DbTableAlias>(_aliasScope);
                    MapAliases(a.Left, b.Left);

                    return Compare(a.Right, b.Right)
                        && Compare(a.Condition, b.Condition);
                }
                finally
                {
                    _aliasScope = save;
                }
            }
            else
            {
                return Compare(a.Right, b.Right)
                    && Compare(a.Condition, b.Condition);
            }
        }

        protected virtual bool CompareAggregate(DbAggregateExpression a, DbAggregateExpression b)
        {
            return (a.AggregateType == b.AggregateType && Compare(a.Argument, b.Argument));
        }

        protected virtual bool CompareIsNull(DbIsNullExpression a, DbIsNullExpression b)
        {
            return Compare(a.Expression, b.Expression);
        }

        protected virtual bool CompareBetween(DbBetweenExpression a, DbBetweenExpression b)
        {
            return Compare(a.Expression, b.Expression)
                && Compare(a.Lower, b.Lower)
                && Compare(a.Upper, b.Upper);
        }

        protected virtual bool CompareRowNumber(DbRowNumberExpression a, DbRowNumberExpression b)
        {
            return CompareOrderList(a.OrderBy, b.OrderBy);
        }

        protected virtual bool CompareNamedValue(DbNamedValueExpression a, DbNamedValueExpression b)
        {
            return a.Name == b.Name && Compare(a.Value, b.Value);
        }

        protected virtual bool CompareSubquery(DbSubqueryExpression a, DbSubqueryExpression b)
        {
            if (a.NodeType != b.NodeType)
                return false;

            switch ((DbExpressionType)a.NodeType)
            {
                case DbExpressionType.Scalar:
                    return CompareScalar((DbScalarExpression)a, (DbScalarExpression)b);
                case DbExpressionType.Exists:
                    return CompareExists((DbExistsExpression)a, (DbExistsExpression)b);
                case DbExpressionType.In:
                    return CompareIn((DbInExpression)a, (DbInExpression)b);
            }
            return false;
        }

        protected virtual bool CompareScalar(DbScalarExpression a, DbScalarExpression b)
        {
            return Compare(a.Select, b.Select);
        }

        protected virtual bool CompareExists(DbExistsExpression a, DbExistsExpression b)
        {
            return Compare(a.Select, b.Select);
        }

        protected virtual bool CompareIn(DbInExpression a, DbInExpression b)
        {
            return Compare(a.Expression, b.Expression)
                && Compare(a.Select, b.Select)
                && CompareExpressionList(a.Values, b.Values);
        }

        protected virtual bool CompareAggregateSubquery(
            DbAggregateSubqueryExpression a, DbAggregateSubqueryExpression b)
        {
            return Compare(a.AggregateAsSubquery, b.AggregateAsSubquery)
                && Compare(a.AggregateInGroupSelect, b.AggregateInGroupSelect)
                && a.GroupByAlias == b.GroupByAlias;
        }

        protected virtual bool CompareProjection(DbProjectionExpression a, 
            DbProjectionExpression b)
        {
            if (!Compare(a.Select, b.Select))
                return false;

            ScopedDictionary<DbTableAlias, DbTableAlias> save = _aliasScope;
            try
            {
                _aliasScope = new ScopedDictionary<DbTableAlias, DbTableAlias>(_aliasScope);
                _aliasScope.Add(a.Select.Alias, b.Select.Alias);

                return Compare(a.Projector, b.Projector)
                    && Compare(a.Aggregator, b.Aggregator)
                    && a.IsSingleton == b.IsSingleton;
            }
            finally
            {
                _aliasScope = save;
            }
        }

        protected virtual bool CompareInsert(DbInsertExpression x, DbInsertExpression y)
        {
            return Compare(x.Table, y.Table)
                && CompareColumnAssignments(x.Assignments, y.Assignments);
        }

        protected virtual bool CompareColumnAssignments(ReadOnlyCollection<DbColumnAssignment> x, 
            ReadOnlyCollection<DbColumnAssignment> y)
        {
            if (x == y)
                return true;
            if (x.Count != y.Count)
                return false;

            for (int i = 0, n = x.Count; i < n; i++)
            {
                if (!Compare(x[i].Column, y[i].Column) || !Compare(x[i].Expression, y[i].Expression))
                    return false;
            }
            return true;
        }

        protected virtual bool CompareUpdate(DbUpdateExpression x, DbUpdateExpression y)
        {
            return Compare(x.Table, y.Table) && Compare(x.Where, y.Where) && 
                CompareColumnAssignments(x.Assignments, y.Assignments);
        }

        protected virtual bool CompareDelete(DbDeleteExpression x, DbDeleteExpression y)
        {
            return Compare(x.Table, y.Table) && Compare(x.Where, y.Where);
        }

        protected virtual bool CompareBatch(BatchExpression x, BatchExpression y)
        {
            return Compare(x.Input, y.Input) && Compare(x.Operation, y.Operation)
                && Compare(x.Size, y.Size) && Compare(x.Stream, y.Stream);
        }

        protected virtual bool CompareIf(DbConditionalExpression x, DbConditionalExpression y)
        {
            return Compare(x.Check, y.Check) && Compare(x.IfTrue, y.IfTrue) 
                && Compare(x.IfFalse, y.IfFalse);
        }

        protected virtual bool CompareBlock(DbExpressionSet x, DbExpressionSet y)
        {
            if (x.Expressions.Count != y.Expressions.Count)
                return false;

            for (int i = 0, n = x.Expressions.Count; i < n; i++)
            {
                if (!Compare(x.Expressions[i], y.Expressions[i]))
                    return false;
            }
            return true;
        }

        protected virtual bool CompareFunction(DbFunctionExpression x, DbFunctionExpression y)
        {
            return x.Name == y.Name && CompareExpressionList(x.Arguments, y.Arguments);
        }

        protected virtual bool CompareEntity(DbEntityExpression x, DbEntityExpression y)
        {
            return x.Entity == y.Entity && Compare(x.Expression, y.Expression);
        }
        #endregion
    }
}
