using System;
using System.Collections.Generic;
using System.IO;
using System.Web.SessionState;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Mvc.Validation;
using Radischevo.Wahha.Web.Mvc.Configurations;

namespace Radischevo.Wahha.Web.Mvc
{
    public class BindingContext : ControllerContext
    {
        #region Instance Fields
        private object _model;
        private Type _modelType;
        private string _modelName;
        private string _inputStream;
		private IValueProvider _valueProvider;
        private Predicate<string> _updateFilter;
        private ValidationErrorCollection _errors;
        private bool _fallbackToEmptyPrefix;
        private ModelMetadata _metadata;
        private ModelValidator _validator;
        #endregion

        #region Constructors
        public BindingContext(ControllerContext context, Type modelType, 
            string modelName, IValueProvider valueProvider, 
			Predicate<string> updateFilter, ValidationErrorCollection errors) 
            : base(context)
        {
            Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));
			Precondition.Require(valueProvider, () => Error.ArgumentNull("valueProvider"));

            _modelType = modelType;
            _modelName = modelName;
			_valueProvider = valueProvider;
            _updateFilter = updateFilter;
            _errors = errors;
        }

		public BindingContext(BindingContext parent)
			: base(parent)
		{
			_modelType = parent._modelType;
			_modelName = parent._modelName;
			_valueProvider = parent._valueProvider;
			_updateFilter = parent._updateFilter;
			_errors = parent._errors;
			_model = parent._model;
		}
        #endregion

        #region Instance Properties
		protected ModelMetadataProvider MetadataProvider
		{
			get
			{
				return Configuration.Instance
					.Models.MetadataProvider;
			}
		}

		protected ModelValidatorProvider ValidatorProvider
		{
			get
			{
				return Configuration.Instance
					.Models.ValidatorProvider;
			}
		}

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

        public ValidationErrorCollection Errors
        {
            get
            {
                if (_errors == null)
                    _errors = new ValidationErrorCollection();

                return _errors;
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

        public ModelMetadata Metadata 
        {
            get
            {
                if(_metadata == null)
                    _metadata = MetadataProvider.GetMetadata(ModelType);

                return _metadata;
            }
            set
            {
                _metadata = value;
            }
        }

        public ModelValidator Validator
        {
            get
            {
                if (_validator == null)
                    _validator = ValidatorProvider.GetValidator(ModelType);

                return _validator;
            }
            set
            {
                _validator = value;
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

        public virtual bool AllowMemberUpdate(string memberName)
        {
            return (_updateFilter == null || _updateFilter(memberName));
        }
        #endregion
    }
}
