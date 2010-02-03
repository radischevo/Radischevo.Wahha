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

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    /// <summary>
    /// Converts user arguments into named-value parameters
    /// </summary>
    public class Parameterizer : DbExpressionVisitor
    {
        QueryLanguage language;
        Dictionary<object, DbNamedValueExpression> map = new Dictionary<object, DbNamedValueExpression>();
        Dictionary<HashedExpression, DbNamedValueExpression> pmap = new Dictionary<HashedExpression, DbNamedValueExpression>();

        private Parameterizer(QueryLanguage language)
        {
            this.language = language;
        }

        public static Expression Parameterize(QueryLanguage language, Expression expression)
        {
            return new Parameterizer(language).Visit(expression);
        }

        protected override Expression VisitProjection(DbProjectionExpression proj)
        {
            // don't parameterize the projector or aggregator!
            DbSelectExpression select = (DbSelectExpression)this.Visit(proj.Select);
            return this.UpdateProjection(proj, select, proj.Projector, proj.Aggregator);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            if (left.NodeType == (ExpressionType)DbExpressionType.NamedValue
             && right.NodeType == (ExpressionType)DbExpressionType.Column)
            {
                DbNamedValueExpression nv = (DbNamedValueExpression)left;
                DbColumnExpression c = (DbColumnExpression)right;
                left = new DbNamedValueExpression(nv.Name, c.DbType, nv.Value);
            }
            else if (b.Right.NodeType == (ExpressionType)DbExpressionType.NamedValue
             && b.Left.NodeType == (ExpressionType)DbExpressionType.Column)
            {
                DbNamedValueExpression nv = (DbNamedValueExpression)right;
                DbColumnExpression c = (DbColumnExpression)left;
                right = new DbNamedValueExpression(nv.Name, c.DbType, nv.Value);
            }
            return this.UpdateBinary(b, left, right, b.Conversion, b.IsLiftedToNull, b.Method);
        }

        protected override DbColumnAssignment VisitColumnAssignment(DbColumnAssignment ca)
        {
            ca = base.VisitColumnAssignment(ca);
            Expression expression = ca.Expression;
            DbNamedValueExpression nv = expression as DbNamedValueExpression;
            if (nv != null)
            {
                expression = new DbNamedValueExpression(nv.Name, ca.Column.DbType, nv.Value);
            }
            return this.UpdateColumnAssignment(ca, ca.Column, expression);
        }

        int iParam = 0;
        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value != null && !IsNumeric(c.Value.GetType())) {
                DbNamedValueExpression nv;
                if (!this.map.TryGetValue(c.Value, out nv)) { // re-use same name-value if same value
                    string name = "p" + (iParam++);
                    nv = new DbNamedValueExpression(name, this.language.TypeSystem.GetColumnType(c.Type), c);
                    this.map.Add(c.Value, nv);
                }
                return nv;
            }
            return c;
        }

        protected override Expression VisitParameter(ParameterExpression p) 
        {
            return this.GetNamedValue(p);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            m = (MemberExpression)base.VisitMemberAccess(m);
            DbNamedValueExpression nv = m.Expression as DbNamedValueExpression;
            if (nv != null) 
            {
                Expression x = Expression.MakeMemberAccess(nv.Value, m.Member);
                return GetNamedValue(x);
            }
            return m;
        }

        private Expression GetNamedValue(Expression e)
        {
            DbNamedValueExpression nv;
            HashedExpression he = new HashedExpression(e);
            if (!this.pmap.TryGetValue(he, out nv))
            {
                string name = "p" + (iParam++);
                nv = new DbNamedValueExpression(name, this.language.TypeSystem.GetColumnType(e.Type), e);
                this.pmap.Add(he, nv);
            }
            return nv;
        }

        private bool IsNumeric(Type type)
        {
            switch (Type.GetTypeCode(type)) {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        struct HashedExpression : IEquatable<HashedExpression>
        {
            Expression expression;
            int hashCode;

            public HashedExpression(Expression expression)
            {
                this.expression = expression;
                this.hashCode = Hasher.ComputeHash(expression);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is HashedExpression))
                    return false;
 	            return this.Equals((HashedExpression)obj);
            }

            public bool Equals(HashedExpression other)
            {
                return this.hashCode == other.hashCode &&
                    DbExpressionComparer.AreEqual(this.expression, other.expression);
            }

            public override int GetHashCode()
            {
                return this.hashCode;
            }

            class Hasher : DbExpressionVisitor
            {
                int hc;

                internal static int ComputeHash(Expression expression)
                {
                    var hasher = new Hasher();
                    hasher.Visit(expression);
                    return hasher.hc;
                }

                protected override Expression VisitConstant(ConstantExpression c)
                {
                    hc = hc + ((c.Value != null) ? c.Value.GetHashCode() : 0);
                    return c;
                }
            }
        }
    }
}