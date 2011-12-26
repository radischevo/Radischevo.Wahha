using System;
using System.Configuration;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    [ConfigurationCollection(typeof(ViewConfigurationElement))]
    internal sealed class ViewConfigurationElementCollection : ConfigurationElementCollection, IConfigurator<ViewConfigurationSettings>
    {
		#region Instance Properties
		public ViewConfigurationElement this[int index]
        {
            get
            {
                return (ViewConfigurationElement)BaseGet(index);
            }
        }
		#endregion
		
		#region Static Methods
        private static IViewEngine CreateViewEngine(Type type, IValueSet settings)
        {
			Precondition.Require(type, () => Error.ArgumentNull("type"));
            if (!typeof(IViewEngine).IsAssignableFrom(type))
                throw Error.IncompatibleViewEngineType(type);

			IViewEngine engine = (IViewEngine)ServiceLocator.Instance.GetService(type);
            if (engine == null)
                throw Error.IncompatibleViewEngineType(type);

            engine.Init(settings);
            return engine;
        }
        #endregion
		
		#region Instance Methods
		protected override ConfigurationElement CreateNewElement()
        {
            return new ViewConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return new object();
        }
		
		public void Configure (ViewConfigurationSettings module)
		{			
            switch (Count)
            {
                case 0:
                    break;
                case 1: // один элемент
                    module.ViewEngines.Clear();
                    module.ViewEngines.Add(CreateViewEngine(
						Type.GetType(this[0].Type, false, true), this[0].Parameters));
                    break;
                default: // много элементов
                    module.ViewEngines.Clear();
                    foreach (ViewConfigurationElement elem in this)
                        module.ViewEngines.Add(CreateViewEngine(
							Type.GetType(elem.Type, false, true), elem.Parameters));
                    break;
            }
		}
		#endregion
    }
}
