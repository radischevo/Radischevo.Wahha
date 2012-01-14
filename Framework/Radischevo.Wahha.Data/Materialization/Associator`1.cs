using System;
using System.Collections.Generic;
using System.Linq;

namespace Radischevo.Wahha.Data
{
	public abstract class Associator<T>
	{
		#region Instance Fields
		private List<AssociatorAction<T>> _actions;
		#endregion

		#region Constructors
		protected Associator()
		{
			_actions = new List<AssociatorAction<T>>();
		}
		#endregion

		#region Instance Properties
		public ICollection<AssociatorAction<T>> Actions
		{
			get
			{
				return _actions;
			}
		}
		#endregion

		#region Instance Methods
		protected virtual IEnumerable<AssociatorAction<T>> GetOrderedActions()
		{
			return Actions.Where(a => a.Order > 0).OrderBy(a => a.Order)
				.Concat(Actions.Where(a => a.Order <= 0));
		}

		public virtual T Execute(T association)
		{
			T value = association;
			foreach (AssociatorAction<T> action in GetOrderedActions())
				value = action.Execute(value);

			return value;
		}
		#endregion
	}
}
