using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public class Indexable : IValueSet
    {
        #region Nested Types
        private sealed class MemberAccessorCache : ReaderWriterCache<string, IDynamicAccessor>
        {
            #region Instance Fields
            private Type _type;
            #endregion

            #region Constructors
            public MemberAccessorCache(Type type)
                : base()
            {
                Precondition.Require(type, Error.ArgumentNull("type"));
                _type = type;
            }
            #endregion

            #region Static Methods
            private static IDynamicAccessor CreateAccessor(Type type, string memberName)
            {
                PropertyInfo property = type.GetProperty(memberName, _memberFlags);
                if (property != null)
                    return property.CreateAccessor();

                FieldInfo field = type.GetField(memberName, _memberFlags);

                if (field != null)
                    return field.CreateAccessor();

                return null;
            }
            #endregion

            #region Instance Methods
            public IDynamicAccessor GetAccessor(string memberName)
            {
                return base.GetOrCreate(memberName, () => {
                    return CreateAccessor(_type, memberName);
                });
            }
            #endregion
        }
        #endregion

		#region Constants
		private const BindingFlags _memberFlags = BindingFlags.Instance |
			BindingFlags.Public | BindingFlags.FlattenHierarchy;
		#endregion

		#region Instance Fields
		private object _instance;
        private Type _type;
        private MemberAccessorCache _cache;
		private IEnumerable<string> _keys;
        #endregion

        #region Constructors
        public Indexable(object instance)
        {
            Precondition.Require(instance, 
                Error.ArgumentNull("instance"));

            _instance = instance;
            _type = instance.GetType();
            _cache = new MemberAccessorCache(_type);
        }
        #endregion

        #region Instance Properties
        public object Instance
        {
            get
            {
                return _instance;
            }
        }

        public object this[string memberName]
        {
            get
            {
                IDynamicAccessor accessor = GetAccessorAndValidate(memberName);
                return accessor.GetValue(_instance);
            }
            set
            {
                IDynamicAccessor accessor = GetAccessorAndValidate(memberName);
                accessor.SetValue(_instance, value);
            }
        }
        #endregion

        #region Instance Methods
		private IDynamicAccessor GetAccessorAndValidate(string memberName)
		{
			IDynamicAccessor accessor = GetAccessor(memberName);
			if (accessor == null)
				throw Error.MissingMember(_type, memberName);

			return accessor;
		}

		protected virtual IDynamicAccessor GetAccessor(string memberName)
		{
			Precondition.Require(!String.IsNullOrEmpty(memberName),
				Error.ArgumentNull("memberName"));

			return _cache.GetAccessor(memberName);
		}

        public TValue GetValue<TValue>(string memberName)
        {
			return GetValue<TValue>(memberName, default(TValue));
        }

		public virtual TValue GetValue<TValue>(string memberName, TValue defaultValue)
		{
			IDynamicAccessor accessor = GetAccessor(memberName);
			if (accessor == null)
				return defaultValue;

			return Converter.ChangeType<TValue>(accessor.GetValue(_instance));
		}

        public virtual void SetValue<TValue>(string memberName, TValue value)
        {
            this[memberName] = value;
        }
        #endregion

		#region IValueSet Members
		IEnumerable<string> IValueSet.Keys
		{
			get
			{
				if (_keys == null)
					_keys = _type.GetFields(_memberFlags).Select(f => f.Name)
						.Concat(_type.GetProperties(_memberFlags).Select(p => p.Name))
						.ToArray();

				return _keys;
			}
		}
		#endregion
	}

    public class Indexable<T> : Indexable
        where T : class
    {
        #region Constructors
        public Indexable(T instance) 
            : base(instance)
        {
        }
        #endregion

        #region Instance Properties
        public new T Instance
        {
            get
            {
                return (T)base.Instance;
            }
        }
        #endregion
    }
}
