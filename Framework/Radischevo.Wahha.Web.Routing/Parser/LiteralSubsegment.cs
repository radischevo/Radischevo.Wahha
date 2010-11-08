using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Routing
{
    internal class LiteralSubsegment : PathSubsegment
    {
        #region Instance Fields
        private string _literal;
        #endregion

        #region Constructors
        public LiteralSubsegment(string literal)
        {
            Precondition.Require(literal, () => Error.ArgumentNull("literal"));
            _literal = literal;
        }
        #endregion

        #region Instance Properties
        public virtual string Literal
        {
            get
            {
                return _literal;
            }
        }
        #endregion

		#region Instance Methods
		public override string ToString()
        {
            return _literal;
        }
        #endregion
    }
}
