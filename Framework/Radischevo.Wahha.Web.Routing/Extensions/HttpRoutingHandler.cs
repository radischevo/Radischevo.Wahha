using System;
using System.Web;
using System.Reflection;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    public class HttpRoutingHandler<THandler> : RoutingHandlerBase
        where THandler : IHttpHandler
    {
        #region Instance Fields
        private Func<THandler> _factory;
        #endregion

        #region Constructors
        public HttpRoutingHandler()
            : this(null)
        {
        }

        public HttpRoutingHandler(Func<THandler> factory)
            : base()
        {
            _factory = factory;
        }
        #endregion

        #region Instance Properties
        public Func<THandler> Factory
        {
            get
            {
                if (_factory == null)
                    _factory = CreateDefaultFactory();

                return _factory;
            }
            set
            {
                _factory = value;
            }
        }
        #endregion

        #region Static Methods
        private static Func<THandler> CreateDefaultFactory()
        {
            Type type = typeof(THandler);

            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            Precondition.Require(constructor, Error.CouldNotCreateHttpHandler(type));

            return () => (THandler)constructor.Invoke(null);
        }
        #endregion

        #region Instance Methods
        protected override IHttpHandler GetHttpHandler(RequestContext context)
        {
            return Factory();
        }
        #endregion
    }
}
