using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        #endregion
    }
}
