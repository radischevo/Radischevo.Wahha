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
        private ParameterSource _source;
        private ValueDictionary _data;
        private Predicate<string> _updateFilter;
        private ValidationErrorCollection _errors;
        private bool _fallbackToEmptyPrefix;
        private ModelMetadata _metadata;
        private ModelValidator _validator;
        #endregion

        #region Constructors
        public BindingContext(ControllerContext context, Type modelType, 
            string modelName, ParameterSource source, 
            ValueDictionary bindingData, Predicate<string> updateFilter, 
            ValidationErrorCollection errors) 
            : base(context)
        {
            Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));

            _data = bindingData;
            _modelType = modelType;
            _modelName = modelName;
            _source = source;
            _updateFilter = updateFilter;
            _errors = errors;
        }

		public BindingContext(BindingContext parent)
			: base(parent)
		{
			_data = parent._data;
			_modelType = parent._modelType;
			_modelName = parent._modelName;
			_source = parent._source;
			_updateFilter = parent._updateFilter;
			_errors = parent._errors;
			_model = parent._model;
		}
        #endregion

        #region Instance Properties
		protected ModelMetadataProviderCollection MetadataProviders
		{
			get
			{
				return Configuration.Instance
					.Models.MetadataProviders;
			}
		}

		protected ModelValidatorProviderCollection ValidatorProviders
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

        public ParameterSource Source
        {
            get
            {
                return _source;
            }
        }

        public virtual ValueDictionary Data
        {
            get
            {
                if (_data == null)
                    _data = GetBindingData(this, _source);

                return _data;
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
                    _metadata = MetadataProviders
						.GetProvider(ModelType).GetMetadata(ModelType);

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
                    _validator = ValidatorProviders
						.GetProvider(ModelType).GetValidator(ModelType);

                return _validator;
            }
            set
            {
                _validator = value;
            }
        }
        #endregion

        #region Static Methods
        public static ValueDictionary GetBindingData(
            ControllerContext context, ParameterSource source)
        {
			Precondition.Require(context, () => Error.ArgumentNull("context"));

            ValueDictionary data = new ValueDictionary();
            HttpParameters parameters = context.Context.Request.Parameters;

            if ((source & ParameterSource.Header) == ParameterSource.Header)
                data.Merge(parameters.Headers);

            if ((source & ParameterSource.Cookie) == ParameterSource.Cookie)
                data.Merge(parameters.Cookies);

			if ((source & ParameterSource.Session) == ParameterSource.Session)
				data.Merge(new HttpSessionStateSet(context.Context.Session));

            if ((source & ParameterSource.QueryString) == ParameterSource.QueryString)
                data.Merge(parameters.QueryString);

            if ((source & ParameterSource.Form) == ParameterSource.Form)
                data.Merge(parameters.Form).Merge(new HttpPostedFileSet(context.Context.Request.Files));

            if ((source & ParameterSource.Url) == ParameterSource.Url)
                data.Merge((IValueSet)context.RouteData.Values);

            return data;
        }
        #endregion

        #region Instance Methods
        private string ReadInputStream()
        {
            if (_inputStream == null)
            {
                using (StreamReader reader = new StreamReader(
                    Context.Request.InputStream, 
                    Context.Request.ContentEncoding))
                {
                    _inputStream = reader.ReadToEnd();
                }
            }
            return _inputStream;
        }

        public bool TryGetValue(out object value)
        {
            value = null;
            if (String.IsNullOrEmpty(_modelName))
                return false;

            if ((_source & ParameterSource.InputStream) ==
                ParameterSource.InputStream)
            {
                value = ReadInputStream();
                return true;
            }
            return Data.TryGetValue(_modelName, out value);
        }

		public virtual bool Contains(string prefix)
		{
			foreach (string key in Data.Keys)
			{
				if (key != null)
				{
					if (prefix.Length == 0)
						return true;

					if (key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
					{
						if (key.Length == prefix.Length || key[prefix.Length] == '-')
							return true;
					}
				}
			}
			return false;
		}

        public virtual bool AllowMemberUpdate(string memberName)
        {
            return (_updateFilter == null || _updateFilter(memberName));
        }
        #endregion
    }
}
