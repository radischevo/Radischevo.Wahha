using System;
using System.Collections.Generic;
using System.IO;
using System.Web.SessionState;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Mvc.Validation;

namespace Radischevo.Wahha.Web.Mvc
{
    public sealed class BindingContext : ControllerContext
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
            Precondition.Require(modelType, Error.ArgumentNull("modelType"));

            _data = bindingData;
            _modelType = modelType;
            _modelName = modelName;
            _source = source;
            _updateFilter = updateFilter;
            _errors = errors;
            
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

        public ParameterSource Source
        {
            get
            {
                return _source;
            }
        }

        public ValueDictionary Data
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
                    _metadata = Configuration.Configuration.Instance.Models
                        .MetadataProviders.GetProvider(ModelType).GetMetadata(ModelType);

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
                    _validator = Configuration.Configuration.Instance.Models
                        .ValidatorProviders.GetProvider(ModelType).GetValidator(ModelType);

                return _validator;
            }
            set
            {
                _validator = value;
            }
        }
        #endregion

        #region Static Methods
        private static ValueDictionary GetBindingData(
            ControllerContext context, ParameterSource source)
        {
            ValueDictionary data = new ValueDictionary();
            HttpParameters parameters = context.Context.Request.Parameters;

            if ((source & ParameterSource.Header) == ParameterSource.Header)
                data.Merge(parameters.Headers);

            if ((source & ParameterSource.Cookie) == ParameterSource.Cookie)
                data.Merge(parameters.Cookies);

            if ((source & ParameterSource.Session) == ParameterSource.Session)
                MergeWithSessionState(context.Context.Session, data);

            if ((source & ParameterSource.QueryString) == ParameterSource.QueryString)
                data.Merge(parameters.QueryString);

            if ((source & ParameterSource.Form) == ParameterSource.Form)
                data.Merge(parameters.Form);

            if ((source & ParameterSource.Url) == ParameterSource.Url)
                data.Merge((IValueSet)context.RouteData.Values);

            return data;
        }

        private static void MergeWithSessionState(
            HttpSessionStateBase session, ValueDictionary values)
        {
            if (session == null)
                return;

            for (int i = 0; i < session.Keys.Count; ++i)
                values[session.Keys[i]] = session[i];
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

        public bool AllowMemberUpdate(string memberName)
        {
            return (_updateFilter == null || _updateFilter(memberName));
        }
        #endregion
    }
}
