using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	public class AttributedMetaMappingFactory : ReaderWriterCache<Type, MetaType>, IMetaMappingFactory
	{
		#region Constructors
		public AttributedMetaMappingFactory ()
			: base()
		{
		}
		#endregion
		
		#region Instance Methods
		public MetaType CreateMapping (Type type)
		{
			return base.GetOrCreate(type, () => new AttributedMetaType(type));
		}
		#endregion
	}
}

