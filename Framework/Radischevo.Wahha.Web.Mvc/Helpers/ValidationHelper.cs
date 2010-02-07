using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Mvc.UI;
using Radischevo.Wahha.Web.Mvc.Validation;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ValidationHelper
    {
        #region Instance Fields
        private ModelValidatorProviderCollection _providers;
        private ViewContext _context;
        #endregion

        #region Constructors
        public ValidationHelper(ViewContext context)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
            _context = context;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the current 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ViewContext"/>
        /// </summary>
        public ViewContext Context
        {
            get
            {
                return _context;
            }
        }

        /// <summary>
        /// Gets the <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection"/> object 
        /// containing the binding validation errors.
        /// </summary>
        public ValidationErrorCollection Errors
        {
            get
            {
                return _context.Controller.Errors;
            }
        }

        public ModelValidatorProviderCollection Providers
        {
            get
            {
                if (_providers == null)
                    _providers = Configuration.Configuration.Instance.Models.ValidatorProviders;

                return _providers;
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Returns a list of validation messages from the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection">Errors collection</see>, 
        /// using the specified <paramref name="action"/> to render each of them.
        /// </summary>
        /// <param name="action">The action used to render an error message.</param>
        public void Summary(Action<ValidationError> action)
        {
            Precondition.Require(action, Error.ArgumentNull("action"));
            
            foreach (ValidationError error in Errors)
                action(error);
        }

        /// <summary>
        /// Displays a validation message if the specified field contains an error in 
        /// the <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection">Errors collection</see>.
        /// </summary>
        /// <param name="key">The name of the field.</param>
        /// <param name="action">The action used to render an error message.</param>
        public void Messages(string key, Action<IEnumerable<ValidationError>> action)
        {
            Precondition.Require(action, Error.ArgumentNull("action"));
			IEnumerable<ValidationError> errors = Errors[key];

			if(errors != null && errors.Any())
				action(errors);
        }

		/// <summary>
		/// Displays a validation message if the specified field contains an error in 
		/// the <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection">Errors collection</see>.
		/// </summary>
		/// <param name="key">The name of the field.</param>
		/// <param name="action">The action used to render an error message.</param>
		public void Message(string key,
			Action<ValidationError> action)
		{
			Precondition.Require(action, Error.ArgumentNull("action"));
			foreach (ValidationError error in Errors[key])
				action(error);
		}

		/// <summary>
		/// Gets the value indicating whether the 
		/// <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection"/> 
		/// contains no errors.
		/// </summary>
		public bool Valid()
		{
			return Errors.IsValid();
		}

        /// <summary>
        /// Gets the value indicating whether the
        /// <see cref="Radischevo.Wahha.Web.Mvc.ValidationErrorCollection">Errors collection</see> 
        /// contains an error with the specified field name.
        /// </summary>
        /// <param name="key">The name of the field.</param>
        public bool Valid(string key)
        {
			return Errors.IsValid(key);
        }
        #endregion

        #region Helper Methods
        protected ModelValidator GetValidatorFromExpression(string expression)
        {
            ViewDataInfo vdi = Context.ViewData.GetViewDataInfo(expression);
            if (vdi == null)
                return null;

            Type containerType = null;
            Type modelType = null;
            string propertyName = null;
            object model = vdi.Value;

            if (vdi.Container != null)
                containerType = vdi.Container.GetType();

            if (vdi.Descriptor != null)
            {
                propertyName = vdi.Descriptor.Name;
                modelType = vdi.Descriptor.PropertyType;
            }

            if (model != null && modelType == null)
                modelType = model.GetType();

            if (modelType == null)
                model = typeof(string);

            return GetValidatorFromProvider(containerType, modelType ?? typeof(string), propertyName);
        }

        protected virtual ModelValidator GetValidatorFromProvider(Type containerType,
            Type modelType, string propertyName)
        {
            if (containerType != null && !String.IsNullOrEmpty(propertyName))
                return Providers.GetProvider(containerType).GetValidator(containerType)
                    .GetPropertyValidator(propertyName);

            return Providers.GetProvider(modelType).GetValidator(modelType);
        }

        protected virtual IEnumerable<ClientModelValidationRule> CreateClientRules(
            IEnumerable<ModelValidationRule> rules)
        {
            Precondition.Require(rules, Error.ArgumentNull("rules"));
            return rules.Select(r => new ClientModelValidationRule(r));
        }

        protected virtual IEnumerable<ModelValidationRule> GetRulesInternal<TModel, TValue>(
            Expression<Func<TModel, TValue>> expression)
            where TModel : class
        {
            Precondition.Require(expression, Error.ArgumentNull("expression"));

            object modelValue = LinqHelper.WrapModelAccessor(
                expression, (TModel)Context.ViewData.Model)();

            Type modelType = typeof(TValue);
            Type parentModelType = null;
            string propertyName = null;

            switch (expression.Body.NodeType)
            {
                case ExpressionType.Parameter:
                    break;

                case ExpressionType.MemberAccess:
                    MemberExpression memberExpression = (MemberExpression)expression.Body;
                    propertyName = (memberExpression.Member is PropertyInfo) ? memberExpression.Member.Name : null;
                    parentModelType = memberExpression.Member.DeclaringType;
                    break;

                default:
                    throw Error.TemplateExpressionLimitations();
            }

            ModelValidator validator = GetValidatorFromProvider(parentModelType, modelType, propertyName);
            if (validator != null)
            {
                if(parentModelType == null) // model => model
                    return validator.GetValidationRules().Concat(validator.Properties
                        .SelectMany(m => m.GetValidationRules()));

                return validator.GetValidationRules(); // model => model.Property
            }

            return Enumerable.Empty<ModelValidationRule>();
        }

        protected virtual IEnumerable<ModelValidationRule> GetRulesInternal(string expression)
        {
            ModelValidator validator = Context.ViewData.Validator;

            if (String.IsNullOrEmpty(expression))
            {
                if(validator != null)
                    return validator.GetValidationRules().Concat(validator.Properties
                        .SelectMany(m => m.GetValidationRules()));

                return Enumerable.Empty<ModelValidationRule>();
            }
            validator = validator.GetPropertyValidator(expression);
            if (validator != null)
                return validator.GetValidationRules();

            validator = GetValidatorFromExpression(expression);
            if (validator != null)
                return validator.GetValidationRules();
            
            return Enumerable.Empty<ModelValidationRule>();
        }
        #endregion

        #region Client Validation
        /// <summary>
        /// Gets the collection of client validation rules, 
        /// which are applied to the current model.
        /// </summary>
        public FormValidationMetadata Rules()
        {
            return Rules(null, null);
        }

        /// <summary>
        /// Gets the collection of client validation rules, 
        /// which are applied to the current model.
        /// </summary>
        /// <param name="modelName">Specifies the name of the model, 
        /// which will prefix the validation rule member name.</param>
        public FormValidationMetadata Rules(string modelName)
        {
            return Rules(modelName, null);
        }

        /// <summary>
        /// Gets the collection of client validation rules, 
        /// which are applied to the property of the 
        /// model, accessed through the specified <paramref name="expression"/>.
        /// </summary>
        /// <param name="modelName">Specifies the name of the model, 
        /// which will prefix the validation rule member name.</param>
        /// <param name="expression">A string expression to access the 
        /// model property.</param>
        public FormValidationMetadata Rules(string modelName, string expression)
        {
            TemplateDescriptor descriptor = new TemplateDescriptor(
                Context.ViewData.Template.Type) { Prefix = modelName };
            descriptor.Prefix = descriptor.GetHtmlElementName(expression);
            bool hasEmptyExpression = (String.IsNullOrEmpty(expression));

            return new FormValidationMetadata(
                CreateClientRules(GetRulesInternal(expression))
                    .Each(r => { r.Field = (hasEmptyExpression)
                        ? descriptor.GetHtmlElementName(r.Field)
                        : descriptor.Prefix; }));
        }
        #endregion
    }

    public class ValidationHelper<TModel> : ValidationHelper
        where TModel : class
    {
        #region Constructors
        public ValidationHelper(ViewContext context)
            : base(context)
        {
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Gets the collection of client validation rules, 
        /// which are applied to the property of the 
        /// model, accessed through the specified <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">A lambda expression accessor for the 
        /// model property.</param>
        public FormValidationMetadata Rules<TValue>(
            Expression<Func<TModel, TValue>> expression)
        {
            return Rules(null, expression);
        }

        /// <summary>
        /// Gets the collection of client validation rules, 
        /// which are applied to the property of the 
        /// model, accessed through the specified <paramref name="expression"/>.
        /// </summary>
        /// <param name="modelName">Specifies the name of the model, 
        /// which will prefix the validation rule member name.</param>
        /// <param name="expression">A lambda expression accessor for the 
        /// model property.</param>
        public FormValidationMetadata Rules<TValue>(string modelName, 
            Expression<Func<TModel, TValue>> expression)
        {
            TemplateDescriptor descriptor = new TemplateDescriptor(
                Context.ViewData.Template.Type) { Prefix = modelName };
            bool hasEmptyExpression = (expression.Body.NodeType == 
                ExpressionType.Parameter);
            descriptor.Prefix = descriptor.GetHtmlElementName(
                LinqHelper.GetExpressionText(expression));

            return new FormValidationMetadata(
                CreateClientRules(GetRulesInternal(expression))
                    .Each(r => { r.Field = (hasEmptyExpression)
                        ? descriptor.GetHtmlElementName(r.Field)
                        : descriptor.Prefix; 
                    }));
        }
        #endregion
    }
}