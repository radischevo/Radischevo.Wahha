using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
    public class CollectionResult : ActionResult
    {
        #region Instance Fields
        private List<ActionResult> _results;
        #endregion

        #region Constructors
        public CollectionResult()
        {
            _results = new List<ActionResult>();
        }

        public CollectionResult(IEnumerable<ActionResult> list)
        {
            Precondition.Require(list, () => Error.ArgumentNull("list"));
            _results = new List<ActionResult>(list);
        }
        #endregion

        #region Instance Properties
        public ICollection<ActionResult> Results
        {
            get
            {
                return _results;
            }
        }
        #endregion

        #region Instance Methods
        public override void Execute(ControllerContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            foreach (ActionResult result in _results)
                result.Execute(context);
        }
        #endregion
    }
}
