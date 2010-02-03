using System;
using System.Collections.Generic;
using System.Text;

using Jeltofiol.Wahha.Core;
using Jeltofiol.Wahha.Data.Query.Expressions;

namespace Jeltofiol.Wahha.Data.Query
{
    public class SqlDbFormatter : DbFormatter
    {
        #region Nested Types
        public class Visitor : SqlVisitor
        {
            #region Instance Fields
            private bool _parenthesizeTop;
            private StringBuilder _builder;
            private List<SqlAlias> _suppressedAliases;
            private Dictionary<SqlExpression, string> _nameMap;
            private Dictionary<SqlExpression, SqlAlias> _aliasMap;
            #endregion

            #region Constructors
            public Visitor() 
                : base()
            {
                _suppressedAliases = new List<SqlAlias>();
                _nameMap = new Dictionary<SqlExpression, string>();
                _aliasMap = new Dictionary<SqlExpression, SqlAlias>();
                _builder = new StringBuilder();
            }
            #endregion

            #region Instance Methods
            public override string ToString()
            {
                return _builder.ToString();
            }

            protected string InferName(SqlExpression exp, string def)
            {
                if (exp == null)
                    return null;
                
                SqlExpressionType nodeType = exp.NodeType;
                switch (nodeType)
                {
                    case SqlExpressionType.Field:
                        return ((SqlField)exp).Name;
                    case SqlExpressionType.Alias:
                        return InferName(((SqlAlias)exp).Expression, def);
                    case SqlExpressionType.ExprSet:
                        return InferName(((SqlExpressionSet)exp).First(), def);
                }
                return def;
            }

            protected virtual void WriteName(string name)
            {
                _builder.Append("[");
                _builder.Append(name); // переписать этот вызов
                _builder.Append("]");
            }

            protected bool RequiresOnCondition(SqlJoinType joinType)
            {
                switch (joinType)
                {
                    case SqlJoinType.Cross:
                    case SqlJoinType.RightOuter:
                        return false;
                }
                return true;
            }

            protected void AddParentheses(SqlExpression node, SqlExpression outer)
            {
                switch (node.NodeType)
                {
                    case SqlExpressionType.Function:
                    case SqlExpressionType.TableValuedFunction:
                    case SqlExpressionType.Raw:
                    case SqlExpressionType.Parameter:
                    case SqlExpressionType.Constant:
                    case SqlExpressionType.Variable:
                    case SqlExpressionType.Field:
                        Visit(node);
                        break;
                    case SqlExpressionType.Binary:
                        SqlBinary binary = (SqlBinary)node;
                        switch (binary.Operation)
                        {
                            case SqlBinaryOperation.Add:
                            case SqlBinaryOperation.And:
                            case SqlBinaryOperation.BitAnd:
                            case SqlBinaryOperation.BitNot:
                            case SqlBinaryOperation.BitOr:
                            case SqlBinaryOperation.BitXor:
                            case SqlBinaryOperation.Multiply:
                            case SqlBinaryOperation.Or:
                                if (node.NodeType == outer.NodeType)
                                    Visit(node);
                                break;
                            default:
                                _builder.Append('(');
                                Visit(node);
                                _builder.Append(')');
                                break;
                        }
                        break;
                    default:
                        _builder.Append('(');
                        Visit(node);
                        _builder.Append(')');
                        break;
                }
            }

            public virtual string EscapeSingleQuotes(string value)
            {
                if (value.IndexOf('\'') < 0)
                    return value;
                
                StringBuilder builder = new StringBuilder();
                foreach (char ch in value)
                {
                    if (ch == '\'')
                        builder.Append("''");
                    else
                        builder.Append("'");
                }
                return builder.ToString();
            }

            public string Format(SqlExpression node)
            {
                _builder = new StringBuilder();
                _aliasMap.Clear();
                
                Visit(node);
                return _builder.ToString();
            }

            public virtual string GetBooleanValue(bool value)
            {
                if (value)
                    return "1";
                
                return "0";
            }

