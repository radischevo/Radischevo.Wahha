using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Radischevo.Wahha.Core
{
    public static class ParameterInfoExtensions
    {
        #region Static Extensions Methods
        /// <summary>
        /// Helper method used to find attributes of type <typeparamref name="TAttribute"/>, 
        /// associated with the specified member.
        /// </summary>
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this ParameterInfo parameter)
        {
            return GetCustomAttributes<TAttribute>(parameter, false);
        }

        /// <summary>
        /// Helper method used to find attributes of type <typeparamref name="TAttribute"/>, 
        /// associated with the specified member.
        /// </summary>
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this ParameterInfo parameter, bool inherit)
        {
			Precondition.Require(parameter, () => Error.ArgumentNull("parameter"));
            return parameter.GetCustomAttributes(inherit).OfType<TAttribute>();
        }
        #endregion
    }
}
