using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public sealed class ControllerDescriptorCache : ReaderWriterCache<Type, ControllerDescriptor>
    {
        #region Constructors
        public ControllerDescriptorCache() 
            : base()
        {   }
        #endregion

        #region Instance Methods
        public ControllerDescriptor GetDescriptor(Type type)
        {
			return GetDescriptor(type, () => {
				return new ReflectedControllerDescriptor(type);
			});
        }

		public ControllerDescriptor GetDescriptor(Type type, 
			Func<ControllerDescriptor> creator)
		{
			return base.GetOrCreate(type, creator);
		}  
        #endregion
    }
}