            public virtual void FormatValue(object value)
            {
                if (value == null)
                    _builder.Append("NULL");
                else
                {
                    Type type = value.GetType();
                    if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        type = type.GetGenericArguments()[0];
                    
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Boolean:
                            _builder.Append(GetBooleanValue((bool)value));
                            break;
                        case TypeCode.Char:
                        case TypeCode.DateTime:
                        case TypeCode.String:
                            _builder.Append("'");
                            _builder.Append(EscapeSingleQuotes(value.ToString()));
                            _builder.Append("'");
                            break;
                        case TypeCode.SByte:
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            _builder.Append(value);
                            break;
                        default:
                            if (type.IsArray)
                            {
                                Array arr = (Array)value;
                                _builder.Append('(');
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    if (i > 0)
                                        _builder.Append(", ");

                                    FormatValue(arr.GetValue(i));
                                }
                                _builder.Append(')');
                                break;
                            }
                            _builder.Append("'");
                            Type t = (value as Type);
                            _builder.Append((t == null) ? value : t.AssemblyQualifiedName);
                            _builder.Append("'");
                            break;
                    }
                }
            }

            public virtual string GetBinaryOperator(SqlBinaryOperation op)
            {
                switch (op)
                {
                    case SqlBinaryOperation.Add:
                        return "+";
                    case SqlBinaryOperation.And:
                        return "AND";
                    case SqlBinaryOperation.Assign:
                        return "=";
                    case SqlBinaryOperation.BitAnd:
                        return "&";
                    case SqlBinaryOperation.BitNot:
                        return "~";
                    case SqlBinaryOperation.BitOr:
                        return "|";
                    case SqlBinaryOperation.BitXor:
                        return "^";
                    case SqlBinaryOperation.Concat:
                        return "+";
                    case SqlBinaryOperation.Equal:
                        return "=";
                    case SqlBinaryOperation.Divide:
                        return "/";
                    case SqlBinaryOperation.GreaterOrEqual:
                        return ">=";
                    case SqlBinaryOperation.GreaterThan:
                        return ">";
                    case SqlBinaryOperation.In:
                        return "IN";
                    case SqlBinaryOperation.LessOrEqual:
                        return "<=";
                    case SqlBinaryOperation.LessThan:
                        return "<";
                    case SqlBinaryOperation.Like:
                        return "LIKE";
                    case SqlBinaryOperation.Modulo:
                        return "%";
                    case SqlBinaryOperation.Multiply:
                        return "*";
                    case SqlBinaryOperation.NotEqual:
                        return "<>";
                    case SqlBinaryOperation.Or:
                        return "OR";
                    case SqlBinaryOperation.Substract:
                        return "-";
                }
                return String.Empty;
            }

            public virtual string GetUnaryOperator(SqlUnaryOperation op)
            {
                switch (op)
                {
                    case SqlUnaryOperation.BitNot:
                        return "~";
                    case SqlUnaryOperation.IsNotNull:
                        return " IS NOT NULL ";
                    case SqlUnaryOperation.IsNull:
                        return " IS NULL ";
                    case SqlUnaryOperation.Negate:
                        return "-";
                    case SqlUnaryOperation.Not:
                        return " NOT ";
                }
                return String.Empty;
            }

            public virtual string GetAggregateOperator(SqlAggregateType aggr)
            {
                switch (aggr)
                {
                    case SqlAggregateType.Avg:
                        return "AVG";
                    case SqlAggregateType.Count:
                        return "COUNT";
                    case SqlAggregateType.Max:
                        return "MAX";
                    case SqlAggregateType.Min:
                        return "MIN";
                    case SqlAggregateType.StdDev:
                        return "STDDEV";
                    case SqlAggregateType.Sum:
                        return "SUM";
                }
                return String.Empty;
            }

            protected virtual bool IsSimpleCrossJoinList(SqlExpression node)
            {
                SqlJoin join = (node as SqlJoin);
                if (join != null)
                    return (join.JoinType == SqlJoinType.Cross && 
                        IsSimpleCrossJoinList(join.Left) && IsSimpleCrossJoinList(join.Right));
                
                SqlAlias alias = (node as SqlAlias);
                return (alias != null && alias.Expression is SqlTable);
            }

            public virtual void NewLine()
            {
                if (_builder.Length > 0)
                    _builder.AppendLine();
                
                for (int i = 0; i < _depth; i++)
                    _builder.Append("    ");
            }

