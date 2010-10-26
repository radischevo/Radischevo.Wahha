using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public class EntityAssociator<TAssociation>
		: Associator<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private TAssociation _entity;
		private List<EntityAssociatorAction<TAssociation>> _actions;
		#endregion

		#region Constructors
		public EntityAssociator(TAssociation entity)
		{
			Precondition.Require(entity, () => Error.ArgumentNull("entity"));

			_entity = entity;
			_actions = new List<EntityAssociatorAction<TAssociation>>();
		}
		#endregion

		#region Instance Properties
		public TAssociation Entity
		{
			get
			{
				return _entity;
			}
		}

		public ICollection<EntityAssociatorAction<TAssociation>> Actions
		{
			get
			{
				return _actions;
			}
		}
		#endregion

		#region Instance Methods
		protected virtual IEnumerable<EntityAssociatorAction<TAssociation>> GetOrderedActions()
		{
			return Actions.Where(a => a.Order > 0).OrderBy(a => a.Order)
				.Concat(Actions.Where(a => a.Order <= 0));
		}

		public override void Execute()
		{
			foreach (EntityAssociatorAction<TAssociation> action in GetOrderedActions())
				action.Execute(Entity);
		}
		#endregion
	}
}
