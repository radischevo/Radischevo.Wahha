using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	public class MetaMemberCollection : KeyedCollection<string, MetaMember>, IKeyedEnumerable<string, MetaMember>
	{
		#region Constructors
		public MetaMemberCollection ()
			: this (StringComparer.OrdinalIgnoreCase)
		{
		}
		
		public MetaMemberCollection(IEqualityComparer<string> comparer)
			: base (comparer)
		{
		}
		#endregion
		
		#region Instance Methods
		protected override string GetKeyForItem (MetaMember item)
		{
			return item.GetMemberKey();
		}
		#endregion
	}
}

