using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class ModelValidatorProviderCollection 
		: IEnumerable<IModelValidatorProvider>, IModelValidatorProvider
	{
		#region Instance Fields
		private List<IModelValidatorProvider> _collection;
		#endregion

		#region Constructors
		public ModelValidatorProviderCollection()
		{
			_collection = new List<IModelValidatorProvider>();
		}
		#endregion

		#region Instance Methods
		public void Add(IModelValidatorProvider provider)
		{
			Precondition.Require(provider, () => Error.ArgumentNull("provider"));
			_collection.Add(provider);
		}

		public void Clear()
		{
			_collection.Clear();
		}
		
		public IEnumerable<ModelValidator> GetValidators (Type modelType)
		{
			Precondition.Require(modelType, () => Error.ArgumentNull("modelType"));
			return _collection.SelectMany(a => a.GetValidators(modelType));
		}
		
		public bool Remove(IModelValidatorProvider provider)
		{
			return _collection.Remove(provider);
		}
		#endregion

		#region IEnumerable Members
		public IEnumerator<IModelValidatorProvider> GetEnumerator()
		{
			return _collection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}

