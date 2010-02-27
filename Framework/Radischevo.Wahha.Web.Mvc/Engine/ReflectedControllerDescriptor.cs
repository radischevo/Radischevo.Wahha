using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ReflectedControllerDescriptor : ControllerDescriptor
    {
        #region Instance Fields
        private Type _controllerType;
        private ActionMethodSelector _selector;
        #endregion

        #region Constructors
        public ReflectedControllerDescriptor(Type controllerType)
            : base()
        {
            Precondition.Require(controllerType, () => Error.ArgumentNull("controllerType"));
            
            _controllerType = controllerType;
            _selector = new ActionMethodSelector(_controllerType);
        }
        #endregion

        #region Instance Properties
        public sealed override Type Type
        {
            get
            {
                return _controllerType;
            }
        }
        #endregion

        #region Instance Methods
        public override ActionDescriptor FindAction(ControllerContext context, string actionName)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            Precondition.Defined(actionName, () => Error.ArgumentNull("actionName"));
            MethodInfo method = _selector.GetActionMethod(context, actionName);
            
            if (method == null)
                return null;
            
            return new ReflectedActionDescriptor(method, actionName, this);
        }

        public override IEnumerable<ActionDescriptor> GetCanonicalActions()
        {
            return _selector.Methods.Select(a => ReflectedActionDescriptor.CreateDescriptor(a, a.Name, this)).ToArray();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _controllerType.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type type, bool inherit)
        {
            return _controllerType.GetCustomAttributes(type, inherit);
        }

        public override bool IsDefined(Type type, bool inherit)
        {
            return _controllerType.IsDefined(type, inherit);
        }
        #endregion
    }
}
