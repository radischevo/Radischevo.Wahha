using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class DataAnnotationsModelMetadata : ModelMetadata
    {
        #region Instance Fields
        private string _displayColumn;
        #endregion

        #region Constructors
        public DataAnnotationsModelMetadata(Type type, Func<object, object> accessor)
            : this(null, null, type, accessor)
        {
        }

        public DataAnnotationsModelMetadata(DataAnnotationsModelMetadata container, 
            string propertyName, Type type, Func<object, object> accessor)
            : base(container, propertyName, type, accessor)
        {
            LoadMetadataFromAttributes(GetTypeDescriptor()
                .GetAttributes().Cast<Attribute>());
        }
        #endregion

        #region Instance Properties
        public string DisplayColumn
        {
            get
            {
                return _displayColumn;
            }
            set
            {
                _displayColumn = value;
            }
        }
        #endregion

        #region Static Methods
        private static void ValidateDisplayColumnAttribute(
            string displayColumn, PropertyInfo property, Type type)
        {
            Precondition.Require(property, 
                Error.UnknownProperty(type, displayColumn));
            Precondition.Require(property.GetGetMethod(), 
                Error.UnreadableProperty(type, displayColumn));
        }
        #endregion

        #region Instance Methods
        public override string GetDisplayText(object container)
        {
            if (container != null)
            {
                if (!String.IsNullOrEmpty(_displayColumn))
                {
                    PropertyInfo property = Type.GetProperty(_displayColumn, 
                        BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

                    ValidateDisplayColumnAttribute(_displayColumn, property, Type);

                    object simpleDisplayTextValue = property.GetValue(container, new object[0]);
                    if (simpleDisplayTextValue != null)
                        return simpleDisplayTextValue.ToString();
                }
            }
            return base.GetDisplayText(container);
        }

        protected virtual ICustomTypeDescriptor GetTypeDescriptor()
        {
            return new AssociatedMetadataTypeTypeDescriptionProvider(Type).GetTypeDescriptor(Type);
        }

        protected virtual void LoadMetadataFromAttributes(IEnumerable<Attribute> attributes)
        {
            DisplayColumnAttribute displayColumn =
                attributes.OfType<DisplayColumnAttribute>().FirstOrDefault();
            if (displayColumn != null)
                DisplayColumn = displayColumn.DisplayColumn;

            HiddenInputAttribute hiddenInput =
                attributes.OfType<HiddenInputAttribute>().FirstOrDefault();
            if (hiddenInput != null)
            {
                TemplateHint = "HiddenInput";
                HideSurroundingChrome = !hiddenInput.DisplayValue;
            }

            IEnumerable<UIHintAttribute> uiHint = attributes.OfType<UIHintAttribute>();
            UIHintAttribute uiHintAttribute = uiHint.FirstOrDefault(a =>
                String.Equals(a.PresentationLayer, "MVC", StringComparison.OrdinalIgnoreCase))
                    ?? uiHint.FirstOrDefault(a => String.IsNullOrEmpty(a.PresentationLayer));
            if (uiHintAttribute != null)
                TemplateHint = uiHintAttribute.UIHint;

            DataTypeAttribute dataType = attributes.OfType<DataTypeAttribute>().FirstOrDefault();
            if (dataType != null)
                DataType = dataType.GetDataTypeName();

            ReadOnlyAttribute readOnly = attributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
            if (readOnly != null)
                IsReadOnly = readOnly.IsReadOnly;

            DisplayFormatAttribute displayFormat = attributes
                .OfType<DisplayFormatAttribute>().FirstOrDefault();
            if (displayFormat != null)
            {
                NullDisplayText = displayFormat.NullDisplayText;
                DisplayFormat = displayFormat.DataFormatString;
                ConvertEmptyStringToNull = displayFormat.ConvertEmptyStringToNull;

                if (displayFormat.ApplyFormatInEditMode)
                    EditFormat = displayFormat.DataFormatString;
            }

            ScaffoldColumnAttribute scaffoldColumn = attributes
                .OfType<ScaffoldColumnAttribute>().FirstOrDefault();
            if (scaffoldColumn != null)
                Visible = Editable = scaffoldColumn.Scaffold;

			FieldOrderAttribute fieldOrder = attributes
				.OfType<FieldOrderAttribute>().FirstOrDefault();
			FieldOrder = (fieldOrder == null) ? 2048 : fieldOrder.Order;

            DisplayNameAttribute displayNameAttribute = attributes
                .OfType<DisplayNameAttribute>().FirstOrDefault();
            if (displayNameAttribute != null)
                DisplayName = displayNameAttribute.DisplayName;
        }

        protected override IEnumerable<ModelMetadata> CreateMetadataForProperties()
        {
            foreach (PropertyDescriptor property in GetTypeDescriptor().GetProperties())
            {
                yield return CreateMetadataForProperty(property);
            }
        }

        protected override ModelMetadata CreateMetadataForProperty(PropertyDescriptor property)
        {
            DataAnnotationsModelMetadata metadata = new DataAnnotationsModelMetadata(this, 
                property.Name, property.PropertyType, (obj) => property.GetValue(obj));
            metadata.LoadMetadataFromAttributes(property.Attributes.Cast<Attribute>());
            
            return metadata;
        }
        #endregion
    }
}
