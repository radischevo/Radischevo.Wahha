using System;
using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Mvc
{
    public class ViewDataDictionary<TModel> : ViewDataDictionary 
        where TModel : class
    {
        #region Constructors
        public ViewDataDictionary() :
            base()
        {
        }

        public ViewDataDictionary(TModel model) :
            base(model)
        {
        }

        public ViewDataDictionary(TModel model, object values) :
            base(model, values)
        {
        }

        public ViewDataDictionary(ViewDataDictionary viewDataDictionary) :
            base(viewDataDictionary)
        {
        }
        #endregion

        #region Instance Properties
        public new TModel Model
        {
            get
            {
                return (TModel)base.Model;
            }
            set
            {
                SetModel(value);
            }
        }
        #endregion

        #region Instance Methods
        protected override void SetModel(object value)
        {
            TModel model = value as TModel;

            if (value != null && model == null)
                throw Error.InvalidViewDataType(typeof(TModel), value.GetType());

            base.SetModel(value);
        }
        #endregion
    }
}
