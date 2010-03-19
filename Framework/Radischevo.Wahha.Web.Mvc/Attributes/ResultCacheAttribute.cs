using System;
using System.IO;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
		Inherited = true, AllowMultiple = false)]
	public class ResultCacheAttribute : ActionCacheAttribute
	{
		#region Instance Fields
		private string _cacheKey;
		private Stream _filter;
		private bool _suppressResultFiltering;
		#endregion

		#region Constructors
		public ResultCacheAttribute()
            : base()
        {
        }

        public ResultCacheAttribute(string tags)
            : base(tags)
        {
        }

		public ResultCacheAttribute(string varyByKeys, string tags)
			: base(varyByKeys, tags)
        {
        }
		#endregion

		#region Instance Methods
		public override void OnExecuting(ActionExecutionContext context)
		{
			_cacheKey = CreateCacheKey(context);
			string cachedResult = GetCachedValue<string>(_cacheKey);

			if (cachedResult != null)
			{
				_suppressResultFiltering = true;

				context.Cancel = true;
				context.Result = new ContentResult() {
					Content = cachedResult
				};
			}
		}

		public override void OnExecuted(ActionExecutedContext context)
		{
			// does nothing, just overrides the base implementation.
		}

		public override void OnResultExecuting(ResultExecutionContext context)
		{
			if (_suppressResultFiltering)
				return;

			HttpResponseBase response = context.HttpContext.Response;
			context.HttpContext.Session.UpdateSessionId();

			_filter = response.Filter;
			response.Flush();

			response.Filter = new CaptureFilter(response.Filter);
		}

		public override void OnResultExecuted(ResultExecutedContext context)
		{
			if (_suppressResultFiltering)
				return;

			HttpResponseBase response = context.HttpContext.Response;
			CaptureFilter filter = (response.Filter as CaptureFilter);
			if (filter == null)
				return;

			response.Flush();
			response.Filter = _filter;

			string content = filter.GetContents(response.ContentEncoding);
			response.Write(content);

			if (context.Exception == null)
				UpdateCachedValue(_cacheKey, content);
		}
		#endregion
	}
}
