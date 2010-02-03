// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Jeltofiol.Wahha.Data.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq
{
    /// <summary>
    /// Formats a query expression into common SQL language syntax
    /// </summary>
    public class SqlFormatter : DbExpressionVisitor
    {
        QueryLanguage language;
        StringBuilder sb;
        int indent = 2;
        int depth;
        Dictionary<DbTableAlias, string> aliases;
        bool hideColumnAliases;
        bool hideTableAliases;
        bool isNested;

        protected SqlFormatter(QueryLanguage language)
        {
            this.language = language;
            this.sb = new StringBuilder();
            this.aliases = new Dictionary<DbTableAlias, string>();
        }

        public static string Format(Expression expression)
        {
            var formatter = new SqlFormatter(null);
            formatter.Visit(expression);
            return formatter.ToString();
        }

        public override string ToString()
        {
            return this.sb.ToString();
        }

        protected virtual QueryLanguage Language
        {
            get { return this.language; }
        }

        protected bool HideColumnAliases
        {
            get { return this.hideColumnAliases; }
            set { this.hideColumnAliases = value; }
        }

        protected bool HideTableAliases
        {
            get { return this.hideTableAliases; }
            set { this.hideTableAliases = value; }
        }

        protected bool IsNested
        {
            get { return this.isNested; }
            set { this.isNested = value; }
        }

        protected enum Indentation
        {
            Same,
            Inner,
            Outer
        }

        public int IndentationWidth
        {
            get { return this.indent; }
            set { this.indent = value; }
        }

        protected void Write(object value)
        {
            this.sb.Append(value);
        }

        protected virtual void WriteParameterName(string name)
        {
            this.Write("@" + name);
        }

        protected virtual void WriteVariableName(string name)
        {
            this.WriteParameterName(name);
        }

        protected virtual void WriteAliasName(string aliasName)
        {
            this.Write(aliasName);
        }

        protected virtual void WriteColumnName(string columnName)
        {
            string name = (this.Language != null) ? this.Language.Quote(columnName) : columnName;
            this.Write(name);
        }

        protected virtual void WriteTableName(string tableName)
        {
            string name = (this.Language != null) ? this.Language.Quote(tableName) : tableName;
            this.Write(name);
        }

        protected void WriteLine(Indentation style)
        {
            sb.AppendLine();
            this.Indent(style);
            for (int i = 0, n = this.depth * this.indent; i < n; i++)
            {
                this.Write(" ");
            }
        }

        protected void Indent(Indentation style)
        {
            if (style == Indentation.Inner)
            {
                this.depth++;
            }
            else if (style == Indentation.Outer)
            {
                this.depth--;
                System.Diagnostics.Debug.Assert(this.depth >= 0);
            }
        }

        protected virtual string GetAliasName(DbTableAlias alias)
        {
            string name;
            if (!this.aliases.TryGetValue(alias, out name))
            {
                name = "t" + this.aliases.Count;
                this.aliases.Add(alias, name);
            }
            return name;
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == null) return null;

            // check for supported node types first 
            // non-supported ones should not be visited (as they would produce bad SQL)
            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.UnaryPlus:
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.Power:
                case ExpressionType.Conditional:
                case ExpressionType.Constant:
                case ExpressionType.MemberAccess:
                case ExpressionType.Call:
                case ExpressionType.New:
                case (ExpressionType)DbExpressionType.Table:
                case (ExpressionType)DbExpressionType.Column:
                case (ExpressionType)DbExpressionType.Select:
                case (ExpressionType)DbExpressionType.Join:
                case (ExpressionType)DbExpressionType.Aggregate:
                case (ExpressionType)DbExpressionType.Scalar:
                case (ExpressionType)DbExpressionType.Exists:
                case (ExpressionType)DbExpressionType.In:
                case (ExpressionType)DbExpressionType.AggregateSubquery:
                case (ExpressionType)DbExpressionType.IsNull:
                case (ExpressionType)DbExpressionType.Between:
                case (ExpressionType)DbExpressionType.RowCount:
                case (ExpressionType)DbExpressionType.Projection:
                case (ExpressionType)DbExpressionType.NamedValue:
                case (ExpressionType)DbExpressionType.Insert:
                case (ExpressionType)DbExpressionType.Update:
                case (ExpressionType)DbExpressionType.Delete:
                case (ExpressionType)DbExpressionType.Block:
                case (ExpressionType)DbExpressionType.If:
                case (ExpressionType)DbExpressionType.Declaration:
                case (ExpressionType)DbExpressionType.Variable:
                case (ExpressionType)DbExpressionType.Function:
                    return base.Visit(exp);

                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.ArrayIndex:
                case ExpressionType.TypeIs:
                case ExpressionType.Parameter:
                case ExpressionType.Lambda:
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                case ExpressionType.Invoke:
                case ExpressionType.MemberInit:
                case ExpressionType.ListInit:
                default:
                    throw new NotSupportedException(string.Format("The LINQ expression node of type {0} is not supported", exp.NodeType));
            }
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            throw new NotSupportedException(string.Format("The member access '{0}' is not supported", m.Member));
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Decimal))
            {
                switch (m.Method.Name)
                {
                    case "Add":
                    case "Subtract":
                    case "Multiply":
                    case "Divide":
                    case "Remainder":
                        this.Write("(");
                        this.VisitValue(m.Arguments[0]);
                        this.Write(" ");
                        this.Write(GetOperator(m.Method.Name));
                        this.Write(" ");
                        this.VisitValue(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Negate":
                        this.Write("-");
                        this.Visit(m.Arguments[0]);
                        this.Write("");
                        return m;
                }
            }
            else if (m.Method.Name == "ToString" && m.Object.Type == typeof(string))
            {
                return this.Visit(m.Object);  // no op
            }
            else if (m.Method.Name == "Equals")
            {
                if (m.Method.IsStatic && m.Method.DeclaringType == typeof(object))
                {
                    this.Write("(");
                    this.Visit(m.Arguments[0]);
                    this.Write(" = ");
                    this.Visit(m.Arguments[1]);
                    this.Write(")");
                    return m;
                }
                else if (!m.Method.IsStatic && m.Arguments.Count == 1 && m.Arguments[0].Type == m.Object.Type)
                {
                    this.Write("(");
                    this.Visit(m.Object);
                    this.Write(" = ");
                    this.Visit(m.Arguments[0]);
                    this.Write(")");
                    return m;
                }
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override NewExpression VisitNew(NewExpression nex)
        {
            throw new NotSupportedException(string.Format("The construtor for '{0}' is not supported", nex.Constructor.DeclaringType));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            string op = this.GetOperator(u);
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    if (IsBoolean(u.Operand.Type) || op.Length > 1)
                    {
                        this.Write(op);
                        this.Write(" ");
                        this.VisitPredicate(u.Operand);
                    }
                    else
                    {
                        this.Write(op);
                        this.VisitValue(u.Operand);
                    }
                    break;
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    this.Write(op);
                    this.VisitValue(u.Operand);
                    break;
                case ExpressionType.UnaryPlus:
                    this.VisitValue(u.Operand);
                    break;
                case ExpressionType.Convert:
                    // ignore conversions for now
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            string op = this.GetOperator(b);
            Expression left = b.Left;
            Expression right = b.Right;

            this.Write("(");
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    if (this.IsBoolean(left.Type))
                    {
                        this.VisitPredicate(left);
                        this.Write(" ");
                        this.Write(op);
                        this.Write(" ");
                        this.VisitPredicate(right);
                    }
                    else
                    {
                        this.VisitValue(left);
                        this.Write(" ");
                        this.Write(op);
                        this.Write(" ");
                        this.VisitValue(right);
                    }
                    break;
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    if (right.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression ce = (ConstantExpression)right;
                        if (ce.Value == null)
                        {
                            this.Visit(left);
                            this.Write(" IS NULL");
                            break;
                        }
                    }
                    else if (left.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression ce = (ConstantExpression)left;
                        if (ce.Value == null)
                        {
                            this.Visit(right);
                            this.Write(" IS NULL");
                            break;
                        }
                    }
                    goto case ExpressionType.LessThan;
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    // check for special x.CompareTo(y) && type.Compare(x,y)
                    if (left.NodeType == ExpressionType.Call && right.NodeType == ExpressionType.Constant)
                    {
                        MethodCallExpression mc = (MethodCallExpression)left;
                        ConstantExpression ce = (ConstantExpression)right;
                        if (ce.Value != null && ce.Value.GetType() == typeof(int) && ((int)ce.Value) == 0)
                        {
                            if (mc.Method.Name == "CompareTo" && !mc.Method.IsStatic && mc.Arguments.Count == 1)
                            {
                                left = mc.Object;
                                right = mc.Arguments[0];
                            }
                            else if (
                                (mc.Method.DeclaringType == typeof(string) || mc.Method.DeclaringType == typeof(decimal))
                                  && mc.Method.Name == "Compare" && mc.Method.IsStatic && mc.Arguments.Count == 2)
                            {
                                left = mc.Arguments[0];
                                right = mc.Arguments[1];
                            }
                        }
                    }
                    goto case ExpressionType.Add;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.ExclusiveOr:
                    this.VisitValue(left);
                    this.Write(" ");
                    this.Write(op);
                    this.Write(" ");
                    this.VisitValue(right);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
            this.Write(")");
            return b;
        }

        protected virtual string GetOperator(string methodName)
        {
            switch (methodName)
            {
                case "Add": return "+";
                case "Subtract": return "-";
                case "Multiply": return "*";
                case "Divide": return "/";
                case "Negate": return "-";
                case "Remainder": return "%";
                default: return null;
            }
        }

        protected virtual string GetOperator(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return "-";
                case ExpressionType.UnaryPlus:
                    return "+";
                case ExpressionType.Not:
                    return IsBoolean(u.Operand.Type) ? "NOT" : "~";
                default:
                    return "";
            }
        }

        protected virtual string GetOperator(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return (IsBoolean(b.Left.Type)) ? "AND" : "&";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return (IsBoolean(b.Left.Type) ? "OR" : "|");
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.ExclusiveOr:
                    return "^";
                default:
                    return "";
            }
        }

        protected virtual bool IsBoolean(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        protected virtual bool IsPredicate(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return IsBoolean(((BinaryExpression)expr).Type);
                case ExpressionType.Not:
                    return IsBoolean(((UnaryExpression)expr).Type);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case (ExpressionType)DbExpressionType.IsNull:
                case (ExpressionType)DbExpressionType.Between:
                case (ExpressionType)DbExpressionType.Exists:
                case (ExpressionType)DbExpressionType.In:
                    return true;
                case ExpressionType.Call:
                    return IsBoolean(((MethodCallExpression)expr).Type);
                default:
                    return false;
            }
        }

        protected virtual Expression VisitPredicate(Expression expr)
        {
            this.Visit(expr);
            if (!IsPredicate(expr))
            {
                this.Write(" <> 0");
            }
            return expr;
        }

        protected virtual Expression VisitValue(Expression expr)
        {
            return this.Visit(expr);
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            throw new NotSupportedException(string.Format("Conditional expressions not supported"));
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            this.WriteValue(c.Value);
            return c;
        }

        protected virtual void WriteValue(object value)
        {
            if (value == null)
            {
                this.Write("NULL");
            }
            else if (value.GetType().IsEnum)
            {
                this.Write(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));
            }
            else
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Boolean:
                        this.Write(((bool)value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        this.Write("'");
                        this.Write(value);
                        this.Write("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
                    default:
                        this.Write(value);
                        break;
                }
            }
        }

        protected override Expression VisitColumn(DbColumnExpression column)
        {
            if (column.Alias != null && !this.HideColumnAliases)
            {
                this.WriteAliasName(GetAliasName(column.Alias));
                this.Write(".");
            }
            this.WriteColumnName(column.Name);
            return column;
        }

        protected override Expression VisitProjection(DbProjectionExpression proj)
        {
            // treat these like scalar subqueries
            if (proj.Projector is DbColumnExpression)
            {
                this.Write("(");
                this.WriteLine(Indentation.Inner);
                this.Visit(proj.Select);
                this.Write(")");
                this.Indent(Indentation.Outer);
            }
            else
            {
                throw new NotSupportedException("Non-scalar projections cannot be translated to SQL.");
            }
            return proj;
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            this.Write("SELECT ");
            if (select.IsDistinct)
            {
                this.Write("DISTINCT ");
            }
            if (select.Take != null)
            {
                this.WriteTopClause(select.Take);
            }
            this.WriteColumns(select.Columns);
            if (select.From != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("FROM ");
                this.VisitSource(select.From);
            }
            if (select.Where != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("WHERE ");
                this.VisitPredicate(select.Where);
            }
            if (select.GroupBy != null && select.GroupBy.Count > 0)
            {
                this.WriteLine(Indentation.Same);
                this.Write("GROUP BY ");
                for (int i = 0, n = select.GroupBy.Count; i < n; i++)
                {
                    if (i > 0)
                    {
                        this.Write(", ");
                    }
                    this.VisitValue(select.GroupBy[i]);
                }
            }
            if (select.OrderBy != null && select.OrderBy.Count > 0)
            {
                this.WriteLine(Indentation.Same);
                this.Write("ORDER BY ");
                for (int i = 0, n = select.OrderBy.Count; i < n; i++)
                {
                    DbOrderExpression exp = select.OrderBy[i];
                    if (i > 0)
                    {
                        this.Write(", ");
                    }
                    this.VisitValue(exp.Expression);
                    if (exp.OrderType != DbOrderType.Ascending)
                    {
                        this.Write(" DESC");
                    }
                }
            }
            return select;
        }

        protected virtual void WriteTopClause(Expression expression)
        {
            this.Write("TOP (");
            this.Visit(expression);
            this.Write(") ");
        }

        protected virtual void WriteColumns(ReadOnlyCollection<DbColumnDeclaration> columns)
        {
            if (columns.Count > 0)
            {
                for (int i = 0, n = columns.Count; i < n; i++)
                {
                    DbColumnDeclaration column = columns[i];
                    if (i > 0)
                    {
                        this.Write(", ");
                    }
                    DbColumnExpression c = this.VisitValue(column.Expression) as DbColumnExpression;
                    if (!string.IsNullOrEmpty(column.Name) && (c == null || c.Name != column.Name))
                    {
                        this.Write(" AS ");
                        this.WriteColumnName(column.Name);
                    }
                }
            }
            else
            {
                this.Write("NULL ");
                if (this.isNested)
                {
                    this.Write("AS ");
                    this.WriteColumnName("tmp");
                    this.Write(" ");
                }
            }
        }

        protected override Expression VisitSource(Expression source)
        {
            bool saveIsNested = this.isNested;
            this.isNested = true;
            switch ((DbExpressionType)source.NodeType)
            {
                case DbExpressionType.Table:
                    DbTableExpression table = (DbTableExpression)source;
                    this.WriteTableName(table.Name);
                    if (!this.HideTableAliases)
                    {
                        this.Write(" AS ");
                        this.WriteAliasName(GetAliasName(table.Alias));
                    }
                    break;
                case DbExpressionType.Select:
                    DbSelectExpression select = (DbSelectExpression)source;
                    this.Write("(");
                    this.WriteLine(Indentation.Inner);
                    this.Visit(select);
                    this.WriteLine(Indentation.Same);
                    this.Write(")");
                    this.Write(" AS ");
                    this.WriteAliasName(GetAliasName(select.Alias));
                    this.Indent(Indentation.Outer);
                    break;
                case DbExpressionType.Join:
                    this.VisitJoin((DbJoinExpression)source);
                    break;
                default:
                    throw new InvalidOperationException("Select source is not valid type");
            }
            this.isNested = saveIsNested;
            return source;
        }

        protected override Expression VisitJoin(DbJoinExpression join)
        {
            this.VisitJoinLeft(join.Left);
            this.WriteLine(Indentation.Same);
            switch (join.Join)
            {
                case DbJoinType.CrossJoin:
                    this.Write("CROSS JOIN ");
                    break;
                case DbJoinType.InnerJoin:
                    this.Write("INNER JOIN ");
                    break;
                case DbJoinType.CrossApply:
                    this.Write("CROSS APPLY ");
                    break;
                case DbJoinType.OuterApply:
                    this.Write("OUTER APPLY ");
                    break;
                case DbJoinType.LeftOuter:
                case DbJoinType.SingletonLeftOuter:
                    this.Write("LEFT OUTER JOIN ");
                    break;
            }
            this.VisitJoinRight(join.Right);
            if (join.Condition != null)
            {
                this.WriteLine(Indentation.Inner);
                this.Write("ON ");
                this.VisitPredicate(join.Condition);
                this.Indent(Indentation.Outer);
            }
            return join;
        }

        protected virtual Expression VisitJoinLeft(Expression source)
        {
            return this.VisitSource(source);
        }

        protected virtual Expression VisitJoinRight(Expression source)
        {
            return this.VisitSource(source);
        }

        protected virtual string GetAggregateName(DbAggregateType aggregateType)
        {
            switch (aggregateType)
            {
                case DbAggregateType.Count: return "COUNT";
                case DbAggregateType.Min: return "MIN";
                case DbAggregateType.Max: return "MAX";
                case DbAggregateType.Sum: return "SUM";
                case DbAggregateType.Average: return "AVG";
                default: throw new Exception(string.Format("Unknown aggregate type: {0}", aggregateType));
            }
        }

        protected virtual bool RequiresAsteriskWhenNoArgument(DbAggregateType aggregateType)
        {
            return aggregateType == DbAggregateType.Count;
        }

        protected override Expression VisitAggregate(DbAggregateExpression aggregate)
        {
            this.Write(GetAggregateName(aggregate.AggregateType));
            this.Write("(");
            if (aggregate.IsDistinct)
            {
                this.Write("DISTINCT ");
            }
            if (aggregate.Argument != null)
            {
                this.VisitValue(aggregate.Argument);
            }
            else if (RequiresAsteriskWhenNoArgument(aggregate.AggregateType))
            {
                this.Write("*");
            }
            this.Write(")");
            return aggregate;
        }

        protected override Expression VisitIsNull(DbIsNullExpression isnull)
        {
            this.VisitValue(isnull.Expression);
            this.Write(" IS NULL");
            return isnull;
        }

        protected override Expression VisitBetween(DbBetweenExpression between)
        {
            this.VisitValue(between.Expression);
            this.Write(" BETWEEN ");
            this.VisitValue(between.Lower);
            this.Write(" AND ");
            this.VisitValue(between.Upper);
            return between;
        }

        protected override Expression VisitRowNumber(DbRowNumberExpression rowNumber)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitScalar(DbScalarExpression subquery)
        {
            this.Write("(");
            this.WriteLine(Indentation.Inner);
            this.Visit(subquery.Select);
            this.WriteLine(Indentation.Same);
            this.Write(")");
            this.Indent(Indentation.Outer);
            return subquery;
        }

        protected override Expression VisitExists(DbExistsExpression exists)
        {
            this.Write("EXISTS(");
            this.WriteLine(Indentation.Inner);
            this.Visit(exists.Select);
            this.WriteLine(Indentation.Same);
            this.Write(")");
            this.Indent(Indentation.Outer);
            return exists;
        }

        protected override Expression VisitIn(DbInExpression @in)
        {
            this.VisitValue(@in.Expression);
            this.Write(" IN (");
            if (@in.Select != null)
            {
                this.WriteLine(Indentation.Inner);
                this.Visit(@in.Select);
                this.WriteLine(Indentation.Same);
                this.Write(")");
                this.Indent(Indentation.Outer);
            }
            else if (@in.Values != null)
            {
                for (int i = 0, n = @in.Values.Count; i < n; i++)
                {
                    if (i > 0) this.Write(", ");
                    this.VisitValue(@in.Values[i]);
                }
                this.Write(")");
            }
            return @in;
        }

        protected override Expression VisitNamedValue(DbNamedValueExpression value)
        {
            this.WriteParameterName(value.Name);
            return value;
        }

        protected override Expression VisitInsert(DbInsertExpression insert)
        {
            this.Write("INSERT INTO ");
            this.WriteTableName(insert.Table.Name);
            this.Write("(");
            for (int i = 0, n = insert.Assignments.Count; i < n; i++)
            {
                DbColumnAssignment ca = insert.Assignments[i];
                if (i > 0) this.Write(", ");
                this.Write(ca.Column.Name);
            }
            this.Write(")");
            this.WriteLine(Indentation.Same);
            this.Write("VALUES (");
            for (int i = 0, n = insert.Assignments.Count; i < n; i++)
            {
                DbColumnAssignment ca = insert.Assignments[i];
                if (i > 0) this.Write(", ");
                this.Visit(ca.Expression);
            }
            this.Write(")");
            return insert;
        }

        protected override Expression VisitUpdate(DbUpdateExpression update)
        {
            this.Write("UPDATE ");
            this.WriteTableName(update.Table.Name);
            this.WriteLine(Indentation.Same);
            bool saveHide = this.HideColumnAliases;
            this.HideColumnAliases = true;
            this.Write("SET ");
            for (int i = 0, n = update.Assignments.Count; i < n; i++)
            {
                DbColumnAssignment ca = update.Assignments[i];
                if (i > 0) this.Write(", ");
                this.Visit(ca.Column);
                this.Write(" = ");
                this.Visit(ca.Expression);
            }
            if (update.Where != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("WHERE ");
                this.Visit(update.Where);
            }
            this.HideColumnAliases = saveHide;
            return update;
        }

        protected override Expression VisitDelete(DbDeleteExpression delete)
        {
            this.Write("DELETE FROM ");
            bool saveHideTable = this.HideTableAliases;
            bool saveHideColumn = this.HideColumnAliases;
            this.HideTableAliases = true;
            this.HideColumnAliases = true;
            this.VisitSource(delete.Table);
            if (delete.Where != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("WHERE ");
                this.Visit(delete.Where);
            }
            this.HideTableAliases = saveHideTable;
            this.HideColumnAliases = saveHideColumn;
            return delete;
        }

        protected override Expression VisitIf(DbConditionalExpression ifx)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitBlock(DbExpressionSet block)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitDeclaration(DbDeclaration decl)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitVariable(DbVariableExpression vex)
        {
            this.WriteVariableName(vex.Name);
            return vex;
        }

        protected virtual void VisitStatement(Expression expression)
        {
            var p = expression as DbProjectionExpression;
            if (p != null)
            {
                this.Visit(p.Select);
            }
            else
            {
                this.Visit(expression);
            }
        }

        protected override Expression VisitFunction(DbFunctionExpression func)
        {
            this.Write(func.Name);
            if (func.Arguments.Count > 0)
            {
                this.Write("(");
                for (int i = 0, n = func.Arguments.Count; i < n; i++)
                {
                    if (i > 0) this.Write(", ");
                    this.Visit(func.Arguments[i]);
                }
                this.Write(")");
            }
            return func;
        }
    }
}
