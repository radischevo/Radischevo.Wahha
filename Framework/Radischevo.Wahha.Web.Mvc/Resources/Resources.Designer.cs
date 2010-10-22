﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Radischevo.Wahha.Web.Mvc.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Radischevo.Wahha.Web.Mvc.Resources.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicate action name &apos;{0}&apos; found at controller &apos;{1}&apos;..
        /// </summary>
        internal static string Error_AmbiguousActionName {
            get {
                return ResourceManager.GetString("Error_AmbiguousActionName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided IAsyncResult has already been consumed..
        /// </summary>
        internal static string Error_AsyncResultAlreadyConsumed {
            get {
                return ResourceManager.GetString("Error_AsyncResultAlreadyConsumed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value &apos;{0}&apos; is not valid for {1}..
        /// </summary>
        internal static string Error_BinderValueInvalid {
            get {
                return ResourceManager.GetString("Error_BinderValueInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A value is required..
        /// </summary>
        internal static string Error_BinderValueRequired {
            get {
                return ResourceManager.GetString("Error_BinderValueRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot call instance method &apos;{0}&apos; on non-controller type &apos;{1}&apos;..
        /// </summary>
        internal static string Error_CannotCallInstanceMethodOnNonControllerType {
            get {
                return ResourceManager.GetString("Error_CannotCallInstanceMethodOnNonControllerType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Child actions are not allowed to perform redirect actions..
        /// </summary>
        internal static string Error_CannotRedirectFromChildAction {
            get {
                return ResourceManager.GetString("Error_CannotRedirectFromChildAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Execution of the child request failed. Please examine the InnerException for more information..
        /// </summary>
        internal static string Error_ChildRequestExecutionError {
            get {
                return ResourceManager.GetString("Error_ChildRequestExecutionError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A single instance of controller &apos;{0}&apos; cannot be used to handle multiple requests. If a custom controller factory is in use, make sure that it creates a new instance of the controller for each request..
        /// </summary>
        internal static string Error_ControllerCannotHandleMultipleRequests {
            get {
                return ResourceManager.GetString("Error_ControllerCannotHandleMultipleRequests", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The controller type &apos;{0}&apos; must have a parameterless public constructor..
        /// </summary>
        internal static string Error_ControllerMustHaveDefaultConstructor {
            get {
                return ResourceManager.GetString("Error_ControllerMustHaveDefaultConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The controller of type &apos;{0}&apos; must subclass AsyncController or implement the IAsyncManagerContainer interface..
        /// </summary>
        internal static string Error_ControllerMustImplementAsyncManagerContainer {
            get {
                return ResourceManager.GetString("Error_ControllerMustImplementAsyncManagerContainer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The ViewUserControl cannot find an IViewDataContainer. The ViewUserControl must be inside a ViewPage, ViewMasterPage, or another ViewUserControl..
        /// </summary>
        internal static string Error_ControlRequiresViewDataProvider {
            get {
                return ResourceManager.GetString("Error_ControlRequiresViewDataProvider", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The controller &apos;{0}&apos; could not be found..
        /// </summary>
        internal static string Error_CouldNotCreateController {
            get {
                return ResourceManager.GetString("Error_CouldNotCreateController", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The view found at &apos;{0}&apos; could not be created..
        /// </summary>
        internal static string Error_CouldNotCreateView {
            get {
                return ResourceManager.GetString("Error_CouldNotCreateView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deserialization failed. Verify that the data is being deserialized using the same SerializationMode with which it was serialized. Otherwise see the inner exception..
        /// </summary>
        internal static string Error_CouldNotDeserializeModelState {
            get {
                return ResourceManager.GetString("Error_CouldNotDeserializeModelState", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not locate the completion action method named &apos;{0}&apos; on controller type {1}..
        /// </summary>
        internal static string Error_CouldNotFindAsyncCompletionMethod {
            get {
                return ResourceManager.GetString("Error_CouldNotFindAsyncCompletionMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user control of type &apos;{0}&apos; was not successfully initialized..
        /// </summary>
        internal static string Error_CouldNotSetControlProperties {
            get {
                return ResourceManager.GetString("Error_CouldNotSetControlProperties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The controller name &apos;{0}&apos; is ambiguous between several types..
        /// </summary>
        internal static string Error_DuplicateControllerName {
            get {
                return ResourceManager.GetString("Error_DuplicateControllerName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expression parameter &apos;{0}&apos; must be a method call..
        /// </summary>
        internal static string Error_ExpressionMustBeAMethodCall {
            get {
                return ResourceManager.GetString("Error_ExpressionMustBeAMethodCall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid output type &apos;{0}&apos;. Only objects of type &apos;HttpPostedFileBase&apos; can be retrieved from the HttpPostedFileSet..
        /// </summary>
        internal static string Error_HttpPostedFileSetTypeLimitations {
            get {
                return ResourceManager.GetString("Error_HttpPostedFileSetTypeLimitations", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The controller activator type &apos;{0}&apos; must implement the IControllerActivator interface..
        /// </summary>
        internal static string Error_IncompatibleControllerActivatorType {
            get {
                return ResourceManager.GetString("Error_IncompatibleControllerActivatorType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The controller factory type &apos;{0}&apos; must implement the IControllerFactory interface..
        /// </summary>
        internal static string Error_IncompatibleControllerFactoryType {
            get {
                return ResourceManager.GetString("Error_IncompatibleControllerFactoryType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The model binder type &apos;{0}&apos; must implement the IModelBinder interface..
        /// </summary>
        internal static string Error_IncompatibleModelBinderType {
            get {
                return ResourceManager.GetString("Error_IncompatibleModelBinderType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The model metadata provider type &apos;{0}&apos; must inherit from the ModelMetadataProvider class..
        /// </summary>
        internal static string Error_IncompatibleModelMetadataProviderType {
            get {
                return ResourceManager.GetString("Error_IncompatibleModelMetadataProviderType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The model validator provider type &apos;{0}&apos; must inherit from the ModelValidatorProvider class..
        /// </summary>
        internal static string Error_IncompatibleModelValidatorProviderType {
            get {
                return ResourceManager.GetString("Error_IncompatibleModelValidatorProviderType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The view engine type &apos;{0}&apos; must implement the IViewEngine interface..
        /// </summary>
        internal static string Error_IncompatibleViewEngineType {
            get {
                return ResourceManager.GetString("Error_IncompatibleViewEngineType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided IAsyncResult is not valid for this method..
        /// </summary>
        internal static string Error_InvalidAsyncResult {
            get {
                return ResourceManager.GetString("Error_InvalidAsyncResult", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The controller type &apos;{0}&apos; must implement the &apos;{1}&apos; interface..
        /// </summary>
        internal static string Error_InvalidControllerType {
            get {
                return ResourceManager.GetString("Error_InvalidControllerType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type &apos;{0}&apos; must have a public constructor which accepts the parameters of types &apos;{1}&apos; and &apos;{2}&apos;..
        /// </summary>
        internal static string Error_InvalidDataAnnotationsValidationRuleConstructor {
            get {
                return ResourceManager.GetString("Error_InvalidDataAnnotationsValidationRuleConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expression compiler was unable to evaluate the indexer expression &apos;{0}&apos; because it references the model parameter &apos;{1}&apos; which is unavailable..
        /// </summary>
        internal static string Error_InvalidIndexerExpression {
            get {
                return ResourceManager.GetString("Error_InvalidIndexerExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided SerializationMode &apos;{0}&apos; is invalid..
        /// </summary>
        internal static string Error_InvalidModelStateSerializationMode {
            get {
                return ResourceManager.GetString("Error_InvalidModelStateSerializationMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A public action method &apos;{0}&apos; could not be found on controller &apos;{1}&apos;..
        /// </summary>
        internal static string Error_InvalidMvcAction {
            get {
                return ResourceManager.GetString("Error_InvalidMvcAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The referrer URL &apos;{0}&apos; is invalid for the current request..
        /// </summary>
        internal static string Error_InvalidReferrerUrl {
            get {
                return ResourceManager.GetString("Error_InvalidReferrerUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The timeout value must be non-negative or Timeout.Infinite..
        /// </summary>
        internal static string Error_InvalidTimeout {
            get {
                return ResourceManager.GetString("Error_InvalidTimeout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The model item passed into the dictionary is of type &apos;{0}&apos; but this dictionary requires a model item of type &apos;{1}&apos;..
        /// </summary>
        internal static string Error_InvalidViewDataType {
            get {
                return ResourceManager.GetString("Error_InvalidViewDataType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A route that matches the requested values could not be located in the route table..
        /// </summary>
        internal static string Error_MatchingRouteCouldNotBeLocated {
            get {
                return ResourceManager.GetString("Error_MatchingRouteCouldNotBeLocated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expression parameter &apos;{0}&apos; must target the lambda argument..
        /// </summary>
        internal static string Error_MethodCallMustTargetLambdaArgument {
            get {
                return ResourceManager.GetString("Error_MethodCallMustTargetLambdaArgument", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameters dictionary does not contain an entry for parameter &apos;{0}&apos; of type &apos;{1}&apos; for method &apos;{2}&apos; in &apos;{3}&apos;..
        /// </summary>
        internal static string Error_MissingActionParameter {
            get {
                return ResourceManager.GetString("Error_MissingActionParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot create an instance of type &apos;{0}&apos; since it does not provide a default constructor..
        /// </summary>
        internal static string Error_MissingParameterlessConstructor {
            get {
                return ResourceManager.GetString("Error_MissingParameterlessConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type &apos;{0}&apos; contains multiple attributes inheriting from ModelBinderAttribute..
        /// </summary>
        internal static string Error_MultipleModelBinderAttributes {
            get {
                return ResourceManager.GetString("Error_MultipleModelBinderAttributes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter &apos;{0}&apos; contains multiple attributes inheriting from ModelBinderAttribute..
        /// </summary>
        internal static string Error_MultipleModelBinderAttributes_Parameter {
            get {
                return ResourceManager.GetString("Error_MultipleModelBinderAttributes_Parameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The GetBinder method on the ModelBinderAttribute class must be overridden if the binder type is not initialized in the constructor..
        /// </summary>
        internal static string Error_MustOverrideGetBinderToUseEmptyType {
            get {
                return ResourceManager.GetString("Error_MustOverrideGetBinderToUseEmptyType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot call action method &apos;{0}&apos; on controller &apos;{1}&apos; since it is a generic method..
        /// </summary>
        internal static string Error_MvcActionCannotBeGeneric {
            get {
                return ResourceManager.GetString("Error_MvcActionCannotBeGeneric", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter &apos;{0}&apos; at action method &apos;{1}&apos; doesn&apos;t allow null value..
        /// </summary>
        internal static string Error_ParameterCannotBeNull {
            get {
                return ResourceManager.GetString("Error_ParameterCannotBeNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameters dictionary contains an invalid entry for parameter &apos;{0}&apos; for method &apos;{1}&apos; in &apos;{2}&apos;. A value of type &apos;{3}&apos; required..
        /// </summary>
        internal static string Error_ParameterValueHasWrongType {
            get {
                return ResourceManager.GetString("Error_ParameterValueHasWrongType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The property {0}.{1} could not be found..
        /// </summary>
        internal static string Error_PropertyNotFound {
            get {
                return ResourceManager.GetString("Error_PropertyNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reference parameter &apos;{0}&apos; at method &apos;{1}&apos; on type &apos;{2}&apos; is not supported..
        /// </summary>
        internal static string Error_ReferenceActionParametersNotSupported {
            get {
                return ResourceManager.GetString("Error_ReferenceActionParametersNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A required request validation token was not supplied or was invalid..
        /// </summary>
        internal static string Error_RequestValidationError {
            get {
                return ResourceManager.GetString("Error_RequestValidationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested resource can only be accessed via HTTPS..
        /// </summary>
        internal static string Error_SecureConnectionRequired {
            get {
                return ResourceManager.GetString("Error_SecureConnectionRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The SessionStateTempDataProvider requires SessionState to be enabled..
        /// </summary>
        internal static string Error_SessionStateDisabled {
            get {
                return ResourceManager.GetString("Error_SessionStateDisabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An operation that crossed a synchronization context failed. See the inner exception for more information..
        /// </summary>
        internal static string Error_SynchronizationContextError {
            get {
                return ResourceManager.GetString("Error_SynchronizationContextError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The controller type &apos;{0}&apos; must inherit from the &apos;{1}&apos; type..
        /// </summary>
        internal static string Error_TargetMustSubclassController {
            get {
                return ResourceManager.GetString("Error_TargetMustSubclassController", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Templates can be used only with field and property accessor expressions..
        /// </summary>
        internal static string Error_TemplateExpressionLimitations {
            get {
                return ResourceManager.GetString("Error_TemplateExpressionLimitations", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The open model type &apos;{0}&apos; has {1} generic type argument(s), but the open binder type &apos;{2}&apos; has {3} generic type argument(s). The binder type must not be an open generic type or must have the same number of generic arguments as the open model type..
        /// </summary>
        internal static string Error_TypeArgumentCountMismatch {
            get {
                return ResourceManager.GetString("Error_TypeArgumentCountMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type &apos;{0}&apos; must derive from &apos;{1}&apos;..
        /// </summary>
        internal static string Error_TypeMustDeriveFromType {
            get {
                return ResourceManager.GetString("Error_TypeMustDeriveFromType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Collection template was used with an object of type &apos;{0}&apos;, which does not implement System.IEnumerable..
        /// </summary>
        internal static string Error_TypeMustImplementIEnumerable {
            get {
                return ResourceManager.GetString("Error_TypeMustImplementIEnumerable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user control found at &apos;{0}&apos; could not be created..
        /// </summary>
        internal static string Error_UnableToInstantiateUserControl {
            get {
                return ResourceManager.GetString("Error_UnableToInstantiateUserControl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not parse the configuration. Refer to the inner exception for details..
        /// </summary>
        internal static string Error_UnableToLoadConfiguration {
            get {
                return ResourceManager.GetString("Error_UnableToLoadConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} has a DisplayColumn attribute for {1}, but property {1} does not exist..
        /// </summary>
        internal static string Error_UnknownProperty {
            get {
                return ResourceManager.GetString("Error_UnknownProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} has a DisplayColumn attribute for {1}, but property {1} does not have a public getter..
        /// </summary>
        internal static string Error_UnreadableProperty {
            get {
                return ResourceManager.GetString("Error_UnreadableProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The dictionary of type &apos;{0}&apos; cannot be used as an action parameter..
        /// </summary>
        internal static string Error_UnsupportedDictionaryType {
            get {
                return ResourceManager.GetString("Error_UnsupportedDictionaryType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This model binder does not support the model type &apos;{0}&apos;..
        /// </summary>
        internal static string Error_UnsupportedModelType {
            get {
                return ResourceManager.GetString("Error_UnsupportedModelType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A ViewUserControl can only be used inside pages that derive from ViewPage or ViewPage&lt;TViewItem&gt;..
        /// </summary>
        internal static string Error_ViewControlRequiresViewPage {
            get {
                return ResourceManager.GetString("Error_ViewControlRequiresViewPage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to View location format collection cannot be empty..
        /// </summary>
        internal static string Error_ViewLocationFormatsAreEmpty {
            get {
                return ResourceManager.GetString("Error_ViewLocationFormatsAreEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A ViewMasterPage can only be used with content pages that derive from ViewPage or ViewPage&lt;TViewItem&gt;..
        /// </summary>
        internal static string Error_ViewMasterPageRequiresViewPage {
            get {
                return ResourceManager.GetString("Error_ViewMasterPageRequiresViewPage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The view &apos;{0}&apos; could not be found..
        /// </summary>
        internal static string Error_ViewNotFound {
            get {
                return ResourceManager.GetString("Error_ViewNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The view type &apos;{0}&apos; must derive from ViewPage, ViewPage&lt;TViewData&gt;, ViewUserControl, or ViewUserControl&lt;TViewData&gt;..
        /// </summary>
        internal static string Error_WrongViewBase {
            get {
                return ResourceManager.GetString("Error_WrongViewBase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to date.
        /// </summary>
        internal static string String_DateTimeDataTypeRule {
            get {
                return ResourceManager.GetString("String_DateTimeDataTypeRule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to number.
        /// </summary>
        internal static string String_NumberDataTypeRule {
            get {
                return ResourceManager.GetString("String_NumberDataTypeRule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No.
        /// </summary>
        internal static string String_TriState_No {
            get {
                return ResourceManager.GetString("String_TriState_No", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not set.
        /// </summary>
        internal static string String_TriState_NotSet {
            get {
                return ResourceManager.GetString("String_TriState_NotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Yes.
        /// </summary>
        internal static string String_TriState_Yes {
            get {
                return ResourceManager.GetString("String_TriState_Yes", resourceCulture);
            }
        }
    }
}
