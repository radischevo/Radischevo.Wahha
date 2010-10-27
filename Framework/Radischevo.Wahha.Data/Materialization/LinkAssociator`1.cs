using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public abstract class LinkAssociator<TAssociation>
		: Associator<ILink<TAssociation>>
		where TAssociation : class
	{
		#region Constructors
		protected LinkAssociator()
			: base()
		{
		}
		#endregion
		
		#region Instance Methods
		public override ILink<TAssociation> Execute(ILink<TAssociation> association)
		{
			Precondition.Require(association, 
				() => Error.ArgumentNull("association"));

			return base.Execute(association);
		}
		#endregion
	}
}
