using System;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public abstract class LinkAssociator<TAssociation>
		: Associator<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private ILink<TAssociation> _link;
		private List<LinkAssociatorAction<TAssociation>> _actions;
		#endregion

		#region Constructors
		protected LinkAssociator(ILink<TAssociation> link)
		{
			Precondition.Require(link, () => Error.ArgumentNull("link"));

			_link = link;
			_actions = new List<LinkAssociatorAction<TAssociation>>();
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

		public ICollection<LinkAssociatorAction<TAssociation>> Actions
		{
			get
			{
				return _actions;
			}
		}
		#endregion

		#region Instance Methods
		protected virtual IEnumerable<LinkAssociatorAction<TAssociation>> GetOrderedActions()
		{
			return Actions.Where(a => a.Order > 0).OrderBy(a => a.Order)
				.Concat(Actions.Where(a => a.Order <= 0));
		}

		public override void Execute()
		{
			foreach (LinkAssociatorAction<TAssociation> action in GetOrderedActions())
				action.Execute(Link);
		}
		#endregion
	}
}
