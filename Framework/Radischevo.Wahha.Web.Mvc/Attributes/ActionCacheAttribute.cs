using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Security;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Caching;
using Radischevo.Wahha.Web;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
        Inherited = true, AllowMultiple = false)]
    public class ActionCacheAttribute : ActionFilterAttribute
    {
        #region Instance Fields
        private string[] _keys;
        private bool _matchAnyKey;
        private string[] _tags;
        private int _duration;
        private bool _varyByUser;
        private string _cacheKey;
        #endregion

        #region Constructors
        public ActionCacheAttribute()
            : this("*", null)
        {
        }

        public ActionCacheAttribute(string tags)
            : this("*", tags)
        {
        }

        public ActionCacheAttribute(string varyByKeys, string tags)
        {
            Precondition.Require(varyByKeys, () => Error.ArgumentNull("varyByKeys"));
            char[] splitters = new char[] { ',' };

            if (String.Equals(varyByKeys, "*", 
                StringComparison.InvariantCulture))
                _matchAnyKey = true;
            else
                _keys = varyByKeys.Split(splitters,
                    StringSplitOptions.RemoveEmptyEntries);

            _tags = (tags == null) ? null : tags.Split(splitters,
                StringSplitOptions.RemoveEmptyEntries);
            _varyByUser = true;
            _duration = 60;
        }
        #endregion

        #region Instance Properties
        protected CacheProvider Cache
        {
            get
            {
                return CacheProvider.Instance;
            }
        }

        public int Duration
        {
            get
            {
                return _duration;
            }
            set
            {
				Precondition.Require(value > -1, () => 
					Error.ArgumentOutOfRange("value"));

                _duration = value;
            }
        }

        public bool VaryByUser
        {
            get
            {
                return _varyByUser;
            }
            set
            {
                _varyByUser = value;
            }
        }
        #endregion

        #region Static Methods
        private static string CreateCacheKey(ActionExecutionContext context, 
            bool varyByUser, string[] keys, bool matchAny)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(context.Action.Method.ReflectedType.FullName)
                .Append("::").Append(ActionMethodSelector.GetNameOrAlias(context.Action.Method));

            sb.Append("=>{");

            IEnumerable<string> collection = (matchAny) ? 
                (IEnumerable<string>)context.Context.Parameters.Keys : keys;

            foreach (string key in collection)
                sb.Append(GetParameterKey(context, key));

            if (varyByUser)
				sb.Append("[UserIdentity=").Append(GetUserIdentity(context.HttpContext)).Append("]");

            sb.Append("}");
            return FormsAuthentication.HashPasswordForStoringInConfigFile(sb.ToString(), "MD5");
        }

        private static string GetUserIdentity(HttpContextBase context)
        {
            if (context.User == null || context.User.Identity == null)
                return "<NULL>";

            if (context.User.Identity.IsAuthenticated)
                return context.User.Identity.Name;

            return "<ANONYMOUS-USER>";
        }

        private static string GetParameterKey(ActionExecutionContext context, string parameterName)
        {
            object value;
            context.Context.Parameters.TryGetValue(parameterName, out value);
            
            return String.Concat("[", parameterName, "=", 
                (value == null) ? "<NULL>" : Convert.ToString(value, CultureInfo.InvariantCulture), "]");
        }
        #endregion

        #region Instance Methods
		protected virtual string CreateCacheKey(ActionExecutionContext context)
		{
			return CreateCacheKey(context, _varyByUser, _keys, _matchAnyKey);
		}

		protected virtual TValue GetCachedValue<TValue>(string key)
		{
			return Cache.Get<TValue>(key);
		}

		protected virtual void UpdateCachedValue<TValue>(string key, TValue value)
		{
			if(value != null)
				Cache.Insert(key, value, DateTime.UtcNow.AddMinutes(_duration), _tags);
		}

        public override void OnExecuting(ActionExecutionContext context)
        {
			_cacheKey = CreateCacheKey(context);
			ActionResult cachedResult = GetCachedValue<ActionResult>(_cacheKey);

            if (cachedResult != null)
            {
                context.Cancel = true;
                context.Result = cachedResult;
            }
        }

        public override void OnExecuted(ActionExecutedContext context)
        {
			if (context.Exception != null)
				return;

			if (context.Result != null && !String.IsNullOrEmpty(_cacheKey))
				UpdateCachedValue(_cacheKey, context.Result);
        }
        #endregion
    }
}
