using System;
using System.Collections.Generic;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ReflectedParameterDescriptor : ParameterDescriptor
    {
        #region Instance Fields
        private ParameterInfo _parameter;
        private ActionDescriptor _action;
        private ParameterBinding _binding;
        #endregion

        #region Constructors
        public ReflectedParameterDescriptor(ParameterInfo parameter, ActionDescriptor action)
            : base()
        {
            Precondition.Require(parameter, Error.ArgumentNull("parameter"));
            Precondition.Require(action, Error.ArgumentNull("action"));

            _parameter = parameter;
            _action = action;
            _binding = new ReflectedParameterBinding(parameter);
        }
        #endregion

        #region Instance Properties
        public override ActionDescriptor Action
        {
            get
            {
                return _action;
            }
        }

        public override ParameterBinding Binding
        {
            get
            {
                return _binding;
            }
        }

        public override ParameterInfo Parameter
        {
            get
            {
                return _parameter;
            }
        }

        public override string Name
        {
            get
            {
                return _parameter.Name;
            }
        }

        public override Type Type
        {
            get
            {
                return _parameter.ParameterType;
            }
        }
        #endregion

        #region Instance Methods
        public override object[] GetCustomAttributes(bool inherit)
        {
            return _parameter.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type type, bool inherit)
        {
            return _parameter.GetCustomAttributes(type, inherit);
        }

        public override bool IsDefined(Type type, bool inherit)
        {
            return _parameter.IsDefined(type, inherit);
        }
        #endregion
    }
}
