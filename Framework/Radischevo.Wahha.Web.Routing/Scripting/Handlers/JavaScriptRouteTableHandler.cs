using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Routing.Scripting
{
	public class JavaScriptRouteTableHandler : IHttpHandler
	{
		#region Constants
		private const string _cacheKey = "wahha.scripting.routetable.cache";
		#endregion

		#region Instance Fields
		private IScriptRouteTableProvider _provider;
		private IScriptRouteTableWriter _writer;
		private TimeSpan _cacheDuration;
		#endregion

		#region Constructors
		public JavaScriptRouteTableHandler()
		{
			_provider = new ScriptRouteTableProvider();
			_writer = new JavaScriptRouteTableWriter();
			_cacheDuration = TimeSpan.FromMinutes(10);
		}
		#endregion

		#region Instance Properties
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public TimeSpan CacheDuration
		{
			get
			{
				return _cacheDuration;
			}
			set
			{
				_cacheDuration = value;
			}
		}
		#endregion

		#region Instance Methods
		protected virtual void ProcessRequest(HttpContextBase context)
		{
			context.Response.ContentType = "text/javascript";
			SetHttpCachePolicy(context.Response.Cache);

			IEnumerable<RouteDefinition> routes = GetRouteTable(context);
			_writer.Write(routes, context.Response.Output);
		}

		protected virtual IEnumerable<RouteDefinition> GetRouteTable(HttpContextBase context)
		{
			IEnumerable<RouteDefinition> routes =
				(IEnumerable<RouteDefinition>)context.Cache.Get(_cacheKey);
			if (routes == null)
			{
				routes = _provider.GetRouteTable(RouteTable.Routes);
				context.Cache.Insert(_cacheKey, routes, null, 
					DateTime.Now.Add(CacheDuration), Cache.NoSlidingExpiration);
			}
			return routes;
		}

		protected virtual void SetHttpCachePolicy(HttpCachePolicyBase policy)
		{
			policy.SetCacheability(HttpCacheability.Private);
			policy.SetExpires(DateTime.Now.Add(CacheDuration));
			policy.SetMaxAge(CacheDuration);
			policy.SetValidUntilExpires(true);
		}
		#endregion

		#region IHttpHandler Members
		void IHttpHandler.ProcessRequest(HttpContext context)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));
			ProcessRequest(new HttpContextWrapper(context));	
		}
		#endregion
	}
}
