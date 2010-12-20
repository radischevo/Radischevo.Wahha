using System;

namespace Radischevo.Wahha.Data
{
	[Serializable]
	public class DefaultAssociationLoader<TAssociation> : IAssociationLoader<TAssociation>
		where TAssociation : class
	{
		#region Constructors
		public DefaultAssociationLoader()
		{
		}
		#endregion

		#region Instance Methods
		public TAssociation Load()
		{
			return default(TAssociation);
		}
		#endregion
	}
}
