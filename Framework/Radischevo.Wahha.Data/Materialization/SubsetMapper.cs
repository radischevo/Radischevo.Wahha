using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public sealed class SubsetMapper : IDbValueSetTransformer
	{
		#region Instance Fields
		private string _prefix;
		#endregion

		#region Constructors
		public SubsetMapper()
		{
		}

		public SubsetMapper(string prefix)
			: this()
		{
			_prefix = prefix;
		}
		#endregion

		#region Instance Properties
		public string Prefix
		{
			get
			{
				return _prefix;
			}
			set
			{
				_prefix = value;
			}
		}
		#endregion

		#region Instance Methods
		public IDbValueSet Transform(IDbValueSet source)
		{
			if (String.IsNullOrEmpty(_prefix))
				return source;

			int prefixLength = _prefix.Length;
			return source.Subset(k => k.StartsWith(_prefix))
				.Transform(k => k.Substring(prefixLength));
		}
		#endregion
	}
}
