using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public sealed class LinkValueInitializer<TEntity> : InitializerBase<TEntity>
		where TEntity : class
	{
		#region Constructors
		public LinkValueInitializer(Type materializerType)
			: base(materializerType)
		{
		}
		#endregion

		#region Instance Methods
		public void Initialize(Link<TEntity> link, IValueSet source)
		{
			Precondition.Require(link, () => Error.ArgumentNull("link"));
			Precondition.Require(source, () => Error.ArgumentNull("source"));

			if (link.HasValue && !Object.ReferenceEquals(link.Value, null))
				LoadLinkValue(link, source);
			else
				CreateLinkValue(link, source);
		}

		private void CreateLinkValue(Link<TEntity> link, IValueSet source)
		{
			Func<IValueSet, TEntity> action = Creator.Build(MaterializerType);
			link.Value = action(source);
		}

		private void LoadLinkValue(Link<TEntity> link, IValueSet source)
		{
			Func<TEntity, IValueSet, TEntity> action = Loader.Build(MaterializerType);
			action(link.Value, source);
		}
		#endregion
	}
}
