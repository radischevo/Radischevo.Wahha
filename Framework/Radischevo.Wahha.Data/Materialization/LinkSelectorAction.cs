using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
	public abstract class LinkSelectorAction<TAssociation> 
		: LinkAssociatorAction<TAssociation>
		where TAssociation : class
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

	internal abstract class SelectorActionImpl<TAssociation> : LinkSelectorAction<TAssociation>
		where TAssociation : class
	{
		#region Instance Fields
		private MethodCallExpression _selector;
		#endregion

		#region Constructors
		protected SelectorActionImpl(LambdaExpression expression)
			: base()
		{
			_selector = ConvertExpression(expression);
		}
		#endregion

		#region Instance Properties
		public MethodCallExpression Selector
		{
			get
			{
				return _selector;
			}
		}
		#endregion

		#region Static Methods
		private static MethodCallExpression ConvertExpression(LambdaExpression expression)
		{
			MethodCallExpression method = (expression.Body as MethodCallExpression);
			Precondition.Require(method, () => Error.SelectorMustBeAMethodCall("selector"));
			Precondition.Require(method.Object == expression.Parameters[0],
				() => Error.MethodCallMustTargetLambdaArgument("selector"));

			return method;
		}

		protected static object[] ExtractMethodParameters(MethodCallExpression method)
		{
			int length = method.Arguments.Count;
			object[] values = new object[length];
			if (length > 0)
			{
				for (int i = 0; i < length; i++)
					values[i] = ExtractMethodParameter(method.Arguments[i]);
			}
			return values;
		}

		private static object ExtractMethodParameter(Expression expression)
		{
			object value = null;
			ConstantExpression ce = (expression as ConstantExpression);
			if (ce == null)
			{
				Expression<Func<object>> le =
					Expression.Lambda<Func<object>>(
					Expression.Convert(expression, typeof(object)),
					new ParameterExpression[0]);
				try
				{
					value = le.Compile()();
				}
				catch
				{
					value = null;
				}
			}
			else
				value = ce.Value;

			return value;
		}
		#endregion
	}

	internal sealed class SelectorAction<TAssociation, TRepository> 
		: SelectorActionImpl<TAssociation>
		where TAssociation : class
		where TRepository : IRepository<TAssociation>
	{
		#region Constructors
		public SelectorAction(Expression<Func<TRepository, TAssociation>> selector)
			: base(selector)
		{
		}
		#endregion

		#region Instance Methods
		protected override IAssociationLoader<TAssociation> CreateSelector()
		{
			return new SingleAssociationLoader<TRepository, TAssociation>(
				Selector.Method, ExtractMethodParameters(Selector));
		}
		#endregion
	}

	internal sealed class CollectionLinkSelectorAction<TAssociation, TRepository>
		: SelectorActionImpl<IEnumerable<TAssociation>>
		where TAssociation : class
		where TRepository : IRepository<TAssociation>
	{
		#region Constructors
		public CollectionLinkSelectorAction(Expression<Func<TRepository,
			IEnumerable<TAssociation>>> selector)
			: base(selector)
		{
		}
		#endregion

		#region Instance Methods
		protected override IAssociationLoader<IEnumerable<TAssociation>> CreateSelector()
		{
			return new CollectionAssociationLoader<TRepository, TAssociation>(
				Selector.Method, ExtractMethodParameters(Selector));
		}
		#endregion
	}
}
