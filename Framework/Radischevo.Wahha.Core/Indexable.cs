using System;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public class Indexable
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
                PropertyInfo property = type.GetProperty(memberName, BindingFlags.Instance |
                    BindingFlags.Public | BindingFlags.FlattenHierarchy);
                if (property != null)
                    return property.CreateAccessor();

                FieldInfo field = type.GetField(memberName, BindingFlags.Instance |
                    BindingFlags.Public | BindingFlags.FlattenHierarchy);

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

        #region Instance Fields
        private object _instance;
        private Type _type;
        private MemberAccessorCache _cache;
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
        protected virtual IDynamicAccessor GetAccessorAndValidate(string memberName)
        {
            Precondition.Require(!String.IsNullOrEmpty(memberName), 
                Error.ArgumentNull("memberName"));

            IDynamicAccessor accessor = _cache.GetAccessor(memberName);
            if (accessor == null)
                throw Error.MissingMember(_type, memberName);

            return accessor;
        }

        public virtual TValue GetValue<TValue>(string memberName)
        {
            return Converter.ChangeType<TValue>(this[memberName]);
        }

        public virtual void SetValue<TValue>(string memberName, TValue value)
        {
            this[memberName] = value;
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
