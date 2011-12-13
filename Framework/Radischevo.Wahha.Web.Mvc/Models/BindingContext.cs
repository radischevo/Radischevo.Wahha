using System;
using System.Collections.Generic;
using System.IO;
using System.Web.SessionState;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.Configurations;

namespace Radischevo.Wahha.Web.Mvc
{
    public class BindingContext : ControllerContext
    {
        #region Instance Fields
        private object _model;
        private Type _modelType;
        private string _modelName;
		private IValueProvider _valueProvider;
        private ModelStateCollection _modelState;
        private bool _fallbackToEmptyPrefix;
        #endregion

        #region Constructors
        public BindingContext(ControllerContext context, Type modelType, 
            string modelName, IValueProvider valueProvider, 
			ModelStateCollection modelState) 
            : base(context)
        {
            Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));
			Precondition.Require(valueProvider, () => Error.ArgumentNull("valueProvider"));

            _modelType = modelType;
            _modelName = modelName;
			_valueProvider = valueProvider;
            _modelState = modelState;
        }

		public BindingContext(BindingContext parent)
			: base(parent)
		{
			_modelType = parent._modelType;
			_modelName = parent._modelName;
			_valueProvider = parent._valueProvider;
			_modelState = parent._modelState;
			_model = parent._model;
		}
        #endregion

        #region Instance Properties
        public object Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
            }
        }
        
        public Type ModelType
        {
            get
            {
                return _modelType;
            }
        }

        public string ModelName
        {
            get
            {
                return _modelName;
            }
            set
            {
                _modelName = value;
            }
        }

        public IValueProvider ValueProvider
        {
            get
            {
				return _valueProvider;
            }
        }

        public ModelStateCollection ModelState
        {
            get
            {
                if (_modelState == null)
                    _modelState = new ModelStateCollection();

                return _modelState;
            }
        }

        public bool FallbackToEmptyPrefix
        {
            get
            {
                return _fallbackToEmptyPrefix;
            }
            set
            {
                _fallbackToEmptyPrefix = value;
            }
        }
        #endregion

		#region Instance Methods
		public ValueProviderResult GetValue()
		{
			if (String.IsNullOrEmpty(_modelName))
				return null;

			return ValueProvider.GetValue(_modelName);
		}

        public bool TryGetValue(out ValueProviderResult value)
        {
			return ((value = GetValue()) != null);
        }

		public virtual bool Contains(string prefix)
		{
			return ValueProvider.Contains(prefix);
		}
        #endregion
    }
}
