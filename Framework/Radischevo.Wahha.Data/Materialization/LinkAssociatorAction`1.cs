using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public abstract class LinkAssociatorAction<TAssociation>
		: AssociatorAction<TAssociation>
		where TAssociation : class
	{
		#region Constructors
		protected LinkAssociatorAction()
		{
		}
		#endregion

		#region Instance Methods
		public abstract void Execute(ILink<TAssociation> link);
		#endregion
	}
}
