using System;

namespace Radischevo.Wahha.Data.Mapping
{
	public interface IMetaMappingFactory
	{
		#region Instance Methods
		MetaType CreateMapping (Type type);
		#endregion
	}
}

