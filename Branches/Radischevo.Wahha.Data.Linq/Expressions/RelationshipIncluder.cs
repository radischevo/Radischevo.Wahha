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
    /// Adds relationship to query results depending on policy
    /// </summary>
    public class RelationshipIncluder : DbExpressionVisitor
    {
        MetaMapping mapping;
        QueryPolicy policy;
        ScopedDictionary<MemberInfo, bool> includeScope = new ScopedDictionary<MemberInfo, bool>(null);

        private RelationshipIncluder(MetaMapping mapping, QueryPolicy policy)
        {
            this.mapping = mapping;
            this.policy = policy;
        }

        public static Expression Include(MetaMapping mapping, QueryPolicy policy, Expression expression)
        {
            return new RelationshipIncluder(mapping, policy).Visit(expression);
        }

        protected override Expression VisitProjection(DbProjectionExpression proj)
        {
            Expression projector = this.Visit(proj.Projector);
            return this.UpdateProjection(proj, proj.Select, projector, proj.Aggregator);
        }

        protected override Expression VisitEntity(DbEntityExpression entity)
        {
            var save = this.includeScope;
            this.includeScope = new ScopedDictionary<MemberInfo,bool>(this.includeScope);
            try
            {
                entity = this.mapping.IncludeMembers(
                    entity,  
                    m => {
                        if (this.includeScope.ContainsKey(m))
                        {
                            return false;
                        }
                        if (this.policy.IsIncluded(m))
                        {
                            this.includeScope.Add(m, true);
                            return true;
                        }
                        return false;
                    });

                return base.VisitEntity(entity);
            }
            finally
            {
                this.includeScope = save;
            }
        }
    }
}
