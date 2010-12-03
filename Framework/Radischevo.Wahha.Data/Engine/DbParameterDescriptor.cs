using System;
using System.Data;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data
{
    public class DbParameterDescriptor
    {
        #region Instance Fields
        private string _name;
        private ParameterDirection _direction;
        private object _value;
        private byte _precision;
        private byte _scale;
        private int? _size;
        #endregion

        #region Constructors
        public DbParameterDescriptor(string name)
            : this(name, null)
        {
        }

        public DbParameterDescriptor(string name, object value)
            : this(name, ParameterDirection.Input, value)
        {
        }

        public DbParameterDescriptor(string name, 
			ParameterDirection direction, object value)
        {
            Precondition.Defined(name,
				() => Error.ArgumentNull("name"));

            _name = name;
            _direction = direction;
            _value = value;
        }
        #endregion

        #region Instance Properties
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Precondition.Defined(value,
					() => Error.ArgumentNull("value"));
                _name = value;
            }
        }

        public object Value
        {
            get
            {
                return _value ?? DBNull.Value;
            }
            set
            {
                _value = value;
            }
        }

        public ParameterDirection Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        public byte Precision
        {
            get
            {
                return _precision;
            }
            set
            {
                _precision = value;
            }
        }

        public byte Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }

        public int? Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }
        #endregion
	}
}
