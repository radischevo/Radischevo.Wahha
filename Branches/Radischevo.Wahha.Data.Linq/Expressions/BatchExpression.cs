using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Jeltofiol.Wahha.Data.Linq.Expressions
{
    public class BatchExpression : Expression
    {
        #region Instance Fields
        private Expression _input;
        private LambdaExpression _operation;
        private Expression _size;
        private Expression _stream;
        #endregion

        #region Constructors
        public BatchExpression(Expression input, LambdaExpression operation, 
            Expression size, Expression stream)
            : base((ExpressionType)DbExpressionType.Batch, 
            typeof(IEnumerable<>).MakeGenericType(operation.Body.Type))
        {
            _input = input;
            _operation = operation;
            _size = size;
            _stream = stream;
        }
        #endregion

        #region Instance Properties
        public Expression Input
        {
            get 
            { 
                return _input; 
            }
        }

        public LambdaExpression Operation
        {
            get 
            { 
                return _operation; 
            }
        }

        public Expression Size
        {
            get 
            { 
                return _size; 
            }
        }

        public Expression Stream
        {
            get 
            { 
                return _stream; 
            }
        }
        #endregion
    }
}
