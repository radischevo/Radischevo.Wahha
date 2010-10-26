using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public class SingleLinkAssociator<TAssociation> 
		: LinkAssociator<TAssociation>
		where TAssociation : class
	{
		#region Constructors
		public SingleLinkAssociator(Link<TAssociation> link)
			: base(link)
		{
		}
		#endregion

		#region Instance Properties
		public new Link<TAssociation> Link
		{
			get
			{
				return (base.Link as Link<TAssociation>);
			}
		}
		#endregion
	}
}
