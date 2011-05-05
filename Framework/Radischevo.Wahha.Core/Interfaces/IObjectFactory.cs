using System;

namespace Radischevo.Wahha.Core
{
	public interface IObjectFactory<TObject>
		where TObject : class
	{
		#region Instance Methods
		TObject Create();
		#endregion
	}

	public interface IObjectFactory<TObject, TParameter>
		where TObject : class
	{
		#region Instance Methods
		TObject Create(TParameter parameter);
		#endregion
	}

	public interface IObjectFactory<TObject, TParameter1, TParameter2>
		where TObject : class
	{
		#region Instance Methods
		TObject Create(TParameter1 parameter1, TParameter2 parameter2);
		#endregion
	}

	public interface IObjectFactory<TObject, TParameter1, 
		TParameter2, TParameter3>
		where TObject : class
	{
		#region Instance Methods
		TObject Create(TParameter1 parameter1, 
			TParameter2 parameter2, TParameter3 parameter3);
		#endregion
	}

	public interface IObjectFactory<TObject, TParameter1, 
		TParameter2, TParameter3, TParameter4>
		where TObject : class
	{
		#region Instance Methods
		TObject Create(TParameter1 parameter1, TParameter2 parameter2, 
			TParameter3 parameter3, TParameter4 parameter4);
		#endregion
	}
}