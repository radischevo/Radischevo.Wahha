using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ModelMetadata
    {
        #region Instance Fields
        private ModelMetadata _container;
        private string _propertyName;
        private Type _type;
        private Func<object, object> _modelAccessor;
        private IEnumerable<ModelMetadata> _properties;
        #endregion

        #region Metadata Fields
		private int _fieldOrder;
        private bool _isRequired;
        private bool _isReadOnly;
        private bool _visible;
        private bool _editable;
        private bool _convertEmptyStringToNull;
        private bool _hideSurroundingChrome;
        private string _templateHint;
        private string _simpleDisplayText;
        private string _nullDisplayText;
        private string _dataType;
        private string _description;
        private string _displayName;
        private string _shortDisplayName;
        private string _displayFormat;
        private string _editFormat;
        private string _watermark;
        #endregion

        #region Constructors
        public ModelMetadata(Type modelType, Func<object, object> accessor)
            : this(null, null, modelType, accessor)
        {
        }

        public ModelMetadata(ModelMetadata container, string propertyName, 
            Type modelType, Func<object, object> accessor)
        {
            Precondition.Require(modelType, () => Error.ArgumentNull("type"));

            _container = container;
            _propertyName = propertyName;
            _type = modelType;
            _visible = true;
            _editable = true;
            _convertEmptyStringToNull = true;
            _isRequired = !modelType.IsNullable();
            _modelAccessor = accessor;
        }
        #endregion

        #region Instance Properties
        public ModelMetadata Container
        {
            get
            {
                return _container;
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }

        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public IEnumerable<ModelMetadata> Properties
        {
            get
            {
                if (_properties == null)
                    _properties = CreateMetadataForProperties();

                return _properties;
            }
        }
        
        public virtual bool IsComplexType
        {
            get
            {
                return !(TypeDescriptor.GetConverter(_type)
                    .CanConvertFrom(typeof(string)));
            }
        }

        public bool IsNullableValueType
        {
            get
            {
                return (Nullable.GetUnderlyingType(_type) != null);
            }
        }
        #endregion

        #region Metadata Properties
		public virtual int FieldOrder
		{
			get
			{
				return _fieldOrder;
			}
			set
			{
				_fieldOrder = value;
			}
		}

        public virtual bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                _isReadOnly = value;
            }
        }

        public virtual bool IsRequired
        {
            get
            {
                return _isRequired;
            }
            set
            {
                _isRequired = value;
            }
        }

        public virtual bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        public virtual bool Editable
        {
            get
            {
                return _editable;
            }
            set
            {
                _editable = value;
            }
        }

        public virtual bool ConvertEmptyStringToNull
        {
            get
            {
                return _convertEmptyStringToNull;
            }
            set
            {
                _convertEmptyStringToNull = value;
            }
        }
        
        public virtual bool HideSurroundingChrome
        {
            get
            {
                return _hideSurroundingChrome;
            }
            set
            {
                _hideSurroundingChrome = value;
            }
        }

        public virtual string DataType
        {
            get
            {
                return _dataType;
            }
            set
            {
                _dataType = value;
            }
        }

        public virtual string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        public virtual string ShortDisplayName
        {
            get
            {
                return _shortDisplayName;
            }
            set
            {
                _shortDisplayName = value;
            }
        }

        public virtual string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public virtual string TemplateHint
        {
            get
            {
                return _templateHint;
            }
            set
            {
                _templateHint = value;
            }
        }

        public virtual string NullDisplayText
        {
            get
            {
                return _nullDisplayText;
            }
            set
            {
                _nullDisplayText = value;
            }
        }

        public virtual string DisplayFormat
        {
            get
            {
                return _displayFormat;
            }
            set
            {
                _displayFormat = value;
            }
        }

        public virtual string EditFormat
        {
            get
            {
                return _editFormat;
            }
            set
            {
                _editFormat = value;
            }
        }

        public virtual string Watermark
        {
            get
            {
                return _watermark;
            }
            set
            {
                _watermark = value;
            }
        }
        #endregion

        #region Instance Methods
        protected virtual IEnumerable<ModelMetadata> CreateMetadataForProperties()
        {
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(_type))
            {
				yield return CreateMetadataForProperty(property);
            }
        }

        protected virtual ModelMetadata CreateMetadataForProperty(PropertyDescriptor property)
        {
            return new ModelMetadata(this, property.Name, 
				property.PropertyType, (obj) => property.GetValue(obj));
        }

        public virtual string GetDisplayText(object container)
        {
            return (container == null) ? String.Empty : container.ToString();
        }

        public virtual object GetModel(object container)
        {
            return (_modelAccessor == null) ? container : _modelAccessor(container);
        }

        public ModelMetadata GetPropertyMetadata(string propertyName)
        {
            if (Properties == null)
                return null;

            return Properties.FirstOrDefault(p => String.Equals(propertyName, 
                p.PropertyName, StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion
    }
}
