using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing;
using Radischevo.Wahha.Web.Mvc.Configurations;

namespace Radischevo.Wahha.Web.Mvc
{
    public class DefaultControllerFactory
        : IControllerFactory
    {
        #region Static Fields
        private static ControllerTypeCache _staticTypeCache = new ControllerTypeCache();
        #endregion

        #region Instance Fields
        private IBuildManager _buildManager;
        private ControllerBuilder _builder;
        private ControllerTypeCache _instanceTypeCache;
		private IControllerActivator _activator;
        #endregion

        #region Constructors
		public DefaultControllerFactory()
			: this(new DefaultControllerActivator())
		{
		}

        public DefaultControllerFactory(IControllerActivator activator)
        {
			Precondition.Require(activator, () => Error.ArgumentNull("activator"));
			_activator = activator;
        }
        #endregion

        #region Instance Properties
        public IBuildManager BuildManager
        {
            get
            {
                if (_buildManager == null)
                    _buildManager = new BuildManagerWrapper();

                return _buildManager;
            }
            set
            {
                _buildManager = value;
            }
        }

		public IControllerActivator Activator
		{
			get
			{
				return _activator;
			}
		}

        internal ControllerTypeCache TypeCache
        {
            get
            {
                if (_instanceTypeCache == null)
                    return _staticTypeCache;

                return _instanceTypeCache;
            }
            set
            {
                _instanceTypeCache = value;
            }
        }
        #endregion

        #region Instance Methods
        protected virtual void Init(IValueSet settings)
        {
        }

        protected virtual IController CreateController(
            RequestContext context, string controllerName)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            Precondition.Defined(controllerName, () => Error.InvalidArgument("controllerName"));
            
            return GetControllerInstance(context, GetControllerType(controllerName));
        }

        protected virtual void ReleaseController(IController controller)
        {
            IDisposable d = (controller as IDisposable);
            if (d != null)
                d.Dispose();
        }

        protected virtual IController GetControllerInstance(RequestContext context, Type type)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));

            if (!typeof(IController).IsAssignableFrom(type))
                throw Error.InvalidControllerType(type.Name);

			return Activator.Create(context, type);
        }

        protected virtual Type GetControllerType(string controllerName)
        {
            Precondition.Defined(controllerName, () => Error.InvalidArgument("controllerName"));

            TypeCache.EnsureInitialized(BuildManager);
            
            Type type;
            if (!TypeCache.TryGetController(controllerName, out type))
                if (!TypeCache.TryGetController(
                    String.Concat(controllerName, "Controller"), out type))
                    throw Error.CouldNotCreateController(controllerName);
            
            return type;          
        }

        void IControllerFactory.Init(IValueSet settings)
        {
            Init(settings);
        }

        IController IControllerFactory.CreateController(
            RequestContext context, string controllerName)
        {
            return CreateController(context, controllerName);
        }

        void IControllerFactory.ReleaseController(IController controller)
        {
            ReleaseController(controller);
        }
        #endregion
    }
}
