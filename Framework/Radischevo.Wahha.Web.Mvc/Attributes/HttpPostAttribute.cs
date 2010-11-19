using System;

namespace Radischevo.Wahha.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class HttpPostAttribute : AcceptHttpVerbsAttribute
	{
		#region Constructors
		public HttpPostAttribute()
			: base(HttpMethod.Post)
		{
		}
		#endregion
	}
}