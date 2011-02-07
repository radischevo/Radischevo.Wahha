using System;
using System.Collections.Specialized;
using System.Web;

namespace Radischevo.Wahha.Web
{
	public static class HttpValueCollectionExtensions
	{
		#region Extension Methods
		/// <summary>
		/// Creates a wrapper over the specified 
		/// <see cref="System.Collections.Specialized.NameValueCollection"/>
		/// making it act as <see cref="Radischevo.Wahha.Web.IHttpValueSet"/>.
		/// </summary>
		/// <param name="collection">The collection to wrap over.</param>
		public static IHttpValueSet AsValueSet(this NameValueCollection collection)
		{
			return new HttpCollectionWrapper<NameValueCollection>(
				collection, (col, key) => col[key]);
		}

		/// <summary>
		/// Creates a wrapper over the specified 
		/// <see cref="System.Web.HttpCookieCollection"/>
		/// making it act as <see cref="Radischevo.Wahha.Web.IHttpValueSet"/>.
		/// </summary>
		/// <param name="collection">The collection to wrap over.</param>
		public static IHttpValueSet AsValueSet(this HttpCookieCollection collection)
		{
			return new HttpCollectionWrapper<HttpCookieCollection>(
				collection, (col, key) => {
					HttpCookie c = col[key];
					return (c == null) ? null : c.Value;
				});
		}
		#endregion
	}
}
