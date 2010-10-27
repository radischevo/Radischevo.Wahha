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
		#region Instance Fields
		private MethodCallExpression _selector;
		#endregion

		#region Constructors
		protected LinkSelectorAction(MethodCallExpression selector)
			: base()
		{
			Precondition.Require(selector, () => Error.ArgumentNull("selector"));
			_selector = selector;
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
		private static IEnumerable<Expression> ExtractMethodParameters(MethodCallExpression method)
		{
			List<Expression> list = new List<Expression>();

			ParameterInfo[] parameters = method.Method.GetParameters();
			if (parameters.Length > 0)
			{
				for (int i = 0; i < parameters.Length; i++)
					list.Add(ExtractMethodParameter(
						method.Arguments[i], parameters[i].ParameterType));
			}
			return list;
		}

		private static Expression ExtractMethodParameter(Expression expression, Type type)
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

			return Expression.Constant(value, type);
		}
		#endregion

		#region Instance Methods
		protected abstract Func<TAssociation> CreateSelector();

		protected Func<TAssociation> CreateSelector(Type repositoryType)
		{
			BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public;

			MethodInfo resolveMethod = typeof(IServiceProvider).GetMethod("GetService",
				instanceFlags, null, new Type[] { typeof(Type) }, null);

			ConstantExpression service = Expression.Constant(ServiceLocator.Instance, typeof(IServiceLocator));
			ConstantExpression serviceType = Expression.Constant(repositoryType, typeof(Type));

			MethodCallExpression resolver = Expression.Call(service, resolveMethod, serviceType);
			UnaryExpression conversion = Expression.Convert(resolver, repositoryType);

			MethodCallExpression single = Expression.Call(conversion,
				Selector.Method, ExtractMethodParameters(Selector));
			Expression<Func<TAssociation>> func = Expression.Lambda<Func<TAssociation>>(single);

			return func.Compile();
		}

		public override ILink<TAssociation> Execute(ILink<TAssociation> link)
		{
			Precondition.Require(link, () => Error.ArgumentNull("link"));
			Func<TAssociation> selector = CreateSelector();

			link.Source = (selector == null) ? () => null : selector;
			return link;
		}
		#endregion
	}

	public class SelectorAction<TAssociation, TRepository> 
		: LinkSelectorAction<TAssociation>
		where TAssociation : class
		where TRepository : IRepository<TAssociation>
	{
		#region Constructors
		public SelectorAction(Expression<Func<TRepository, TAssociation>> selector)
			: base(ConvertExpression(selector))
		{
		}
		#endregion

		#region Static Methods
		private static MethodCallExpression ConvertExpression(Expression<Func<TRepository, TAssociation>> expression)
		{
			MethodCallExpression method = (expression.Body as MethodCallExpression);
			Precondition.Require(method, () => Error.SelectorMustBeAMethodCall("selector"));
			Precondition.Require(method.Object == expression.Parameters[0],
				() => Error.MethodCallMustTargetLambdaArgument("selector"));

			return method;
		}
		#endregion

		#region Instance Methods
		protected override Func<TAssociation> CreateSelector()
		{
			return CreateSelector(typeof(TRepository));
		}
		#endregion
	}
}
