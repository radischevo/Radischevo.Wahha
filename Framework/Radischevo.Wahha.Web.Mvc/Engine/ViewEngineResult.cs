using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public sealed class ViewEngineResult
    {
        #region Instance Fields
        private IView _view;
        private IViewEngine _engine;
        #endregion

        #region Constructors
        public ViewEngineResult(IView view, IViewEngine engine)
        {
            Precondition.Require(view, () => Error.ArgumentNull("view"));
            Precondition.Require(engine, () => Error.ArgumentNull("engine"));

            _view = view;
            _engine = engine;
        }
        #endregion

        #region Instance Properties
        public IView View
        {
            get
            {
                return _view;
            }
        }

        public IViewEngine Engine
        {
            get
            {
                return _engine;
            }
        }
        #endregion
    }
}
