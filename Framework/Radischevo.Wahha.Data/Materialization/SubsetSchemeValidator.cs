using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public sealed class SubsetSchemeValidator : IDbValueSetValidator
	{
		#region Instance Fields
		private List<string> _scheme;
		#endregion

		#region Constructors
		public SubsetSchemeValidator(params string[] columns)
		{
			Precondition.Require(columns, () => Error.ArgumentNull("columns"));
			_scheme = new List<string>(columns);
		}
		#endregion

		#region Instance Properties
		public ICollection<string> Scheme
		{
			get
			{
				return _scheme;
			}
		}
		#endregion

		#region Instance Methods
		public bool Valid(IDbValueSet source)
		{
			Precondition.Require(source, () => Error.ArgumentNull("source"));
			return source.ContainsAll(_scheme.ToArray());
		}
		#endregion
	}
}
