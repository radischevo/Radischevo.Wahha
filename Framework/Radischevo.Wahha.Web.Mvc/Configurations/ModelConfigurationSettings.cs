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
        private ModelMetadataProviderCollection _metadataProviders;
        private ModelValidatorProviderCollection _validatorProviders;
        #endregion

        #region Constructors
        internal ModelConfigurationSettings()
        {
            _binders = new ModelBinderCollection();
            _metadataProviders = new ModelMetadataProviderCollection();
            _validatorProviders = new ModelValidatorProviderCollection();

			InitDefaultBinders();
			InitDefaultBinderProviders();
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

        public ModelMetadataProviderCollection MetadataProviders
        {
            get
            {
                return _metadataProviders;
            }
        }

        public ModelValidatorProviderCollection ValidatorProviders
        {
            get
            {
                return _validatorProviders;
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

        internal void Init(ModelConfigurationElementCollection element)
        {
            Precondition.Require(element, () => Error.ArgumentNull("element"));

            if (!String.IsNullOrEmpty(element.BinderType))
                _binders.DefaultBinder = CreateModelBinder(
                    Type.GetType(element.BinderType, true, true));

            if (!String.IsNullOrEmpty(element.MetadataProviderType))
                _metadataProviders.Default = CreateMetadataProvider(
                    Type.GetType(element.MetadataProviderType, true, true));

            if (!String.IsNullOrEmpty(element.ValidatorProviderType))
                _validatorProviders.Default = CreateValidatorProvider(
                    Type.GetType(element.ValidatorProviderType, true, true));

            foreach (ModelConfigurationElement elem in element)
            {
                Type modelType = Type.GetType(elem.ModelType, true, true);

                if(!String.IsNullOrEmpty(elem.BinderType))
                    _binders.Add(modelType,
                        CreateModelBinder(Type.GetType(elem.BinderType, true, true)));

                if(!String.IsNullOrEmpty(elem.MetadataProviderType))
                    _metadataProviders.Add(modelType, 
                        CreateMetadataProvider(Type.GetType(elem.MetadataProviderType, true, true)));

                if(!String.IsNullOrEmpty(elem.ValidatorProviderType))
                    _validatorProviders.Add(modelType,
                        CreateValidatorProvider(Type.GetType(elem.ValidatorProviderType, true, true)));
            }
        }
        #endregion
    }
}
