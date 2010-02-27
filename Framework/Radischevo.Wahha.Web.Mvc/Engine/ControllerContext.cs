using System;
using System.Collections.Generic;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Encapsulates information about an HTTP request that matches a defined 
    /// <see cref="Radischevo.Wahha.Web.Routing.RouteBase">Route</see> and 
    /// <see cref="Radischevo.Wahha.Web.Mvc.ControllerBase">Controller</see>. 
    /// </summary>
    public class ControllerContext : RequestContext
	{
		#region Constants
		public const string ParentContextKey = "ParentActionContext";
		#endregion

		#region Instance Fields
		private ControllerBase _controller;
        private ValueDictionary _parameters;
        #endregion

        #region Constructors
        protected ControllerContext(ControllerContext context)
            : this(context, ControllerContext.GetControllerContext(context).Controller)
        {
            _parameters = new ValueDictionary((IDictionary<string, object>)context.Parameters);
        }

        public ControllerContext(RequestContext context, ControllerBase controller)
            : this(ControllerContext.GetRequestContext(context).Context, 
            ControllerContext.GetRequestContext(context).RouteData, controller)
        {
        }

        public ControllerContext(HttpContextBase context, 
            RouteData routeData, ControllerBase controller)
            : base(context, routeData)
        {
            Precondition.Require(controller, () => Error.ArgumentNull("controller"));
            _controller = controller;
        }
        #endregion

        #region Instance Properties
        public ControllerBase Controller
        {
            get
            {
                return _controller;
            }
            set
            {
                _controller = value;
            }
        }

		public virtual bool IsChild
		{
			get
			{
				RouteData routeData = RouteData;
				if (routeData == null)
					return false;
				
				return routeData.Tokens.ContainsKey(ParentContextKey);
			}
		}

		public virtual ControllerContext Parent
		{
			get
			{
				return (RouteData.Tokens[ParentContextKey] as ControllerContext);
			}
		}

        public ValueDictionary Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new ValueDictionary();

                return _parameters;
            }
        }
        #endregion

        #region Static Methods
        internal static RequestContext GetRequestContext(RequestContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            return context;
        }

        internal static ControllerContext GetControllerContext(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            return context;
        }
        #endregion
    }
}
