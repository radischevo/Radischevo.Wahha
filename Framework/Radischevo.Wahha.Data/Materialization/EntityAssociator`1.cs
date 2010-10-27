using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public class EntityAssociator<TAssociation>
		: Associator<TAssociation>
		where TAssociation : class
	{
		#region Constructors
		public EntityAssociator()
			: base()
		{
		}
		#endregion
	}
}
