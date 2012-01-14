using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public abstract class LinkAssociatorAction<TAssociation>
		: AssociatorAction<ILink<TAssociation>>
	{
		#region Constructors
		protected LinkAssociatorAction()
		{
		}
		#endregion
	}
}
