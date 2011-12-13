using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;

namespace Radischevo.Wahha.Web.Mvc
{
	public class DataAnnotationsModelValidatorProvider : AssociatedModelValidatorProvider
	{
		#region Static Fields
		private static Dictionary<Type, DataAnnotationsModelValidationFactory> _factories;
		private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
		#endregion
		
		#region Constructors
		static DataAnnotationsModelValidatorProvider()
		{
			_factories = new Dictionary<Type, DataAnnotationsModelValidationFactory>();
			_factories.Add(typeof(RangeAttribute), a => 
				new DataAnnotationsModelValidator<RangeAttribute>((RangeAttribute)a));
			_factories.Add(typeof(RegularExpressionAttribute), a => 
				new DataAnnotationsModelValidator<RegularExpressionAttribute>((RegularExpressionAttribute)a));
			_factories.Add(typeof(StringLengthAttribute), a => 
				new DataAnnotationsModelValidator<StringLengthAttribute>((StringLengthAttribute)a));
			_factories.Add(typeof(RequiredAttribute), a => 
				new DataAnnotationsModelValidator<RequiredAttribute>((RequiredAttribute)a));
		}
		
		public DataAnnotationsModelValidatorProvider ()
			: base()
		{
		}
		#endregion
		
		#region Static Methods
		public static void Register<TAttribute, TAdapter>() 
			where TAttribute : ValidationAttribute
			where TAdapter: ModelValidator
		{
            ConstructorInfo constructor = typeof(TAdapter).GetConstructor(new Type[] { typeof(TAttribute) });
			Precondition.Require(constructor, () => Error.InvalidDataAnnotationsValidationRuleConstructor(
				typeof(TAdapter), typeof(TAttribute)));
			
			_lock.EnterWriteLock();
			
            try
			{
                _factories[typeof(TAttribute)] = (a) => (ModelValidator)constructor.Invoke(new object[] { a });
            }
            finally
			{
                _lock.ExitWriteLock();
            }
			
        }

        public static void Register<TAttribute> (DataAnnotationsModelValidationFactory factory) 
		{
			Precondition.Require(factory, () => Error.ArgumentNull("factory"));
			_lock.EnterWriteLock();
			
            try 
			{
                _factories[typeof(TAttribute)] = factory;
            }
            finally
			{
                _lock.ExitWriteLock();
            }
        }
		#endregion
		
		#region Instance Methods
		protected override ICustomTypeDescriptor GetTypeDescriptor(Type modelType)
		{
			return new AssociatedMetadataTypeTypeDescriptionProvider(modelType).GetTypeDescriptor(modelType);
		}
		
		protected override IEnumerable<ModelValidator> GetValidators (IEnumerable<Attribute> attributes)
		{
			List<ModelValidator> results = new List<ModelValidator>();
			_lock.EnterReadLock();
			try
			{
				foreach (ValidationAttribute attr in attributes.OfType<ValidationAttribute>())
				{
					DataAnnotationsModelValidationFactory factory;
					if (_factories.TryGetValue(attr.GetType(), out factory)) 
						results.Add(factory(attr));
				}
				return results;
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}
		#endregion
	}
}

