using System;

using Radischevo.Wahha.Core;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
    public class TemplateDescriptor
    {
        #region Instance Fields
        private object _value;
        private string _prefix;
        private Type _type;
        private bool _isNullableType;
        private HashSet<object> _visitedObjects;
        #endregion

        #region Constructors
        public TemplateDescriptor(Type type)
            : this(type, null)
        {
        }

		public TemplateDescriptor(Type type, object value)
        {
            Precondition.Require(type, () => Error.ArgumentNull("type"));
            _type = type;
            _value = value;
        }
        #endregion

        #region Instance Properties
        public object Value
        {
            get
            {
                return _value ?? String.Empty;
            }
            set
            {
                _value = value;
            }
        }

        public string Prefix
        {
            get
            {
                return _prefix ?? String.Empty;
            }
            set
            {
                _prefix = value;
            }
        }

        public Type Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public int Depth
        {
            get
            {
                return _visitedObjects.Count;
            }
        }

        public bool IsNullableType
        {
            get
            {
                return _isNullableType;
            }
            set
            {
                _isNullableType = value;
            }
        }

        public HashSet<object> VisitedObjects
        {
            get
            {
                if (_visitedObjects == null)
                    _visitedObjects = new HashSet<object>();

                return _visitedObjects;
            }
            set
            {
                _visitedObjects = value;
            }
        }
        #endregion

        #region Instance Methods
        public virtual string GetHtmlElementName(string expression)
        {
			if (String.IsNullOrEmpty(expression))
				return Prefix;

			string rightPart = expression.Replace('.', '-')
				.Replace("[", "").Replace("]", "")
				.Trim('-').ToLowerInvariant();

			if (String.IsNullOrEmpty(Prefix))
				return rightPart;

            return String.Concat(Prefix, "-", rightPart);
        }
        #endregion
    }
}