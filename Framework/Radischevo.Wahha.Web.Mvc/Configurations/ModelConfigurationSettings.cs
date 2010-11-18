using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Mvc.Validation;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    public sealed class ModelConfigurationSettings
    {
        #region Instance Fields
        private ModelBinderCollection _binders;
		private ValueProviderFactoryCollection _valueProviders;
		private ModelMetadataProvider _metadataProvider;
		private ModelValidatorProvider _validatorProvider;
        #endregion

        #region Constructors
        internal ModelConfigurationSettings()
        {
            _binders = new ModelBinderCollection();
			_valueProviders = new ValueProviderFactoryCollection();

			InitDefaultBinders();
			InitDefaultBinderProviders();
			InitDefaultValueProviders();
        }
        #endregion

        #region Instance Properties
        public ModelBinderCollection Binders
        {
            get
            {
                return _binders;
            }
        }

		public ValueProviderFactoryCollection ValueProviders
		{
			get
			{
				return _valueProviders;
			}
		}

		public ModelMetadataProvider MetadataProvider
		{
			get
			{
				if (_metadataProvider == null)
					_metadataProvider = new EmptyModelMetadataProvider();

				return _metadataProvider;
			}
			set
			{
				_metadataProvider = value;
			}
		}

		public ModelValidatorProvider ValidatorProvider
		{
			get
			{
				if (_validatorProvider == null)
					_validatorProvider = new EmptyModelValidatorProvider();

				return _validatorProvider;
			}
			set
			{
				_validatorProvider = value;
			}
		}
        #endregion

        #region Static Methods
        private static IModelBinder CreateModelBinder(Type type)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));
            if (!typeof(IModelBinder).IsAssignableFrom(type))
                throw Error.IncompatibleModelBinderType(type);

			return (IModelBinder)ServiceLocator.Instance.GetService(type);
        }

		private static IValueProviderFactory CreateValueProviderFactory(Type type)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			if (!typeof(IValueProviderFactory).IsAssignableFrom(type))
				throw Error.IncompatibleModelBinderType(type); // TODO: Кинуть нормальный еррор

			return (IValueProviderFactory)ServiceLocator.Instance.GetService(type);
		}

        private static ModelMetadataProvider CreateMetadataProvider(Type type)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));
            if (!typeof(ModelMetadataProvider).IsAssignableFrom(type))
                throw Error.IncompatibleModelMetadataProviderType(type);

			ModelMetadataProvider p = (ModelMetadataProvider)ServiceLocator.Instance.GetService(type);
            p.Init();

            return p;
        }

        private static ModelValidatorProvider CreateValidatorProvider(Type type)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));
            if (!typeof(ModelValidatorProvider).IsAssignableFrom(type))
                throw Error.IncompatibleModelValidatorProviderType(type);

			ModelValidatorProvider p = (ModelValidatorProvider)ServiceLocator.Instance.GetService(type);
            p.Init();

            return p;
        }
        #endregion

        #region Instance Methods
		private void InitDefaultBinders()
		{
			_binders.Add(typeof(HttpPostedFileBase), new HttpPostedFileBinder());
			_binders.Add(typeof(HttpPostedFileBase[]), new HttpPostedFileCollectionBinder());
			_binders.Add(typeof(IEnumerable<HttpPostedFileBase>), new HttpPostedFileCollectionBinder());
			_binders.Add(typeof(IList<HttpPostedFileBase>), new HttpPostedFileCollectionBinder());
			_binders.Add(typeof(List<HttpPostedFileBase>), new HttpPostedFileCollectionBinder());
			_binders.Add(typeof(ICollection<HttpPostedFileBase>), new HttpPostedFileCollectionBinder());
			_binders.Add(typeof(Collection<HttpPostedFileBase>), new HttpPostedFileCollectionBinder());
			_binders.Add(typeof(FormCollection), new FormCollectionBinder());
		}

		private void InitDefaultBinderProviders()
		{
			_binders.Add(new SimpleTypeModelBinderProvider() {
				Order = 16
			});
			_binders.Add(new ArrayModelBinderProivider() {
				Order = 17
			});
			_binders.Add(new DictionaryModelBinderProvider() {
				Order = 18
			});
			_binders.Add(new CollectionModelBinderProvider() {
				Order = 19
			});
			_binders.Add(new EnumerableModelBinderProvider() {
				Order = 20
			});
		}

		private void InitDefaultValueProviders()
		{
			_valueProviders.Insert(ParameterSource.Parameters, 1, new ParameterValueProviderFactory());
			_valueProviders.Insert(ParameterSource.Url, 2, new RouteDataValueProviderFactory());
			_valueProviders.Insert(ParameterSource.Form, 3, new FormValueProviderFactory());
			_valueProviders.Insert(ParameterSource.File, 4, new HttpFileCollectionValueProviderFactory());
			_valueProviders.Insert(ParameterSource.QueryString, 5, new QueryStringValueProviderFactory());
			_valueProviders.Insert(ParameterSource.Token, 6, new RouteTokenValueProviderFactory());
			_valueProviders.Insert(ParameterSource.Session, 7, new SessionStateValueProviderFactory());
			_valueProviders.Insert(ParameterSource.Cookie, 8, new CookieValueProviderFactory());
			_valueProviders.Insert(ParameterSource.Header, 9, new HeaderValueProviderFactory());
			_valueProviders.Insert(ParameterSource.InputStream, 10, new InputStreamValueProviderFactory());
		}

		private void InitBinders(ModelBinderConfigurationElementCollection element)
		{
			if (element == null)
				return;

			if (!String.IsNullOrEmpty(element.DefaultType))
				_binders.DefaultBinder = CreateModelBinder(
					Type.GetType(element.DefaultType, true, true));

			foreach (ModelBinderConfigurationElement elem in element)
			{
				Type modelType = Type.GetType(elem.ModelType, true, true);

				if (!String.IsNullOrEmpty(elem.BinderType))
					_binders.Add(modelType,
						CreateModelBinder(Type.GetType(elem.BinderType, true, true)));
			}
		}

		private void InitValueProviders(ValueProviderConfigurationElementCollection element)
		{
			if (element == null)
				return;

			foreach (ValueProviderConfigurationElement elem in element)
			{
				_valueProviders.Insert(elem.Name, elem.Order,
					CreateValueProviderFactory(Type.GetType(elem.FactoryType, true, true)));
			}
		}

        internal void Init(ModelConfigurationElement element)
        {
            Precondition.Require(element, () => Error.ArgumentNull("element"));

			InitBinders(element.Binders);
			InitValueProviders(element.ValueProviders);

            if (!String.IsNullOrEmpty(element.MetadataProviderType))
                _metadataProvider = CreateMetadataProvider(
                    Type.GetType(element.MetadataProviderType, true, true));

            if (!String.IsNullOrEmpty(element.ValidatorProviderType))
                _validatorProvider = CreateValidatorProvider(
                    Type.GetType(element.ValidatorProviderType, true, true));
        }
        #endregion
    }
}
