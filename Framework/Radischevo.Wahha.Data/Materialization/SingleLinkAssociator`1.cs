using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public class SingleLinkAssociator<TAssociation> 
		: LinkAssociator<TAssociation>
		where TAssociation : class
	{
		#region Constructors
		public SingleLinkAssociator()
			: base()
		{
		}
		#endregion
	}
}
