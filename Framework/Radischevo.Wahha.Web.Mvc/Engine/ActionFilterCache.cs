using System;
using System.Collections.ObjectModel;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	internal sealed class ActionFilterCache : ReaderWriterCache<MethodInfo, ReadOnlyCollection<FilterAttribute>>
	{
		#region Constructors
		public ActionFilterCache()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public ReadOnlyCollection<FilterAttribute> GetFilters(MethodInfo method)
		{
			return base.GetOrCreate(method, () => method.GetCustomAttributes<FilterAttribute>(true).AsReadOnly());
		}
		#endregion
	}
}
