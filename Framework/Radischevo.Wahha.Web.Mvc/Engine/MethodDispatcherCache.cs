using System;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class MethodDispatcherCache 
		: ReaderWriterCache<MethodInfo, MethodDispatcher>
	{
		#region Constructors
		public MethodDispatcherCache()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public MethodDispatcher GetDispatcher(MethodInfo method)
		{
			return GetOrCreate(method, () => new MethodDispatcher(method));
		}
		#endregion
	}
}
