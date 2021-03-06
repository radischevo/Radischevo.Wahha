﻿using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public sealed class LinkValueInitializer<TEntity> : InitializerBase<TEntity>
	{
		#region Constructors
		public LinkValueInitializer(Type materializerType)
			: base(materializerType)
		{
		}
		#endregion

		#region Instance Methods
		public void Initialize(ILink<TEntity> link, IDbValueSet source)
		{
			Precondition.Require(link, () => Error.ArgumentNull("link"));
			Precondition.Require(source, () => Error.ArgumentNull("source"));

			if (link.HasValue && !Object.ReferenceEquals(link.Value, null))
				LoadLinkValue(link, source);
			else
				CreateLinkValue(link, source);
		}

		private void CreateLinkValue(ILink<TEntity> link, IDbValueSet source)
		{
			Func<IDbValueSet, TEntity> action = Creator.Build(MaterializerType);
			link.Value = action(source);
		}

		private void LoadLinkValue(ILink<TEntity> link, IDbValueSet source)
		{
			Func<TEntity, IDbValueSet, TEntity> action = Loader.Build(MaterializerType);
			action(link.Value, source);
		}
		#endregion
	}
}
