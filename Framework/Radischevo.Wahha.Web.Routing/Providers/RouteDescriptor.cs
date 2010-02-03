using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
	public sealed class RouteDescriptor
	{
		#region Instance Fields
		private string _url;
		private string _type;
		private IValueSet _attributes;
		private Type _handlerType;
		#endregion

		#region Constructors
		public RouteDescriptor(string url, 
			string type, IValueSet attributes)
			: this(url, type, attributes, null)
		{
		}

		public RouteDescriptor(string url,
			string type, IValueSet attributes, 
			Type handlerType)
		{
			_url = url;
			_type = type;
			_attributes = attributes;
			_handlerType = handlerType;
		}
		#endregion

		#region Instance Properties
		public string Url
		{
			get
			{
				return _url ?? String.Empty;
			}
			set
			{
				_url = value;
			}
		}

		public string Type
		{
			get
			{
				return _type ?? String.Empty;
			}
			set
			{
				_type = value;
			}
		}

		public IValueSet Attributes
		{
			get
			{
				if (_attributes == null)
					_attributes = new ValueDictionary();

				return _attributes;
			}
		}

		public Type HandlerType
		{
			get
			{
				return _handlerType;
			}
			set
			{
				if(value != null && !typeof(IRouteHandler).IsAssignableFrom(value))
					throw Error.InvalidRouteHandlerType(value);

				_handlerType = value;
			}
		}
		#endregion
	}
}
