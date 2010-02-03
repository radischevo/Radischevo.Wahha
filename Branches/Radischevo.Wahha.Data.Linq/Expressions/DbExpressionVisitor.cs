using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// An extended expression visitor including custom DbExpression nodes
    /// </summary>
    public abstract class DbExpressionVisitor : ExpressionVisitor
    {
        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;
            
            switch ((DbExpressionType)exp.NodeType)
            {
                case DbExpressionType.Table:
                    return VisitTable((DbTableExpression)exp);
                case DbExpressionType.Column:
                    return VisitColumn((DbColumnExpression)exp);
                case DbExpressionType.Select:
                    return VisitSelect((DbSelectExpression)exp);
                case DbExpressionType.Join:
                    return VisitJoin((DbJoinExpression)exp);
                case DbExpressionType.OuterJoined:
                    return VisitOuterJoined((DbOuterJoinedExpression)exp);
                case DbExpressionType.Aggregate:
                    return VisitAggregate((DbAggregateExpression)exp);
                case DbExpressionType.Scalar:
                case DbExpressionType.Exists:
                case DbExpressionType.In:
                    return VisitSubquery((DbSubqueryExpression)exp);
                case DbExpressionType.AggregateSubquery:
                    return VisitAggregateSubquery((DbAggregateSubqueryExpression)exp);
                case DbExpressionType.IsNull:
                    return VisitIsNull((DbIsNullExpression)exp);
                case DbExpressionType.Between:
                    return VisitBetween((DbBetweenExpression)exp);
                case DbExpressionType.RowCount:
                    return VisitRowNumber((DbRowNumberExpression)exp);
                case DbExpressionType.Projection:
                    return VisitProjection((DbProjectionExpression)exp);
                case DbExpressionType.NamedValue:
                    return VisitNamedValue((DbNamedValueExpression)exp);
                case DbExpressionType.ClientJoin:
                    return VisitClientJoin((DbClientJoinExpression)exp);
                case DbExpressionType.Insert:
                case DbExpressionType.Update:
                case DbExpressionType.Delete:
                case DbExpressionType.If:
                case DbExpressionType.Block:
                case DbExpressionType.Declaration:
                    return VisitStatement((DbStatementExpression)exp);
                case DbExpressionType.Batch:
                    return VisitBatch((BatchExpression)exp);
                case DbExpressionType.Variable:
                    return VisitVariable((DbVariableExpression)exp);
                case DbExpressionType.Function:
                    return VisitFunction((DbFunctionExpression)exp);
                case DbExpressionType.Entity:
                    return VisitEntity((DbEntityExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        protected virtual Expression VisitEntity(DbEntityExpression entity)
        {
            Expression exp = Visit(entity.Expression);
            return UpdateEntity(entity, exp);
        }

        protected DbEntityExpression UpdateEntity(DbEntityExpression entity, 
            Expression expression)
        {
            if (expression != entity.Expression)
                return new DbEntityExpression(entity.Entity, expression);
            
            return entity;
        }

        protected virtual Expression VisitTable(DbTableExpression table)
        {
            return table;
        }

        protected virtual Expression VisitColumn(DbColumnExpression column)
        {
            return column;
        }

        protected virtual Expression VisitSelect(DbSelectExpression select)
        {
            Expression from = VisitSource(select.From);
            Expression where = Visit(select.Where);
            Expression skip = Visit(select.Skip);
            Expression take = Visit(select.Take);
            ReadOnlyCollection<DbOrderExpression> orderBy = 
                VisitOrderBy(select.OrderBy);
            ReadOnlyCollection<Expression> groupBy = 
                VisitExpressionList(select.GroupBy);
            ReadOnlyCollection<DbColumnDeclaration> columns = 
                VisitColumnDeclarations(select.Columns);
            
            return UpdateSelect(select, from, where, orderBy, groupBy, 
                skip, take, select.IsDistinct, columns);
        }

        protected DbSelectExpression UpdateSelect(DbSelectExpression select, 
            Expression from, Expression where, IEnumerable<DbOrderExpression> orderBy, 
            IEnumerable<Expression> groupBy, Expression skip, Expression take,
            bool isDistinct, IEnumerable<DbColumnDeclaration> columns)
        {
            if (from != select.From || where != select.Where || 
                orderBy != select.OrderBy || groupBy != select.GroupBy || 
                take != select.Take || skip != select.Skip || 
                isDistinct != select.IsDistinct || columns != select.Columns)
                return new DbSelectExpression(select.Alias, columns, from, where, 
                    orderBy, groupBy, isDistinct, skip, take);
            
            return select;
        }

        protected virtual Expression VisitJoin(DbJoinExpression join)
        {
            Expression left = VisitSource(join.Left);
            Expression right = VisitSource(join.Right);
            Expression condition = Visit(join.Condition);

            return UpdateJoin(join, join.Join, left, right, condition);
        }

        protected DbJoinExpression UpdateJoin(DbJoinExpression join, DbJoinType joinType, 
            Expression left, Expression right, Expression condition)
        {
            if (joinType != join.Join || left != join.Left || 
                right != join.Right || condition != join.Condition)
                return new DbJoinExpression(joinType, left, right, condition);
            
            return join;
        }

        protected virtual Expression VisitOuterJoined(DbOuterJoinedExpression outer)
        {
            Expression test = Visit(outer.Test);
            Expression expression = Visit(outer.Expression);

            return UpdateOuterJoined(outer, test, expression);
        }

        protected DbOuterJoinedExpression UpdateOuterJoined(DbOuterJoinedExpression outer, 
            Expression test, Expression expression)
        {
            if (test != outer.Test || expression != outer.Expression)
                return new DbOuterJoinedExpression(test, expression);
            
            return outer;
        }

        protected virtual Expression VisitAggregate(DbAggregateExpression aggregate)
        {
            Expression arg = Visit(aggregate.Argument);
            return UpdateAggregate(aggregate, aggregate.Type, 
                aggregate.AggregateType, arg, aggregate.IsDistinct);
        }

        protected DbAggregateExpression UpdateAggregate(DbAggregateExpression aggregate, Type type, 
            DbAggregateType aggType, Expression arg, bool isDistinct)
        {
            if (type != aggregate.Type || aggType != aggregate.AggregateType || 
                arg != aggregate.Argument || isDistinct != aggregate.IsDistinct)
                return new DbAggregateExpression(type, aggType, arg, isDistinct);
            
            return aggregate;
        }

        protected virtual Expression VisitIsNull(DbIsNullExpression isnull)
        {
            Expression expr = Visit(isnull.Expression);
            return UpdateIsNull(isnull, expr);
        }

        protected DbIsNullExpression UpdateIsNull(DbIsNullExpression isnull, 
            Expression expression)
        {
            if (expression != isnull.Expression)
                return new DbIsNullExpression(expression);
            
            return isnull;
        }

        protected virtual Expression VisitBetween(DbBetweenExpression between)
        {
            Expression expr = Visit(between.Expression);
            Expression lower = Visit(between.Lower);
            Expression upper = Visit(between.Upper);

            return UpdateBetween(between, expr, lower, upper);
        }

        protected DbBetweenExpression UpdateBetween(DbBetweenExpression between, 
            Expression expression, Expression lower, Expression upper)
        {
            if (expression != between.Expression || 
                lower != between.Lower || upper != between.Upper)
                return new DbBetweenExpression(expression, lower, upper);
            
            return between;
        }

        protected virtual Expression VisitRowNumber(DbRowNumberExpression rowNumber)
        {
            ReadOnlyCollection<DbOrderExpression> orderby = VisitOrderBy(rowNumber.OrderBy);
            return UpdateRowNumber(rowNumber, orderby);
        }

        protected DbRowNumberExpression UpdateRowNumber(
            DbRowNumberExpression rowNumber, IEnumerable<DbOrderExpression> orderBy)
        {
            if (orderBy != rowNumber.OrderBy)
                return new DbRowNumberExpression(orderBy);
            
            return rowNumber;
        }

        protected virtual Expression VisitNamedValue(DbNamedValueExpression value)
        {
            return value;
        }

        protected virtual Expression VisitSubquery(DbSubqueryExpression subquery)
        {
            switch ((DbExpressionType)subquery.NodeType)
            {
                case DbExpressionType.Scalar:
                    return VisitScalar((DbScalarExpression)subquery);
                case DbExpressionType.Exists:
                    return VisitExists((DbExistsExpression)subquery);
                case DbExpressionType.In:
                    return VisitIn((DbInExpression)subquery);
            }
            return subquery;
        }

        protected virtual Expression VisitScalar(DbScalarExpression scalar)
        {
            DbSelectExpression select = (DbSelectExpression)Visit(scalar.Select);
            return UpdateScalar(scalar, select);
        }

        protected DbScalarExpression UpdateScalar(DbScalarExpression scalar, 
            DbSelectExpression select)
        {
            if (select != scalar.Select)
                return new DbScalarExpression(scalar.Type, select);
            
            return scalar;
        }

        protected virtual Expression VisitExists(DbExistsExpression exists)
        {
            DbSelectExpression select = (DbSelectExpression)Visit(exists.Select);
            return UpdateExists(exists, select);
        }

        protected DbExistsExpression UpdateExists(DbExistsExpression exists, 
            DbSelectExpression select)
        {
            if (select != exists.Select)
                return new DbExistsExpression(select);
            
            return exists;
        }

        protected virtual Expression VisitIn(DbInExpression ixp)
        {
            Expression expr = Visit(ixp.Expression);
            DbSelectExpression select = (DbSelectExpression)Visit(ixp.Select);
            ReadOnlyCollection<Expression> values = VisitExpressionList(ixp.Values);

            return UpdateIn(ixp, expr, select, values);
        }

        protected DbInExpression UpdateIn(DbInExpression ixp, Expression expression, 
            DbSelectExpression select, IEnumerable<Expression> values)
        {
            if (expression != ixp.Expression || 
                select != ixp.Select || values != ixp.Values)
            {
                if (select != null)
                    return new DbInExpression(expression, select);
                
                return new DbInExpression(expression, values);
            }
            return ixp;
        }

        protected virtual Expression VisitAggregateSubquery(DbAggregateSubqueryExpression aggregate)
        {
            DbScalarExpression subquery = (DbScalarExpression)Visit(aggregate.AggregateAsSubquery);
            return UpdateAggregateSubquery(aggregate, subquery);
        }

        protected DbAggregateSubqueryExpression UpdateAggregateSubquery(
            DbAggregateSubqueryExpression aggregate, DbScalarExpression subquery)
        {
            if (subquery != aggregate.AggregateAsSubquery)
                return new DbAggregateSubqueryExpression(aggregate.GroupByAlias, 
                    aggregate.AggregateInGroupSelect, subquery);
            
            return aggregate;
        }

        protected virtual Expression VisitSource(Expression source)
        {
            return Visit(source);
        }

        protected virtual Expression VisitProjection(DbProjectionExpression proj)
        {
            DbSelectExpression select = (DbSelectExpression)Visit(proj.Select);
            Expression projector = Visit(proj.Projector);

            return UpdateProjection(proj, select, projector, proj.Aggregator);
        }

        protected DbProjectionExpression UpdateProjection(DbProjectionExpression proj, 
            DbSelectExpression select, Expression projector, LambdaExpression aggregator)
        {
            if (select != proj.Select || projector != proj.Projector || aggregator != proj.Aggregator)
                return new DbProjectionExpression(select, projector, aggregator);
            
            return proj;
        }

        protected virtual Expression VisitClientJoin(DbClientJoinExpression join)
        {
            DbProjectionExpression projection = (DbProjectionExpression)Visit(join.Projection);
            ReadOnlyCollection<Expression> outerKey = VisitExpressionList(join.OuterKey);
            ReadOnlyCollection<Expression> innerKey = VisitExpressionList(join.InnerKey);

            return UpdateClientJoin(join, projection, outerKey, innerKey);
        }

        protected DbClientJoinExpression UpdateClientJoin(DbClientJoinExpression join, 
            DbProjectionExpression projection, IEnumerable<Expression> outerKey, 
            IEnumerable<Expression> innerKey)
        {
            if (projection != join.Projection || outerKey != join.OuterKey || innerKey != join.InnerKey)
                return new DbClientJoinExpression(projection, outerKey, innerKey);
            
            return join;
        }

        protected virtual Expression VisitStatement(DbStatementExpression command)
        {
            switch ((DbExpressionType)command.NodeType)
            {
                case DbExpressionType.Insert:
                    return VisitInsert((DbInsertExpression)command);
                case DbExpressionType.Update:
                    return VisitUpdate((DbUpdateExpression)command);
                case DbExpressionType.Delete:
                    return VisitDelete((DbDeleteExpression)command);
                case DbExpressionType.If:
                    return VisitIf((DbConditionalExpression)command);
                case DbExpressionType.Block:
                    return VisitBlock((DbExpressionSet)command);
                case DbExpressionType.Declaration:
                    return VisitDeclaration((DbDeclaration)command);
                default:
                    return VisitUnknown(command);
            }
        }

        protected virtual Expression VisitInsert(DbInsertExpression insert)
        {
            DbTableExpression table = (DbTableExpression)Visit(insert.Table);
            ReadOnlyCollection<DbColumnAssignment> assignments = 
                VisitColumnAssignments(insert.Assignments);

            return UpdateInsert(insert, table, assignments);
        }

        protected DbInsertExpression UpdateInsert(DbInsertExpression insert, 
            DbTableExpression table, IEnumerable<DbColumnAssignment> assignments)
        {
            if (table != insert.Table || assignments != insert.Assignments)
                return new DbInsertExpression(table, assignments);
            
            return insert;
        }

        protected virtual Expression VisitUpdate(DbUpdateExpression update)
        {
            DbTableExpression table = (DbTableExpression)Visit(update.Table);
            Expression where = Visit(update.Where);
            ReadOnlyCollection<DbColumnAssignment> assignments = 
                VisitColumnAssignments(update.Assignments);

            return UpdateUpdate(update, table, where, assignments);
        }

        protected DbUpdateExpression UpdateUpdate(DbUpdateExpression update, 
            DbTableExpression table, Expression where, IEnumerable<DbColumnAssignment> assignments)
        {
            if (table != update.Table || where != update.Where || 
                assignments != update.Assignments)
                return new DbUpdateExpression(table, where, assignments);
            
            return update;
        }

        protected virtual Expression VisitDelete(DbDeleteExpression delete)
        {
            DbTableExpression table = (DbTableExpression)Visit(delete.Table);
            Expression where = Visit(delete.Where);
            return UpdateDelete(delete, table, where);
        }

        protected DbDeleteExpression UpdateDelete(DbDeleteExpression delete, 
            DbTableExpression table, Expression where)
        {
            if (table != delete.Table || where != delete.Where)
                return new DbDeleteExpression(table, where);
            
            return delete;
        }

        protected virtual Expression VisitBatch(BatchExpression batch)
        {
            LambdaExpression operation = (LambdaExpression)Visit(batch.Operation);
            Expression batchSize = Visit(batch.Size);
            Expression stream = Visit(batch.Stream);

            return UpdateBatch(batch, batch.Input, operation, batchSize, stream);
        }

        protected BatchExpression UpdateBatch(BatchExpression batch, Expression input, 
            LambdaExpression operation, Expression batchSize, Expression stream)
        {
            if (input != batch.Input || operation != batch.Operation || 
                batchSize != batch.Size || stream != batch.Stream)
                return new BatchExpression(input, operation, batchSize, stream);
            
            return batch;
        }

        protected virtual Expression VisitIf(DbConditionalExpression ifx)
        {
            var check = Visit(ifx.Check);
            var ifTrue = Visit(ifx.IfTrue);
            var ifFalse = Visit(ifx.IfFalse);

            return UpdateIf(ifx, check, ifTrue, ifFalse);
        }

        protected DbConditionalExpression UpdateIf(DbConditionalExpression ifx, Expression check, 
            Expression ifTrue, Expression ifFalse)
        {
            if (check != ifx.Check || ifTrue != ifx.IfTrue || ifFalse != ifx.IfFalse)
                return new DbConditionalExpression(check, ifTrue, ifFalse);
            
            return ifx;
        }

        protected virtual Expression VisitBlock(DbExpressionSet block)
        {
            var commands = VisitExpressionList(block.Expressions);
            return UpdateBlock(block, commands);
        }

        protected DbExpressionSet UpdateBlock(DbExpressionSet block, IList<Expression> commands)
        {
            if (block.Expressions != commands)
                return new DbExpressionSet(commands);
            
            return block;
        }

        protected virtual Expression VisitDeclaration(DbDeclaration decl)
        {
            ReadOnlyCollection<DbVariableDeclaration> variables = VisitVariableDeclarations(decl.Variables);
            DbSelectExpression source = (DbSelectExpression)Visit(decl.Source);

            return UpdateDeclaration(decl, variables, source);
        }

        protected DbDeclaration UpdateDeclaration(DbDeclaration decl, 
            IEnumerable<DbVariableDeclaration> variables, DbSelectExpression source)
        {
            if (variables != decl.Variables || source != decl.Source)
                return new DbDeclaration(variables, source);
            
            return decl;
        }

        protected virtual Expression VisitVariable(DbVariableExpression vex)
        {
            return vex;
        }

        protected virtual Expression VisitFunction(DbFunctionExpression func)
        {
            ReadOnlyCollection<Expression> arguments = VisitExpressionList(func.Arguments);
            return UpdateFunction(func, func.Name, arguments);
        }

        protected DbFunctionExpression UpdateFunction(DbFunctionExpression func, string name, IEnumerable<Expression> arguments)
        {
            if (name != func.Name || arguments != func.Arguments)
                return new DbFunctionExpression(func.Type, name, arguments);
            
            return func;
        }

        protected virtual DbColumnAssignment VisitColumnAssignment(DbColumnAssignment ca)
        {
            DbColumnExpression c = (DbColumnExpression)Visit(ca.Column);
            Expression e = Visit(ca.Expression);
            return UpdateColumnAssignment(ca, c, e);
        }

        protected DbColumnAssignment UpdateColumnAssignment(
            DbColumnAssignment ca, DbColumnExpression c, Expression e)
        {
            if (c != ca.Column || e != ca.Expression)
                return new DbColumnAssignment(c, e);
            
            return ca;
        }

        protected virtual ReadOnlyCollection<DbColumnAssignment> VisitColumnAssignments(
            ReadOnlyCollection<DbColumnAssignment> assignments)
        {
            List<DbColumnAssignment> alternate = null;
            for (int i = 0, n = assignments.Count; i < n; i++)
            {
                DbColumnAssignment assignment = VisitColumnAssignment(assignments[i]);
                if (alternate == null && assignment != assignments[i])
                    alternate = assignments.Take(i).ToList();
                
                if (alternate != null)
                    alternate.Add(assignment);
            }
            if (alternate != null)
                return alternate.AsReadOnly();
            
            return assignments;
        }

        protected virtual ReadOnlyCollection<DbColumnDeclaration> VisitColumnDeclarations(
            ReadOnlyCollection<DbColumnDeclaration> columns)
        {
            List<DbColumnDeclaration> alternate = null;
            for (int i = 0, n = columns.Count; i < n; i++)
            {
                DbColumnDeclaration column = columns[i];
                Expression e = Visit(column.Expression);
                if (alternate == null && e != column.Expression)
                    alternate = columns.Take(i).ToList();
                
                if (alternate != null)
                    alternate.Add(new DbColumnDeclaration(column.Name, e));
            }
            if (alternate != null)
                return alternate.AsReadOnly();
            
            return columns;
        }

        protected virtual ReadOnlyCollection<DbVariableDeclaration> VisitVariableDeclarations(
            ReadOnlyCollection<DbVariableDeclaration> decls)
        {
            List<DbVariableDeclaration> alternate = null;
            for (int i = 0, n = decls.Count; i < n; i++)
            {
                DbVariableDeclaration decl = decls[i];
                Expression e = Visit(decl.Expression);
                if (alternate == null && e != decl.Expression)
                    alternate = decls.Take(i).ToList();
                
                if (alternate != null)
                    alternate.Add(new DbVariableDeclaration(decl.Name, decl.DbType, e));
            }
            if (alternate != null)
                return alternate.AsReadOnly();
            
            return decls;
        }

        protected virtual ReadOnlyCollection<DbOrderExpression> VisitOrderBy(
            ReadOnlyCollection<DbOrderExpression> expressions)
        {
            if (expressions != null)
            {
                List<DbOrderExpression> alternate = null;
                for (int i = 0, n = expressions.Count; i < n; i++)
                {
                    DbOrderExpression expr = expressions[i];
                    Expression e = Visit(expr.Expression);
                    if (alternate == null && e != expr.Expression)
                        alternate = expressions.Take(i).ToList();
                    
                    if (alternate != null)
                        alternate.Add(new DbOrderExpression(expr.OrderType, e));
                }
                if (alternate != null)
                    return alternate.AsReadOnly();
            }
            return expressions;
        }
    }
}
