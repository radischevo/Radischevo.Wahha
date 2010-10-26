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

		#region Instance Methods
		public abstract void Execute(TAssociation entity);
		#endregion
	}
}