            protected override SqlExpression VisitAlias(SqlAlias alias)
            {
                bool isSelect = (alias.Expression is SqlSelect);
                
                int depth = _depth;
                string name = String.Empty;
                string aliasName = null;

                SqlField field = (alias.Expression as SqlField);
                if (field != null)
                    name = field.Name;

                SqlTable table = (alias.Expression as SqlTable);
                if (table != null)
                    name = table.Name;
                
                if (alias.Name == null)
                {
                    if (!_nameMap.TryGetValue(alias, out aliasName))
                    {
                        aliasName = "A" + _nameMap.Count;
                        _nameMap[alias] = aliasName;
                    }
                }
                else
                    aliasName = alias.Name;
                
                if (isSelect)
                {
                    _depth++;
                    _builder.Append("(");
                    this.NewLine();
                }
                
                Visit(alias.Expression);

                if (isSelect)
                {
                    this.NewLine();
                    _builder.Append(")");
                    
                    _depth = depth;
                }
                if ((!_suppressedAliases.Contains(alias) && aliasName != null) && name != aliasName)
                {
                    _builder.Append(" AS ");
                    WriteName(aliasName);
                }

                return alias;
            }

            protected override SqlExpression VisitBetween(SqlBetween between)
            {
                AddParentheses(between.Expression, between);
                
                _builder.Append(" BETWEEN ");
                Visit(between.From);

                _builder.Append(" AND ");
                Visit(between.To);

                return between;
            }

            protected override SqlExpression VisitBinary(SqlBinary binary)
            {
                AddParentheses(binary.Left, binary);
                _builder.Append(" ");
                _builder.Append(GetBinaryOperator(binary.Operation));
                _builder.Append(" ");
                AddParentheses(binary.Right, binary);

                return binary;
            }

            protected override SqlExpression VisitWhen(SqlWhen expr)
            {
                _builder.Append("WHEN ");
                Visit(expr.Match);

                _builder.Append(" THEN ");
                Visit(expr.Value);
                    
                return expr;
            }

            protected override SqlExpression VisitVariable(SqlVariable expr)
            {
                _builder.Append(expr.Name);
                return expr;
            }

            protected override SqlExpression VisitUpdate(SqlUpdate expr)
            {
                return expr;
            }

            protected override SqlExpression VisitUnion(SqlUnion expr)
            {
                _builder.Append("(");
                _depth++;

                NewLine();
                Visit(expr.Left);
                NewLine();

                _builder.Append("UNION");
                if (expr.All)
                    _builder.Append(" ALL");
                
                NewLine();
                Visit(expr.Right);
                NewLine();

                _builder.Append(")");
                _depth--;

                return expr;
            }

            protected override SqlExpression VisitUnary(SqlUnary expr)
            {
                string op = GetUnaryOperator(expr.Operation);
                switch (expr.Operation)
                {
                    case SqlUnaryOperation.IsNull:
                    case SqlUnaryOperation.IsNotNull:
                        AddParentheses(expr.Operand, expr);
                        _builder.Append(op);
                        break;
                    default:
                        _builder.Append(op);
                        AddParentheses(expr.Operand, expr);
                        break;
                }
                return expr;
            }

            protected override SqlExpression VisitTableValuedFunction(
                SqlTableValuedFunctionCall expr)
            {
                return expr;
            }

            protected override SqlExpression VisitTable(SqlTable expr)
            {
                string tableAlias = null;
                if (!_nameMap.TryGetValue(expr, out tableAlias))
                {
                    tableAlias = "T" + _nameMap.Count;
                    _nameMap[expr] = tableAlias;
                }

                WriteName(expr.Name);

                if (!String.IsNullOrEmpty(tableAlias) && tableAlias != expr.Name)
                {
                    _builder.Append(" AS ");
                    WriteName(tableAlias);
                }
                return expr;
            }

            protected virtual void VisitCrossJoinList(SqlExpression node)
            {
                SqlJoin join = (node as SqlJoin);
                if (join != null)
                {
                    VisitCrossJoinList(join.Left);
                    _builder.Append(", ");
                    VisitCrossJoinList(join.Right);
                }
                else
                    Visit(node);
            }

