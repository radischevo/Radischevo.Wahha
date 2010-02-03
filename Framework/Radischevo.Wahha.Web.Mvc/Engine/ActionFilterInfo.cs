using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
    public sealed class ActionFilterInfo
    {
        #region Instance Fields
        private IList<IActionFilter> _actionFilters;
        private IList<IResultFilter> _resultFilters;
        private IList<IAuthorizationFilter> _authorizationFilters;
        private IList<IExceptionFilter> _exceptionFilters;
        #endregion

        #region Constructors
        internal ActionFilterInfo()
        {
        }

        internal ActionFilterInfo(IList<IActionFilter> actionFilters, 
            IList<IResultFilter> resultFilters, 
            IList<IAuthorizationFilter> authorizationFilters, 
            IList<IExceptionFilter> exceptionFilters)
        {
            _actionFilters = actionFilters;
            _resultFilters = resultFilters;
            _authorizationFilters = authorizationFilters;
            _exceptionFilters = exceptionFilters;
        }
        #endregion

        #region Instance Properties
        public IList<IActionFilter> ActionFilters
        {
            get
            {
                return GetListOrEmpty<IActionFilter>(ref _actionFilters);
            }
        }

        public IList<IResultFilter> ResultFilters
        {
            get
            {
                return GetListOrEmpty<IResultFilter>(ref _resultFilters);
            }
        }

        public IList<IAuthorizationFilter> AuthorizationFilters
        {
            get
            {
                return GetListOrEmpty<IAuthorizationFilter>(ref _authorizationFilters);
            }
        }

        public IList<IExceptionFilter> ExceptionFilters
        {
            get
            {
                return GetListOrEmpty<IExceptionFilter>(ref _exceptionFilters);
            }
        }
        #endregion

        #region Static Methods
        private static IList<T> GetListOrEmpty<T>(ref IList<T> list)
        {
            if (list == null)
                return new List<T>();

            return list;
        }
        #endregion
    }
}
