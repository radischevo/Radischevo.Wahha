using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public class CollectionLinkAssociator<TAssociation> 
		: LinkAssociator<IEnumerable<TAssociation>>
	{
		#region Constructors
		public CollectionLinkAssociator()
			: base()
		{
		}
		#endregion
	}
}
