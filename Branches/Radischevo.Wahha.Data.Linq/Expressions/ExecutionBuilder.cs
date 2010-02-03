using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Jeltofiol.Wahha.Data.Linq.Mapping;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Data;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Builds an execution plan for a query expression
    /// </summary>
    public class ExecutionBuilder : DbExpressionVisitor
    {
        #region Nested Types

        #endregion

        QueryPolicy policy;
        MetaMapping mapping;
        Expression provider;
        Scope scope;
        bool isTop = true;
        MemberInfo receivingMember;
        int nReaders = 0;
        List<ParameterExpression> variables = new List<ParameterExpression>();
        List<Expression> initializers = new List<Expression>();
        Dictionary<string, Expression> variableMap = new Dictionary<string, Expression>();

        private ExecutionBuilder(MetaMapping mapping, QueryPolicy policy, Expression provider)
        {
            this.mapping = mapping;
            this.policy = policy;
            this.provider = provider;
        }

        public static Expression Build(MetaMapping mapping, QueryPolicy policy, Expression expression, Expression provider)
        {
            return new ExecutionBuilder(mapping, policy, provider).Build(expression);
        }

        private Expression Build(Expression expression)
        {
            expression = this.Visit(expression);
            expression = this.AddVariables(expression);
            return expression;
        }

        private Expression AddVariables(Expression expression)
        {
            // add variable assignments up front
            if (this.variables.Count > 0)
            {
                List<Expression> exprs = new List<Expression>();
                for (int i = 0, n = this.variables.Count; i < n; i++)
                {
                    exprs.Add(MakeAssign(this.variables[i], this.initializers[i]));
                }
                exprs.Add(expression);
                Expression sequence = MakeSequence(exprs);  // yields last expression value

                // use invoke/lambda to create variables via parameters in scope
                Expression[] nulls = this.variables.Select(v => Expression.Constant(null, v.Type)).ToArray();
                expression = Expression.Invoke(Expression.Lambda(sequence, this.variables.ToArray()), nulls);
            }

            return expression;
        }

        private static Expression MakeSequence(IList<Expression> expressions)
        {
            Expression last = expressions[expressions.Count - 1];
            expressions = expressions.Select(e => e.Type.IsValueType ? Expression.Convert(e, typeof(object)) : e).ToList();
            return Expression.Convert(Expression.Call(typeof(ExecutionBuilder), "Sequence", null, Expression.NewArrayInit(typeof(object), expressions)), last.Type);
        }

        public static object Sequence(params object[] values)
        {
            return values[values.Length - 1];
        }

        public static IEnumerable<R> Batch<T, R>(IEnumerable<T> items, Func<T, R> selector, bool stream)
        {
            var result = items.Select(selector);
            if (!stream)
            {
                return result.ToList();
            }
            else
            {
                return new EnumerateOnce<R>(result);
            }
        }

        private static Expression MakeAssign(ParameterExpression variable, Expression value)
        {
            return Expression.Call(typeof(ExecutionBuilder), "Assign", new Type[] { variable.Type }, variable, value);
        }

        public static T Assign<T>(ref T variable, T value)
        {
            variable = value;
            return value;
        }

        private Expression BuildInner(Expression expression)
        {
            var eb = new ExecutionBuilder(this.mapping, this.policy, this.provider);
            eb.scope = this.scope;
            eb.receivingMember = this.receivingMember;
            eb.nReaders = this.nReaders;
            eb.nLookup = this.nLookup;
            eb.variableMap = this.variableMap;
            return eb.Build(expression);
        }

        protected override MemberBinding VisitBinding(MemberBinding binding)
        {
            var save = this.receivingMember;
            this.receivingMember = binding.Member;
            var result = base.VisitBinding(binding);
            this.receivingMember = save;
            return result;
        }

        int nLookup = 0;

        private Expression MakeJoinKey(IList<Expression> key)
        {
            if (key.Count == 1)
            {
                return key[0];
            }
            else
            {
                return Expression.New(
                    typeof(CompoundKey).GetConstructors()[0],
                    Expression.NewArrayInit(typeof(object), key.Select(k => (Expression)Expression.Convert(k, typeof(object))))
                    );
            }
        }

        protected override Expression VisitClientJoin(DbClientJoinExpression join)
        {
            // convert client join into a up-front lookup table builder & replace client-join in tree with lookup accessor

            // 1) lookup = query.Select(e => new KVP(key: inner, value: e)).ToLookup(kvp => kvp.Key, kvp => kvp.Value)
            Expression innerKey = MakeJoinKey(join.InnerKey);
            Expression outerKey = MakeJoinKey(join.OuterKey);

            ConstructorInfo kvpConstructor = typeof(KeyValuePair<,>).MakeGenericType(innerKey.Type, join.Projection.Projector.Type).GetConstructor(new Type[] { innerKey.Type, join.Projection.Projector.Type });
            Expression constructKVPair = Expression.New(kvpConstructor, innerKey, join.Projection.Projector);
            DbProjectionExpression newProjection = new DbProjectionExpression(join.Projection.Select, constructKVPair);

            int iLookup = ++nLookup;
            Expression execution = this.ExecuteProjection(newProjection, false);

            ParameterExpression kvp = Expression.Parameter(constructKVPair.Type, "kvp");

            // filter out nulls
            if (join.Projection.Projector.NodeType == (ExpressionType)DbExpressionType.OuterJoined)
            {
                LambdaExpression pred = Expression.Lambda(
                    Expression.PropertyOrField(kvp, "Value").NotEqual(TypeHelper.GetNullConstant(join.Projection.Projector.Type)),
                    kvp
                    );
                execution = Expression.Call(typeof(Enumerable), "Where", new Type[] { kvp.Type }, execution, pred);
            }

            // make lookup
            LambdaExpression keySelector = Expression.Lambda(Expression.PropertyOrField(kvp, "Key"), kvp);
            LambdaExpression elementSelector = Expression.Lambda(Expression.PropertyOrField(kvp, "Value"), kvp);
            Expression toLookup = Expression.Call(typeof(Enumerable), "ToLookup", new Type[] { kvp.Type, outerKey.Type, join.Projection.Projector.Type }, execution, keySelector, elementSelector);

            // 2) agg(lookup[outer])
            ParameterExpression lookup = Expression.Parameter(toLookup.Type, "lookup" + iLookup);
            PropertyInfo property = lookup.Type.GetProperty("Item");
            Expression access = Expression.Call(lookup, property.GetGetMethod(), this.Visit(outerKey));
            if (join.Projection.Aggregator != null)
            {
                // apply aggregator
                access = DbExpressionReplacer.Replace(join.Projection.Aggregator.Body, join.Projection.Aggregator.Parameters[0], access);
            }

            this.variables.Add(lookup);
            this.initializers.Add(toLookup);

            return access;
        }

        protected override Expression VisitProjection(DbProjectionExpression projection)
        {
            if (this.isTop)
            {
                this.isTop = false;
                return this.ExecuteProjection(projection, this.scope != null);
            }
            else
            {
                return this.BuildInner(projection);
            }
        }

        protected virtual Expression Parameterize(Expression expression)
        {
            if (this.variableMap.Count > 0)
            {
                expression = VariableSubstitutor.Substitute(this.variableMap, expression);
            }
            return this.mapping.Language.Parameterize(expression);
        }

        private Expression ExecuteProjection(DbProjectionExpression projection, bool okayToDefer)
        {
            // parameterize query
            projection = (DbProjectionExpression)this.Parameterize(projection);

            if (this.scope != null)
            {
                // also convert references to outer alias to named values!  these become SQL parameters too
                projection = (DbProjectionExpression)OuterParameterizer.Parameterize(this.scope.Alias, projection);
            }

            string commandText = this.mapping.Language.Format(projection.Select);
            ReadOnlyCollection<DbNamedValueExpression> namedValues = NamedValueGatherer.Gather(projection.Select);
            QueryCommand command = new QueryCommand(commandText, namedValues.Select(v => new QueryParameter(v.Name, v.Type, v.DbType)), null);
            Expression[] values = namedValues.Select(v => Expression.Convert(this.Visit(v.Value), typeof(object))).ToArray();

            return this.ExecuteProjection(projection, okayToDefer, command, values);
        }

        private Expression ExecuteProjection(DbProjectionExpression projection, bool okayToDefer, QueryCommand command, Expression[] values)
        {
            okayToDefer &= (this.receivingMember != null && this.policy.IsDeferLoaded(this.receivingMember));

            var saveScope = this.scope;
            ParameterExpression reader = Expression.Parameter(typeof(IDataReader), "r" + nReaders++);
            this.scope = new Scope(this.scope, reader, projection.Select.Alias, projection.Select.Columns);
            LambdaExpression projector = Expression.Lambda(this.Visit(projection.Projector), reader);
            this.scope = saveScope;

            string methExecute = okayToDefer
                ? "ExecuteDeferred"
                : "Execute";

            // call low-level execute directly on supplied DbQueryProvider
            Expression result = Expression.Call(this.provider, methExecute, new Type[] { projector.Body.Type },
                Expression.Constant(command),
                projector,
                Expression.NewArrayInit(typeof(object), values)
                );

            if (projection.Aggregator != null)
            {
                // apply aggregator
                result = DbExpressionReplacer.Replace(projection.Aggregator.Body, projection.Aggregator.Parameters[0], result);
            }
            return result;
        }

        protected override Expression VisitBatch(BatchExpression batch)
        {
            if (this.mapping.Language.AllowsMultipleCommands || !IsMultipleCommands(batch.Operation.Body as DbStatementExpression))
            {
                return this.BuildExecuteBatch(batch);
            }
            else
            {
                var source = this.Visit(batch.Input);
                var op = this.Visit(batch.Operation.Body);
                var fn = Expression.Lambda(op, batch.Operation.Parameters[1]);
                return Expression.Call(this.GetType(), "Batch", new Type[] { source.Type.GetSequenceElementType(), 
                    batch.Operation.Body.Type }, source, fn, batch.Stream);
            }
        }

        protected virtual Expression BuildExecuteBatch(BatchExpression batch)
        {
            // parameterize query
            Expression operation = this.Parameterize(batch.Operation.Body);

            string commandText = this.mapping.Language.Format(operation);
            var namedValues = NamedValueGatherer.Gather(operation);
            QueryCommand command = new QueryCommand(commandText, namedValues.Select(v => new QueryParameter(v.Name, v.Type, v.DbType)), null);
            Expression[] values = namedValues.Select(v => Expression.Convert(this.Visit(v.Value), typeof(object))).ToArray();

            Expression paramSets = Expression.Call(typeof(Enumerable), "Select", new Type[] { batch.Operation.Parameters[1].Type, typeof(object[]) },
                batch.Input,
                Expression.Lambda(Expression.NewArrayInit(typeof(object), values), new[] { batch.Operation.Parameters[1] })
                );

            Expression plan = null;

            DbProjectionExpression projection = ProjectionFinder.FindProjection(operation);
            if (projection != null)
            {
                var saveScope = this.scope;
                ParameterExpression reader = Expression.Parameter(typeof(IDataReader), "r" + nReaders++);
                this.scope = new Scope(this.scope, reader, projection.Select.Alias, projection.Select.Columns);
                LambdaExpression projector = Expression.Lambda(this.Visit(projection.Projector), reader);
                this.scope = saveScope;

                var columns = ColumnGatherer.Gather(projection.Projector);
                command = new QueryCommand(command.CommandText, command.Parameters, columns);

                plan = Expression.Call(this.provider, "ExecuteBatch", new Type[] { projector.Body.Type },
                    Expression.Constant(command),
                    paramSets,
                    projector,
                    batch.Size,
                    batch.Stream
                    );
            }
            else
            {
                plan = Expression.Call(this.provider, "ExecuteBatch", null,
                    Expression.Constant(command),
                    paramSets,
                    batch.Size,
                    batch.Stream
                    );
            }

            return plan;
        }

        protected override Expression VisitStatement(DbStatementExpression command)
        {
            if (this.mapping.Language.AllowsMultipleCommands || !IsMultipleCommands(command))
            {
                return this.BuildExecuteCommand(command);
            }
            else
            {
                return base.VisitStatement(command);
            }
        }

        protected virtual bool IsMultipleCommands(DbStatementExpression command)
        {
            if (command == null)
                return false;
            switch ((DbExpressionType)command.NodeType)
            {
                case DbExpressionType.Insert:
                case DbExpressionType.Delete:
                case DbExpressionType.Update:
                    return false;
                default:
                    return true;
            }
        }

        protected override Expression VisitInsert(DbInsertExpression insert)
        {
            return this.BuildExecuteCommand(insert);
        }

        protected override Expression VisitUpdate(DbUpdateExpression update)
        {
            return this.BuildExecuteCommand(update);
        }

        protected override Expression VisitDelete(DbDeleteExpression delete)
        {
            return this.BuildExecuteCommand(delete);
        }

        protected override Expression VisitBlock(DbExpressionSet block)
        {
            return MakeSequence(this.VisitExpressionList(block.Expressions));
        }

        protected override Expression VisitIf(DbConditionalExpression ifx)
        {
            var test =
                Expression.Condition(
                    ifx.Check,
                    ifx.IfTrue,
                    ifx.IfFalse != null
                        ? ifx.IfFalse
                        : ifx.IfTrue.Type == typeof(int)
                            ? (Expression)Expression.Property(this.provider, "RowsAffected")
                            : (Expression)Expression.Constant(ifx.IfTrue.Type.CreateInstance(), ifx.IfTrue.Type)
                            );
            return this.Visit(test);
        }

        protected override Expression VisitFunction(DbFunctionExpression func)
        {
            if (func.Name == "@@ROWCOUNT")
            {
                return Expression.Property(this.provider, "RowsAffected");
            }
            return base.VisitFunction(func);
        }

        protected override Expression VisitExists(DbExistsExpression exists)
        {
            // how did we get here? Translate exists into count query
            var newSelect = exists.Select.SetColumns(
                new[] { new DbColumnDeclaration("value", new DbAggregateExpression(typeof(int), DbAggregateType.Count, null, false)) }
                );

            var projection =
                new DbProjectionExpression(
                    newSelect,
                    new DbColumnExpression(typeof(int), null, newSelect.Alias, "value"),
                    Aggregator.Aggregate(typeof(int), typeof(IEnumerable<int>))
                    );

            var expression = projection.GreaterThan(Expression.Constant(0));

            return this.Visit(expression);
        }

        protected override Expression VisitDeclaration(DbDeclaration decl)
        {
            if (decl.Source != null)
            {
                // make query that returns all these declared values as an object[]
                var projection = new DbProjectionExpression(
                    decl.Source,
                    Expression.NewArrayInit(
                        typeof(object),
                        decl.Variables.Select(v => v.Expression.Type.IsValueType
                            ? Expression.Convert(v.Expression, typeof(object))
                            : v.Expression).ToArray()
                        ),
                    Aggregator.Aggregate(typeof(object[]), typeof(IEnumerable<object[]>))
                    );

                // create execution variable to hold the array of declared variables
                var vars = Expression.Parameter(typeof(object[]), "vars");
                this.variables.Add(vars);
                this.initializers.Add(Expression.Constant(null, typeof(object[])));

                // create subsitution for each variable (so it will find the variable value in the new vars array)
                for (int i = 0, n = decl.Variables.Count; i < n; i++)
                {
                    var v = decl.Variables[i];
                    DbNamedValueExpression nv = new DbNamedValueExpression(
                        v.Name, v.DbType,
                        Expression.Convert(Expression.ArrayIndex(vars, Expression.Constant(i)), v.Expression.Type)
                        );
                    this.variableMap.Add(v.Name, nv);
                }

                // make sure the execution of the select stuffs the results into the new vars array
                return MakeAssign(vars, this.Visit(projection));
            }

            // probably bad if we get here since we must not allow mulitple commands
            throw new InvalidOperationException("Declaration query not allowed for this langauge");
        }

        protected virtual Expression BuildExecuteCommand(DbStatementExpression command)
        {
            // parameterize query
            var expression = this.Parameterize(command);

            string commandText = this.mapping.Language.Format(expression);
            ReadOnlyCollection<DbNamedValueExpression> namedValues = NamedValueGatherer.Gather(expression);
            QueryCommand qc = new QueryCommand(commandText, namedValues.Select(v => new QueryParameter(v.Name, v.Type, v.DbType)), null);
            Expression[] values = namedValues.Select(v => Expression.Convert(this.Visit(v.Value), typeof(object))).ToArray();

            DbProjectionExpression projection = ProjectionFinder.FindProjection(expression);
            if (projection != null)
            {
                return this.ExecuteProjection(projection, false, qc, values);
            }

            Expression plan = Expression.Call(this.provider, "ExecuteCommand", null,
                Expression.Constant(qc),
                Expression.NewArrayInit(typeof(object), values)
                );

            return plan;
        }

        protected override Expression VisitEntity(DbEntityExpression entity)
        {
            return this.Visit(entity.Expression);
        }

        protected override Expression VisitOuterJoined(DbOuterJoinedExpression outer)
        {
            Expression expr = this.Visit(outer.Expression);
            DbColumnExpression column = (DbColumnExpression)outer.Test;
            ParameterExpression reader;
            int iOrdinal;
            if (this.scope.TryGetValue(column, out reader, out iOrdinal))
            {
                return Expression.Condition(
                    Expression.Call(reader, "IsDbNull", null, Expression.Constant(iOrdinal)),
                    Expression.Constant(outer.Type.CreateInstance(), outer.Type),
                    expr
                    );
            }
            return expr;
        }

        public static T GetValue<T>(DbQueryProvider provider, IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return default(T);
            }
            object value = reader.GetValue(ordinal);
            Type ttype = typeof(T);
            Type vtype = value.GetType();
            if (vtype != ttype)
            {
                return (T)provider.Convert(value, ttype);
            }
            return (T)value;
        }

        protected override Expression VisitColumn(DbColumnExpression column)
        {
            ParameterExpression reader;
            int iOrdinal;

            if (this.scope != null && this.scope.TryGetValue(column, out reader, out iOrdinal))
            {
                return Expression.Call(this.GetType(), "GetValue", new Type[] { column.Type }, this.provider, reader, Expression.Constant(iOrdinal));
            }
            else
            {
                System.Diagnostics.Debug.Fail(string.Format("column not in scope: {0}", column));
            }
            return column;
        }

        class Scope
        {
            Scope outer;
            ParameterExpression dbDataReader;
            internal DbTableAlias Alias { get; private set; }
            Dictionary<string, int> nameMap;

            internal Scope(Scope outer, ParameterExpression dbDataReaderParam, 
                DbTableAlias alias, IEnumerable<DbColumnDeclaration> columns)
            {
                this.outer = outer;
                this.dbDataReader = dbDataReaderParam;
                this.Alias = alias;
                this.nameMap = columns.Select((c, i) => new { c, i }).ToDictionary(x => x.c.Name, x => x.i);
            }

            internal bool TryGetValue(DbColumnExpression column, out ParameterExpression dbDataReader, out int ordinal)
            {
                for (Scope s = this; s != null; s = s.outer)
                {
                    if (column.Alias == s.Alias && this.nameMap.TryGetValue(column.Name, out ordinal))
                    {
                        dbDataReader = this.dbDataReader;
                        return true;
                    }
                }
                dbDataReader = null;
                ordinal = 0;
                return false;
            }
        }

        /// <summary>
        /// columns referencing the outer alias are turned into special named-value parameters
        /// </summary>
        class OuterParameterizer : DbExpressionVisitor
        {
            int iParam;
            DbTableAlias outerAlias;
            Dictionary<DbColumnExpression, DbNamedValueExpression> map = new Dictionary<DbColumnExpression, DbNamedValueExpression>();

            internal static Expression Parameterize(DbTableAlias outerAlias, Expression expr)
            {
                OuterParameterizer op = new OuterParameterizer();
                op.outerAlias = outerAlias;
                return op.Visit(expr);
            }

            protected override Expression VisitProjection(DbProjectionExpression proj)
            {
                DbSelectExpression select = (DbSelectExpression)this.Visit(proj.Select);
                return this.UpdateProjection(proj, select, proj.Projector, proj.Aggregator);
            }

            protected override Expression VisitColumn(DbColumnExpression column)
            {
                if (column.Alias == this.outerAlias)
                {
                    DbNamedValueExpression nv;
                    if (!this.map.TryGetValue(column, out nv))
                    {
                        nv = new DbNamedValueExpression("n" + (iParam++), column.DbType, column);
                        this.map.Add(column, nv);
                    }
                    return nv;
                }
                return column;
            }
        }

        class ColumnGatherer : DbExpressionVisitor
        {
            Dictionary<string, DbColumnExpression> columns = new Dictionary<string, DbColumnExpression>();

            internal static IEnumerable<DbColumnExpression> Gather(Expression expression)
            {
                var gatherer = new ColumnGatherer();
                gatherer.Visit(expression);
                return gatherer.columns.Values;
            }

            protected override Expression VisitColumn(DbColumnExpression column)
            {
                if (!this.columns.ContainsKey(column.Name))
                {
                    this.columns.Add(column.Name, column);
                }
                return column;
            }
        }

        class ProjectionFinder : DbExpressionVisitor
        {
            DbProjectionExpression found = null;

            internal static DbProjectionExpression FindProjection(Expression expression)
            {
                var finder = new ProjectionFinder();
                finder.Visit(expression);
                return finder.found;
            }

            protected override Expression VisitProjection(DbProjectionExpression proj)
            {
                this.found = proj;
                return proj;
            }
        }

        class VariableSubstitutor : DbExpressionVisitor
        {
            Dictionary<string, Expression> map;

            private VariableSubstitutor(Dictionary<string, Expression> map)
            {
                this.map = map;
            }

            public static Expression Substitute(Dictionary<string, Expression> map, Expression expression)
            {
                return new VariableSubstitutor(map).Visit(expression);
            }

            protected override Expression VisitVariable(DbVariableExpression vex)
            {
                Expression sub;
                if (this.map.TryGetValue(vex.Name, out sub))
                {
                    return sub;
                }
                return vex;
            }
        }
    }
}