            protected override SqlExpression VisitSelect(SqlSelect expr)
            {
                string fromExpression = null;

                if (expr.From != null)
                {
                    StringBuilder inner = _builder;
                    _builder = new StringBuilder();

                    if (IsSimpleCrossJoinList(expr.From))
                        VisitCrossJoinList(expr.From);
                    else
                        Visit(expr.From);
                    
                    fromExpression = _builder.ToString();
                    _builder = inner;
                }

                _builder.Append("SELECT ");

                if (expr.IsDistinct)
                    _builder.Append("DISTINCT ");
                
                if (expr.Top != null)
                {
                    _builder.Append("TOP ");
                    if (_parenthesizeTop)
                        _builder.Append("(");
                    
                    Visit(expr.Top);

                    if (_parenthesizeTop)
                        _builder.Append(")");
                    
                    _builder.Append(" ");
                    if (expr.IsPercent)
                        _builder.Append("PERCENT ");
                }

                if (expr.Selection.Expressions.Count > 0)
                    VisitSelection(expr.Selection);
                else
                    _builder.Append("* ");

                if (fromExpression != null)
                {
                    NewLine();
                    _builder.Append("FROM ");
                    _builder.Append(fromExpression);
                }

                if (expr.Where != null)
                {
                    NewLine();
                    _builder.Append("WHERE ");

                    Visit(expr.Where);
                }

                if (expr.GroupBy.Expressions.Count > 0)
                {
                    NewLine();
                    _builder.Append("GROUP BY ");

                    for(int i = 0; i < expr.GroupBy.Expressions.Count; ++i)
                    {
                        SqlExpression node = expr.GroupBy.Expressions[i];
                        if (i > 0)
                            _builder.Append(", ");
                        
                        Visit(node);
                    }

                    if (expr.Having != null)
                    {
                        NewLine();
                        _builder.Append("HAVING ");

                        Visit(expr.Having);
                    }
                }
                if (expr.OrderBy.Expressions.Count > 0)
                {
                    NewLine();
                    _builder.Append("ORDER BY ");

                    for (int i = 0; i < expr.OrderBy.Expressions.Count; ++i)
                    {
                        SqlOrderBy ob = expr.OrderBy.Expressions[i];
                        if (i > 0)
                            _builder.Append(", ");

                        Visit(ob.Expression);
                        if (ob.OrderType == SqlOrderType.Descending)
                            _builder.Append(" DESC");
                    }
                }
                return expr;
            }

            protected virtual SqlExpression VisitSelection(SqlExpressionSet row)
            {
                for(int i = 0; i < row.Expressions.Count; ++i)
                {
                    SqlExpression column = row.Expressions[i];
                    if (i > 0)
                        _builder.Append(", ");

                    VisitColumn(column);

                    string columnName = null;
                    if (column is SqlField)
                        columnName = ((SqlField)column).Name;
                    string inferredName = InferName(column, null);

                    if (columnName == null)
                        columnName = inferredName;

                    if (columnName == null && !_nameMap.TryGetValue(column, out columnName))
                    {
                        columnName = "C" + _nameMap.Count;
                        _nameMap[column] = columnName;
                    }

                    if (columnName != inferredName && !String.IsNullOrEmpty(columnName))
                    {
                        _builder.Append(" AS ");
                        WriteName(columnName);
                    }
                }
                return row;
            }

            protected virtual SqlExpression VisitColumn(SqlExpression column)
            {
                switch (column.NodeType)
                {
                    case SqlExpressionType.Case:
                    case SqlExpressionType.Exists:
                    case SqlExpressionType.Binary:
                    case SqlExpressionType.Function:
                    case SqlExpressionType.Select:
                        _builder.Append('(');
                        Visit(column);
                        _builder.Append(')');
                        break;
                    default:
                        Visit(column);
                        break;
                }
                return column;
            }

            protected override SqlExpression VisitRaw(SqlRawExpression expr)
            {
                _builder.Append(expr.Text);
                return expr;
            }

            protected override SqlExpression VisitParameter(SqlParameter expr)
            {
                _builder.Append(expr.Name);
                return expr;
            }

            protected override SqlExpression VisitLimit(SqlLimit expr)
            {
                
                return expr;
            }

