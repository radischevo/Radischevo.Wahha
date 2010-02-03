using System;
using System.Reflection;
using System.Web;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class ErrorHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        #region Instance Fields
        private Type _type = typeof(Exception);
        private string _view;
        #endregion

        #region Constructors
        public ErrorHandlerAttribute() 
            : base()
        {
        }

        public ErrorHandlerAttribute(string viewName)
            : this(viewName, typeof(Exception))
        {
        }

        public ErrorHandlerAttribute(Type exceptionType)
            : this(null, exceptionType)
        {
        }

        public ErrorHandlerAttribute(string viewName, Type exceptionType) 
            : base()
        {
            _view = viewName;
            Type = exceptionType;
        }
        #endregion

        #region Instance Properties
        public Type Type
        {
            get
            {
                return _type;
            }
            set
            {
                Precondition.Require(value, Error.ArgumentNull("value"));
                if (!typeof(Exception).IsAssignableFrom(value))
                    throw Error.InvalidArgument("value");

                _type = value;
            }
        }

        public string View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
            }
        }
        #endregion

        #region IExceptionFilter Members
        public void OnException(ExceptionContext context)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            Controller controller = (context.Controller as Controller);
            if (controller == null || context.Handled || !context.Context.IsCustomErrorEnabled)
                return;

			if (context.IsChild)
				return;

            Exception exception = context.Exception;
            if (new HttpException(null, exception).GetHttpCode() != 500)
                return;

            if (exception is TargetInvocationException)
                exception = exception.InnerException;

            if (!_type.IsInstanceOfType(exception))
                return;
            
            ErrorHandlerInfo model = new ErrorHandlerInfo(context.Exception);
            context.Handled = true;
            context.Context.Response.Clear();
            context.Context.Response.StatusCode = 500;

            if (String.IsNullOrEmpty(_view))
                context.Result = EmptyResult.Instance;
            else
                context.Result = new ViewResult() {
                    TempData = controller.TempData,
                    ViewData = new ViewDataDictionary<ErrorHandlerInfo>(model), 
                    ViewName = View };
        }
        #endregion
    }
}
