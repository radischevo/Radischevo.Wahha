using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.UI;
using Radischevo.Wahha.Web.Mvc.Validation;
using Radischevo.Wahha.Web.Mvc.Configurations;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ViewDataDictionary : Dictionary<string, object>
    {
        #region Instance Fields
        private object _model;
        private ViewDataEvaluator _evaluator;
        private TemplateDescriptor _template;
        private ModelMetadata _metadata;
        private ModelValidator _validator;
        private IDictionary<string, ViewDataDictionary> _subDataItems;
        #endregion

        #region Constructors
        public ViewDataDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
            _subDataItems = new Dictionary<string, ViewDataDictionary>(StringComparer.OrdinalIgnoreCase);
            _evaluator = new ViewDataEvaluator(this);
        }

        public ViewDataDictionary(object model)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            _subDataItems = new Dictionary<string, ViewDataDictionary>(StringComparer.OrdinalIgnoreCase);
            _model = model;
            _evaluator = new ViewDataEvaluator(this);
        }

        public ViewDataDictionary(object model, object values)
            : base(new ValueDictionary(values), StringComparer.OrdinalIgnoreCase)
        {
            _subDataItems = new Dictionary<string, ViewDataDictionary>(StringComparer.OrdinalIgnoreCase);
            _model = model;
            _evaluator = new ViewDataEvaluator(this);
        }

        public ViewDataDictionary(ViewDataDictionary dictionary)
            : base(dictionary, StringComparer.OrdinalIgnoreCase)
        {
            _subDataItems = new Dictionary<string, ViewDataDictionary>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, ViewDataDictionary> item 
                in dictionary.SubDataItems)
                _subDataItems.Add(item);
            
            _evaluator = new ViewDataEvaluator(this);
            _model = dictionary.Model;
            _template = dictionary._template;
			_metadata = dictionary._metadata;
			_validator = dictionary._validator;
        }
        #endregion

        #region Instance Properties
		protected virtual ModelMetadataProviderCollection MetadataProviders
		{
			get
			{
				return Configuration.Instance
					.Models.MetadataProviders;
			}
		}

		protected virtual ModelValidatorProviderCollection ValidatorProviders
		{
			get
			{
				return Configuration.Instance
					.Models.ValidatorProviders;
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
                SetModel(value);
            }
        }

        public virtual ModelMetadata Metadata
        {
            get
            {
                if (_metadata == null && _model != null)
                {
                    Type modelType = _model.GetType();
                    _metadata = MetadataProviders.GetProvider(modelType).GetMetadata(modelType);
                }
                return _metadata;
            }
            set
            {
                _metadata = value;
            }
        }

        public virtual ModelValidator Validator
        {
            get
            {
                if (_validator == null && _model != null)
                {
                    Type modelType = _model.GetType();
                    _validator = ValidatorProviders.GetProvider(modelType).GetValidator(modelType);
                }
                return _validator;
            }
            set
            {
                _validator = value;
            }
        }

        public IDictionary<string, ViewDataDictionary> SubDataItems
        {
            get
            {
                return _subDataItems;
            }
        }

        public TemplateDescriptor Template
        {
            get
            {
                if (_template == null)
                    _template = new TemplateDescriptor((Model == null) ? 
                        typeof(object) : Model.GetType());

                return _template;
            }
            set
            {
                _template = value;
            }
        }

        public new object this[string key]
        {
            get
            {
                object value;
                if (base.TryGetValue(key, out value))
                    return value;

                return null;
            }
            set
            {
                base[key] = value;
            }
        }
        #endregion

        #region Instance Methods
        public TValue GetValue<TValue>(string key)
        {
            return GetValue<TValue>(key, default(TValue));
        }

        public TValue GetValue<TValue>(string key, TValue defaultValue)
        {
            object value;
            if (!base.TryGetValue(key, out value))
                return defaultValue;

            return Converter.ChangeType<TValue>(value, defaultValue);
        }

        public ViewDataInfo GetViewDataInfo(string expression)
        {
            Precondition.Defined(expression, () => Error.ArgumentNull("expression"));
            return _evaluator.Eval(expression);
        }

        protected virtual void SetModel(object model)
        {
            _metadata = null;
            _model = model;
        }
        #endregion
    }
}
