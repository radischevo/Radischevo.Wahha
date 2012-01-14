using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public abstract class LinkSelectorAction<TAssociation> 
		: LinkAssociatorAction<TAssociation>
	{
		#region Static Fields
		private static readonly IAssociationLoader<TAssociation> _defaultLoader 
			= new DefaultAssociationLoader<TAssociation>();
		#endregion

		#region Constructors
		protected LinkSelectorAction()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		protected abstract IAssociationLoader<TAssociation> CreateSelector();

		public override ILink<TAssociation> Execute(ILink<TAssociation> link)
		{
			Precondition.Require(link, () => Error.ArgumentNull("link"));
			IAssociationLoader<TAssociation> loader = CreateSelector() ?? _defaultLoader;
			Func<TAssociation> selector = loader.Load;

			link.Source = selector;
			return link;
		}
		#endregion
	}
}
