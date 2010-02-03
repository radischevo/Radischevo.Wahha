using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Radischevo.Wahha.Core.Expressions
{
    internal class ParserContext
    {
        #region Static Fields
        public static readonly ParameterExpression HoistedValuesParameter =
            Expression.Parameter(typeof(object[]), "hoistedValues");
        #endregion

        #region Instance Fields
        private ExpressionFingerprint _fingerprint;
        private List<object> _hoistedValues;
        private ParameterExpression _instance;
        #endregion

        #region Constructors
        public ParserContext()
        {
            _hoistedValues = new List<object>();
        }
        #endregion

        #region Instance Properties
        public ExpressionFingerprint Fingerprint
        {
            get
            {
                return _fingerprint;
            }
            set
            {
                _fingerprint = value;
            }
        }

        public List<object> HoistedValues
        {
            get
            {
                return _hoistedValues;
            }
        }

        public ParameterExpression Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        #endregion
    }
}
