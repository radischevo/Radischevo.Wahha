using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbInsertExpression : DbStatementExpression
    {
        #region Instance Fields
        private DbTableExpression _table;
        private ReadOnlyCollection<DbColumnAssignment> _assignments;
        #endregion

        #region Constructors
        public DbInsertExpression(DbTableExpression table, 
            IEnumerable<DbColumnAssignment> assignments)
            : base(DbExpressionType.Insert, typeof(int))
        {
            _table = table;
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
