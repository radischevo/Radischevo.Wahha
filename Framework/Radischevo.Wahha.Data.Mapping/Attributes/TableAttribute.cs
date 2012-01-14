using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	/// <summary>
    /// Use this attribute on classes that should be persistable. Only classes decorated
    /// with this attribute are supported by the framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class TableAttribute : Attribute
	{
		#region Instance Fields
		private string _schema;
		private string _name;
        private string _projection;
		private Type _serializer;
		#endregion
		
		#region Constructors
		/// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Data.Mapping.TableAttribute"/> attribute.
        /// </summary>
		public TableAttribute ()
		{
			_schema = String.Empty;
		}
		#endregion
		
		#region Instance Properties
		/// <summary>
        /// Gets or sets the name of the database table which is 
        /// used to store instances of this class.
        /// </summary>
        public string Name
        {
            get 
            { 
                return _name; 
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the optional schema name with 
        /// which to prefix the table name in queries.
        /// </summary>
        public string Schema
        {
            get 
            { 
                return _schema; 
            }
            set 
            { 
                _schema = value; 
            }
        }
		
		/// <summary>
        /// Gets or sets the name of the optional database view which is 
        /// used to select instances of this class.
        /// </summary>
        public string Projection
        {
            get 
            { 
                return _projection;
            }
            set
            {
                _projection = value;
            }
        }
		
		/// <summary>
		/// Gets or sets the type of the 
		/// <see cref="Radischevo.Wahha.Data.IDbSerializer"/> 
		/// which is used to materialize instances of 
		/// this class and send them back to the database.
		/// </summary>
		public Type Serializer
		{
			get
			{
				return _serializer;
			}
			set
			{
				_serializer = value;
			}
		}
		#endregion
	}
}

