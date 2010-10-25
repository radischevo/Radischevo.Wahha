using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public class CollectionAssociator<TAssociation> 
		: Associator<IEnumerable<TAssociation>>
		where TAssociation : class
	{
		#region Constructors
		public CollectionAssociator(EnumerableLink<TAssociation> link)
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
