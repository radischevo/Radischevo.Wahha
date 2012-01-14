using System;

namespace Radischevo.Wahha.Data.Mapping
{
	/// <summary>
    /// Use this attribute to mark properties referencing data in a remote table. It is
    /// used by the framework to map property names of one object to other objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class AssociationAttribute : Attribute
    {
        #region Instance Fields
		private bool _projected;
		private bool _persistent;
		private bool _canBeNull;
		private string _thisKey;
        private string _otherKey;
		private string _prefix;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Data.Mapping.AssociationAttribute"/> 
        /// attribute. This will relate the decorated property
        /// to the specified column in the given table.
        /// </summary>
        public AssociationAttribute()
        {
			_canBeNull = true;
        }
        #endregion

        #region Instance Properties
		/// <summary>
        /// Gets ot sets the name of the column, 
        /// which relates to the other type.
        /// </summary>
        public string ThisKey
        {
            get
            {
                return _thisKey;
            }
            set
            {
                _thisKey = value;
            }
        }
		
        /// <summary>
        /// Gets or sets the name of the column, 
        /// which this instance relates to.
        /// </summary>
        public string OtherKey
        {
            get 
            { 
                return _otherKey; 
            }
            set
            {
                _otherKey = value;
            }
        }
		
		/// <summary>
		/// Gets or sets a value indicating whether this 
		/// association is included in the projection.
		/// </summary>
		public bool Projected
		{
			get
			{
				return _projected;
			}
			set
			{
				_projected = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this 
		/// association key is persisted by the <c>ThisKey</c> column.
		/// </summary>
		public bool Persistent
		{
			get
			{
				return _persistent;
			}
			set
			{
				_persistent = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the prefix for the projection subset.
		/// </summary>
		public string Prefix
		{
			get
			{
				return _prefix;
			}
			set
			{
				_prefix = value;
			}	
		}

		/// <summary>
		/// Gets or sets a value indicating whether this 
		/// association can be set to null.
		/// </summary>
		public bool CanBeNull
		{
			get
			{
				return _canBeNull;
			}
			set
			{
				_canBeNull = value;
			}
		}
        #endregion
    }
}

