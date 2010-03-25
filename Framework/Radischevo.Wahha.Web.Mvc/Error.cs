using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Runtime.Serialization;

namespace Radischevo.Wahha.Web.Mvc
{
    internal static class Error
    {
        internal static Exception ArgumentNull(string argumentName)
        {
            return new ArgumentNullException(argumentName);
        }

        internal static Exception InvalidArgument(string parameter)
        {
            return new ArgumentException(parameter);
        }

        internal static Exception ArgumentOutOfRange(string parameterName)
        {
            return new ArgumentOutOfRangeException(parameterName);
        }

        internal static Exception TypeMustDeriveFromType(Type type, Type baseType)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_TypeMustDeriveFromType,
                type.FullName, baseType.FullName));
        }

        internal static Exception InvalidMvcAction(Type controller, string actionName)
        {
            return new InvalidOperationException(
                String.Format(Resources.Resources.Error_InvalidMvcAction, actionName, controller.Name));
        }

        internal static Exception MvcActionCannotBeGeneric(MethodInfo method)
        {
            return new NotImplementedException(
                String.Format(Resources.Resources.Error_MvcActionCannotBeGeneric, 
                    method.Name, method.ReflectedType.FullName));
        }

        internal static Exception MissingParameterlessConstructor(Type type)
        {
            return new MissingMethodException(String.Format(
                Resources.Resources.Error_MissingParameterlessConstructor, type.FullName));
        }

        internal static Exception InvalidControllerType(string controllerName)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_InvalidControllerType,
                controllerName, typeof(IController).Name));
        }

        internal static Exception ControllerMustHaveDefaultConstructor(Type type)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_ControllerMustHaveDefaultConstructor, type.FullName));
        }

        internal static Exception TargetMustSubclassController(string controllerName)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_TargetMustSubclassController,
                controllerName, typeof(Controller).Name));
        }

        internal static Exception ExpressionMustBeAMethodCall(string parameterName)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_ExpressionMustBeAMethodCall, parameterName));
        }

        internal static Exception MethodCallMustTargetLambdaArgument(string parameterName)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_MethodCallMustTargetLambdaArgument, parameterName));
        }

        internal static Exception ReferenceActionParametersNotSupported(MethodInfo method, string parameterName)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_ReferenceActionParametersNotSupported,
                parameterName, method.Name, method.ReflectedType.FullName));
        }

        internal static Exception MissingActionParameter(MethodInfo action, ParameterInfo parameter)
        {
            return new InvalidOperationException(
                String.Format(Resources.Resources.Error_MissingActionParameter,
                parameter.Name, parameter.ParameterType.Name, action.Name, action.ReflectedType.Name));
        }

        internal static Exception IncompatibleControllerFactoryType(Type type)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_IncompatibleControllerFactoryType, type.Name));
        }

        internal static Exception DuplicateControllerName(string name)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_DuplicateControllerName, name));
        }

        internal static Exception ViewNotFound(string viewName)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_ViewNotFound, viewName));
        }

        internal static Exception CouldNotCreateView(string location)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_CouldNotCreateView, location));
        }

        internal static Exception WrongViewBase(Type type)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_WrongViewBase, type.Name));
        }

        internal static Exception CouldNotCreateController(string controllerName)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_CouldNotCreateController, controllerName));
        }

        internal static Exception ViewLocationFormatsAreEmpty()
        {
            return new InvalidOperationException(Resources.Resources.Error_ViewLocationFormatsAreEmpty);
        }

        internal static Exception UnableToInstantiateUserControl(string virtualPath)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_UnableToInstantiateUserControl, virtualPath));
        }

        internal static Exception CouldNotSetControlProperties(object instance, Exception inner)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_CouldNotSetControlProperties, instance.GetType().Name), inner);
        }

        internal static Exception InvalidViewDataType(Type viewDataType, Type type)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_InvalidViewDataType,
                type.Name, viewDataType.Name));
        }

        internal static Exception ViewControlRequiresViewPage()
        {
            return new InvalidOperationException(
                Resources.Resources.Error_ViewControlRequiresViewPage);
        }

        internal static Exception ControlRequiresViewDataProvider()
        {
            return new InvalidOperationException(
                Resources.Resources.Error_ControlRequiresViewDataProvider);
        }

        internal static Exception SessionStateDisabled()
        {
            return new InvalidOperationException(
                Resources.Resources.Error_SessionStateDisabled);
        }

        internal static Exception AmbiguousActionName(Type controllerType, string methodName)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_AmbiguousActionName, methodName, controllerType.Name));
        }

        internal static Exception MultipleModelBinderAttributes(Type type)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_MultipleModelBinderAttributes, type.FullName));
        }

        internal static Exception MultipleModelBinderAttributes(ParameterInfo parameter)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_MultipleModelBinderAttributes_Parameter, parameter.Name));
        }

        internal static Exception UnsupportedDictionaryType(Type type)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_UnsupportedDictionaryType, type.FullName));
        }

        internal static Exception UnableToLoadConfiguration(Exception inner)
        {
            return new InvalidOperationException(
                Resources.Resources.Error_UnableToLoadConfiguration, inner);
        }

        internal static Exception ViewMasterPageRequiresViewPage()
        {
            return new InvalidOperationException(
                Resources.Resources.Error_ViewMasterPageRequiresViewPage);
        }

        internal static Exception IncompatibleViewEngineType(Type type)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_IncompatibleViewEngineType, type.Name));
        }

        internal static Exception CannotCallInstanceMethodOnNonControllerType(MethodInfo method)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_CannotCallInstanceMethodOnNonControllerType, 
                method.Name, method.ReflectedType.FullName));
        }

        internal static Exception ParameterCannotBeNull(MethodInfo action, ParameterInfo parameter)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_ParameterCannotBeNull,
                parameter.Name, action.Name));
        }

        internal static Exception ParameterValueHasWrongType(MethodInfo action, ParameterInfo parameter)
        {
            return new ArgumentException(
                String.Format(Resources.Resources.Error_ParameterValueHasWrongType,
                parameter.Name, action.Name, action.ReflectedType.Name, parameter.ParameterType.Name));
        }

        internal static Exception IncompatibleModelBinderType(Type type)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_IncompatibleModelBinderType, type.Name));
        }

        internal static Exception ModelBinderMustHaveDefaultConstructor(Type type)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_ModelBinderMustHaveDefaultConstructor, type.Name));
        }

        internal static Exception RequestValidationError()
        {
            return new RequestValidationException(Resources.Resources.Error_RequestValidationError);
        }

        internal static Exception InvalidReferrerUrl(Uri referrer)
        {
            return new HttpException((int)HttpStatusCode.BadRequest, String.Format(Resources.Resources
                .Error_InvalidReferrerUrl, (referrer == null) ? String.Empty : referrer.OriginalString));
        }

        internal static Exception RequestValidationError(Exception inner)
        {
            return new RequestValidationException(Resources.Resources.Error_RequestValidationError, inner);
        }

        internal static Exception UnsupportedModelType(Type type)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_UnsupportedModelType, type.FullName));
        }

        internal static Exception TemplateExpressionLimitations()
        {
            return new InvalidOperationException(Resources.Resources
                .Error_TemplateExpressionLimitations);
        }

        internal static Exception MatchingRouteCouldNotBeLocated()
        {
            return new InvalidOperationException(Resources.Resources
                .Error_MatchingRouteCouldNotBeLocated);
        }

        internal static Exception PropertyNotFound(Type type, string propertyName)
        {
            return new InvalidOperationException(String.Format(Resources.Resources
                .Error_PropertyNotFound, type.FullName, propertyName));
        }

        internal static Exception UnknownProperty(Type type, string propertyName)
        {
            return new InvalidOperationException(String.Format(Resources.Resources
                .Error_UnknownProperty, type.FullName, propertyName));
        }

        internal static Exception UnreadableProperty(Type type, string propertyName)
        {
            return new InvalidOperationException(String.Format(Resources.Resources
                .Error_UnreadableProperty, type.FullName, propertyName));
        }

        internal static Exception IncompatibleModelValidatorProviderType(Type type)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_IncompatibleModelValidatorProviderType, type.Name));
        }

        internal static Exception IncompatibleModelMetadataProviderType(Type type)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_IncompatibleModelMetadataProviderType, type.Name));
        }

        internal static Exception InvalidDataAnnotationsValidationRuleConstructor(Type adapterType, 
            Type validatorType, Type attributeType)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_InvalidDataAnnotationsValidationRuleConstructor,
                adapterType.FullName, validatorType.FullName, attributeType.FullName));
        }

		internal static Exception ChildRequestExecutionError(HttpException ex)
		{
			return new HttpException(500, Resources.Resources.Error_ChildRequestExecutionError, ex);
		}

		internal static Exception ControllerCannotHandleMultipleRequests(Type type)
		{
			return new InvalidOperationException(String.Format(
				Resources.Resources.Error_ControllerCannotHandleMultipleRequests,
				type.FullName));
		}

		internal static Exception ControllerMustImplementAsyncManagerContainer(Type type)
		{
			return new InvalidOperationException(String.Format(Resources.Resources
				.Error_ControllerMustImplementAsyncManagerContainer, type.FullName));
		}

		internal static Exception InvalidTimeout(string parameterName)
		{
			return new ArgumentOutOfRangeException(parameterName, 
				Resources.Resources.Error_InvalidTimeout);
		}

		internal static Exception SynchronizationContextError(Exception error)
		{
			return new InvalidOperationException(Resources.Resources
				.Error_SynchronizationContextError, error);
		}

		internal static Exception InvalidAsyncResult(string parameterName)
		{
			return new ArgumentException(Resources.Resources
				.Error_InvalidAsyncResult, parameterName);
		}

		internal static Exception AsyncResultAlreadyConsumed()
		{
			return new InvalidOperationException(
				Resources.Resources.Error_AsyncResultAlreadyConsumed);
		}

		internal static Exception AsyncTimeout()
		{
			return new TimeoutException();
		}

		internal static Exception CannotRedirectFromChildAction()
		{
			return new InvalidOperationException(Resources.Resources
				.Error_CannotRedirectFromChildAction);
		}

		internal static Exception CouldNotFindAsyncCompletionMethod(Type type, 
			string completionMethodName)
		{
			return new InvalidOperationException(String.Format(Resources.Resources
				.Error_CouldNotFindAsyncCompletionMethod, completionMethodName, type.FullName));
		}

		internal static Exception SecureConnectionRequired()
		{
			throw new InvalidOperationException(Resources.Resources.Error_SecureConnectionRequired);
		}

		internal static Exception CollectionTypeMustBeEnumerable(Type type)
		{
			return new InvalidOperationException(String.Format(
				Resources.Resources.Error_TypeMustImplementIEnumerable, type.FullName));
		}

		internal static Exception InvalidIndexerExpression(Expression expression, ParameterExpression parameter)
		{
			return new InvalidOperationException(String.Format(
				Resources.Resources.Error_InvalidIndexerExpression, 
				expression, parameter.Name));
		}

		internal static Exception HttpPostedFileSetTypeLimitations(Type type)
		{
			return new InvalidOperationException(String.Format(Resources.Resources
				.Error_HttpPostedFileSetTypeLimitations, type.FullName));
		}

		internal static Exception TypeArgumentCountMismatch(Type modelType, Type binderType)
		{
			return new ArgumentException(String.Format(Resources.Resources.Error_TypeArgumentCountMismatch,
				modelType.FullName, modelType.GetGenericArguments().Length, 
				binderType.FullName, binderType.GetGenericArguments().Length));
		}

		internal static Exception CouldNotDeserializeModelState(Exception inner)
		{
			return new SerializationException(Resources.Resources
				.Error_CouldNotDeserializeModelState, inner);
		}

		internal static Exception InvalidModelStateSerializationMode(SerializationMode mode)
		{
			return new ArgumentOutOfRangeException("mode", String.Format(
				Resources.Resources.Error_InvalidModelStateSerializationMode, mode));
		}

		internal static Exception MustOverrideGetBinderToUseEmptyType()
		{
			return new InvalidOperationException(Resources.Resources.Error_MustOverrideGetBinderToUseEmptyType);
		}
	}
}
