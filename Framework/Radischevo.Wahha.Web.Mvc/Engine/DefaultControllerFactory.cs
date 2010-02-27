using System;

using Radischevo.Wahha.Web.Routing;
using Radischevo.Wahha.Core;

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
        #endregion

        #region Constructors
        public DefaultControllerFactory()
        {
        }
        #endregion

        #region Instance Properties
        internal ControllerBuilder Builder
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

        internal IBuildManager BuildManager
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
            
            return GetControllerInstance(GetControllerType(controllerName));
        }

        protected virtual void ReleaseController(IController controller)
        {
            IDisposable d = (controller as IDisposable);
            if (d != null)
                d.Dispose();
        }

        protected virtual IController GetControllerInstance(Type type)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));

            if (!typeof(IController).IsAssignableFrom(type))
                throw Error.InvalidControllerType(type.Name);

            if (type.GetConstructor(Type.EmptyTypes) == null)
                throw Error.ControllerMustHaveDefaultConstructor(type);
            
            return (IController)Activator.CreateInstance(type);
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
