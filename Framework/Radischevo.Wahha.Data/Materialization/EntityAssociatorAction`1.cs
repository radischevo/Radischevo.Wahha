using System;

namespace Radischevo.Wahha.Data
{
	public abstract class EntityAssociatorAction<TAssociation>
		: AssociatorAction<TAssociation>
		where TAssociation : class
	{
		#region Constructors
		protected EntityAssociatorAction()
		{
		}
		#endregion
	}
}
