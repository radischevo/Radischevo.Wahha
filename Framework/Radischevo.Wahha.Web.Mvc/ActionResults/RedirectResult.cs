using System;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class RedirectResult : ActionResult
    {
        #region Instance Fields
        private string _url;
        #endregion

        #region Constructors
        public RedirectResult(string url)
        {
            if (String.IsNullOrEmpty(url))
                throw Error.ArgumentNull("url");

            _url = url;
        }
        #endregion

        #region Instance Properties
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw Error.ArgumentNull("value");

                _url = value;
            }
        }
        #endregion

        #region Instance Methods
        public override void Execute(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
			if (context.IsChild)
				throw Error.CannotRedirectFromChildAction();

            context.Context.Response.Redirect(_url);
        }
        #endregion
    }
}
