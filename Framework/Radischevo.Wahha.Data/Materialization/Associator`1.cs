using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public abstract class Associator<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private ILink<TAssociation> _link;
		private List<AssociatorAction<TAssociation>> _actions;
		#endregion

		#region Constructors
		protected Associator(ILink<TAssociation> link)
		{
			Precondition.Require(link, () => Error.ArgumentNull("link"));

			_link = link;
			_actions = new List<AssociatorAction<TAssociation>>();
		}
		#endregion

		#region Instance Properties
		public ILink<TAssociation> Link
		{
			get
			{
				return _link;
			}
		}

		public ICollection<AssociatorAction<TAssociation>> Actions
		{
			get
			{
				return _actions;
			}
		}
		#endregion

		#region Instance Methods
		protected virtual IEnumerable<AssociatorAction<TAssociation>> GetOrderedActions()
		{
			return Actions.Where(a => a.Order > 0).OrderBy(a => a.Order)
				.Concat(Actions.Where(a => a.Order <= 0));
		}

		public void Execute()
		{
			foreach (AssociatorAction<TAssociation> action in GetOrderedActions())
				action.Execute(Link);
		}
		#endregion
	}
}
