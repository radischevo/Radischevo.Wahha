using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class ActionFilterInfo
	{
		#region Instance Fields
		private List<IActionFilter> _actionFilters;
		private List<IAuthorizationFilter> _authorizationFilters;
		private List<IExceptionFilter> _exceptionFilters;
		private List<IResultFilter> _resultFilters;
		#endregion

		#region Constructors
		public ActionFilterInfo()
		{
			_actionFilters = new List<IActionFilter>();
			_authorizationFilters = new List<IAuthorizationFilter>();
			_exceptionFilters = new List<IExceptionFilter>();
			_resultFilters = new List<IResultFilter>();
		}

		public ActionFilterInfo(IEnumerable<Filter> filters)
			: this()
		{
			Precondition.Require(filters, () => Error.ArgumentNull("filters"));
			List<object> instances = filters.Select(f => f.Instance).ToList();

			_actionFilters.AddRange(instances.OfType<IActionFilter>());
			_authorizationFilters.AddRange(instances.OfType<IAuthorizationFilter>());
			_exceptionFilters.AddRange(instances.OfType<IExceptionFilter>());
			_resultFilters.AddRange(instances.OfType<IResultFilter>());
		}
		#endregion

		#region Instance Properties
		public ICollection<IActionFilter> ActionFilters
		{
			get
			{
				return _actionFilters;
			}
		}

		public ICollection<IAuthorizationFilter> AuthorizationFilters
		{
			get
			{
				return _authorizationFilters;
			}
		}

		public ICollection<IExceptionFilter> ExceptionFilters
		{
			get
			{
				return _exceptionFilters;
			}
		}

		public ICollection<IResultFilter> ResultFilters
		{
			get
			{
				return _resultFilters;
			}
		}
		#endregion
	}
}
