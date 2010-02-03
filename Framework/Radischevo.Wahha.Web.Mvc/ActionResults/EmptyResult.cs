using System;

namespace Radischevo.Wahha.Web.Mvc
{
    /// <summary>
    /// Represents a result that doesn't do anything, 
    /// like a controller action returning null.
    /// </summary>
    public class EmptyResult : ActionResult
    {
        #region Static Fields
        private static readonly EmptyResult _instance = new EmptyResult();
        #endregion

        #region Static Properties
        public static EmptyResult Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        #region Instance Methods
        public override void Execute(ControllerContext context)
        {
        }
        #endregion
    }
}