            protected override SqlExpression VisitJoin(SqlJoin join)
            {
                Visit(join.Left);
                NewLine();
                
                switch (join.JoinType)
                {
                    case SqlJoinType.Cross:
                        _builder.Append("CROSS JOIN ");
                        break;
                    case SqlJoinType.Inner:
                        _builder.Append("INNER JOIN ");
                        break;
                    case SqlJoinType.LeftOuter:
                        _builder.Append("LEFT OUTER JOIN ");
                        break;
                    case SqlJoinType.RightOuter:
                        _builder.Append("RIGHT OUTER JOIN ");
                        break;
                }

                VisitJoinSource(join.Right);

                if (join.Condition != null)
                {
                    _builder.Append(" ON ");
                    Visit(join.Condition);

                    return join;
                }
                
                if (RequiresOnCondition(join.JoinType))
                    _builder.Append(" ON 1 = 1 ");

                return join;
            }

            protected virtual void VisitJoinSource(SqlSource src) // с этим нужно что-то делать в случае join-деревьев...
            {
                switch (src.NodeType)
                {
                    case SqlExpressionType.Raw:
                    case SqlExpressionType.Select:
                    case SqlExpressionType.TableValuedFunction:
                    case SqlExpressionType.Union:
                        _depth++;
                        _builder.Append("(");
                        Visit(src);
                        _builder.Append(")");
                        _depth--;
                        break;
                    default:
                        Visit(src);
                        break;
                }
            }

            protected override SqlExpression VisitInsert(SqlInsert expr)
            {
                return expr;
            }

            protected override SqlExpression VisitFunction(SqlFunctionCall expr)
            {
                if (expr.Name.Contains("."))
                    WriteName(expr.Name);
                else
                    _builder.Append(expr.Name);
                
                _builder.Append("(");

                for (int i = 0; i < expr.Arguments.Expressions.Count; ++i)
                {
                    if (i > 0)
                        _builder.Append(", ");

                    Visit(expr.Arguments.Expressions[i]);
                }
                _builder.Append(")");
                
                return expr;
            }

            protected override SqlExpression VisitField(SqlField expr)
            {
                // нужно вытащить имя источника поля
                string sourceName = null;
                if (!_nameMap.TryGetValue(expr.Source, out sourceName))
                {
                    sourceName = "T" + _nameMap.Count;
                    _nameMap[expr.Source] = sourceName;
                }

                if (!String.IsNullOrEmpty(sourceName))
                {
                    WriteName(sourceName);
                    _builder.Append('.');
                }
                WriteName(expr.Name);                    
                return expr;
            }

            protected override SqlExpression VisitExists(SqlExists expr)
            {
                _builder.Append("EXISTS(");
                _depth++;
                NewLine();

                Visit(expr.Select);

                NewLine();
                _depth--;
                _builder.Append(") ");

                return expr;
            }

            protected override SqlExpression VisitDelete(SqlDelete expr)
            {
                return expr;
            }

            protected override SqlExpression VisitConstant(SqlConstant expr)
            {
                FormatValue(expr.Value);
                return expr;
            }

            protected override SqlExpression VisitCase(SqlCase expr)
            {
                _depth++;               
                _builder.Append("CASE");
                _depth++;
                
                if (expr.Expression != null)
                {
                    _builder.Append(" ");
                    Visit(expr.Expression);
                }

                for(int i = 0; i < expr.Whens.Expressions.Count; ++i)
                {
                    NewLine();
                    VisitWhen(expr.Whens.Expressions[i]);
                }

                if (expr.Else != null)
                {
                    NewLine();
                    _builder.Append("ELSE ");
                    Visit(expr.Else);
                }

                _depth--;
                NewLine();

                _builder.Append(" END");
                _depth--;

                return expr;
            }

            protected override SqlExpression VisitAggregate(SqlAggregate expr)
            {
                _builder.Append(GetAggregateOperator(expr.Type));
                _builder.Append("(");

                if (expr.Expression == null)
                    _builder.Append("*");
                else
                    Visit(expr.Expression);

                _builder.Append(")");

                return expr;
            }
            #endregion
        }
        #endregion

        #region Instance Fields

        #endregion

        #region Constructors

        #endregion

        #region Instance Properties

        #endregion

        #region Instance Methods
        public override string Format(SqlExpression node)
        {
            Visitor v = new Visitor();
            v.Visit(node);

            return v.ToString();
        }
        #endregion
    }
}
