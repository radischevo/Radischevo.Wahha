using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Core
{
	public interface IEnumerableLink<T> : ILink<IEnumerable<T>>, IEnumerable<T>
	{
	}
}
