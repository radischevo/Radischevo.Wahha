using System;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public class MvcHandler
        : IHttpHandler, IRequiresSessionState
    {
        #region Static Fields
        protected const string MvcHeaderName = "X-Wahha-Version";
        protected static readonly string MvcVersion;
        #endregion

        #region Instance Fields
        private ControllerBuilder _builder;
        private RequestContext _context;
        private bool _disableVersionHeader;
        #endregion

        #region Constructors
        static MvcHandler()
        {
            MvcVersion = GetMvcVersionString();
        }

        public MvcHandler(RequestContext context)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            _context = context;
        }
        #endregion

        #region Instance Properties
        protected ControllerBuilder Builder
        {
            get
            {
                if (_builder == null)
                    _builder = ControllerBuilder.Instance;
                
                return _builder;
            }
            set
            {
                _builder = value;
            }
        }

        public bool DisableVersionHeader
        {
            get
            {
                return _disableVersionHeader;
            }
            set
            {
                _disableVersionHeader = value;
            }
        }

        protected virtual bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public RequestContext Context
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
            }
        }
        #endregion

        #region Static Methods
        private static string GetMvcVersionString()
        {
            return new AssemblyName(typeof(MvcHandler).Assembly.FullName).Version.ToString(2);
        }
        #endregion

        #region Instance Methods
		protected virtual void AppendVersionHeader(HttpContextBase context)
		{
			if (!DisableVersionHeader)
				context.Response.AppendHeader(MvcHeaderName, MvcVersion);
		}

        protected virtual void ProcessRequest(HttpContextBase context)
        {
			AppendVersionHeader(context);
            string controllerName = _context.RouteData.GetRequiredValue<string>("controller");

            IControllerFactory factory = Builder.GetControllerFactory();
            IController controller = factory.CreateController(Context, controllerName);
            if (controller == null)
                throw Error.CouldNotCreateController(controllerName);

            try
            {
                controller.Execute(Context);
            }
            finally
            {
                factory.ReleaseController(controller);
            }
        }
		#endregion

		#region IHttpHandler Members
		private void ProcessRequestImpl(HttpContext context)
		{
			ProcessRequest(new HttpContextWrapper(context));
		}

		bool IHttpHandler.IsReusable
		{
			get
			{
				return IsReusable;
			}
		}

		void IHttpHandler.ProcessRequest(HttpContext context)
		{
			ProcessRequestImpl(context);
		}
		#endregion
	}
}
