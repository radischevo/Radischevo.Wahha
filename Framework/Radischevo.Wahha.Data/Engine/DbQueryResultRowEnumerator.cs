using System;
using System.Collections;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	internal sealed class DbQueryResultRowEnumerator : IEnumerator<DbQueryResultRow>, 
		IEnumerator<IDbDataRecord>, IEnumerator, IDisposable
	{
		#region Instance Fields
		private DbQueryResultRow _current;
		private IEnumerator<DbQueryResultRow> _enumerator;
		#endregion

		#region Constructors
		public DbQueryResultRowEnumerator(IEnumerable<DbQueryResultRow> collection)
		{
			Precondition.Require(collection, () => Error.ArgumentNull("collection"));
			_enumerator = collection.GetEnumerator();
		}
		#endregion

		#region Instance Properties
		public DbQueryResultRow Current
		{
			get
			{
				return _current;
			}
		}

		IDbDataRecord IEnumerator<IDbDataRecord>.Current
		{
			get
			{
				return Current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}
		#endregion

		#region Instance Methods
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			IDisposable d = (_enumerator as IDisposable);
			if (d != null)
				d.Dispose();
		}

		public bool MoveNext()
		{
			if (_enumerator.MoveNext())
			{
				_current = _enumerator.Current;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			_enumerator.Reset();
		}
		#endregion
	}
}
