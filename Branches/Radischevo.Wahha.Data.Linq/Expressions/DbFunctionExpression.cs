using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

using Jeltofiol.Wahha.Core;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class DbFunctionExpression : DbExpression
    {
        #region Instance Fields
        private string _name;
        private ReadOnlyCollection<Expression> _arguments;
        #endregion

        #region Constructors
        public DbFunctionExpression(Type type, string name, 
            IEnumerable<Expression> arguments)
            : base(DbExpressionType.Function, type)
        {
            _name = name;
            _arguments = arguments.AsReadOnly();
        }
        #endregion

        #region Instance Properties
        public string Name
        {
            get 
            {
                return _name; 
            }
        }

        public ReadOnlyCollection<Expression> Arguments
        {
            get 
            { 
                return _arguments; 
            }
        }
        #endregion
    }
}
