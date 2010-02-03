using System;
using System.Net;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class PermanentRedirectResult : RedirectResult
    {
        #region Constructors
        public PermanentRedirectResult(string url)
            : base(url)
        {
        }
        #endregion

        #region Instance Methods
        public override void Execute(ControllerContext context)
        {
            Precondition.Require(context, Error.ArgumentNull("context"));
			if (context.IsChild)
				throw Error.CannotRedirectFromChildAction();

            context.Context.Response.StatusCode = (int)HttpStatusCode.MovedPermanently;
            context.Context.Response.StatusDescription = "Moved Permanently";
            context.Context.Response.RedirectLocation = Url;
        }
        #endregion
    }
}
