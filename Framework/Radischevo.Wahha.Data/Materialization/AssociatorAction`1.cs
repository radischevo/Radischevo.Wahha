﻿using System;

namespace Radischevo.Wahha.Data
{
	public abstract class AssociatorAction<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private int _order;
		#endregion

		#region Constructors
		protected AssociatorAction()
		{
		}
		#endregion

		#region Instance Properties
		public int Order
		{
			get
			{
				return _order;
			}
			set
			{
				_order = value;
			}
		}
		#endregion
	}
}
