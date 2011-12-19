using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;

namespace Radischevo.Wahha.Web.Mvc.Html
{
    public class ValidationHelper : IHideObjectMembers
    {
        #region Instance Fields
        private ViewContext _context;
        #endregion

        #region Constructors
        public ValidationHelper(ViewContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            _context = context;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// Gets the current 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ViewContext"/>.
        /// </summary>
        public ViewContext Context
        {
            get
            {
                return _context;
            }
        }
		
		/// <summary>
		/// Gets the state of the model.
		/// </summary>
		public ModelStateCollection ModelState 
		{
			get
			{
				return _context.ModelState;
			}
		}
		#endregion

        #region Instance Methods
		public bool Valid()
		{
			return ModelState.IsValid();
		}
		
		public bool Valid(string key)
		{
			return ModelState.IsValid(key);
		}
		
        public void Message (string key, Action<IEnumerable<ValidationError>> action)
		{
			Precondition.Require (action, () => Error.ArgumentNull ("action"));
			IEnumerable<ValidationError> enumerable = ModelState.Errors[key];
			
			if (enumerable != null && enumerable.Any())
				action (enumerable);
		}
		
		public void Messages (string key, Action<ValidationError> action) 
		{
			Precondition.Require (action, () => Error.ArgumentNull ("action"));
			Message(key, a => a.ForEach(action));
		}
		
		public void Summary (Action<ValidationError> action)
		{
			Precondition.Require (action, () => Error.ArgumentNull ("action"));
			foreach (ValidationError current in ModelState.Errors)
				action (current);
		}
        #endregion
    }
}
