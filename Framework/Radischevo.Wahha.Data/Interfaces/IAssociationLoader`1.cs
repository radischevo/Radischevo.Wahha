using System;

namespace Radischevo.Wahha.Data
{
	public interface IAssociationLoader<TAssociation>
	{
		#region Instance Methods
		TAssociation Load();
		#endregion
	}
}
