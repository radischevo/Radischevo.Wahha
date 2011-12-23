using System;

namespace Radischevo.Wahha.Core.Configurations
{
	public sealed class ServiceLocationSettings
	{
		#region Instance Fields
		private Type _providerType;
		#endregion

		#region Constructors
		internal ServiceLocationSettings()
		{
		}
		#endregion

		#region Instance Properties
		public Type ServiceLocatorType
		{
			get
			{
				if (_providerType == null)
					_providerType = typeof(DefaultServiceLocator);

				return _providerType;
			}
			set
			{
				_providerType = value;
			}
		}
		#endregion
	}
}
