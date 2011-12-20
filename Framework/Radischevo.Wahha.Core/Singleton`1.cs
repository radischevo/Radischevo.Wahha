using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
	public abstract class Singleton<T>
		where T : class
	{
		#region Static Fields
		private static volatile T _instance;
		private static Func<T> _factory;
		private static object _lock;
		#endregion
		
		#region Constructors
		static Singleton()
		{
			_lock = new object();
			_factory = CreateFactory();
		}
		
		protected Singleton()
		{
		}
		#endregion
		
		#region Static Properties
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
							_instance = _factory();
					}
				}
				return _instance;
			}
		}
		#endregion
		
		#region Static Methods
		private static Func<T> CreateFactory()
		{
			ConstructorInfo constructor = typeof(T).GetConstructor(BindingFlags.Instance | 
				BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
			
			Precondition.Require(constructor, () => 
				Error.CouldNotFindAppropriateConstructor(typeof(T)));
			
			return Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile();
		}
		#endregion
	}
}

