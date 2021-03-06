﻿using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.Configurations;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ControllerBuilder
    {
        #region Static Fields
        private static ControllerBuilder _instance = new ControllerBuilder();
        #endregion

        #region Instance Fields
        private Func<IControllerFactory> _factoryThunk;
        #endregion

        #region Constructors
        public ControllerBuilder()
        {
            IControllerFactory configuredFactory = 
                Configuration.Instance.Controllers.Factory;

            if (configuredFactory == null)
                SetDefaultControllerFactory();
            else
                SetControllerFactory(configuredFactory);
        }
        #endregion

        #region Static Properties
        public static ControllerBuilder Instance
        {
            get
            {
                return ControllerBuilder._instance;
            }
        }
        #endregion

        #region Instance Methods
        public IControllerFactory GetControllerFactory()
        {
            return _factoryThunk();
        }

        private void SetDefaultControllerFactory()
        {
			SetControllerFactory(new DefaultControllerFactory());
        }

        public void SetControllerFactory(IControllerFactory factory)
        {
            Precondition.Require(factory, () => Error.ArgumentNull("factory"));
            _factoryThunk = () => factory;
        }

        public void SetControllerFactory(Type factoryType)
        {
            Type type = factoryType;
            Precondition.Require(type, () => Error.ArgumentNull("factoryType"));
            if (!typeof(IControllerFactory).IsAssignableFrom(type))
                throw Error.IncompatibleControllerFactoryType(type);

            _factoryThunk = () => (IControllerFactory)ServiceLocator.Instance.GetService(type);
        }
        #endregion
    }
}
