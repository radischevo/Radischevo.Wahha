using System;

namespace Radischevo.Wahha.Data
{
	public interface IAssociationLoader<TAssociation>
		where TAssociation : class
	{
		#region Instance Methods
		TAssociation Load();
		#endregion
	}
}
