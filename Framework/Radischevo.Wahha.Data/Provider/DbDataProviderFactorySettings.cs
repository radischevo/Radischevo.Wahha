using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public sealed class DbDataProviderFactorySettings
	{
		#region Instance Fields
		private NameValueCollection<string> _connectionStrings;
		private IValueSet _parameters;
		#endregion
		
		#region Constructors
		public DbDataProviderFactorySettings()
		{
			_connectionStrings = new NameValueCollection<string>();
		}
		#endregion
		
		#region Instance Properties
		public NameValueCollection<string> ConnectionStrings
		{
			get
			{
				return _connectionStrings;
			}
		}
		
		public IValueSet Parameters
		{
			get
			{
				if (_parameters == null)
					_parameters = ValueSet.Empty;
				
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}
		#endregion
	}
}

