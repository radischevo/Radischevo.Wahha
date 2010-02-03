using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbDeclaration : DbStatementExpression
    {
        #region Instance Fields
        private ReadOnlyCollection<DbVariableDeclaration> _variables;
        private DbSelectExpression _source;
        #endregion

        #region Constructors
        public DbDeclaration(IEnumerable<DbVariableDeclaration> variables, 
            DbSelectExpression source)
            : base(DbExpressionType.Declaration, typeof(void))
        {
            _variables = variables.AsReadOnly();
            _source = source;
        }
        #endregion

        #region Instance Properties
        public ReadOnlyCollection<DbVariableDeclaration> Variables
        {
            get
            {
                return _variables;
            }
        }

        public DbSelectExpression Source
        {
            get
            {
                return _source;
            }
        }
        #endregion
    }
}
