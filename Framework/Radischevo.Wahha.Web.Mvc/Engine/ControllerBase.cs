using System;
using System.Security.Principal;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Core.Async;
using Radischevo.Wahha.Data.Caching;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    public abstract class ControllerBase : IController, IDisposable
    {
        #region Instance Fields
		private readonly SingleEntryGate _executeGate;
        private ControllerContext _context;
        private ViewDataDictionary _viewData;
        private TempDataDictionary _tempData;
        private ValidationErrorCollection _errors;
        private CacheProvider _cache;
        private bool _validateRequest;
        #endregion

		#region Constructors
		protected ControllerBase()
		{
			_executeGate = new SingleEntryGate();
		}
		#endregion

		#region Instance Properties
		public ControllerContext Context
        {
            get
            {
                return _context;
            }
        }

        /// <summary>
        /// Encapsulates all HTTP-specific information 
        /// about an individual HTTP request.
        /// </summary>
        public HttpContextBase HttpContext
        {
            get
            {
                return (_context == null) ? null : _context.Context;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.HttpRequest"/> 
        /// object for the current HTTP request.
        /// </summary>
        public HttpRequestBase Request
        {
            get
            {
                return (HttpContext == null) ?
                    null : HttpContext.Request;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.HttpResponse"/> 
        /// object for the current HTTP request.
        /// </summary>
        public HttpResponseBase Response
        {
            get
            {
                return (HttpContext == null) ?
                    null : HttpContext.Response;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Radischevo.Wahha.Data.Caching.CacheProvider"/> 
        /// for the current controller.
        /// </summary>
        public CacheProvider Cache
        {
            get
            {
                if (_cache == null)
                    _cache = CacheProvider.Instance;

                return _cache;
            }
            set
            {
                _cache = value;
            }
        }

        /// <summary>
        /// Returns the <see cref="Radischevo.Wahha.Web.Routing.RouteData"/> 
        /// for the current request.
        /// </summary>
        public RouteData RouteData
        {
            get
            {
                return (_context == null) ?
                    null : _context.RouteData;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.HttpServerUtility"/> 
        /// object that provides methods used in processing Web requests.
        /// </summary>
        public HttpServerUtilityBase Server
        {
            get
            {
                return (HttpContext == null) ?
                    null : HttpContext.Server;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.HttpSessionState"/> 
        /// object for the current HTTP request.
        /// </summary>
        public HttpSessionStateBase Session
        {
            get
            {
                return (HttpContext == null) ? null : HttpContext.Session;
            }
        }

        /// <summary>
        /// Gets data associated with this request which 
        /// only lives for one request.
        /// </summary>
        public TempDataDictionary TempData
        {
            get
            {
				if(_tempData == null)
					_tempData = new TempDataDictionary();

                return _tempData;
            }
        }

        /// <summary>
        /// Gets the view data supplied to the view.
        /// </summary>
        public ViewDataDictionary ViewData
        {
            get
            {
                if (_viewData == null)
                    _viewData = new ViewDataDictionary();

                return _viewData;
            }
        }

        public ValidationErrorCollection Errors
        {
            get
            {
				if (_errors == null)
					_errors = new ValidationErrorCollection();

                return _errors;
            }
        }

        /// <summary>
        /// Gets the combined HTTP value collection 
        /// for the current <see cref="Radischevo.Wahha.Web.Routing.RequestContext"/>.
        /// </summary>
        public HttpParameters HttpParameters
        {
            get
            {
                return (Request == null) ? null : Request.Parameters;
            }
        }

        /// <summary>
        /// Gets the security information for the current HTTP request.
        /// </summary>
        public IPrincipal User
        {
            get
            {
                return (HttpContext == null) ? null : HttpContext.User;
            }
        }

        public bool ValidateRequest
        {
            get
            {
                return _validateRequest;
            }
            set
            {
                _validateRequest = value;
            }
        }
        #endregion

        #region Instance Methods
		protected void VerifyExecuteCalledOnce()
		{
			if (!_executeGate.TryEnter())
				throw Error.ControllerCannotHandleMultipleRequests(GetType());
		}

        protected void Execute(RequestContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));

			VerifyExecuteCalledOnce();
            Initialize(context);
            ProcessRequest();
        }

        protected virtual void Initialize(RequestContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            _context = new ControllerContext(context, this);
        }

        protected virtual ChildContextOperator InitializeChildRequest(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
			_tempData = context.Controller.TempData;
			Initialize(context);

			return new ChildContextOperator(context);
        }

        protected abstract void ProcessRequest();

        protected void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion

        #region IController Members
        void IController.Execute(RequestContext context)
        {
            Execute(context);
        }
        #endregion

        #region IDisposable Members
        void IDisposable.Dispose()
        {
            Dispose();
        }
        #endregion
    }
}
