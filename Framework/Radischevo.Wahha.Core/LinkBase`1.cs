using System;
using System.Runtime.Serialization;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
	/// <summary>
	/// Defines a base class that implements lazy object retrieval.
	/// </summary>
	/// <typeparam name="T">The type of linked object.</typeparam>
	[Serializable]
	public abstract class LinkBase<T> : ILink<T>, ISerializable
	{
		#region Nested Types
		[Serializable]
		private class Boxed
		{
			#region Instance Fields
			public T Value;
			#endregion

			#region Constructors
			public Boxed(T value)
			{
				Value = value;
			}
			#endregion
		}
		#endregion

		#region Static Methods
		private static Func<T> _defaultSource;
		#endregion

		#region Instance Fields
		private readonly object _lock = new object();
        private bool _hasLoadedValue;
        private bool _hasAssignedValue;
        private Func<T> _source;
		private object _tag;
        private volatile Boxed _value;
        #endregion

        #region Constructors
		static LinkBase()
		{
			_defaultSource = () => default(T);
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="Radischevo.Wahha.Core.LinkBase{T}"/> class.
		/// </summary>
		protected LinkBase()
			: this(null)
		{
		}

        /// <summary>
        /// Initializes a new instance 
        /// of the <see cref="Radischevo.Wahha.Core.LinkBase{T}"/> class.
        /// </summary>
        protected LinkBase(Func<T> source)
        {
			Source = source ?? _defaultSource;
        }

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="Radischevo.Wahha.Core.LinkBase{T}"/> class.
		/// </summary>
		protected LinkBase(T value)
		{
			Value = value;
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="Radischevo.Wahha.Core.LinkBase{T}"/> class 
		/// with serialized data.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> 
		/// that holds the serialized object data.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> 
		/// that contains contextual information about the source or destination.</param>
		protected LinkBase(SerializationInfo info, StreamingContext context)
		{
			if (info.GetBoolean("serializable"))
			{
				_source = (Func<T>)info.GetValue("source", typeof(Func<T>));
			}
			else
			{
				if (!info.GetBoolean("dynamicMethod"))
				{
					MethodInfo method = (MethodInfo)info.GetValue("sourceMethod", typeof(MethodInfo));

					object target = Activator.CreateInstance(method.DeclaringType, true);
					foreach (FieldInfo field in method.DeclaringType.GetFields())
						field.SetValue(target, info.GetValue("source" + field.Name, field.FieldType));

					_source = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), target, method);
				}
			}
			_value = (Boxed)info.GetValue("value", typeof(Boxed));
			_hasAssignedValue = info.GetBoolean("hasAssignedValue");
			_hasLoadedValue = info.GetBoolean("hasLoadedValue");
			_tag = info.GetValue("tag", typeof(object));
		}
        #endregion

        #region Instance Properties
		/// <summary>
		/// Gets a value, indicating 
		/// whether a linked object value 
		/// has been retrieved from the 
		/// provided source.
		/// </summary>
		protected bool HasLoadedValue
		{
			get
			{
				return _hasLoadedValue;
			}
		}

		/// <summary>
		/// Gets a value, indicating 
		/// whether a linked object value 
		/// has been explicitly assigned.
		/// </summary>
		protected bool HasAssignedValue
		{
			get
			{
				return _hasAssignedValue;
			}
		}

        /// <summary>
        /// Gets a value, indicating 
        /// whether a linked object 
        /// is loaded into the link.
        /// </summary>
        public virtual bool HasValue
        {
            get 
            {
                return (_hasLoadedValue || _hasAssignedValue);
            }
        }

        /// <summary>
        /// Gets or sets a delegate function, 
        /// which will be used to load the 
        /// linked object.
        /// </summary>
        public Func<T> Source
        {
            get
            {
                return _source;
            }
            set
            {
				_hasLoadedValue = false;
                _source = value;
            }
        }

		/// <summary>
		/// Gets or sets the specific 
		/// tag that can help to determine the 
		/// linked object without loading.
		/// </summary>
		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

		/// <summary>
		/// Gets or sets a linked object 
		/// for this instance.
		/// </summary>
		public T Value
		{
			get
			{
				if (!HasValue)
					Load();

				return _value.Value;
			}
			set
			{
				_hasAssignedValue = true;
				_value = new Boxed(value);
			}
		}
        #endregion

		#region Instance Methods
		/// <summary>
		/// When overriden in a derived class, 
		/// loads the value using the provided delegate.
		/// </summary>
		protected virtual T CreateValue()
		{
			Precondition.Require(_source, () =>
				Error.LinkSourceIsNotInitialized());

			return _source();
		}

		/// <summary>
        /// Explicitly loads the linked object 
        /// into the link.
        /// </summary>
        public virtual void Load()
        {
            if (!_hasLoadedValue)
            {
				lock (_lock)
				{
					if (!_hasLoadedValue)
					{
						_value = new Boxed(CreateValue());
						_hasLoadedValue = true;
					}
				}
            }
        }

		/// <summary>
		/// When overridden in a derived class, 
		/// resets the link state.
		/// </summary>
		public virtual void Reset()
		{
			_hasLoadedValue = false;
		}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents 
        /// the current <see cref="Radischevo.Wahha.Core.LinkBase{T}"/>.
        /// </summary>
        public override string ToString()
        {
			return (_value == null) ?
                String.Format("LinkBase`1[[{0}]]", typeof(T).FullName) :
				_value.ToString();
        }
        #endregion

		#region Serialization Methods
		/// <summary>
		/// When overridden in a derived class, populates a 
		/// <see cref="T:System.Runtime.Serialization.SerializationInfo"/> 
		/// with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination for this serialization.</param>
		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			Precondition.Require(info, () => Error.ArgumentNull("info"));

			if (_source == null || _source.IsSerializable())
			{
				info.AddValue("serializable", true);
				info.AddValue("source", _source);
			}
			else
			{
				info.AddValue("serializable", false);

				Type declaringType = _source.Method.DeclaringType;
				if (declaringType == null)
				{
					info.AddValue("dynamicMethod", true);
					T value = Value;
				}
				else
				{
					info.AddValue("dynamicMethod", false);
					info.AddValue("sourceMethod", _source.Method);
					foreach (FieldInfo field in declaringType.GetFields())
						info.AddValue("source" + field.Name, field.GetValue(_source.Target));
				}
			}

			info.AddValue("value", _value);
			info.AddValue("hasAssignedValue", _hasAssignedValue);
			info.AddValue("hasLoadedValue", _hasLoadedValue);
			info.AddValue("tag", _tag);
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			GetObjectData(info, context);
		}
		#endregion
	}
}