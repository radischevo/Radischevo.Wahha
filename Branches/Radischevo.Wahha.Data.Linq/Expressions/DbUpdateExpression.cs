using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbUpdateExpression : DbStatementExpression
    {
        #region Instance Fields
        private DbTableExpression _table;
        private Expression _where;
        private ReadOnlyCollection<DbColumnAssignment> _assignments;
        #endregion

        #region Constructors
        public DbUpdateExpression(DbTableExpression table, 
            Expression where, IEnumerable<DbColumnAssignment> assignments)
            : base(DbExpressionType.Update, typeof(int))
        {
            _table = table;
            _where = where;
            _assignments = assignments.AsReadOnly();
        }
        #endregion

        #region Instance Properties
        public DbTableExpression Table
        {
            get 
            { 
                return _table; 
            }
        }

        public Expression Where
        {
            get 
            { 
                return _where; 
            }
        }

        public ReadOnlyCollection<DbColumnAssignment> Assignments
        {
            get 
            { 
                return _assignments; 
            }
        }
        #endregion
    }
}
