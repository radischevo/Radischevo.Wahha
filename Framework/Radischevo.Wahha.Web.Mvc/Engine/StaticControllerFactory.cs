using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing;

namespace Radischevo.Wahha.Web.Mvc
{
    public class StaticControllerFactory
        : IControllerFactory
    {
        #region Instance Fields
        private RequestContext _context;
        #endregion

        #region Constructors
        public StaticControllerFactory()
        {
        }
        #endregion

        #region Instance Properties
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

        #region Instance Methods
        protected virtual void Init(IValueSet settings)
        {
        }

        protected virtual IController CreateController(
            RequestContext context, string controllerName)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            Precondition.Defined(controllerName, () => Error.InvalidArgument("controllerName"));

            _context = context;
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

            Type type;
            if (!Configuration.Configuration.Instance
                .Controllers.Mappings.TryGetValue(controllerName, out type))
                if (!Configuration.Configuration.Instance
                    .Controllers.Mappings.TryGetValue(
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
