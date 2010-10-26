﻿using System;

namespace Radischevo.Wahha.Data
{
	public abstract class Associator<TAssociation>
		where TAssociation : class
	{
		#region Constructors
		protected Associator()
		{
		}
		#endregion

		#region Instance Methods
		public abstract void Execute();
		#endregion
	}
}
