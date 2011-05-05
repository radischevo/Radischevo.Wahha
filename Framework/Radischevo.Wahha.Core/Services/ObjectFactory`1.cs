using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Core
{
	public class ObjectFactory<TObject> : IObjectFactory<TObject>
		where TObject : class
	{
		#region Instance Fields
		private Func<TObject> _factory;
		#endregion

		#region Constructors
		public ObjectFactory()
		{
			_factory = ObjectFactoryBuilder.CreateFactory<TObject>();
		}
		#endregion

		#region Instance Methods
		public virtual TObject Create()
		{
			return _factory();
		}
		#endregion
	}

	public class ObjectFactory<TObject, TParameter> : IObjectFactory<TObject, TParameter>
		where TObject : class
	{
		#region Instance Fields
		private Func<TParameter, TObject> _factory;		 
		#endregion

		#region Constructors
		public ObjectFactory()
		{
			_factory = ObjectFactoryBuilder.CreateFactory<TParameter, TObject>();
		}
		#endregion

		#region Instance Methods
		public virtual TObject Create(TParameter parameter)
		{
			return _factory(parameter);
		}
		#endregion
	}

	public class ObjectFactory<TObject, TParameter1, TParameter2> 
		: IObjectFactory<TObject, TParameter1, TParameter2>
		where TObject : class
	{
		#region Instance Fields
		private Func<TParameter1, TParameter2, TObject> _factory;
		#endregion

		#region Constructors
		public ObjectFactory()
		{
			_factory = ObjectFactoryBuilder
				.CreateFactory<TParameter1, TParameter2, TObject>();
		}
		#endregion

		#region Instance Methods
		public virtual TObject Create(TParameter1 parameter1, TParameter2 parameter2)
		{
			return _factory(parameter1, parameter2);
		}
		#endregion
	}

	public class ObjectFactory<TObject, TParameter1, TParameter2, TParameter3>
		: IObjectFactory<TObject, TParameter1, TParameter2, TParameter3>
		where TObject : class
	{
		#region Instance Fields
		private Func<TParameter1, TParameter2, TParameter3, TObject> _factory;
		#endregion

		#region Constructors
		public ObjectFactory()
		{
			_factory = ObjectFactoryBuilder
				.CreateFactory<TParameter1, TParameter2, TParameter3, TObject>();
		}
		#endregion

		#region Instance Methods
		public virtual TObject Create(TParameter1 parameter1, 
			TParameter2 parameter2, TParameter3 parameter3)
		{
			return _factory(parameter1, parameter2, parameter3);
		}
		#endregion
	}

	public class ObjectFactory<TObject, TParameter1, TParameter2, TParameter3, TParameter4>
		: IObjectFactory<TObject, TParameter1, TParameter2, TParameter3, TParameter4>
		where TObject : class
	{
		#region Instance Fields
		private Func<TParameter1, TParameter2, TParameter3, TParameter4, TObject> _factory;
		#endregion

		#region Constructors
		public ObjectFactory()
		{
			_factory = ObjectFactoryBuilder.CreateFactory<TParameter1, TParameter2, 
				TParameter3, TParameter4, TObject>();
		}
		#endregion

		#region Instance Methods
		public virtual TObject Create(TParameter1 parameter1, TParameter2 parameter2, 
			TParameter3 parameter3, TParameter4 parameter4)
		{
			return _factory(parameter1, parameter2, parameter3, parameter4);
		}
		#endregion
	}
}
