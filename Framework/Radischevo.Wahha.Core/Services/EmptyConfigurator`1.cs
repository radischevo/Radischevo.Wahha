using System;

namespace Radischevo.Wahha.Core
{
	public sealed class EmptyConfigurator<TModule> : IConfigurator<TModule>
		where TModule : class
	{
		#region Constructors
		public EmptyConfigurator()
		{
		}
		#endregion
		
		#region Instance Methods
		public void Configure(TModule module)
		{
		}
		#endregion
	}
}

