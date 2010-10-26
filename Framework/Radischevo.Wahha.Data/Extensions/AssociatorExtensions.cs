using System;

namespace Radischevo.Wahha.Data
{
	public static class AssociatorExtensions
	{
		#region Extension Methods
		public static ISingleAssociationBuilder<TAssociation> Subset<TAssociation>(
			this ISingleAssociationBuilder<TAssociation> builder, string prefix)
			where TAssociation : class
		{
			return builder.Subset(new SubsetMapper(prefix));
		}

		public static IEntityAssociationBuilder<TAssociation> Subset<TAssociation>(
			this IEntityAssociationBuilder<TAssociation> builder, string prefix)
			where TAssociation : class
		{
			return builder.Subset(new SubsetMapper(prefix));
		}

		public static ISingleAssociationBuilder<TAssociation> Scheme<TAssociation>(
			this ISingleAssociationBuilder<TAssociation> builder, params string[] columns)
			where TAssociation : class
		{
			return builder.Validate(new SubsetSchemeValidator(columns));
		}

		public static IEntityAssociationBuilder<TAssociation> Scheme<TAssociation>(
			this IEntityAssociationBuilder<TAssociation> builder, params string[] columns)
			where TAssociation : class
		{
			return builder.Validate(new SubsetSchemeValidator(columns));
		}
		#endregion
	}
}
