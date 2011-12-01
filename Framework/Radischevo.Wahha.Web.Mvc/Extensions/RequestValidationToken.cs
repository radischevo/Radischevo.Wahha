using System;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;

using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
    public sealed class RequestValidationToken
    {
        #region Nested Types
        private sealed class TokenPersister : PageStatePersister
        {
            #region Constructors
            private TokenPersister(Page page)
                : base(page)
            {
            }
            #endregion

            #region Static Methods
            public static IStateFormatter CreateFormatter()
            {
                HttpResponse response = new HttpResponse(TextWriter.Null);
                HttpRequest request = new HttpRequest("__token__.aspx", 
                    HttpContext.Current.Request.Url.ToString(), "__EVENTTARGET=true&__VIEWSTATEENCRYPTED=true");
                HttpContext context = new HttpContext(request, response);
                
                Page page = new Page();
                page.EnableViewStateMac = true;
                page.ViewStateEncryptionMode = ViewStateEncryptionMode.Always;
                
                page.ProcessRequest(context);
                return new TokenPersister(page).StateFormatter;
            }
            #endregion

            #region Instance Methods
            public override void Load()
            {
                throw new NotImplementedException();
            }

            public override void Save()
            {
                throw new NotImplementedException();
            }
            #endregion
        }

        [Serializable]
        private sealed class State
        {
            #region Instance Fields
		    public string Value;
            public string Signature;
            public DateTime CreationDate;
	        #endregion

            #region Constructors
            public State(string value, string signature, DateTime creationDate)
            {
                Value = value;
                Signature = signature;
                CreationDate = creationDate;
            }
            #endregion
        }
        #endregion

        #region Instance Fields
        private string _signature;
        private string _value;
        private DateTime _creationDate;
        #endregion

        #region Constructors
        private RequestValidationToken()
        {   }
        #endregion

        #region Instance Properties
        public DateTime CreationDate
        {
            get
            {
                return _creationDate;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        #endregion

        #region Static Methods
        private static string GenerateRequestSignature(HttpContextBase context)
        {
            if (context == null)
                return String.Empty;

            string userAgent = context.Request.UserAgent;
            string hostAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrEmpty(hostAddress))
                hostAddress = context.Request.ServerVariables["REMOTE_ADDR"];

            string userName = (context.User == null) ? "ANONYMOUS" : context.User.Identity.Name;

            return FormsAuthentication.HashPasswordForStoringInConfigFile(
                String.Concat(userName, hostAddress, userAgent), "MD5");
        }

        public static RequestValidationToken Create(HttpContextBase context)
        {
			return Create(context, null);
        }

		public static RequestValidationToken Create(HttpContextBase context, string value)
		{
			RequestValidationToken token = new RequestValidationToken();

			token._creationDate = DateTime.Now;
			token._signature = GenerateRequestSignature(context);
			token._value = value;

			return token;
		}

        public static RequestValidationToken Create(string serializedTokenData)
        {
            RequestValidationToken token = new RequestValidationToken();
            IStateFormatter formatter = TokenPersister.CreateFormatter();
            try
            {
                State state = (State)formatter.Deserialize(serializedTokenData);

                token._value = state.Value;
                token._signature = state.Signature;
                token._creationDate = state.CreationDate;
            }
            catch (Exception exception)
            {
                throw Error.RequestValidationError(exception);
            }
            return token;
        }
        #endregion

        #region Instance Methods
        public string Serialize()
        {
            IStateFormatter formatter = TokenPersister.CreateFormatter();
            State state = new State(_value, _signature, _creationDate);

            return formatter.Serialize(state);
        }

        public bool IsValid(RequestValidationToken obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;

            if (!String.Equals(_value, obj._value, StringComparison.Ordinal))
                return false;

            return String.Equals(_signature, obj._signature, StringComparison.Ordinal);
        }
        #endregion
    }
}
