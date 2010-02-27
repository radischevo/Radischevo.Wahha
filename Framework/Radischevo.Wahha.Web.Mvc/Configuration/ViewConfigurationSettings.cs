using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Configuration
{
    public sealed class ViewConfigurationSettings
    {
        #region Instance Fields
        private ViewEngineCollection _viewEngines;
        #endregion

        #region Constructors
        internal ViewConfigurationSettings()
        {
            _viewEngines = new ViewEngineCollection();
            _viewEngines.Add(new WebFormViewEngine());
        }
        #endregion

        #region Instance Properties
        public ViewEngineCollection ViewEngines
        {
            get
            {
                return _viewEngines;
            }
        }
        #endregion

        #region Static Methods
        private static IViewEngine CreateViewEngine(Type type, IValueSet settings)
        {
            if (type == null)
                return new WebFormViewEngine();

            if (!typeof(IViewEngine).IsAssignableFrom(type))
                throw Error.IncompatibleViewEngineType(type);

            IViewEngine engine = (IViewEngine)Activator.CreateInstance(type);
            if (engine == null)
                throw Error.IncompatibleViewEngineType(type);

            engine.Init(settings);
            return engine;
        }
        #endregion

        #region Instance Methods
        internal void Init(ViewConfigurationElementCollection element)
        {
            Precondition.Require(element, () => Error.ArgumentNull("element"));
            int count = element.Count;

            switch (count)
            {
                case 0:
                    break;
                case 1: // один элемент
                    _viewEngines.Clear();
                    _viewEngines.Add(CreateViewEngine(Type.GetType(element[0].Type, false, true), element[0].Parameters));
                    break;
                default: // много элементов
                    _viewEngines.Clear();
                    foreach (ViewConfigurationElement elem in element)
                        _viewEngines.Add(CreateViewEngine(Type.GetType(elem.Type, false, true), elem.Parameters));
                    break;
            }
        }
        #endregion
    }
}
