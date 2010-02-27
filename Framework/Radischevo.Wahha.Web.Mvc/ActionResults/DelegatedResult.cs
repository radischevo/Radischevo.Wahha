using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class DelegatedResult : ActionResult
    {
        #region Instance Fields
        private Action<ControllerContext> _command;
        #endregion

        #region Constructors
        public DelegatedResult() 
            : this(null)
        {
        }

        public DelegatedResult(Action<ControllerContext> command)
        {
            _command = command;
        }
        #endregion

        #region Instance Properties
        public Action<ControllerContext> Command
        {
            get
            {
                return _command;
            }
            set
            {
                _command = value;
            }
        }
        #endregion

        #region Instance Methods
        public override void Execute(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            if (_command == null)
                return;

            _command(context);
        }
        #endregion
    }
}
