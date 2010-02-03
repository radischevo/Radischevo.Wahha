using System;

namespace Radischevo.Wahha.Web.Routing
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

        internal static Exception IncompatibleRouteTableProvider(Type providerType)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_IncompatibleRouteTableProvider,
                providerType.Name, typeof(IRouteTableProvider).Name));
        }

        internal static Exception NoRouteHandlerFound()
        {
            return new InvalidOperationException(
                Resources.Resources.Error_NoRouteHandlerFound);
        }

        internal static Exception NoHttpHandlerFound(Type handlerType)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_NoHttpHandlerFound, handlerType.FullName));
        }

        internal static Exception PersistenceProviderNotInitialized()
        {
            return new InvalidOperationException(
                Resources.Resources.Error_PersistenceProviderNotInitialized);
        }

        internal static Exception InvalidRouteHandlerType(Type handlerType)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_InvalidRouteHandlerType,
                handlerType.Name, typeof(IRouteHandler).Name));
        }

        internal static Exception InvalidRouteParameterName(string parameterName)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_InvalidRouteParameterName, parameterName));
        }

        internal static Exception DuplicateRouteParameterName(string parameterName)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_DuplicateRouteParameterName, parameterName));
        }

        internal static Exception RequiredValueNotFound(string key)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_RequiredValueNotFound, key));
        }

        internal static Exception MismatchedRouteParameter(string segment)
        {
            return new ArgumentException(String.Format(
                Resources.Resources.Error_MismatchedRouteParameter, segment), "url");
        }

        internal static Exception ConsecutiveRouteParameters()
        {
            return new ArgumentException(
                Resources.Resources.Error_ConsecutiveRouteParameters, "url");
        }

        internal static Exception CatchAllInMultiSegment()
        {
            return new ArgumentException(
                Resources.Resources.Error_CatchAllInMultiSegment, "url");
        }

        internal static Exception CatchAllMustBeLast()
        {
            return new ArgumentException(
                Resources.Resources.Error_CatchAllMustBeLast, "url");
        }

        internal static Exception ConsecutiveSeparators()
        {
            return new ArgumentException(
                Resources.Resources.Error_ConsecutiveSeparators, "url");
        }

        internal static Exception NoRouteMatched()
        {
            return new InvalidOperationException(
                Resources.Resources.Error_NoRouteMatched);
        }

        internal static Exception ConstraintTypeCannotBeEmpty()
        {
            return new InvalidOperationException(Resources.Resources
                .Error_ConstraintTypeCannotBeEmpty);
        }

        internal static Exception CouldNotCreateHttpHandler(Type type)
        {
            return new InvalidOperationException(String.Format(
                Resources.Resources.Error_CouldNotInstantiateHttpHandler, type.FullName));
        }

        internal static Exception MatchingRouteCouldNotBeLocated()
        {
            return new InvalidOperationException(Resources.Resources
                .Error_MatchingRouteCouldNotBeLocated);
        }

        internal static Exception RoutableControlRequiresRoutablePage()
        {
            return new InvalidOperationException(
                Resources.Resources.Error_RoutableControlRequiresRoutablePage);
        }
    }
}
