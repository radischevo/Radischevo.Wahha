using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ReflectedParameterBinding : ParameterBinding
    {
        #region Instance Fields
        private ParameterInfo _parameter;
        private string[] _include;
        private string[] _exclude;
        private Predicate<string> _memberFilter;
        private string _name;
        private IModelBinder _binder;
        private ParameterSource _source;
        private object _defaultValue;
        #endregion

        #region Constructors
        public ReflectedParameterBinding(ParameterInfo parameter)
        {
            _exclude = new string[0];
            _include = new string[0];
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

        public override IEnumerable<string> Include
        {
            get
            {
                return _include;
            }
        }

        public override IEnumerable<string> Exclude
        {
            get
            {
                return _exclude;
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
                string include = attribute.Include;
                string exclude = attribute.Exclude;

                _name = attribute.Name ?? _parameter.Name;
                _include = (String.IsNullOrEmpty(include)) ? new string[0] : 
                    include.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                _exclude = (String.IsNullOrEmpty(exclude)) ? new string[0] :
                    exclude.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                _source = attribute.Source;
                _defaultValue = attribute.Default;
                _memberFilter = s => BindAttribute.IsUpdateAllowed(s, _include, _exclude);
            }
        }

        public override Predicate<string> GetMemberFilter()
        {
            return _memberFilter;
        }
        #endregion
    }
}
