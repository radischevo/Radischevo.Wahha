using System;
using System.Linq;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ReflectedParameterBinding : ParameterBinding
	{
		#region Instance Fields
		private ParameterInfo _parameter;
        private string _name;
        private IModelBinder _binder;
        private ParameterSource _source;
        private object _defaultValue;
        #endregion

        #region Constructors
        public ReflectedParameterBinding(ParameterInfo parameter)
        {
            _parameter = parameter;
            _name = parameter.Name;
            
            InitFromAttributes();
        }
        #endregion

        #region Instance Properties
        public override string Name
        {
            get
            {
                return _name;
            }
        }

        public override IModelBinder Binder
        {
            get
            {
                return _binder;
            }
        }

        public override object DefaultValue
        {
            get
            {
                return _defaultValue;
            }
        }

        public override ParameterSource Source
        {
            get
            {
                return _source;
            }
        }
        #endregion

        #region Static Methods
        private static IModelBinder GetModelBinder(ParameterInfo parameter)
        {
            ModelBinderAttribute[] attrs = parameter.GetCustomAttributes<ModelBinderAttribute>().ToArray();
            switch (attrs.Length)
            {
                case 0:
                    return null;
                case 1:
                    return attrs[0].GetBinder();
                default:
                    throw Error.MultipleModelBinderAttributes(parameter);
            }
        }
        #endregion

        #region Instance Methods
        private void InitFromAttributes()
        {
            _binder = GetModelBinder(_parameter);
            _source = ParameterSource.Default;

            BindAttribute attribute = (BindAttribute)Attribute.GetCustomAttribute(_parameter, typeof(BindAttribute));
            if (attribute != null)
            {
                _name = attribute.Name ?? _parameter.Name;
                _source = ParameterSource.FromString(attribute.Source);
                _defaultValue = attribute.Default;
            }
        }
        #endregion
    }
}
