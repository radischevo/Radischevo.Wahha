using System;

namespace Radischevo.Wahha.Core.Async
{
	/// <summary>
	/// Provides a trigger for the <see cref="Radischevo.Wahha.Core.Async.TriggerListener"/> class.
	/// </summary>
	public sealed class Trigger
	{
		#region Instance Fields
		private readonly Action _fireAction;
		#endregion

		#region Constructors
		internal Trigger(Action fireAction)
		{
			_fireAction = fireAction;
		}
		#endregion

		#region Instance Methods
		public void Fire()
		{
			_fireAction();
		}
		#endregion
	}
}
