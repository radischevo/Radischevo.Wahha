using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc.Configurations
{
    public sealed class ModelConfigurationSettings
    {
        #region Instance Fields
        private ModelBinderCollection _binders;
		private ValueProviderFactoryCollection _valueProviders;
		private ModelValidatorProviderCollection _validatorProviders;
        #endregion

        #region Constructors
        internal ModelConfigurationSettings()
        {
            _binders = new ModelBinderCollection();
			_valueProviders = new ValueProviderFactoryCollection();
			_validatorProviders = new ModelValidatorProviderCollection();

			InitDefaultBinders();
			InitDefaultBinderProviders();
			InitDefaultValueProviders();
			InitDefaultValidatorProviders();
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

		private static IValueProviderFactory CreateValueProviderFactory(Type type)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			if (!typeof(IValueProviderFactory).IsAssignableFrom(type))
				throw Error.IncompatibleValueProviderFactoryType(type);

			return (IValueProviderFactory)ServiceLocator.Instance.GetService(type);
		}

		private static IModelValidatorProvider CreateValidatorProvider(Type type)
		{
			Precondition.Require(type, () => Error.ArgumentNull("type"));
			if (!typeof(IModelValidatorProvider).IsAssignableFrom(type))
				throw Error.IncompatibleModelValidatorProviderType(type);

			return (IModelValidatorProvider)ServiceLocator.Instance.GetService(type);
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
			_valueProviders.Add("Header", new HeaderValueProviderFactory());
			_valueProviders.Add("Cookie", new CookieValueProviderFactory());
			_valueProviders.Add("Session", new SessionStateValueProviderFactory());
			_valueProviders.Add("Token", new RouteTokenValueProviderFactory());
			_valueProviders.Add("QueryString", new QueryStringValueProviderFactory());
			_valueProviders.Add("File", new HttpFileCollectionValueProviderFactory());
			_valueProviders.Add("Form", new FormValueProviderFactory());
			_valueProviders.Add("Url", new RouteDataValueProviderFactory());
			_valueProviders.Add("Json", new JsonValueProviderFactory());
			_valueProviders.Add("Parameters", new ParameterValueProviderFactory());
		}
		
		private void InitDefaultValidatorProviders() 
		{
			_validatorProviders.Add(new EmptyModelValidatorProvider());
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
				_valueProviders.Add(elem.Name, CreateValueProviderFactory(
					Type.GetType(elem.FactoryType, true, true)));
			}
		}
		
		private void InitValidatorProviders(ModelValidatorProviderConfigurationElementCollection element)
		{
			if (element == null)
				return;

			foreach (ModelValidatorProviderConfigurationElement elem in element)
			{
				_validatorProviders.Add(CreateValidatorProvider(
					Type.GetType(elem.Type, true, true)));
			}
		}

        internal void Init(ModelConfigurationElement element)
        {
            Precondition.Require(element, () => Error.ArgumentNull("element"));

			InitBinders(element.Binders);
			InitValueProviders(element.ValueProviders);
			InitValidatorProviders(element.ValidatorProviders);
        }
        #endregion
    }
}
