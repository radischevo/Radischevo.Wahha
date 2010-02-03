// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Jeltofiol.Wahha.Data.Linq.Mapping;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Translates accesses to relationship members into projections or joins
    /// </summary>
    public class RelationshipBinder : DbExpressionVisitor
    {
        MetaMapping mapping;
        Expression currentFrom;

        private RelationshipBinder(MetaMapping mapping)
        {
            this.mapping = mapping;
        }

        public static Expression Bind(MetaMapping mapping, Expression expression)
        {
            return new RelationshipBinder(mapping).Visit(expression);
        }

        protected override Expression VisitSelect(DbSelectExpression select)
        {
            Expression saveCurrentFrom = this.currentFrom;
            this.currentFrom = this.VisitSource(select.From);
            try
            {
                Expression where = this.Visit(select.Where);
                ReadOnlyCollection<DbOrderExpression> orderBy = this.VisitOrderBy(select.OrderBy);
                ReadOnlyCollection<Expression> groupBy = this.VisitExpressionList(select.GroupBy);
                Expression skip = this.Visit(select.Skip);
                Expression take = this.Visit(select.Take);
                ReadOnlyCollection<DbColumnDeclaration> columns = this.VisitColumnDeclarations(select.Columns);
                if (this.currentFrom != select.From
                    || where != select.Where
                    || orderBy != select.OrderBy
                    || groupBy != select.GroupBy
                    || take != select.Take
                    || skip != select.Skip
                    || columns != select.Columns
                    )
                {
                    return new DbSelectExpression(select.Alias, columns, this.currentFrom, where, orderBy, groupBy, select.IsDistinct, skip, take);
                }
                return select;
            }
            finally
            {
                this.currentFrom = saveCurrentFrom;
            }
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            Expression source = this.Visit(m.Expression);
            DbEntityExpression ex = source as DbEntityExpression;

            if (ex != null && this.mapping.IsAssociation(ex.Entity, m.Member))
            {
                DbProjectionExpression projection = (DbProjectionExpression)this.Visit(this.mapping.CreateMemberExpression(source, ex.Entity, m.Member));
                if (this.currentFrom != null && this.mapping.IsSingletonAssociation(ex.Entity, m.Member))
                {
                    // convert singleton associations directly to OUTER APPLY
                    projection = projection.AddOuterJoinTest();
                    Expression newFrom = new DbJoinExpression(DbJoinType.OuterApply, this.currentFrom, projection.Select, null);
                    this.currentFrom = newFrom;
                    return projection.Projector;
                }
                return projection;
            }
            else
            {
                Expression result = QueryBinder.BindMember(source, m.Member);
                MemberExpression mex = result as MemberExpression;
                if (mex != null && mex.Member == m.Member && mex.Expression == m.Expression)
                {
                    return m;
                }
                return result;
            }
        }
    }
}
