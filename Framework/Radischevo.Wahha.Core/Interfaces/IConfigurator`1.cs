using System;

namespace Radischevo.Wahha.Core
{
	public interface IConfigurator<TModule>
		where TModule : class
	{
		#region Instance Methods
		void Configure(TModule module);
		#endregion
	}
}

