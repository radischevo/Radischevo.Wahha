using System;

namespace Radischevo.Wahha.Core
{
	public interface ILink<T>
	{
		#region Instance Properties
		bool HasValue
		{
			get;
		}

		Func<T> Source
		{
			get;
			set;
		}

		object Tag
		{
			get;
			set;
		}
		#endregion

		#region Instance Methods
		void Load();
		#endregion
	}
}
