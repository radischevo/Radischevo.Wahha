using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public class CollectionLinkAssociator<TAssociation> 
		: LinkAssociator<IEnumerable<TAssociation>>
		where TAssociation : class
	{
		#region Constructors
		public CollectionLinkAssociator(EnumerableLink<TAssociation> link)
			: base(link)
		{
		}
		#endregion

		#region Instance Properties
		public new EnumerableLink<TAssociation> Link
		{
			get
			{
				return (base.Link as EnumerableLink<TAssociation>);
			}
		}
		#endregion
	}
}
