﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Routing.Configurations;

namespace Radischevo.Wahha.Web.Routing.Providers
{
	public delegate Route RouteFactory(RouteDescriptor descriptor);

    /// <summary>
    /// Represents the route table persistence 
    /// provider, which stores the routes in 
    /// the XML configuration file.
    /// </summary>
    public class ConfigurationRouteTableProvider : IRouteTableProvider
	{
		#region Static Fields
		private static ReaderWriterLock _lock = new ReaderWriterLock();
		private static IDictionary<string, RouteFactory> _factories;
		private static RouteFactory _defaultFactory;
		#endregion

		#region Constructors
		static ConfigurationRouteTableProvider()
		{
			_factories = new Dictionary<string, RouteFactory>(StringComparer.OrdinalIgnoreCase);

			_factories.Add("ignore", descriptor => new IgnoredRoute(descriptor.Url));
			_factories.Add("page", descriptor => new WebFormRoute(descriptor.Url, 
				descriptor.Attributes.GetValue<string>("page")));

			_defaultFactory = descriptor => {
				Precondition.Require(descriptor.HandlerType, () => Error.ArgumentNull("handlerType"));
				IRouteHandler handler = (IRouteHandler)ServiceLocator.Instance.GetService(descriptor.HandlerType);

				return new Route(descriptor.Url, handler);
			};	
		}

		/// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ConfigurationRouteTableProvider"/> class
        /// </summary>
        public ConfigurationRouteTableProvider()
        {
        }
        #endregion

		#region Static Methods
		public static void RegisterFactory(string routeType, RouteFactory factory)
		{
			Precondition.Defined(routeType, () => Error.ArgumentNull("routeType"));
			Precondition.Require(factory, () => Error.ArgumentNull("factory"));

			_lock.AcquireWriterLock(Timeout.Infinite);

			try
			{
				_factories[routeType] = factory;
			}
			finally
			{
				_lock.ReleaseWriterLock();
			}
		}
		#endregion

		#region Instance Methods
		/// <summary>
        /// Gets the currently configured route table.
        /// </summary>
        public RouteTableProviderResult GetRouteTable()
        {
			RouteTableProviderResult result =
				new RouteTableProviderResult();

            foreach (RouteConfigurationElement element in
                Configurations.Configuration.Instance.Routes)
                result.Routes.Add(element.Name, ProcessRoute(element));
			
			NameValueCollection<string> variables = Configurations.Configuration.Instance.Variables;
			foreach (string name in variables.Keys)
				result.Variables.Add(name, variables[name]);

            return result;
        }

		private RouteBase CreateRoute(RouteDescriptor descriptor,
			IValueSet defaults, IValueSet tokens,
			IEnumerable<IRouteConstraint> constraints)
		{
			RouteFactory factory = GetRouteFactory(descriptor.Type);
			Route route = factory(descriptor);

			route.Defaults.Merge(defaults);
			route.Tokens.Merge(tokens);
			route.SecureConnection = descriptor.Attributes
				.GetValue<SecureConnectionOption>("secure");

			foreach (IRouteConstraint constraint in constraints)
				route.Constraints.Add(constraint);

			return route;
		}

		protected virtual RouteFactory GetRouteFactory(string type)
		{
			_lock.AcquireReaderLock(Timeout.Infinite);
			try
			{
				RouteFactory factory;
				if (!_factories.TryGetValue(type, out factory))
					factory = _defaultFactory;

				return factory ?? _defaultFactory;
			}
			finally
			{
				_lock.ReleaseReaderLock();
			}
		}

		protected virtual Type DetermineHandlerType(RouteConfigurationElement element)
		{
			Type defaultType = Configurations.Configuration.Instance.DefaultHandlerType;
			Type type = Type.GetType(element.HandlerType, false, true);

			return type ?? defaultType;
		}

        protected virtual RouteBase ProcessRoute(RouteConfigurationElement element)
        {
			Type handlerType = DetermineHandlerType(element);
            ValueDictionary defaults = ProcessDefaults(element.Defaults);
            ValueDictionary tokens = ProcessTokens(element.Tokens);
            IEnumerable<IRouteConstraint> constraints = ProcessConstraints(element.Constraints);

			RouteDescriptor descriptor = new RouteDescriptor(
				element.Url, element.Type, element.Attributes, handlerType);

			return CreateRoute(descriptor, defaults, tokens, constraints);
        }

        protected virtual ValueDictionary ProcessDefaults(
            NameValueConfigurationCollection collection)
        {
            ValueDictionary defaults = new ValueDictionary();
            if(collection == null)
                return defaults;

            foreach (string key in collection.AllKeys)
                defaults.Add(key, collection[key].Value);

            return defaults;
        }

        protected virtual ValueDictionary ProcessTokens(
            NameValueConfigurationCollection collection)
        {
            ValueDictionary tokens = new ValueDictionary();
            if (collection == null)
                return tokens;

            foreach (string key in collection.AllKeys)
                tokens.Add(key, collection[key].Value);

            return tokens;
        }

        protected virtual IEnumerable<IRouteConstraint> ProcessConstraints(
            RouteConstraintConfigurationElementCollection collection)
        {
            List<IRouteConstraint> constraints = new List<IRouteConstraint>();
            foreach (RouteConstraintConfigurationElement element in collection)
            {
                IRouteConstraint constraint = ProcessConstraint(element.Type, element.Attributes);

                if(constraint != null)
                    constraints.Add(constraint);
            }
            return constraints;
        }

        protected virtual IRouteConstraint ProcessConstraint(string type, IValueSet attributes)
        {
            if (String.IsNullOrEmpty(type))
                return null;

            switch (type.ToLowerInvariant())
            {
                case "method":
                    HttpMethod method = HttpMethod.None;
                    foreach (string str in attributes.GetValue<string>("verbs", String.Empty)
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        method |= (HttpMethod)Enum.Parse(typeof(HttpMethod), str, true);

                    return new HttpMethodConstraint(method);
                case "regex":
                    string parameter = attributes.GetValue<string>("parameter");
                    string pattern = attributes.GetValue<string>("pattern");
                    string options = attributes.GetValue<string>("options");
                    
                    RegexOptions rx = RegexOptions.None;
                    if(!String.IsNullOrEmpty(options)) 
                    {
                        foreach (string str in options.Split(new char[] { ',' }, 
                            StringSplitOptions.RemoveEmptyEntries))
                            rx |= (RegexOptions)Enum.Parse(typeof(RegexOptions), str, true);
                    }
                    return new RegexConstraint(parameter, pattern, rx);
            }
            return null;
        }

        void IRouteTableProvider.Init(IValueSet settings)
        {
        }
        #endregion
    }
}
