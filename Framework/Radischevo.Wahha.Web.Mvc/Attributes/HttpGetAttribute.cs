using System;

namespace Radischevo.Wahha.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class HttpGetAttribute : AcceptHttpVerbsAttribute
	{
		#region Constructors
		public HttpGetAttribute()
			: base(HttpMethod.Get | HttpMethod.Head)
		{
		}
		#endregion
	}
}